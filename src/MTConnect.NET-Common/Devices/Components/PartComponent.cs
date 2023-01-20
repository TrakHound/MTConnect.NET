// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Devices.Components
{
    /// <summary>
    /// 
    /// </summary>
    public class PartComponent : Component 
    {
        public const string TypeId = "Part";
        public const string NameId = "part";
        public new const string DescriptionText = null;

        public override string TypeDescription => DescriptionText;


        public PartComponent()  { Type = TypeId; }
    }
}
