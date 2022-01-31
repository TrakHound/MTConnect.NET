# Devices
#### Handles MTConnectDevices Response Documents described in **Part 2.0 of the MTConnect Standard**

## Device
The [Device](Device.cs) class is used to implement Device elements described in **Part 2.0 : Section 4.2 of the MTConnect Standard**

## Component
The [Component](Component.cs) class is used to implement Component elements described in **Part 2.0 : Section 5 of the MTConnect Standard**

Component Types are defined in the [Components](Components) directory. Each class represents a Component of the specified Type. The class contains the Type string and a default Name.

## Composition
The [Composition](Composition.cs) class is used to implement Composition elements described in **Part 2.0 : Section 6 of the MTConnect Standard**

Composition Types are defined in the [Compositions](Compositions) directory. Each class represents a Composition of the specified Type. The class contains the Type string and a default Name.

## DataItem
The [DataItem](DataItem.cs) class is used to implement DataItem elements described in **Part 2.0 : Section of 7 the MTConnect Standard**

DataItem Types are defined in the [Samples](Samples), [Events](Events), and [Conditions](Conditions) directories. Each class represents a DataItem of the specified Type. The class contains the Type string and a default Name as well as SubTypes and default attributes such as REPRESENTATION and UNITS.

#### Version Compatiblity
MTConnect Version compatiblity is performed through the DataItem base class and it's 'MaximumVersion' and 'MinimumVersion' properties. These are then set for each derived class (representing each DataItem type) to set the appropriate MTConnect Version that the DataItem is compatible with.
DataItems (or DataItem Attributes) are then filtered out in the 'Process' method which is called by the MTConnectAgent class before data is output.

##### Properties
```c#
public Version MaximumVersion { get; set; }
public Version MinimumVersion { get; set; }
```

##### Process Method
```c#
public static DataItem Process(DataItem dataItem, Version mtconnectVersion)
{
    if (dataItem != null)
    {
        var obj = Create(dataItem.Type);
        if (obj != null)
        {
            obj.DataItemCategory = dataItem.DataItemCategory;
            obj.Id = dataItem.Id;
            obj.Name = dataItem.Name;
            obj.Type = dataItem.Type;
            obj.SubType = dataItem.SubType;
            obj.NativeUnits = dataItem.NativeUnits;
            obj.NativeScale = dataItem.NativeScale;
            obj.SampleRate = dataItem.SampleRate;
            obj.Source = dataItem.Source;
            obj.Relationships = dataItem.Relationships;
            obj.Representation = dataItem.Representation;
            obj.ResetTrigger = dataItem.ResetTrigger;
            obj.CoordinateSystem = dataItem.CoordinateSystem;
            obj.Constraints = dataItem.Constraints;
            obj.Definition = dataItem.Definition;
            obj.Units = dataItem.Units;
            obj.Statistic = dataItem.Statistic;
            obj.SignificantDigits = dataItem.SignificantDigits;
            obj.Filters = dataItem.Filters;
            obj.InitialValue = dataItem.InitialValue;
        }
        else obj = dataItem;

        // Check Version Compatibilty
        if (mtconnectVersion >= obj.MinimumVersion && mtconnectVersion <= obj.MaximumVersion)
        {
            return obj;
        }
    }

    return null;
}
```