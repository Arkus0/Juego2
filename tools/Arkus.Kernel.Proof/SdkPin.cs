using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.Json;

namespace Arkus.Kernel.Proof
{
    /// <summary>
    /// A parsed .NET SDK version.
    /// </summary>
    public sealed class SdkVersion
    {
        private SdkVersion(int major, int minor, int featureBand, int patch, string prerelease, string raw)
        {
            Major = major;
            Minor = minor;
            FeatureBand = featureBand;
            Patch = patch;
            Prerelease = prerelease;
            Raw = raw;
        }

        /// <summary>Major version.</summary>
        public int Major { get; }

        /// <summary>Minor version.</summary>
        public int Minor { get; }

        /// <summary>Feature band (the hundreds digit of the patch component).</summary>
        public int FeatureBand { get; }

        /// <summary>Patch level inside the feature band.</summary>
        public int Patch { get; }

        /// <summary>Prerelease label, or an empty string.</summary>
        public string Prerelease { get; }

        /// <summary>The version exactly as written.</summary>
        public string Raw { get; }

        /// <summary>Parses an SDK version such as <c>8.0.131</c>.</summary>
        /// <param name="value">Version text.</param>
        /// <returns>The parsed version.</returns>
        public static SdkVersion Parse(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ProofToolException("Empty SDK version.");
            }

            var raw = value.Trim();
            var prerelease = string.Empty;
            var core = raw;
            var dash = raw.IndexOf('-', StringComparison.Ordinal);
            if (dash >= 0)
            {
                prerelease = raw.Substring(dash + 1);
                core = raw.Substring(0, dash);
            }

            var parts = core.Split('.');
            if (parts.Length != 3)
            {
                throw new ProofToolException($"SDK version '{raw}' is not major.minor.patch.");
            }

            if (!int.TryParse(parts[0], NumberStyles.None, CultureInfo.InvariantCulture, out var major)
                || !int.TryParse(parts[1], NumberStyles.None, CultureInfo.InvariantCulture, out var minor)
                || !int.TryParse(parts[2], NumberStyles.None, CultureInfo.InvariantCulture, out var patchComponent))
            {
                throw new ProofToolException($"SDK version '{raw}' has non-numeric components.");
            }

            return new SdkVersion(major, minor, patchComponent / 100, patchComponent % 100, prerelease, raw);
        }
    }

    /// <summary>
    /// The SDK pin recorded in <c>global.json</c>.
    /// </summary>
    public sealed class GlobalJsonPin
    {
        /// <summary>Creates a pin.</summary>
        /// <param name="version">Pinned version text.</param>
        /// <param name="rollForward">Roll-forward policy text.</param>
        /// <param name="allowPrerelease">Whether prerelease SDKs are allowed.</param>
        public GlobalJsonPin(string version, string rollForward, bool allowPrerelease)
        {
            Version = version;
            RollForward = rollForward;
            AllowPrerelease = allowPrerelease;
        }

        /// <summary>Pinned version text.</summary>
        public string Version { get; }

        /// <summary>Roll-forward policy text.</summary>
        public string RollForward { get; }

        /// <summary>Whether prerelease SDKs are allowed.</summary>
        public bool AllowPrerelease { get; }

        /// <summary>Reads the pin from a <c>global.json</c> file.</summary>
        /// <param name="path">Absolute file path.</param>
        /// <returns>The pin.</returns>
        public static GlobalJsonPin Load(string path)
        {
            if (path is null)
            {
                throw new ArgumentNullException(nameof(path));
            }

            if (!File.Exists(path))
            {
                throw new ProofToolException($"SDK pin file not found: {path}");
            }

            try
            {
                using var document = JsonDocument.Parse(File.ReadAllText(path));
                if (!document.RootElement.TryGetProperty("sdk", out var sdk))
                {
                    throw new ProofToolException($"SDK pin file has no 'sdk' section: {path}");
                }

                var version = sdk.TryGetProperty("version", out var v) ? v.GetString() ?? string.Empty : string.Empty;
                var rollForward = sdk.TryGetProperty("rollForward", out var r)
                    ? r.GetString() ?? string.Empty
                    : string.Empty;
                var allowPrerelease = sdk.TryGetProperty("allowPrerelease", out var p) && p.GetBoolean();

                if (string.IsNullOrEmpty(version))
                {
                    throw new ProofToolException($"SDK pin file declares no version: {path}");
                }

                return new GlobalJsonPin(version, rollForward, allowPrerelease);
            }
            catch (JsonException ex)
            {
                throw new ProofToolException($"SDK pin file is not valid JSON: {path}", ex);
            }
        }
    }

    /// <summary>
    /// Pin evaluation rules.
    /// </summary>
    /// <remarks>
    /// The narrowness allowlist lives in code rather than in the manifest on
    /// purpose: loosening the pin must require changing tooling and its tests, not
    /// editing one data file.
    /// </remarks>
    public static class SdkPinRules
    {
        private static readonly HashSet<string> NarrowRollForward = new HashSet<string>(StringComparer.Ordinal)
        {
            "disable",
            "patch",
            "latestPatch",
        };

        /// <summary>Whether a roll-forward policy keeps the feature band pinned.</summary>
        /// <param name="rollForward">Roll-forward policy text.</param>
        /// <returns><c>true</c> when the policy is narrow enough.</returns>
        public static bool IsNarrowEnough(string rollForward)
        {
            return rollForward is not null && NarrowRollForward.Contains(rollForward);
        }

        /// <summary>Whether a running SDK satisfies a pin.</summary>
        /// <param name="pin">The declared pin.</param>
        /// <param name="running">The running SDK version.</param>
        /// <param name="reason">Why the SDK does not satisfy the pin.</param>
        /// <returns><c>true</c> when the running SDK satisfies the pin.</returns>
        public static bool IsSatisfiedBy(GlobalJsonPin pin, SdkVersion running, out string reason)
        {
            if (pin is null)
            {
                throw new ArgumentNullException(nameof(pin));
            }

            if (running is null)
            {
                throw new ArgumentNullException(nameof(running));
            }

            var pinned = SdkVersion.Parse(pin.Version);

            if (running.Prerelease.Length > 0 && !pin.AllowPrerelease)
            {
                reason = $"running SDK '{running.Raw}' is a prerelease but allowPrerelease is false";
                return false;
            }

            var ok = pin.RollForward switch
            {
                "disable" => string.Equals(running.Raw, pinned.Raw, StringComparison.Ordinal),
                "patch" => SameFeatureBand(pinned, running) && running.Patch >= pinned.Patch,
                "latestPatch" => SameFeatureBand(pinned, running) && running.Patch >= pinned.Patch,
                "feature" => SameMajorMinor(pinned, running) && running.FeatureBand >= pinned.FeatureBand,
                "latestFeature" => SameMajorMinor(pinned, running) && running.FeatureBand >= pinned.FeatureBand,
                _ => false,
            };

            if (!ok)
            {
                reason =
                    $"running SDK '{running.Raw}' does not satisfy pin '{pinned.Raw}' with rollForward '{pin.RollForward}'";
                return false;
            }

            reason = string.Empty;
            return true;
        }

        private static bool SameMajorMinor(SdkVersion a, SdkVersion b)
        {
            return a.Major == b.Major && a.Minor == b.Minor;
        }

        private static bool SameFeatureBand(SdkVersion a, SdkVersion b)
        {
            return SameMajorMinor(a, b) && a.FeatureBand == b.FeatureBand;
        }
    }
}
