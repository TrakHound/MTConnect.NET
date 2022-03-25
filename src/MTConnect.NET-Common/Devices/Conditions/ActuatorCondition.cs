// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

namespace MTConnect.Devices.Conditions
{
    /// <summary>
    /// An indication of a fault associated with an actuator.
    /// </summary>
    public class ActuatorCondition : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.CONDITION;
        public const string TypeId = "ACTUATOR";
        public const string NameId = "actuator";
        public new const string DescriptionText = "An indication of a fault associated with an actuator.";

        public override string TypeDescription => DescriptionText;


        public ActuatorCondition()
        {
            Category = CategoryId;
            Type = TypeId;
        }

        public ActuatorCondition(string parentId)
        {
            Id = CreateId(parentId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}
