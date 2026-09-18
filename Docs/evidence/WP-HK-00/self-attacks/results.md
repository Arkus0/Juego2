# WP-HK-00 causal self-attacks

| Attack | Intended oracle/check | Result |
|---|---|---|
| `extra-project-anywhere` | `HK00-PROJECT-UNEXPECTED` | RED on injection → pristine reconstruction → GREEN |
| `missing-fixed-project` | `HK00-PROJECT-MISSING` | RED on injection → pristine reconstruction → GREEN |
| `unowned-source-anywhere` | `HK00-SOURCE-UNOWNED` | RED on injection → pristine reconstruction → GREEN |
| `fixed-policy-relaxed-in-build` | `HK00-PROJECT-PROPERTY` | RED on injection → pristine reconstruction → GREEN |
| `dependency-cycle-backedge` | `HK00-GRAPH-UNDECLARED-EDGE` | RED on injection → pristine reconstruction → GREEN |
| `required-edge-made-decorative` | `HK00-ASSEMBLY-REF-MISSING` | RED on injection → pristine reconstruction → GREEN |
| `cross-project-source-compile` | `HK00-SOURCE-FOREIGN-COMPILE` | RED on injection → pristine reconstruction → GREEN |
| `production-package-injected` | `HK00-PACKAGE-FORBIDDEN` | RED on injection → pristine reconstruction → GREEN |
| `engine-raw-reference` | `HK00-ENGINE-REFERENCE` | RED on injection → pristine reconstruction → GREEN |
| `custom-analyzer-source-generator` | `HK00-ANALYZER-UNTRUSTED` | RED on injection → pristine reconstruction → GREEN |
| `late-target-analyzer-source-generator` | `HK00-COMPILER-ANALYZER-UNTRUSTED` | RED on injection → pristine reconstruction → GREEN |
| `explicit-custom-build-import` | `HK00-CUSTOM-IMPORT` | RED on injection → pristine reconstruction → GREEN |
| `source-dropped-at-build-time` | `HK00-SOURCE-NOT-COMPILED-EFFECTIVE` | RED on injection → pristine reconstruction → GREEN |
| `generated-product-source-injected` | `HK00-COMPILER-SOURCE-UNTRACKED` | RED on injection → pristine reconstruction → GREEN |
| `external-source-injected-at-build-time` | `HK00-COMPILER-SOURCE-FOREIGN` | RED on injection → pristine reconstruction → GREEN |
| `tracked-source-mutated-during-build` | `HK00-BUILD-MUTATED-TRACKED` | RED on injection → pristine reconstruction → GREEN |
| `real-warning-is-error` | `CS0219` | RED on injection → pristine reconstruction → GREEN |
| `toolchain-pin-relaxed` | `HK00-TOOLCHAIN-PIN` | RED on injection → pristine reconstruction → GREEN |
| `proof-project-deleted` | `HK00-TRACKED-MISSING` | RED on injection → pristine reconstruction → GREEN |
| `legacy-manifest-cannot-shrink-universe` | `HK00-PROJECT-UNEXPECTED` | RED on injection → pristine reconstruction → GREEN |
| `solution-omits-fixed-project` | `HK00-SOLUTION-MISSING-PROJECT` | RED on injection → pristine reconstruction → GREEN |
| `auto-directory-build-target` | `HK00-MSBUILD-IMPORT-UNTRUSTED` | RED on injection → pristine reconstruction → GREEN |
| `terminal-nul-inventory-entry` | `HK00-PROJECT-UNEXPECTED` | RED on terminal NUL-delimited entry → pristine reconstruction → GREEN |
