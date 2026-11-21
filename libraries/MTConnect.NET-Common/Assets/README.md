# Assets
#### Handles MTConnectAssets Response Documents described in **Part 4.0 of the MTConnect Standard**

## Contents
- [IAsset](IAsset.cs) / [Asset](Asset.cs) : The interface and base class for all Assets (`AssetId`, `DeviceUuid`, `Removed`, etc.)
- [IAssetsResponseDocument](IAssetsResponseDocument.cs) / [AssetsResponseDocument](AssetsResponseDocument.cs) : The MTConnectAssets Response Document
- Asset types, each in its own directory:
    - [CuttingTools](CuttingTools) : `CuttingToolAsset`
    - [Files](Files) : `FileAsset`
    - [RawMaterials](RawMaterials) : `RawMaterialAsset`
    - [QIF](QIF) : `QIFDocumentWrapperAsset`
    - [Fixture](Fixture) : `FixtureAsset`
    - [Pallet](Pallet) : `PalletAsset`
    - [ComponentConfigurationParameters](ComponentConfigurationParameters) : `ComponentConfigurationParametersAsset`

## Usage
Assets are added to an Agent using the `AddAsset()` method and are read back using the `GetAssetsResponseDocument()` method of the MTConnectAgentBroker class.
```c#
using MTConnect.Agents;
using MTConnect.Assets.CuttingTools;

var agent = new MTConnectAgentBroker();

var cuttingTool = new CuttingToolAsset();
cuttingTool.AssetId = "5.12";
cuttingTool.SerialNumber = "12345678946";
cuttingTool.ToolId = "12";

// The Device (Name or Uuid) must already be added to the Agent
agent.AddAsset("OKUMA-Lathe", cuttingTool);

var assetsDocument = agent.GetAssetsResponseDocument();
```
