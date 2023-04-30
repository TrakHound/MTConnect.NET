using System;

namespace Ceen
{
	/// <summary>
	/// List of supported HTTP status codes
	/// </summary>
	public enum HttpStatusCode
	{
		Continue = 100,
		SwitchingProtocols = 101,
		Processing = 102,

		OK = 200,
		Created = 201,
		Accepted = 202,
		NonAuthoritative = 203,
		NoContent = 204,
		ResetContent = 205,
		PartialContent = 206,
		Multistatus = 207,
		Alreadyreported = 208,

		MultipleChoices = 300,
		MovedPermanently = 301,
		Found = 302,
		SeeOther = 303,
		NotModified = 304,
		UseProxy = 305,
		SwitchProxy = 306,
		TemporaryRedirect = 307,
		PermanentRedirect = 308,

		BadRequest = 400,
		Unauthorized = 401,
		PaymentRequired = 402,
		Forbidden = 403,
		NotFound = 404,
		MethodNotAllowed = 405,
		NotAcceptable = 406,
		ProxyAuthenticationRequired = 407,
		RequestTimeout = 408,
		Conflict = 409,
		Gone = 410,
		LengthRequired = 411,
		PreconditionFailed = 412,
		PayloadTooLarge = 413,
		URITooLong = 414,
		UnsupportedMediaType = 415,
		RangeNotSatisfiable = 416,
		ExpectationFailed = 417,
		MisdirectedRequest = 421,
		UnprocessableEntity = 422,
		Locked = 423,
		FailedDependency = 424,
		UpgradeRequired = 426,
		PreconditionRequired = 428,
		TooManyRequests = 429,
		RequestHeaderFieldsTooLarge = 431,
		UnavailableForLegalReasons = 451,

		InternalServerError = 500,
		NotImplemented = 501,
		BadGateway = 502,
		ServiceUnavailable = 503,
		GatewayTimeout = 504,
		HTTPVersionNotSupported = 505,
		VariantAlsoNegotiates = 506,
		InsufficientStorage = 507,
		LoopDetected = 508,
		NotExtended = 510,
		NetworkAuthenticationRequired = 511,
	}

	/// <summary>
	/// List of default messags to return for a HTTP status code
	/// </summary>
	public static class HttpStatusMessages
	{
		public static string DefaultMessage(HttpStatusCode code)
		{
			switch ((int)code)
			{
				case 100:
					return "Continue";
				case 101:
					return "Switching Protocols";
				case 102:
					return "Processing";

				case 200:
					return "OK";
				case 201:
					return "Created";
				case 202:
					return "Accepted";
				case 203:
					return "Non-Authoritative";
				case 204:
					return "No Content";
				case 205:
					return "Reset Content";
				case 206:
					return "Partial Content";
				case 207:
					return "Multi-status";
				case 208:
					return "Already reported";

				case 300:
					return "Multiple Choices";
				case 301:
					return "Moved Permanently";
				case 302:
					return "Found";
				case 303:
					return "See Other";
				case 304:
					return "Not Modified";
				case 305:
					return "Use Proxy";
				case 306:
					return "Switch Proxy";
				case 307:
					return "Temporary Redirect";
				case 308:
					return "Permanent Redirect";

				case 400:
					return "Bad Request";
				case 401:
					return "Unauthorized";
				case 402:
					return "Payment Required";
				case 403:
					return "Forbidden";
				case 404:
					return "Not Found";
				case 405:
					return "Method Not Allowed";
				case 406:
					return "Not Acceptable";
				case 407:
					return "Proxy Authentication Required";
				case 408:
					return "Request Timeout";
				case 409:
					return "Conflict";
				case 410:
					return "Gone";
				case 411:
					return "Length Required";
				case 412:
					return "Precondition Failed";
				case 413:
					return "Payload Too Large";
				case 414:
					return "URI Too Long";
				case 415:
					return "Unsupported Media Type";
				case 416:
					return "Range Not Satisfiable";
				case 417:
					return "Expectation Failed";
				case 421:
					return "Misdirected Request";
				case 422:
					return "Unprocessable Entity";
				case 423:
					return "Locked";
				case 424:
					return "Failed Dependency";
				case 426:
					return "Upgrade Required";
				case 428:
					return "Precondition Required";
				case 429:
					return "Too Many Requests";
				case 431:
					return "Request Header Fields Too Large";
				case 451:
					return "Unavailable For Legal Reasons";

				case 500:
					return "Internal Server Error";
				case 501:
					return "Not Implemented";
				case 502:
					return "Bad Gateway";
				case 503:
					return "Service Unavailable";
				case 504:
					return "Gateway Timeout";
				case 505:
					return "HTTP Version Not Supported";
				case 506:
					return "Variant Also Negotiates";
				case 507:
					return "Insufficient Storage";
				case 508:
					return "Loop Detected";
				case 510:
					return "Not Extended";
				case 511:
					return "Network Authentication Required";

				default:
					return "Unknown status";
			}
		}
	}
}

