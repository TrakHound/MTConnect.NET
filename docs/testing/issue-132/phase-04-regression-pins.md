# Phase 4 — Regression pins

## Surface

- `tests/MTConnect.NET-Common-Tests/Agents/AssetCountFactoryDataSetGuardTests.cs`.

## Why this lives in `MTConnect.NET-Common-Tests`

The original plan called for
`tests/Compliance/MTConnect-Compliance-Tests/L5_Regressions/Issue132_AssetCountDataSetTests.cs`.
That project does not exist on `upstream/master` (the compliance-tests
scaffolding lives on bootstrap, which has not landed). Per
CONVENTIONS §17.3 / §18.4 — STOP and report rather than silently
scaffold the bootstrap deliverable — the regression pin is delivered
inside the existing `MTConnect.NET-Common-Tests` project. When the
compliance-tests project lands upstream, the pin can be moved without
loss of coverage; the current location keeps the assertions live in the
meantime.

## Surface coverage

| Entry point | Test |
|-------------|------|
| `AddDevice(IDevice, bool)` — parameterless `MTConnectAgent` ctor | `AddDevice_AutoInjects_AssetCount_With_DataSet_Representation` (parameterised over three device IDs) |
| `AddDevice(IDevice, bool)` — `MTConnectAgent(IAgentConfiguration, …)` ctor | `AddDevice_With_Configuration_AutoInjects_AssetCount_With_DataSet_Representation` |
| `AddDevices(IEnumerable<IDevice>, bool)` | `AddDevices_AutoInjects_AssetCount_With_DataSet_Representation_For_Every_Device` |

All three entry points funnel through the private `NormalizeDevice`,
which is exactly where the override lives — a future refactor that
splits the auto-injection out into a separate helper would still be
guarded by these three tests.

## Out-of-scope follow-up

The plan's "tests-plan migration" (`plans/11-tests/11-compliance-regression-gates.md`)
edit references an `extra-files.user/` artifact that is not part of the
public repo. No public-tree edit is needed.

## Test outcome

```text
Passed AutoInjected_AssetCount_Has_DataSet_Representation [137 ms]
Passed UserDeclared_AssetCount_Representation_Is_Preserved [117 ms]
Passed AddDevice_AutoInjects_AssetCount_With_DataSet_Representation("lathe-1") [3 ms]
Passed AddDevice_AutoInjects_AssetCount_With_DataSet_Representation("mill-7") [2 ms]
Passed AddDevice_AutoInjects_AssetCount_With_DataSet_Representation("robot-A") [19 ms]
Passed AddDevice_With_Configuration_AutoInjects_AssetCount_With_DataSet_Representation [4 ms]
Passed AddDevices_AutoInjects_AssetCount_With_DataSet_Representation_For_Every_Device [8 ms]
Passed Test1 [52 ms]

Total tests: 8
     Passed: 8
```

## DoD

Regression pin live across every accessible auto-injection code path;
deviation from plan-declared compliance-tests location explicitly
documented.
