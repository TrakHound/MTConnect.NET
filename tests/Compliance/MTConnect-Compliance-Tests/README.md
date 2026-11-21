# MTConnect Compliance Harness

Layered test tree that exercises every MTConnect Standard version the library advertises.

Layout:

- `L1_XsdValidation/` — the XSDs load, and library-emitted XML validates against the matching-version XSD.
- `L2_CrossImpl/` — cppagent parity and envelope-shape tests. The parity fixtures need Docker (`[Category("RequiresDocker")]`): they run when Docker is reachable and are skipped when it is not. Setting `MTCONNECT_E2E_DOCKER` to `true`, `yes`, `on` or `1` turns that skip into a failure.
- `Fixtures/` — input files for the cross-implementation tests. They copy to the test output at build time.
- `Schemas/` — XSD tree, one subdir per version (`v1_0/` to `v1_8/` and `v2_0/` to `v2_7/`), plus `w3c/` for the W3C `xlink.xsd` and `xml.xsd`. The XSDs are embedded in the test assembly as resources.

Per-version compliance matrices live under `contributing/testing/v2-N.md`. Each row names the exact pinned test that validates that row. A new parser / generator symbol without a corresponding row trips CI.
