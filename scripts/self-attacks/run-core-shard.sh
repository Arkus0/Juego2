#!/usr/bin/env bash
set -euo pipefail

ROOT="$(cd "$(dirname "${BASH_SOURCE[0]}")/../.." && pwd)"
SOURCE="${ROOT}/scripts/self-attacks/run-self-attacks.sh"
GROUP="${1:-}"

case "${GROUP}" in
  fast|effective-a|effective-b|effective-c) ;;
  *) echo "CORE SHARD FAILURE: unknown group '${GROUP}'" >&2; exit 2 ;;
esac

# Reuse the canonical attack functions without executing its monolithic main.
# eval preserves this wrapper's BASH_SOURCE, which is in the same directory as
# the canonical script, so its ROOT calculation remains the repository root.
eval "$(sed '/^main "\$@"$/d' "${SOURCE}")"

rm -rf "${WORK}"
mkdir -p "${WORK}" "${LOGDIR}" "$(dirname "${RESULTS}")"
dotnet restore "${TOOL_PROJECT}" >/dev/null
dotnet build "${TOOL_PROJECT}" -c "${CONFIGURATION}" --no-restore -v minimal >/dev/null
[[ -f "${TOOL_DLL}" ]] || fail "proof DLL missing"

copy_clean_tree "${ROOT}" "${PRISTINE}"
cat >"${RESULTS}" <<'MD'
# WP-HK-00 causal self-attacks

| Attack | Intended oracle/check | Result |
|---|---|---|
MD

case "${GROUP}" in
  fast)
    fresh; expect_green repository "${LOGDIR}/baseline-repository.log"
    fresh; expect_green static "${LOGDIR}/baseline-static.log"

    run_attack extra-project-anywhere repository HK00-PROJECT-UNEXPECTED m_extra_project
    run_attack missing-fixed-project repository HK00-PROJECT-MISSING m_missing_project
    run_attack unowned-source-anywhere repository HK00-SOURCE-UNOWNED m_unowned_source
    run_attack fixed-policy-relaxed-in-build static HK00-PROJECT-PROPERTY m_relax_policy
    run_attack dependency-cycle-backedge static HK00-GRAPH-UNDECLARED-EDGE m_cycle
    grep -Fq HK00-GRAPH-CYCLE "${LOGDIR}/dependency-cycle-backedge-red.log" || fail "cycle oracle did not also fire"
    run_attack cross-project-source-compile static HK00-SOURCE-FOREIGN-COMPILE m_cross_compile
    run_attack production-package-injected static HK00-PACKAGE-FORBIDDEN m_package
    run_attack engine-raw-reference static HK00-ENGINE-REFERENCE m_engine_ref
    run_attack custom-analyzer-source-generator static HK00-ANALYZER-UNTRUSTED m_custom_analyzer
    run_attack explicit-custom-build-import static HK00-CUSTOM-IMPORT m_custom_import
    warning_attack
    run_attack toolchain-pin-relaxed repository HK00-TOOLCHAIN-PIN m_toolchain
    run_attack proof-project-deleted repository HK00-TRACKED-MISSING m_proof_deleted
    run_attack legacy-manifest-cannot-shrink-universe repository HK00-PROJECT-UNEXPECTED m_legacy_manifest
    ;;
  effective-a)
    fresh; expect_green effective "${LOGDIR}/baseline-effective.log"
    run_attack required-edge-made-decorative effective HK00-ASSEMBLY-REF-MISSING m_unexercised_edge
    run_attack late-target-analyzer-source-generator effective HK00-COMPILER-ANALYZER-UNTRUSTED m_late_analyzer
    ;;
  effective-b)
    fresh; expect_green effective "${LOGDIR}/baseline-effective.log"
    run_attack source-dropped-at-build-time effective HK00-SOURCE-NOT-COMPILED-EFFECTIVE m_drop_effective
    run_attack generated-product-source-injected effective HK00-COMPILER-SOURCE-UNTRACKED m_generated
    ;;
  effective-c)
    fresh; expect_green effective "${LOGDIR}/baseline-effective.log"
    run_attack external-source-injected-at-build-time effective HK00-COMPILER-SOURCE-FOREIGN m_external
    run_attack tracked-source-mutated-during-build effective HK00-BUILD-MUTATED-TRACKED m_mutate_tracked_source
    ;;
esac

echo "core causal self-attack group '${GROUP}' passed"
