using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using Arkus.H1.UnityHost;
using Arkus.Harness.Projection;
using Arkus.Harness.Protocol;

namespace Arkus.Harness.Cli
{
    public static class H1ReferenceTransportHost
    {
        internal static int Run(Stream input, Stream output, TextWriter diagnostics)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (output == null) throw new ArgumentNullException(nameof(output));
            if (diagnostics == null) throw new ArgumentNullException(nameof(diagnostics));

            using (var projection = ProductionH1ProjectCheckpointHost.Create())
            {
                return RunWithProjection(input, output, diagnostics, projection);
            }
        }

        // Projection injection is a transport seam, not an authority selector: the production entry point
        // above still constructs the fixed H1 production projection. Tests can drive this exact JSONL framing
        // over an independently composed neutral projection without adding a product CLI switch.
        public static int RunWithProjection(Stream input, Stream output, TextWriter diagnostics, NeutralProjectionService projection)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (output == null) throw new ArgumentNullException(nameof(output));
            if (diagnostics == null) throw new ArgumentNullException(nameof(diagnostics));
            if (projection == null) throw new ArgumentNullException(nameof(projection));

            using (var cancellation = new CancellationTokenSource())
            {
                ConsoleCancelEventHandler handler = (sender, eventArgs) =>
                {
                    eventArgs.Cancel = true;
                    cancellation.Cancel();
                };

                Console.CancelKeyPress += handler;
                try
                {
                    var reader = new BoundedJsonLineReader(input, ReferenceTransportHost.MaximumFrameBytes);
                    while (!cancellation.IsCancellationRequested)
                    {
                        var frame = reader.Read();
                        if (frame.Kind == FrameReadKind.EndOfStream) return ReferenceTransportHost.SuccessExitCode;
                        WriteResponse(output, Execute(frame, projection, cancellation.Token));
                    }
                    return ReferenceTransportHost.SuccessExitCode;
                }
                finally
                {
                    Console.CancelKeyPress -= handler;
                }
            }
        }

        private static IReadOnlyDictionary<string, object?> Execute(FrameReadResult frame, NeutralProjectionService projection, CancellationToken cancellationToken)
        {
            if (frame.Kind == FrameReadKind.Oversized)
                return TransportFailure(null, null, "transport.frame_too_large", "The JSON Lines frame exceeded the reference transport framing bound.", "$", false, "Send one frame within the fixed reference transport bound.");
            if (frame.Kind == FrameReadKind.InvalidUtf8)
                return TransportFailure(null, null, "transport.invalid_utf8", "The frame is not valid strict UTF-8.", "$", false, "Encode the complete request frame as strict UTF-8.");

            if (!ReferenceFrameCodec.TryParseRequest(frame.Text ?? string.Empty, out var request, out var requestId, out var capability, out var parseError))
                return TransportFailure(requestId, capability, parseError!.MachineCode, parseError.Message, parseError.Path, parseError.Retryable, parseError.RepairHint ?? "Repair the request frame and retry.");

            var outcome = projection.InvokeAsync(request!, cancellationToken).GetAwaiter().GetResult();
            return WrapResponse(outcome.ToData());
        }

        private static IReadOnlyDictionary<string, object?> TransportFailure(string? requestId, string? capability, string code, string message, string path, bool retryable, string repairHint)
        {
            return WrapResponse(new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["projectionVersion"] = NeutralProjectionRequest.ProjectionVersion,
                ["requestId"] = requestId,
                ["capability"] = capability,
                ["status"] = "error",
                ["failureKind"] = "transport",
                ["error"] = new StructuredError(code, message, path, new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(StringComparer.Ordinal)), retryable, repairHint).ToData()
            }));
        }

        private static IReadOnlyDictionary<string, object?> WrapResponse(IReadOnlyDictionary<string, object?> response)
        {
            return new ReadOnlyDictionary<string, object?>(new Dictionary<string, object?>(StringComparer.Ordinal)
            {
                ["protocol"] = ReferenceTransportHost.ProtocolVersion,
                ["response"] = response
            });
        }

        private static void WriteResponse(Stream output, IReadOnlyDictionary<string, object?> response)
        {
            var bytes = DeterministicJson.Serialize(response);
            output.Write(bytes, 0, bytes.Length);
            output.WriteByte((byte)'\n');
            output.Flush();
        }
    }
}
