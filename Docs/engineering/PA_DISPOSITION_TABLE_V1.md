# PA canonical disposition table v1

Status: ACTIVE PROCESS CONVENTION FOR `WP-PA-06+`
Authority: checker-owned source-shape convention; never capsule-owned.

Future accepted PA result artifacts from `WP-PA-06` onward expose exactly one section:

```markdown
## Canonical disposition table

| Disposition ID | Status | Evidence / rationale |
|---|---|---|
| ... | ... | ... |
```

Rules:

- column 0 is the stable disposition key and column 1 is the complete accepted status;
- every authoritative disposition row for that PA workpack appears exactly once in this table;
- capsules copy the table's key/status pairs but cannot choose another section, column or subset;
- omission, duplicate-key, unknown-row and status-mismatch controls remain fail-closed in the CTX-02 capsule checker;
- PA-01..05 keep their frozen reviewed selectors; they are not rewritten to this format;
- changing this heading or column contract is a reviewed process change, not something a PA result/capsule may redefine locally.

The purpose is to stop editing checker code for every new PA workpack while retaining an oracle that is owned outside the artifact being checked.
