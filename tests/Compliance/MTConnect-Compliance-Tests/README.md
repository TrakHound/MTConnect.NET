# MTConnect Compliance Harness

Layered test tree that exercises every MTConnect Standard version the library advertises.

Layout:

- `L1_XsdValidation/` — every library-emitted envelope validates against the matching-version XSD.
- `L2_XmiOclAssertions/` — version-specific OCL rules from `mtconnect/mtconnect_sysml_model` run against library output.
- `L3_*` — reserved for envelope-shape conformance tests (live spec-shape assertions per version); not yet scaffolded.
- `L4_CrossImpl/` — cppagent parity. Docker-gated (`[Category("RequiresDocker")]`, `MTCONNECT_E2E_DOCKER=true`).
- `L5_Regressions/` — pins that keep prior-version behaviour from silently regressing.
- `Schemas/` — XSD tree, one subdir per version (`v2_6/`, `v2_7/`, …). Schemas copy to test output at build time.

Per-version compliance matrices live under `docs/testing/v2-N.md`. Each row names the exact pinned test that validates that row. A new parser / generator symbol without a corresponding row trips CI.
