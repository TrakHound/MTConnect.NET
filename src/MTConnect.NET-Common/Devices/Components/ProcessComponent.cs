// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// 
    /// </summary>
    public class ProcessComponent : Component 
    {
        public const string TypeId = "Process";
        public const string NameId = "process";
        public new const string DescriptionText = null;

        public override string TypeDescription => DescriptionText;


        public ProcessComponent()  { Type = TypeId; }
    }
}