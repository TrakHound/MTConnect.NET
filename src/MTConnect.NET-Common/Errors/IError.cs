// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE', which is part of this source code package.

namespace MTConnect.Errors
{
    /// <summary>
    /// When an Agent encounters an error when responding to a Request for information from a client software application,
    /// the information describing the error(s) is reported as a Data Entity in an MTConnectErrors Response Document. 
    /// Data Entities are organized in the Errors XML container.
    /// </summary>
    public interface IError
    {
        /// <summary>
        /// Provides a descriptive code that indicates the type of error that was encountered
        /// by an Agent when attempting to respond to a Request for information.
        /// </summary>
        ErrorCode ErrorCode { get; }

        /// <summary>
        /// The CDATA for Error contains a textual description of the error and any additional
        /// information an Agent is capable of providing regarding a specific error. The Valid Data Value returned for Error MAY be any text string.
        /// </summary>
        string CDATA { get; }
    }
}
