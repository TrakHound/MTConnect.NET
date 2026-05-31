// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Errors
{
    /// <summary>
    /// When an Agent encounters an error when responding to a Request for information from a client software application,
    /// the information describing the error(s) is reported as a Data Entity in an MTConnectErrors Response Document. 
    /// Data Entities are organized in the Errors XML container.
    /// </summary>
    public class Error : IError
    {
        /// <summary>
        /// Provides a descriptive code that indicates the type of error that was encountered
        /// by an Agent when attempting to respond to a Request for information.
        /// </summary>
        public ErrorCode ErrorCode { get; set; }

        /// <summary>
        /// The Value for Error contains a textual description of the error and any additional
        /// information an Agent is capable of providing regarding a specific error. The Valid Data Value returned for Error MAY be any text string.
        /// </summary>
        public string Value { get; set; }


        /// <summary>
        /// Initializes an empty Error, leaving the code and description to be set by a deserializer or caller.
        /// </summary>
        public Error() { }

        /// <summary>
        /// Initializes an Error with the given code and optional descriptive text.
        /// </summary>
        /// <param name="errorCode">The error category encountered by the Agent.</param>
        /// <param name="value">An optional textual description of the error.</param>
        public Error(ErrorCode errorCode, string value = null)
        {
            ErrorCode = errorCode;
            Value = value;
        }
    }
}