// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Devices;

namespace MTConnect.Interfaces
{
    /// <summary>
    /// Base class for DataItems that flow across an Interface, distinguishing whether the observation expresses a request for a task or a response to one.
    /// </summary>
    public abstract class InterfaceDataItem : DataItem
    {
        /// <summary>
        /// The direction of an Interface DataItem relative to the handshake it participates in.
        /// </summary>
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



        /// <summary>
        /// The human-readable description of this DataItem's <see cref="SubTypes"/>, resolved from its configured SubType.
        /// </summary>
        public override string SubTypeDescription => GetSubTypeDescription(SubType);

        /// <summary>
        /// Returns the human-readable description for the given Interface DataItem subtype, or null if the value is unrecognized.
        /// </summary>
        /// <param name="subType">The subtype string (e.g. "REQUEST" or "RESPONSE") to describe.</param>
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

        /// <summary>
        /// Returns the short SubType identifier ("req" or "rsp") emitted in documents for the given subtype, or null if unrecognized.
        /// </summary>
        /// <param name="subType">The subtype to map to its document identifier.</param>
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