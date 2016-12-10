// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Data;

namespace MTConnect.Application
{
    public class EventTypes
    {
        public static DataTable Get()
        {
            return Tools.Tables.GetEventTypes();
        }

    }
}
