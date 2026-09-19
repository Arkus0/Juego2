# WP-HK-02A residual risk / trust boundary

## Trust boundary

HK02A proves object-scoped opaque extension identity and declared typed dependency semantics for the finite in-memory canonical `WorldState`, its V2 codec/hash, the accepted HK03 extension query/read routes and the accepted HK04 generic extension operations/change oracle.

Inside the claim:

- normal documented C#/.NET value, collection and UTF-8/Base64 behavior;
- every canonical object, extension, subject and declared dependency in one world aggregate;
- composite extension equality/order/resource identity;
- canonical V2 serialization, hash and round trip;
- current public extension inspection and reconstruction;
- current transactional put/remove-extension planning, validation and change effects.

Inherited and consumed rather than re-proved: HK01 canonical route composition/discovery, HK02 stable object identity, HK03 read-only envelope and HK04 atomic commit/CAS/idempotency/commit-authority guarantees.

Outside the claim/default trusted base: arbitrary compiler/runtime/filesystem corruption, private-reflection/toolchain subversion and unsupported non-canonical execution paths.

## Accepted residuals

### Opaque payload declaration honesty

Arkus validates every declared dependency but does not parse arbitrary payload bytes. A producer that embeds an object identity in opaque bytes and violates the contract by not declaring it cannot be detected generically. Payload-specific schema interpretation is explicitly forbidden in HK02A; future typed extension packages may provide stronger content-specific validation without changing this base identity/dependency surface.

### Whole-world concurrency

Object scoping improves resource identity, inspection, change sets and future rebase reasoning. HK04 still uses whole-world revision/hash CAS. HK02A does not claim per-resource concurrency or automatic merge of disjoint edits.

### Format V1 migration

The canonical format advances explicitly to V2 and fails closed on other schema versions. This early repository has no accepted persisted-world migration requirement. A future import/migration path must be separately versioned and reviewed; it may not silently reinterpret V1 bytes as V2.

### Total-size budgets

HK03 bounds each extension payload chunk and dependency page, while HK04 retains its accepted operation-count bound. Total payload bytes, dependency count and world size are not newly capped here. Later ergonomics/resource-limit work owns measured budgets; this does not weaken present determinism or referential validation.

### Content-specific meaning

Dependency `kind` is a typed stable token at the base layer, not a registry of gameplay relationships. Schedules, transforms, behavior/dialogue, asset bindings, simulation time and world partitioning remain outside HK02A as classified by `CONTENT_SHAPE_PROBE.md`.

No accepted residual can falsify the central claim inside the stated boundary.
