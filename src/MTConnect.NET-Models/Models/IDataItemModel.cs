// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;
using MTConnect.Devices.DataItems;

namespace MTConnect.Models
{
    public interface IDataItemModel : IDataItem
    {
        DataItemCategory DataItemCategory { get; set; }

        string DataItemId { get; set; }

        string DataItemName { get; set; }

        string Type { get; set; }

        string SubType { get; set; }

        DataItemCoordinateSystem CoordinateSystem { get; set; }

        double? NativeScale { get; set; }

        string Units { get; set; }

        string NativeUnits { get; set; }

        DataItemStatistic Statistic { get; set; }

        DataItemRepresentation Representation { get; set; }

        int? SignificantDigits { get; set; }

        double? SampleRate { get; set; }

        //Source Source { get; set; }

        //Constraints Constraints { get; set; }

        //List<Filter> Filters { get; set; }

        string InitialValue { get; set; }

        DataItemResetTrigger ResetTrigger { get; set; }

        //DataItemDefinition Definition { get; set; }

        //List<object> Relationships { get; set; }
    }
}