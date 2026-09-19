# WP-ART-00 — Asset triage dry-run

Date: 2026-09-19  
Bible: `Docs/art/VISUAL_BIBLE.md`  
Branch: `claude/visual-bible-quaternius-cantabria-xzacqe`  
Author pass: Grok (connector)  
Second pass: recorded at owner request on the same day; owner may dissent on PR review by editing this file

## Pack verification notes (2026-09-19)

Checked against https://quaternius.com/ index and pack pages:

| Pack | Status | Note |
|------|--------|------|
| Medieval Village MegaKit | URL-CONFIRMED | Pack page exists; modular village kit |
| Ultimate Modular Men / Women | URL-CONFIRMED | Listed on site index |
| Universal Base Characters | URL-CONFIRMED | Listed on site index |
| Downtown City MegaKit | URL-CONFIRMED | Tags include NYC/Boston — whole blocks remain forbidden |
| Modular Streets Pack | URL-CONFIRMED | Listed on site index |
| Ultimate Nature / Ultimate Stylized Nature | URL-CONFIRMED | Tags include palm — vegetation veto stands |
| Survival Pack | URL-CONFIRMED | Props OK; weapons/camping remain forbidden |
| **Ships Pack** | **URL-CONFIRMED** | Was HYPOTHESIS; index lists ships/boats/viking/raft/sailboat/cruise |
| Modular Character Outfits – Fantasy | REJECTED | Confirmed present as fantasy outfit pack |

License observation (site): Quaternius Asset License (QAL) — commercial use, no credit required, no redistribution as asset pack.  
Adoption status remains **UNVERIFIED-FOR-ADOPTION** until a human records pack identity, download date/version, license line, URL, and classification per `Docs/engineering/DEPENDENCY_IP_POLICY.md`.

## Palette

Status: **APPROVED-DRAFT**  
Hex table in `VISUAL_BIBLE.md` §3 accepted for triage and H2 seed work. Fine-tune only if a flat color-block sheet shows a clear problem.

## Candidates (bible §9)

| ID | Asset (descriptive) | Pack | Author verdict | Second verdict | Notes |
|----|---------------------|------|----------------|----------------|-------|
| T1 | Stone wall module 2–4 m (no fantasy roof) | Medieval Village MegaKit | RECOLOR-THEN-ACCEPT | RECOLOR-THEN-ACCEPT | Recolor to mid stone `#A79F92` + quoins `#7E7568`; darken damp lower ~0.5 m |
| T2 | Thatched roof piece | Medieval Village MegaKit | REJECT | REJECT | Pack veto + No-list (thatch) |
| T3 | Full downtown / brownstone block | Downtown City MegaKit | REJECT | REJECT | Pack authorizes loose props only; whole blocks forbidden |
| T4 | Bench + street lamp (loose props, village scale) | Downtown City MegaKit or Modular Streets | RECOLOR-THEN-ACCEPT | RECOLOR-THEN-ACCEPT | Reject US-coded props (hydrant, mailbox) or city-scale pieces |
| T5 | Palm tree | Ultimate Nature / Ultimate Stylized Nature | REJECT | REJECT | Non-Atlantic vegetation; No-list palms |

**Reject count:** 3 (requirement: ≥ 2).  
**Mismatches:** none between author and second column.

## Open-question recommendations (for bible §11)

1. Boats pack name: **Ships Pack** (URL-CONFIRMED). Use only small working craft (raft / small sailboat); veto viking/cruise/large vessels.
2. Quay in H2 hero target: **defer** (keeps water + extra material set out of ≤6 atlas budget).
3. Roof recolor: prefer **shared atlas edit** first; vertex color only for per-instance exceptions.
4. Source tiers: **free tier** through hero target unless a specific mesh forces Blender edit.
5. Palette: **APPROVED-DRAFT** (this document).

## Result

Dry-run procedure executed. Bible selectivity is not vacuous (three hard rejects). Ready for owner PR review / merge of the ART-00 seed.
