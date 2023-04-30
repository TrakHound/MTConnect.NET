using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Globalization;

namespace Ceen
{
    /// <summary>
    /// The parsed language tag
    /// </summary>
    public class LanguageTag
    {
        /// <summary>
        /// The primary language
        /// </summary>
        public readonly string Primary;
        /// <summary>
        /// The subtag (i.e. region)
        /// </summary>
        public readonly string Subtag;
        /// <summary>
        /// The quality (priority)
        /// </summary>
        public readonly float Quality;

        /// <summary>
        /// The invariant language
        /// </summary>
        public static readonly LanguageTag Invariant = new LanguageTag("", "", 0);

        /// <summary>
        /// The english language
        /// </summary>
        public static readonly LanguageTag English = new LanguageTag("en", "", 0);

        /// <summary>
        /// Constructs a new language tag
        /// </summary>
        /// <param name="primary">The primary language</param>
        /// <param name="subtag">The subtag</param>
        /// <param name="quality">The quality</param>
        public LanguageTag(string primary, string subtag, float quality)
        {
            Primary = primary;
            Subtag = subtag;
            Quality = quality;
        }

        /// <summary>
        /// The IANA style name (aka ietf name, etc)
        /// </summary>
        public string IANAName => Primary + (string.IsNullOrWhiteSpace(Subtag) ? string.Empty : ("-" + Subtag));

        /// <summary>
        /// Attempts to parse the locale and returns a cultureinfo matching it
        /// </summary>
        /// <param name="@default">The default culture info to return</param>
        /// <returns><paramref name="@default" /> if not culture was matched</returns>
        public CultureInfo TryParse(bool allowOnlyMajor = true, CultureInfo @default = null)
        {
            try { return new CultureInfo(IANAName); }
            catch { }

            if (allowOnlyMajor && !string.IsNullOrWhiteSpace(Subtag))
                try { return new CultureInfo(Primary); }
                catch { }

            return @default;
        }
    }

    /// <summary>
    /// Helper methods for performing various common operations
    /// on a request instance
    /// </summary>
    public static class RequestUtility
    {
		/// <summary>
		/// Gets an encoding from a charset string
		/// </summary>
		/// <returns>The encoding for the charset.</returns>
		/// <param name="charset">The charset string.</param>
        public static Encoding GetEncodingForCharset(string charset)
		{
			if (string.Equals("utf-8", charset, StringComparison.OrdinalIgnoreCase))
				return Encoding.UTF8;
			else if (string.Equals("ascii", charset, StringComparison.OrdinalIgnoreCase))
				return Encoding.ASCII;
			else
				return Encoding.GetEncoding(charset);
		}

		/// <summary>
		/// Gets an encoding from a charset string
		/// </summary>
		/// <returns>The encoding for the charset.</returns>
		/// <param name="contenttype">The content type string.</param>
		public static Encoding GetEncodingForContentType(string contenttype)
        {
            var enc = GetHeaderComponent(contenttype, "encoding");
            if (string.IsNullOrWhiteSpace(enc))
                enc = GetHeaderComponent(contenttype, "charset");

            // Defaults to ASCII (7-bit), unless we are using "application" types which are 8-bit
            if (string.IsNullOrWhiteSpace(enc))
                return
                    contenttype.StartsWith("application/", StringComparison.OrdinalIgnoreCase)
                        ? Encoding.UTF8
                        : Encoding.ASCII;

            return GetEncodingForCharset(enc);
		}

		/// <summary>
		/// Gets an encoding from a charset string
		/// </summary>
		/// <returns>The encoding for the charset.</returns>
		/// <param name="request">The request instance.</param>
		public static Encoding GetEncodingForCharset(this IHttpRequest request, string charset)
            => GetEncodingForCharset(charset);

		/// <summary>
		/// Gets an encoding from the content-type string
		/// </summary>
		/// <returns>The encoding for the content-type.</returns>
		/// <param name="request">The request instance.</param>
		public static Encoding GetEncodingForContentType(this IHttpRequest request)
            => GetEncodingForContentType(request.ContentType);

		/// <summary>
		/// Regular expression for parsing the Accept-Language header
		/// </summary>
		private static System.Text.RegularExpressions.Regex LANGUAGE_MATCHER = 
			new System.Text.RegularExpressions.Regex(
                @"\s*(?<primary>[A-z]+)(-(?<subtag>[A-z-_]+))?(;(q=(?<quality>[0-9\.]+)))?\s*,?"
			);

		/// <summary>
		/// The number style for parsing the language quality specifier
		/// </summary>
		private const System.Globalization.NumberStyles QUALITY_NUMBER_STYLE =
            System.Globalization.NumberStyles.AllowLeadingSign |
            System.Globalization.NumberStyles.AllowDecimalPoint |
            System.Globalization.NumberStyles.AllowLeadingWhite |
            System.Globalization.NumberStyles.AllowTrailingWhite;

		/// <summary>
		/// Gets the prefered language that the user accepts, which is also in the list of supported items.
		/// Returns null if no languages are accepted.
		/// Expects fully qualified language names (i.e. &quot;en-US&quot;)
		/// </summary>
		/// <param name="request">The request to examine</param>
		/// <param name="supportedLanguages">The list of supported languages</param>
		/// <returns>The prefered language or null</returns>
		public static LanguageTag GetAcceptLanguage(this IHttpRequest request, params string[] supportedLanguages)
		{
			var lookup = supportedLanguages.ToLookup(x => x, StringComparer.InvariantCultureIgnoreCase);
			return GetAcceptLanguages(request).FirstOrDefault(x => lookup.Contains(x.IANAName));
		}

        /// <summary>
        /// Gets the prefered language that the user accepts, which is also in the list of supported items.
        /// Returns null if no languages are accepted.
		/// Expects only major languages (i.e. expects &quot;en&quot; NOT &quot;en-US&quot;) in the supported languages.
        /// </summary>
        /// <param name="request">The request to examine</param>
        /// <param name="supportedLanguages">The list of supported languages</param>
        /// <returns>The prefered language or null</returns>
        public static LanguageTag GetAcceptMajorLanguage(this IHttpRequest request, params string[] supportedLanguages)
        {
            var lookup = supportedLanguages.ToLookup(x => x, StringComparer.InvariantCultureIgnoreCase);
            return GetAcceptLanguages(request)
				.FirstOrDefault(x => lookup.Contains(x.Primary));
        }

        /// <summary>
        /// Returns an ordered priority list of accepted languages.
        /// </summary>
        /// <param name="request">The request to examine</param>
        /// <returns>The ordered list of accepted languages</returns>
        public static IOrderedEnumerable<LanguageTag> GetAcceptLanguages(this IHttpRequest request)
		{
			return LANGUAGE_MATCHER
				// Match the string
				.Matches(request.Headers["Accept-Language"] ?? string.Empty)
				// Old IEnumerable to Linq
				.Cast<System.Text.RegularExpressions.Match>()
				// Parse each match
				.Select(x => {
					var quality = 1f;
					if (x.Groups["quality"].Success && float.TryParse(x.Groups["quality"].Value, QUALITY_NUMBER_STYLE, System.Globalization.CultureInfo.InvariantCulture, out var q))
						quality = q;

					return new LanguageTag(
						x.Groups["primary"].Value,
						x.Groups["subtag"].Success ? x.Groups["subtag"].Value : string.Empty,
						quality
					);
				})
				// Filter out those with quality zero or less
				.Where(x => x.Quality > 0)
				// Order by quality
				.OrderByDescending(x => x.Quality)
				// But with same quality, we prefer those with a sub-tag
				.ThenBy(x => string.IsNullOrWhiteSpace(x.Subtag));
		}

        /// <summary>
        /// Returns a value indicating if the request is a multi-part request
        /// </summary>
        /// <returns><c>true</c>, if multi-part was used, <c>false</c> otherwise.</returns>
        /// <param name="request">The request to examine.</param>
        public static bool IsMultipartRequest(this IHttpRequest request)
        {
            return IsMultipartRequest(request.ContentType);
        }

		/// <summary>
		/// Returns a value indicating if the request is a multi-part request
		/// </summary>
		/// <returns><c>true</c>, if multi-part was used, <c>false</c> otherwise.</returns>
		/// <param name="contenttype">The request contenttype to examine.</param>
		public static bool IsMultipartRequest(string contenttype)
		{
            return IsContentType(contenttype, "multipart/form-data");
		}

        /// <summary>
        /// Returns a value indicating if the content type is of a certain type
        /// </summary>
        /// <param name="request">The request to evaluate the content type for</param>
        /// <param name="test">The type to test for</param>
        /// <returns><c>true</c> if the content-type matches the test type; <c>false</c> otherwise</returns>
        public static bool IsContentType(this IHttpRequest request, string test)
            => IsContentType(request.ContentType, test);


        /// <summary>
        /// Returns a value indicating if the content type is of a certain type
        /// </summary>
        /// <param name="contenttype">The content-type string</param>
        /// <param name="test">The type to test for</param>
        /// <returns><c>true</c> if the content-type matches the test type; <c>false</c> otherwise</returns>
        public static bool IsContentType(string contenttype, string test)
        {
            if (string.IsNullOrWhiteSpace(contenttype))
                return false;

            var firstdelim = contenttype.IndexOfAny(new char[] { ';', ' ', ',' });
            if (firstdelim < 0)
                firstdelim = contenttype.Length;

            return string.Equals(contenttype.Substring(0, firstdelim), test, StringComparison.OrdinalIgnoreCase);
        }

		/// <summary>
		/// Returns a value indicating if the content type is indicating Json data
		/// </summary>
		/// <returns><c>true</c>, if the content type is json, <c>false</c> otherwise.</returns>
		/// <param name="request">The request instance.</param>
		public static bool IsJsonRequest(this IHttpRequest request)
            => IsJsonRequest(request.ContentType);

		/// <summary>
		/// Returns a value indicating if the content type is indicating Json data
		/// </summary>
		/// <returns><c>true</c>, if the content type is json, <c>false</c> otherwise.</returns>
		/// <param name="contenttype">The request contenttype to examine.</param>
		public static bool IsJsonRequest(string contenttype)
		{
            var ct = contenttype ?? string.Empty;
            if (string.IsNullOrWhiteSpace(ct))
                return false;

            // First is correct, rest is for compatibility
            return IsContentType(contenttype, "application/json")
                || IsContentType(contenttype, "application/x-javascript")
                || IsContentType(contenttype, "text/javascript")
                || IsContentType(contenttype, "text/x-javascript")
                || IsContentType(contenttype, "text/x-json");
		}

		/// <summary>
		/// Splits a header line into its key-value components
		/// </summary>
		/// <returns>The components.</returns>
		/// <param name="line">The line to split.</param>
		public static IEnumerable<KeyValuePair<string, string>> SplitHeaderLine(string line)
		{
			return (line ?? "").Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(x =>
				{
					var c = x.Split(new char[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries);
					var value = (c.Skip(1).FirstOrDefault() ?? "").Trim();
					if (value.StartsWith("\"", StringComparison.Ordinal) && value.EndsWith("\"", StringComparison.Ordinal))
						value = value.Substring(1, value.Length - 2);
					return new KeyValuePair<string, string>(c.First().Trim(), value);
				});
		}


		/// <summary>
		/// Gets a named component from a header line
		/// </summary>
		/// <returns>The header component or null.</returns>
		/// <param name="line">The header line.</param>
		/// <param name="key">The component to find.</param>
		public static string GetHeaderComponent(string line, string key)
		{
			return
				SplitHeaderLine(line)
				.Where(x => string.Equals(x.Key, key, StringComparison.OrdinalIgnoreCase))
				.Select(x => x.Value)
				.FirstOrDefault();
		}

		/// <summary>
		/// Reads all bytes from a stream into a string, using UTF8 encoding
		/// </summary>
		/// <returns>The string from the stream.</returns>
		/// <param name="stream">The stream to read from.</param>
		/// <param name="token">The cancellation token.</param>
		public static Task<string> ReadAllAsStringAsync(this Stream stream, CancellationToken token = default(CancellationToken))
		{
			return ReadAllAsStringAsync(stream, System.Text.Encoding.UTF8, token);
		}

		/// <summary>
		/// Reads all bytes from a stream into a string
		/// </summary>
		/// <returns>The string from the stream.</returns>
		/// <param name="stream">The stream to read from.</param>
		/// <param name="encoding">The encoding to use.</param>
		/// <param name="token">The cancellation token.</param>
		public static async Task<string> ReadAllAsStringAsync(this Stream stream, System.Text.Encoding encoding, CancellationToken token = default(CancellationToken))
		{
			if (encoding == null)
				throw new ArgumentNullException(nameof(encoding));

			using (var ms = new System.IO.MemoryStream())
			{
				await stream.CopyToAsync(ms, 1024 * 8, token);
				return encoding.GetString(ms.ToArray());
			}
		}

		/// <summary>
		/// Logs an exception error
		/// </summary>
		/// <param name="context">The context to log with</param>
		/// <param name="ex">The exception to log</param>
		/// <returns>An awaitable task</returns>
        public static Task LogErrorAsync(this IHttpContext context, Exception ex)
        {
            return context.LogMessageAsync(LogLevel.Error, null, ex);
        }

        /// <summary>
        /// Logs an error message
        /// </summary>
        /// <param name="context">The context to log with</param>
        /// <param name="message">The message to log</param>
        /// <param name="ex">The optional exception to log</param>
        /// <returns>An awaitable task</returns>
        public static Task LogErrorAsync(this IHttpContext context, string message, Exception ex = null)
		{
			return context.LogMessageAsync(LogLevel.Error, message, ex);
		}

        /// <summary>
        /// Logs an exception warning
        /// </summary>
        /// <param name="context">The context to log with</param>
        /// <param name="ex">The exception to log</param>
        /// <returns>An awaitable task</returns>
        public static Task LogWarningAsync(this IHttpContext context, Exception ex)
        {
            return context.LogMessageAsync(LogLevel.Warning, null, ex);
        }

        /// <summary>
        /// Logs a warning message
        /// </summary>
        /// <param name="context">The context to log with</param>
        /// <param name="message">The message to log</param>
        /// <param name="ex">The optional exception to log</param>
        /// <returns>An awaitable task</returns>
        public static Task LogWarningAsync(this IHttpContext context, string message, Exception ex = null)
        {
            return context.LogMessageAsync(LogLevel.Warning, message, ex);
        }

        /// <summary>
        /// Logs an exception for information use
        /// </summary>
        /// <param name="context">The context to log with</param>
        /// <param name="ex">The exception to log</param>
        /// <returns>An awaitable task</returns>
        public static Task LogInformationAsync(this IHttpContext context, Exception ex)
        {
            return context.LogMessageAsync(LogLevel.Information, null, ex);
        }

        /// <summary>
        /// Logs an informaiton message
        /// </summary>
        /// <param name="context">The context to log with</param>
        /// <param name="message">The message to log</param>
        /// <param name="ex">The optional exception to log</param>
        /// <returns>An awaitable task</returns>
        public static Task LogInformationAsync(this IHttpContext context, string message, Exception ex = null)
        {
            return context.LogMessageAsync(LogLevel.Information, message, ex);
        }

        /// <summary>
        /// Logs an exception for debugging
        /// </summary>
        /// <param name="context">The context to log with</param>
        /// <param name="ex">The exception to log</param>
        /// <returns>An awaitable task</returns>
        public static Task LogDebugAsync(this IHttpContext context, Exception ex)
        {
            return context.LogMessageAsync(LogLevel.Debug, null, ex);
        }

        /// <summary>
        /// Logs a debug message
        /// </summary>
        /// <param name="context">The context to log with</param>
        /// <param name="message">The message to log</param>
        /// <param name="ex">The optional exception to log</param>
        /// <returns>An awaitable task</returns>
        public static Task LogDebugAsync(this IHttpContext context, string message, Exception ex = null)
        {
            return context.LogMessageAsync(LogLevel.Debug, message, ex);
        }

        /// <summary>
        /// Gets the remote IP from the request as a string (or null)
        /// </summary>
        /// <param name="self">The request instance</param>
        /// <returns>The remote IP</returns>
        public static string GetRemoteIP(this Ceen.IHttpRequest self)
            => (self.RemoteEndPoint as System.Net.IPEndPoint)?.Address.ToString();

        /// <summary>
        /// Gets the remote IP from the request as a string (or null)
        /// </summary>
        /// <param name="self">The context instance</param>
        /// <returns>The remote IP</returns>
        public static string GetRemoteIP(this Ceen.IHttpContext self)
            => GetRemoteIP(self?.Request);
    }
}
