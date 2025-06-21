// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _2024x_68e0225_1727729265551_24591_24648

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Size of particles counted by their size or other characteristics.
    /// </summary>
    public class ParticleSizeDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "PARTICLE_SIZE";
        public const string NameId = "particleSize";
             
        public const string DefaultUnits = Devices.Units.MILLIMETER;     
        public new const string DescriptionText = "Size of particles counted by their size or other characteristics.";
        
        public override string TypeDescription => DescriptionText;
        
               


        public ParticleSizeDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            Units = DefaultUnits;
        }

        public ParticleSizeDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            Units = DefaultUnits;
        }
    }
}