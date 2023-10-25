// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;

namespace MTConnect.Interfaces
{
    public abstract class InterfaceDataItem : DataItem
    {
        public enum SubTypes
        {
            /// <summary>
            /// Request by an Interface for a task.
            /// </summary>
            REQUEST,

            /// <summary>
            /// Response by an Interface to a request for a task.
            /// </summary>
            RESPONSE
        }



        public override string SubTypeDescription => GetSubTypeDescription(SubType);

        public static string GetSubTypeDescription(string subType)
        {
            var s = subType.ConvertEnum<SubTypes>();
            switch (s)
            {
                case SubTypes.REQUEST: return "Request by an Interface for a task.";
                case SubTypes.RESPONSE: return "Response by an Interface to a request for a task.";
            }

            return null;
        }

        public static string GetSubTypeId(SubTypes subType)
        {
            switch (subType)
            {
                case SubTypes.REQUEST: return "req";
                case SubTypes.RESPONSE: return "rsp";
            }

            return null;
        }
    }
}