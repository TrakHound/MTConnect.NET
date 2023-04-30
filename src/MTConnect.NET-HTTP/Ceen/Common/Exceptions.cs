using System;

namespace Ceen
{
	/// <summary>
	/// Implements an exception that should be reported to the client 
	/// as an HTTP error message
	/// </summary>
	[Serializable]
	public class HttpException : Exception
	{
		/// <summary>
		/// Gets the status code.
		/// </summary>
		/// <value>The status code.</value>
		public HttpStatusCode StatusCode { get; private set; }
		/// <summary>
		/// Gets the status message.
		/// </summary>
		/// <value>The status message.</value>
		public string StatusMessage { get; private set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Ceen.HttpException"/> class.
		/// </summary>
		public HttpException()
			: this(HttpStatusCode.InternalServerError)
		{			
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Ceen.HttpException"/> class.
		/// </summary>
		/// <param name="statuscode">The statuscode.</param>
		public HttpException(HttpStatusCode statuscode)
			: this(statuscode, HttpStatusMessages.DefaultMessage(statuscode))
		{
		}

        /// <summary>
        /// Initializes a new instance of the <see cref="Ceen.HttpException"/> class.
        /// </summary>
        /// <param name="statuscode">The statuscode.</param>
        /// <param name="statusmessage">The statusmessage.</param>
        public HttpException(HttpStatusCode statuscode, string statusmessage)
        {
            this.StatusCode = statuscode;
            this.StatusMessage = statusmessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ceen.HttpException"/> class.
        /// </summary>
        /// <param name="statuscode">The statuscode.</param>
        public HttpException(System.Net.HttpStatusCode statuscode)
            : this((HttpStatusCode)(int)statuscode)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Ceen.HttpException"/> class.
        /// </summary>
        /// <param name="statuscode">The statuscode.</param>
        /// <param name="statusmessage">The statusmessage.</param>
        public HttpException(System.Net.HttpStatusCode statuscode, string statusmessage)
            : this((HttpStatusCode)(int)statuscode, statusmessage)
        {
        }
    }

    /// <summary>
    /// Exception indicating that the stream closed without sending content
    /// </summary>
    [Serializable]
	public class EmptyStreamClosedException : Exception
	{
	}

	/// <summary>
	/// Exception indicating that a requirement failed
	/// </summary>
	[Serializable]
	public class RequirementFailedException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ceen.RequirementFailedException"/> class.
		/// </summary>
		public RequirementFailedException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ceen.RequirementFailedException"/> class.
		/// </summary>
		/// <param name="message">The message to report.</param>
		public RequirementFailedException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ceen.RequirementFailedException"/> class.
		/// </summary>
		/// <param name="message">The message to report.</param>
		/// <param name="innerException">The inner exception.</param>
		public RequirementFailedException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}

}

