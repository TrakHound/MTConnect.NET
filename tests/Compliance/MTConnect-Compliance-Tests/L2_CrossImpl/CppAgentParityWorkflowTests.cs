using NUnit.Framework;

namespace MTConnect.Compliance.L2_CrossImpl
{
    // Workflow W07 — cppagent JSON v2 parity.
    //
    // Spins docker-mtconnect/agent:<version> and the in-process MT.NET
    // agent against the same XML fixture, requests the same endpoint
    // from both, and diffs the JSON responses modulo a runtime-only
    // whitelist (instanceId, timestamps, etc.).
    //
    // Source authority:
    //   - cppagent: github.com/mtconnect/cppagent — the reference
    //     implementation.
    //   - JSON v2: docs.mtconnect.org "Part 1.0 Annex C" (informative
    //     JSON wire format).
    //
    // [Explicit] gates this test out of the default sweep until the
    // campaign's Docker harness lands; the full implementation is
    // tracked alongside the L4 layer in the Compliance plan.
    [TestFixture]
    [Category("E2E")]
    [Category("RequiresDocker")]
    public class CppAgentParityWorkflowTests
    {
        [Test]
        [Explicit("cppagent parity E2E requires docker-spun mtconnect/agent + the cross-impl whitelist file; will be wired in once the cross-impl harness lands.")]
        public void Probe_envelope_byte_diff_is_empty_modulo_whitelist()
        {
            // 1. Pull mtconnect/agent:<pinned-tag> via Testcontainers.
            // 2. Volume-mount the shared XML fixture into the container.
            // 3. Spin both agents, hit /probe on each.
            // 4. Normalise both responses (sort attrs, strip runtime-only
            //    fields per Fixtures/cross-impl-whitelist.json).
            // 5. Assert byte-for-byte equality of the normalised payloads.
        }

        [Test]
        [Explicit("cppagent parity E2E requires docker-spun mtconnect/agent + the cross-impl whitelist file; will be wired in once the cross-impl harness lands.")]
        public void Current_envelope_byte_diff_is_empty_modulo_whitelist()
        {
            // 1. Pull mtconnect/agent:<pinned-tag> via Testcontainers.
            // 2. Volume-mount the shared XML fixture into the container.
            // 3. Spin both agents, hit /current on each.
            // 4. Normalise both responses (sort attrs, strip runtime-only
            //    fields per Fixtures/cross-impl-whitelist.json).
            // 5. Assert byte-for-byte equality of the normalised payloads.
        }

        [Test]
        [Explicit("cppagent parity E2E requires docker-spun mtconnect/agent + the cross-impl whitelist file; will be wired in once the cross-impl harness lands.")]
        public void Sample_envelope_byte_diff_is_empty_modulo_whitelist()
        {
            // 1. Pull mtconnect/agent:<pinned-tag> via Testcontainers.
            // 2. Volume-mount the shared XML fixture into the container.
            // 3. Spin both agents, hit /sample on each.
            // 4. Normalise both responses (sort attrs, strip runtime-only
            //    fields per Fixtures/cross-impl-whitelist.json).
            // 5. Assert byte-for-byte equality of the normalised payloads.
        }
    }
}
