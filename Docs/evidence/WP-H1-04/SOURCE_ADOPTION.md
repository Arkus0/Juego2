# WP-H1-04 Quaternius Source adoption

Adoption boundary: 2026-09-24, before the first H1-04 game-representative Unity import. The owner supplied `C:\Juego2-Assets` as the external, read-only source root. It is neither repository state nor a canonical Arkus authority. The exact distributions below are identified by hashes rather than relying on an unverified vendor version string.

| Source distribution and pin | Origin and license evidence | Selected H1-04 slice |
|---|---|---|
| Quaternius Medieval Village MegaKit **Source**, Unity URP archive `Medieval Village/Engine Projects/Medieval Village MegaKit[Unity URP].zip`, SHA-256 `b9d757dd2608a5cee4d9ee1e8183f6cb4cad9d27480841a905180def9c7d8b10` | [Official pack](https://quaternius.com/packs/medievalvillagemegakit.html); bundled `Medieval Village/License_Source.txt`, SHA-256 `7310dfa8512d7ca12b6591329fda4adab7e8b9cdbe04c7d7061c2fe5c7dc38ee`. Both state CC0 1.0 and commercial use. | `Materials/MI_Plaster.mat` (SHA-256 `3fcfc0359d1460009858533461893c626e25ea52364ae4e85f4a6bf84fc3ce69`) and its `.meta` (SHA-256 `1d4843119c9f80b78a8e4466bf5041e531d0c8072a72f1770e4d013070e9b042`). Its URP Shader Graph and texture references are not adopted: an unresolved material is reported as incompatible, never silently usable. |
| Same Medieval Village MegaKit Source distribution, `Medieval Village/FBX (Unity)/Wall_Plaster_Window_Wide_Flat.fbx`, SHA-256 `45825c565b9d1027036ce7fc922f7e7a7d69bc05e459d886eb738bf1eafc92b8` | Same bundled Source license and official pack page. | One Quaternius facade model and its Unity-imported model prefab/mesh. The local import assigns an Arkus-owned project `.meta` GUID; the source FBX bytes are unmodified. |
| Quaternius Universal Animation Library **Source**, `Animation/Unity/UAL1.fbx`, SHA-256 `0556d52f6bce01c0982b3548ee3cdfa1b8270977507001f62cbdfcc405570842` | [Official pack](https://quaternius.com/packs/universalanimationlibrary.html); bundled `Animation/License.txt`, SHA-256 `6d01f55c6e4c49a2c9963e147e561945ae2c83958c8ca667d90a6bffdbfac061`. Both state CC0 1.0 and commercial use. | Imported civilian animation clip shape. This is catalogue proof, not an animation-production or retargeting claim. |

Distribution mode: private owner-supplied local Source inputs, copied into ignored `Unity/ArkusUnity/Assets/Arkus/H1/SourceSlice/` by `scripts/h1-04-import-source.ps1`; no Source archive or selected asset bytes are committed. The script verifies the entire pinned distributions and selected bytes before copying. Local copied bytes are disposable; upstream files are never written. Exact notices remain in the external distributions. CC0 imposes no attribution requirement, but the source identity is retained here and should flow into later third-party notices/SBOM records.

`SOURCE_ADOPTION.json` is the machine-readable admission record. The pinned Unity model importer also derives one compatible Standard material from the facade FBX; `H1CatalogueInventory.CreateProofScene` copies that imported material to the reviewed top-level slice as `FacadeImportedMaterial.mat` with a stable project GUID. Its resulting content SHA-256 is `e585297c2271fc83378e2eebe7f72ff9f8b66c2fc1b8a1eeb1c7abdfac5d2e29`. This is a source-derived import artifact, not newly authored substitute art. The original `MI_Plaster.mat` stays present and truthfully reports incompatibility because this H1-02 project does not adopt the Source archive's URP Shader Graph dependency.

The Source provides Unity integration inputs only. Arkus owns logical catalogue IDs, schema, snapshot, reference resolution and canonical-world semantics. No Quaternius file defines an Arkus capability or changes `WorldState`. A replacement Source distribution must receive a new reviewed adoption record and mapping; it cannot silently reuse this identity. Security/update owner: Juego2 asset owner. Classification: authoring-time external game art input, not a runtime/tooling code dependency. The repository-owned harness scene and component schema probes are explicitly synthetic proof objects, not replacement game art.

## WP-H1-11 representative extension (same adopted distribution)

`WP-H1-11` admitted twelve further items as an explicitly reviewed extension of this record. They are entries of the already pinned Medieval Village MegaKit Source URP archive (SHA-256 `b9d757dd…c8b10`, stored unmodified in the private vault) and are mounted byte-for-byte from that hash-verified payload together with their original upstream Unity `.meta` sidecars. No new source, version, licence or distribution is adopted; the CC0 identity above applies unchanged. The binding of every item to its archive entry path, sidecar bytes, logical IDs and source-derived import facts is `Docs/evidence/WP-H1-11/REPRESENTATIVE_SLICE.json`.

| Item | Shape class | Content SHA-256 | Upstream Unity GUID |
| --- | --- | --- | --- |
| `Corner_Exterior_Wood.fbx` | corner-module | `9808a8a25a67446f5d4d76836eb62d843414d1a3ed0979bb08bbe9f9e544ffee` | `c9549b3b14121b84baccac5394379647` |
| `Wall_Plaster_Straight.fbx` | wall-module | `ad7d807b28ce4ec564773c9a9637c47b91db53cd9e7e6a8c90de5b1a56d3c96a` | `e9c7f17150cfb7741abda8572d7013f3` |
| `Wall_Plaster_Door_Flat.fbx` | wall-module-door-opening | `d373cff1fc4bd0d3369e1abc792f68e2500562744f39c237ebb184dd49533c90` | `7ffce3dada5ffee4f940a797b9c78aaf` |
| `Door_1_Flat.fbx` | door | `c8a1f7bfc8cff15356ab80aa3deee0b42597c0033c44f04807719c2ea8a77753` | `b008cd8053424984380bebca9426d8ae` |
| `Window_Wide_Flat1.fbx` | window | `da526c2d7f89f001da31626acdc97af4a57491579776e9073a515a64e78827c4` | `16396acda011f6c42b03303d8f57263c` |
| `WindowShutters_Wide_Flat_Open.fbx` | window-shutter | `3b835fc6a896793ab546f9a5602877b5dce78db3ab084a0945f17209983f2a70` | `8b575e2679b732c409d5c7741eb57d62` |
| `Roof_RoundTiles_4x4.fbx` | roof | `051f77ad0ccd0167ea3c65f6b3e8a279cfdb9d2fa17e6f911f228fe3260f7a50` | `05480a035831dec43bbcf445f9f8da56` |
| `Prop_Chimney.fbx` | roof-prop | `d132c893fbf957c92c51697de1329f0603d185849a7d0e94ec9ce5a78a0b90ed` | `f737f5d1d3f1c7c4f9b41c4fc741eb81` |
| `Prop_Crate.fbx` | small-prop | `8e38d27f360ef68fe9f0e3382917d5ae500d78de3cace53e07a2f6d21c94cd43` | `c8380c71919f1694b9e99f69d20dd066` |
| `Prop_WoodenFence_Single.fbx` | small-prop | `f5e0e0210292ab5dfd95a10f684850406b26d0a016b3fbd3ba499542e644d440` | `1bf65997b403411488eef5d3cf692dc3` |
| `Prop_Wagon.fbx` | large-prop | `977e9344cce589a9625a7a7340da42f2ed9783a628b257dfdbfe0e892cd36847` | `612d31ec60a92474d8c28be827233dfe` |
| `Prop_Vine1.fbx` | foliage-prop | `de0bea8119f7b12fac3795718b331658f9bbe6e9e38a02272a1f92e28a4edeca` | `9ded629f09f94a2449f219d511d3d9f1` |
