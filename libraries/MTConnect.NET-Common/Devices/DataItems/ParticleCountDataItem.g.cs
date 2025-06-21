// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

// MTConnect SysML v2.3 : UML ID = _2024x_68e0225_1727728953400_643342_24554

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Number of particles counted by their size or other characteristics.
    /// </summary>
    public class ParticleCountDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.SAMPLE;
        public const string TypeId = "PARTICLE_COUNT";
        public const string NameId = "particleCount";
             
        public const string DefaultUnits = Devices.Units.COUNT;     
        public new const string DescriptionText = "Number of particles counted by their size or other characteristics.";
        
        public override string TypeDescription => DescriptionText;
        
               


        public enum SubTypes
        {
            /// <summary>
            /// GAS
            /// </summary>
            GAS,
            
            /// <summary>
            /// LIQUID
            /// </summary>
            LIQUID,
            
            /// <summary>
            /// SOLID
            /// </summary>
            SOLID
        }


        public ParticleCountDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            Units = DefaultUnits;
        }

        public ParticleCountDataItem(
            string parentId,
            SubTypes subType
            )
        {
            Id = CreateId(parentId, NameId, GetSubTypeId(subType));
            Category = CategoryId;
            Type = TypeId;
            SubType = subType.ToString();
            Name = NameId;
             
            Units = DefaultUnits;
        }

        public override string SubTypeDescription => GetSubTypeDescription(SubType);

        public static string GetSubTypeDescription(string subType)
        {
            var s = subType.ConvertEnum<SubTypes>();
            switch (s)
            {
                case SubTypes.GAS: return "GAS";
                case SubTypes.LIQUID: return "LIQUID";
                case SubTypes.SOLID: return "SOLID";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.GAS: return "GAS";
                case SubTypes.LIQUID: return "LIQUID";
                case SubTypes.SOLID: return "SOLID";
            }

            return null;
        }

    }
}