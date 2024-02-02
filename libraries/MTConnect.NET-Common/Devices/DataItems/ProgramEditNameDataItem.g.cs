// Copyright (c) 2024 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.DataItems
{
    /// <summary>
    /// Name of the program being edited. This is used in conjunction with ProgramEdit when in `ACTIVE` state.
    /// </summary>
    public class ProgramEditNameDataItem : DataItem
    {
        public const DataItemCategory CategoryId = DataItemCategory.EVENT;
        public const string TypeId = "PROGRAM_EDIT_NAME";
        public const string NameId = "programEditName";
             
             
        public new const string DescriptionText = "Name of the program being edited. This is used in conjunction with ProgramEdit when in `ACTIVE` state.";
        
        public override string TypeDescription => DescriptionText;
        
        public override System.Version MinimumVersion => MTConnectVersions.Version13;       


        public ProgramEditNameDataItem()
        {
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
              
            
        }

        public ProgramEditNameDataItem(string deviceId)
        {
            Id = CreateId(deviceId, NameId);
            Category = CategoryId;
            Type = TypeId;
            Name = NameId;
             
            
        }
    }
}