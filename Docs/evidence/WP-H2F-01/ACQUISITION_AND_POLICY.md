# Manual acquisition, blockers versus technical results, dependency-policy questions

## Manual/licensed inputs used or pending

| Input | Status | Kind | Where it lives |
|---|---|---|---|
| Quaternius Medieval Village MegaKit Source (URP archive), UAL1 | used; H1-04 pins inherited | owned, CC0 | owner vault |
| Quaternius UAL2 (+ `_RM`) | used in S07; new to H2F. The vault copy carries `Animation2/License.txt`, which is pinned. | owned, CC0 (per bundled license) | owner vault |
| Quaternius Universal Base Characters, Stylized Nature, Fantasy Props | used through ART-01's `SOURCE_LOCK.json` | owned, CC0 | owner vault |
| Game Creator 2 Core 2.19.61 | used in isolated S06 comparison; not selected | **paid Asset Store**, owner-purchased 2026-09-27, sha256 `1e4f3ba0…2f3380b`, no bundled license file (Asset Store EULA) | owner's Asset Store cache; must never be committed |
| Unity Starter Assets ThirdPerson | **not acquired**; S06 comparison not run | free Asset Store, needs the owner's account | — |
| Mixamo, Poly Haven, ambientCG, paid suites | not needed | — | — |

## Blockers versus technical failures (kept separate)

- **Manual-acquisition gap:** Starter Assets was not acquired. It is not a technical failure and not evidence against it. It is deferred on documented grounds (same composition spiked directly).
- **Environment constraint:** batch mode without `-quit` needs a headless entitlement on this license, so play-mode evidence uses a windowed editor.
- **Technical findings, resolved inside the spike:**
  - URP batch converter API broken (a public API path works);
  - junction apron hole (generator fixed);
  - Hips=root auto-mapping (explicit mapping);
  - loop flags;
  - terrain prototype materials and root mesh.
- **Technical findings handed to H2F-02/ART:**
  - ART `Art01Materials` reverts to Standard;
  - decals need rendering layers;
  - Terrain detail meshes did not render;
  - the minimal camera loses the body at the F01 lintel (camera tuning);
  - interior-window palette.

## Dependency-policy questions H2F-02 must close at exact adopted versions

1. The exact UPM versions and lock for URP 17.3.0 (core), Input System, Cinemachine, Splines, AI Navigation and Animation Rigging. Record Unity Companion License / package licenses and notices per `DEPENDENCY_IP_POLICY.md`.
2. Whether the project-owned shaders (water, interior window) and generators (profile/junction, scatter) live in a Juego2 package or the project. Decide their ownership and tests.
3. UAL2 admission record (distribution identity, per-clip allowlist) and the Universal Base Characters derivative lineage, including the avatar mapping as part of the derivative contract.
4. The provisioning route for owner-vault content in clean restoration (inherit the H1 vault pattern).
5. If GC2 Core is admitted (through the owner-proposed WP-H2F-01A, PR #255):
   - EULA seat and redistribution terms;
   - Assets-only import with a manifest diff/merge policy;
   - `physics2d` module;
   - a lint forbidding GC2 Variables/SaveLoad/Remember/Triggers as keeper-state authority;
   - an adapter keeping Arkus free of GC2 types.
6. The `activeInputHandler` transition and removal of legacy `Input` use in ART-01's walk inspector.

## Owner cost constraint

Respected. No purchase was triggered by this WP. GC2 was bought by the owner on their own initiative, and it was evaluated because it was already owned. Every paid H2F-00 candidate remains `DEFER`.
