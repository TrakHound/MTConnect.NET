// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Indication of the status of the Controller components program editing mode.A program may be edited while another is executed.
    /// </summary>
    public class ProgramEditDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "PROGRAM_EDIT";
        public const string NameId = "programEdit";
             
        public new const string DescriptionText = "Indication of the status of the Controller components program editing mode.A program may be edited while another is executed.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version13;       


        public ProgramEditDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            
        }

        public ProgramEditDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
        }
    }
}