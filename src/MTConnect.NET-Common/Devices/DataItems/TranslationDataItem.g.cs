// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Three space linear displacement of an object or coordinate system relative to a cartesian coordinate system.
    /// </summary>
    public class TranslationDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "TRANSLATION";
        public const string NameId = "translation";
        public const string DefaultUnits = Devices.Units.MILLIMETER_3D;     
        public new const string DescriptionText = "Three space linear displacement of an object or coordinate system relative to a cartesian coordinate system.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version16;       


        public TranslationDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Units = DefaultUnits;
        }

        public TranslationDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}