# Devices
#### Handles MTConnectDevices Response Documents described in **Part 2.0 of the MTConnect Standard**

## Device
The [Device](Device.cs) class is used to implement Device elements described in **Part 2.0 : Section 4.2 of the MTConnect Standard**

## Component
The [Component](Component.cs) class is used to implement Component elements described in **Part 2.0 : Section 5 of the MTConnect Standard**

Component Types are defined in the [Components](Components) directory. Each class represents a Component of the specified Type. The class fixes the Type string and exposes a `NameId` constant the caller can copy from when setting `Name` explicitly.

## Composition
The [Composition](Composition.cs) class is used to implement Composition elements described in **Part 2.0 : Section 6 of the MTConnect Standard**

Composition Types are defined in the [Compositions](Compositions) directory. Each class represents a Composition of the specified Type. The class fixes the Type string and exposes a `NameId` constant the caller can copy from when setting `Name` explicitly.

## DataItem
The [DataItem](DataItem.cs) class is used to implement DataItem elements described in **Part 2.0 : Section 7 of the MTConnect Standard**

DataItem Types (Samples, Events, and Conditions) are all defined in the [DataItems](DataItems) directory. Each class represents a DataItem of the specified Type. The class contains the Type string and a default Name as well as SubTypes and default attributes such as REPRESENTATION and UNITS.

#### Version Compatibility
MTConnect Version compatibility is performed through the DataItem base class and its 'MinimumVersion' and 'MaximumVersion' properties. These are read-only and are overridden in each derived class (representing each DataItem type) to return the MTConnect Version that the DataItem was introduced in.
DataItems (or DataItem Attributes) are then filtered out in the 'Process' method which is called (through 'Device.Process') by the MTConnectAgent class before data is output.

> The 'Process' method for DataItems only checks 'MinimumVersion'. 'MaximumVersion' is declared on DataItem (and is overridden by some Component types) but a DataItem is not removed based on it.

##### Properties
```c#
public virtual Version MaximumVersion => DefaultMaximumVersion; // null
public virtual Version MinimumVersion => DefaultMinimumVersion; // MTConnectVersions.Version10
```

Example of a derived class ([AccumulatedTimeDataItem](DataItems/AccumulatedTimeDataItem.g.cs)):
```c#
public override System.Version MinimumVersion => MTConnectVersions.Version12;
```

##### Process Method
```c#
public static DataItem Process(IDataItem dataItem, Version mtconnectVersion = null)
```
The full source is in [DataItem.cs](DataItem.cs). If 'mtconnectVersion' is null then the latest version (MTConnectVersions.Max) is used. The method returns null (the DataItem is removed from the output) when:
- The requested version is less than the DataItem's 'MinimumVersion'
- The Category is CONDITION and the version is less than 1.1
- The Representation is TIME_SERIES and the version is less than 1.2
- The Representation is DISCRETE and the version is less than 1.3 or is 1.5 or greater
- The Representation is DATA_SET and the version is less than 1.3
- The Representation is TABLE and the version is less than 1.6
- A CompositionId is set and the version is less than 1.4

Otherwise a new instance of the derived class for the DataItem's Type is created and the properties are copied to it. The following properties are only copied when the requested version supports them:

| Property | Copied when version is |
|----------|------------------------|
| Constraints | 1.1 or greater |
| SampleRate, Source, Representation, Statistic | 1.2 or greater |
| Filters | 1.3 or greater |
| Source.CompositionId, ResetTrigger, CompositionId, InitialValue | 1.4 or greater |
| Relationships (ComponentRelationship, DeviceRelationship), CoordinateSystemIdRef, Discrete | 1.5 or greater |
| Definition | 1.6 or greater |
| Relationships (DataItemRelationship, SpecificationRelationship) | 1.7 or greater |
| CoordinateSystem | less than 2.0 |

Finally, the overridable 'OnProcess' method is called so that a derived class can apply changes that are specific to its Type.
