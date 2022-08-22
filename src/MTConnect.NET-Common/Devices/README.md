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
public static IDataItem Process(IDataItem dataItem, Version mtconnectVersion)
{
    if (dataItem != null)
    {
        // Check Version Compatibilty
        if (mtconnectVersion < dataItem.MinimumVersion || mtconnectVersion > dataItem.MaximumVersion) return null;

        // Don't return if Condition and Version < 1.1
        if (dataItem.Category == DataItemCategory.CONDITION && mtconnectVersion < MTConnectVersions.Version11) return null;

        // Don't return if TimeSeries and Version < 1.2
        if (dataItem.Representation == DataItemRepresentation.TIME_SERIES && mtconnectVersion < MTConnectVersions.Version12) return null;

        // Don't return if Discrete and Version < 1.3 OR Version >= 1.5
        if (dataItem.Representation == DataItemRepresentation.DISCRETE && (mtconnectVersion < MTConnectVersions.Version13 || mtconnectVersion >= MTConnectVersions.Version15)) return null;

        // Don't return if DataSet and Version < 1.3
        if (dataItem.Representation == DataItemRepresentation.DATA_SET && mtconnectVersion < MTConnectVersions.Version13) return null;

        // Don't return if Table and Version < 1.6
        if (dataItem.Representation == DataItemRepresentation.TABLE && mtconnectVersion < MTConnectVersions.Version16) return null;

        // Create a new Instance of the DataItem that will instantiate a new Derived class (if found)
        var obj = Create(dataItem);
        if (obj != null)
        {
            obj.Category = dataItem.Category;
            obj.Id = dataItem.Id;
            obj.Name = dataItem.Name;
            obj.Type = dataItem.Type;
            obj.SubType = dataItem.SubType;
            obj.NativeUnits = dataItem.NativeUnits;
            obj.NativeScale = dataItem.NativeScale;
            obj.Units = dataItem.Units;
            obj.SignificantDigits = dataItem.SignificantDigits;

            // Check SampleRate
            if (mtconnectVersion >= MTConnectVersions.Version12) obj.SampleRate = dataItem.SampleRate;

            // Check Source
            if (dataItem.Source != null && mtconnectVersion >= MTConnectVersions.Version12)
            {
                var source = new Source();
                source.ComponentId = dataItem.Source.ComponentId;
                if (mtconnectVersion >= MTConnectVersions.Version14) source.CompositionId = dataItem.Source.CompositionId;
                source.DataItemId = dataItem.Source.DataItemId;
                obj.Source = source;
            }

            // Check Relationships
            obj.Relationships = dataItem.Relationships;
            if (dataItem.Relationships != null && mtconnectVersion >= MTConnectVersions.Version15)
            {
                var relationships = new List<IRelationship>();
                foreach (var relationship in dataItem.Relationships)
                {
                    // Component Relationship
                    if (relationship.GetType() == typeof(ComponentRelationship))
                    {
                        relationships.Add(relationship);
                    }

                    // DataItem Relationship
                    if (relationship.GetType() == typeof(DataItemRelationship))
                    {
                        if (mtconnectVersion >= MTConnectVersions.Version17) relationships.Add(relationship);
                    }

                    // Device Relationship
                    if (relationship.GetType() == typeof(DeviceRelationship))
                    {
                        relationships.Add(relationship);
                    }

                    // Specification Relationship
                    if (relationship.GetType() == typeof(SpecificationRelationship))
                    {
                        if (mtconnectVersion >= MTConnectVersions.Version17) relationships.Add(relationship);
                    }
                }

                obj.Relationships = relationships;
            }

            // Check Representation
            if (mtconnectVersion >= MTConnectVersions.Version12) obj.Representation = dataItem.Representation;

            // Check ResetTrigger
            if (mtconnectVersion >= MTConnectVersions.Version14) obj.ResetTrigger = dataItem.ResetTrigger;

            // Check CoordinateSystem
            if (mtconnectVersion < MTConnectVersions.Version20) obj.CoordinateSystem = dataItem.CoordinateSystem;

            // Check CoordinateSystemIdRef
            if (mtconnectVersion >= MTConnectVersions.Version15) obj.CoordinateSystemIdRef = dataItem.CoordinateSystemIdRef;

            // Check CompositionId
            if (mtconnectVersion >= MTConnectVersions.Version14)
            {                       
                obj.CompositionId = dataItem.CompositionId;
            }
            else if (!string.IsNullOrEmpty(dataItem.CompositionId))
            {
                // Don't return if Composition not compatible with Version as this could cause duplicate Types within the same Component
                return null;
            }

            // Check Constraints
            if (mtconnectVersion >= MTConnectVersions.Version11) obj.Constraints = dataItem.Constraints;

            // Check Definition
            if (mtconnectVersion >= MTConnectVersions.Version16) obj.Definition = dataItem.Definition;

            // Check Statistic
            if (mtconnectVersion >= MTConnectVersions.Version12) obj.Statistic = dataItem.Statistic;

            // Check Filters
            if (mtconnectVersion >= MTConnectVersions.Version13) obj.Filters = dataItem.Filters;

            // Check InitialValue
            if (mtconnectVersion >= MTConnectVersions.Version14) obj.InitialValue = dataItem.InitialValue;

            // Check Discrete
            if (mtconnectVersion >= MTConnectVersions.Version15) obj.Discrete = dataItem.Discrete;
        }

        // Call overridable method (used to process based on Type)
        return obj.OnProcess(obj, mtconnectVersion);
    }

    return null;
}
```