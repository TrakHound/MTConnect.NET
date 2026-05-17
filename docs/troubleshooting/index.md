# Troubleshooting

Common errors when running `MTConnect.NET` and the fixes that work. Each entry follows the same shape: **symptom** (what you see in the logs or on the wire), **cause**, **fix**, and the **prevention** that keeps it from coming back.

## Categories

- **XSD validation failures** — the .NET BCL ships an XSD-1.0 validator, but several MTConnect XSDs use XSD-1.1 assertions. Symptoms include validation errors on documents that pass against the cppagent reference. Fix: route validation through the agent's structural validator (the per-class checks in `MTConnect.NET-Common`) and treat the XSD validator as advisory for the 1.1-only constructs.
- **Schema-version mismatches between agent and consumer** — a consumer requesting `version=2.4` from an agent built against v2.7 receives a v2.4-shaped envelope, but a consumer pinned to a fixed namespace URL sees a mismatch. Fix: have the consumer follow the namespace URL the agent emits in the response root rather than hard-coding it.
- **MQTT TLS handshake failures** — usually a certificate-chain problem when running against AWS IoT, HiveMQ Cloud, or any broker that requires mutual TLS. Fix: regenerate the certificate chain with OpenSSL (see the [OpenSSL guide](/configure/integrations/openssl)), confirm the agent's TLS configuration block points at the right `.pfx` or `.pem` paths, and verify the broker's CA bundle is in the agent's trust store.
- **Empty `name` attributes on probe DataItems** — caused by a `Devices.xml` that omits the `name` attribute on a DataItem and relies on the agent to synthesize one. Fix: add an explicit `name` attribute to every DataItem in `Devices.xml`; the spec does not require it on every DataItem but consumers often do.
- **Asset count emitted as a scalar EVENT instead of a DATA_SET** — happens on agents built against very old spec versions where `AssetCountDataSet` did not yet exist. Fix: bump the target spec version in the agent config to v1.7 or later and re-emit the device.
- **`Devices.xml` validation surprises after a v2.x bump** — new spec versions add new controlled-vocabulary values and tighten existing ones. Fix: validate `Devices.xml` against the per-version XSDs the agent ships under `schemas/` and resolve each error against the spec's release notes for that version.
- **Pallet measurement constructors not present** — the rich-template Pallet measurement types are introduced in newer spec versions. Fix: confirm the library version supports the spec version you target, and consult the API page for `PalletMeasurement` (and its specializations) for the introduced-in badge.
- **HTTP server port already in use** — the default port `5000` is shared with several other .NET application templates. Fix: change `http-server.port` in `agent.config.yaml`, or stop the conflicting process.
- **Devices not appearing in `/probe`** — the most common cause is a `Devices.xml` parse error that the agent logs but does not crash on. Fix: check the agent log for the parse error, fix the malformed XML, restart.
- **Buffer-overflow log messages under load** — the agent's sequence buffer has a configurable size and drops the oldest observations when full. Fix: increase `buffer.maximumSize` in `agent.config.yaml`, or process the buffer faster via more responsive consumers.

## Reading the agent logs

The agent writes to `stdout` by default and to a configurable log file when running as a service. Log levels: `Debug`, `Info`, `Warning`, `Error`, `Fatal`. Most fixable issues surface at `Warning` and `Error`. To capture the most diagnostic information, set the global log level to `Debug` temporarily — the volume is high, so do not leave it on in production.

## When the fix is not here

- Check the [Cookbook](/cookbook/) for the recipe that matches your deployment pattern — the recipe may already work around the issue.
- Open an issue at <https://github.com/TrakHound/MTConnect.NET/issues> with the agent version, the relevant log lines, and a minimal `agent.config.yaml` + `Devices.xml` that reproduces the symptom.

## See also

- [Configure & Use](/configure/) — the per-knob reference for every setting troubleshooting touches.
- [Compliance](/compliance/) — when the agent and the cppagent reference disagree, the compliance section names which one `MTConnect.NET` follows and why.
