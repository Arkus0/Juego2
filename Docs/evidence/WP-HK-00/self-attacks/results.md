# WP-HK-00 causal self-attacks

| Attack | Intended oracle/check | Result |
|---|---|---|
| `extra-project-anywhere` | `HK00-PROJECT-UNEXPECTED` | RED on injection → clean Git revert → GREEN |
| `missing-fixed-project` | `HK00-PROJECT-MISSING` | RED on injection → clean Git revert → GREEN |
| `unowned-source-anywhere` | `HK00-SOURCE-UNOWNED` | RED on injection → clean Git revert → GREEN |
| `fixed-policy-relaxed-in-build` | `HK00-PROJECT-PROPERTY` | RED on injection → clean Git revert → GREEN |
| `dependency-cycle-backedge` | `HK00-GRAPH-UNDECLARED-EDGE` | RED on injection → clean Git revert → GREEN |
| `cross-project-source-compile` | `HK00-SOURCE-FOREIGN-COMPILE` | RED on injection → clean Git revert → GREEN |
| `production-package-injected` | `HK00-PACKAGE-FORBIDDEN` | RED on injection → clean Git revert → GREEN |
| `engine-raw-reference` | `HK00-ENGINE-REFERENCE` | RED on injection → clean Git revert → GREEN |
| `source-dropped-at-build-time` | `HK00-SOURCE-NOT-COMPILED-EFFECTIVE` | RED on injection → clean Git revert → GREEN |
| `generated-product-source-injected` | `HK00-COMPILER-SOURCE-UNTRACKED` | RED on injection → clean Git revert → GREEN |
| `external-source-injected-at-build-time` | `HK00-COMPILER-SOURCE-FOREIGN` | RED on injection → clean Git revert → GREEN |
| `real-warning-is-error` | `CS0219` | RED on injection → clean Git revert → GREEN |
| `toolchain-pin-relaxed` | `HK00-TOOLCHAIN-PIN` | RED on injection → clean Git revert → GREEN |
| `proof-project-deleted` | `HK00-TRACKED-MISSING` | RED on injection → clean Git revert → GREEN |
| `legacy-manifest-cannot-shrink-universe` | `HK00-PROJECT-UNEXPECTED` | RED on injection → clean Git revert → GREEN |
