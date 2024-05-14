using System;
using System.Collections.Generic;
using System.Globalization;

namespace Ceen.Httpd
{
    /// <summary>
    /// Representation of a response cookie
    /// </summary>
    internal class ResponseCookie : IResponseCookie
	{
		/// <summary>
		/// List of settings attached to the cookie
		/// </summary>
		/// <value>The settings.</value>
		public IDictionary<string, string> Settings { get; private set; }

		/// <summary>
		/// The name of the cookie
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; set; }
		/// <summary>
		/// The value of the cookie
		/// </summary>
		/// <value>The value.</value>
		public string Value { get; set; }

		/// <summary>
		/// Gets or sets the cookie path
		/// </summary>
		/// <value>The path.</value>
		public string Path
		{
			get { return Settings["Path"]; }
			set 
			{ 
				if (value == null)
					Settings.Remove("Path");
				else
					Settings["Path"] = value; 
			}
		}

		/// <summary>
		/// Gets or sets the cookie domain
		/// </summary>
		/// <value>The path.</value>
		public string Domain
		{
			get { return Settings["Domain"]; }
			set 
			{ 
				if (value == null)
					Settings.Remove("Domain");
				else
					Settings["Domain"] = value; 
			}
		}

		/// <summary>
		/// Gets or sets the cookie expiration date
		/// </summary>
		/// <value>The path.</value>
		public DateTime? Expires
		{
			get 
			{ 
				if (Settings["Expires"] == null)
					return null;

				DateTime res;
				if (DateTime.TryParseExact(Settings["Expires"], "R", CultureInfo.InvariantCulture, DateTimeStyles.None, out res))
					return res; 
				
				return null;
			}
			set 
			{ 
				if (value == null)
					Settings.Remove("Expires");
				else
					Settings["Expires"] = value.Value.ToString("R", CultureInfo.InvariantCulture);
			}
		}

		/// <summary>
		/// Gets or sets the cookie max age.
		/// Zero or negative values means un-set
		/// </summary>
		/// <value>The max age.</value>
		public long MaxAge
		{
			get
			{
				long res;
				if (!long.TryParse(Settings["Max-Age"], out res))
					res = -1;
				return res;
			}
			set
			{
				if (value <= 0)
					Settings.Remove("Max-Age");
				else
					Settings["Max-Age"] = value.ToString();
			}
		}

		/// <summary>
		/// Gets or sets the cookie secure flag
		/// </summary>
		/// <value>The path.</value>
		public bool Secure
		{
			get { return Settings["Secure"] != null; }
			set 
			{ 
				if (!value)
					Settings.Remove("Secure");
				else
					Settings["Secure"] = string.Empty; 
			}
		}

		/// <summary>
		/// Gets or sets the cookie HttpOnly flag
		/// </summary>
		/// <value>The path.</value>
		public bool HttpOnly
		{
			get { return Settings["HttpOnly"] != null; }
			set 
			{ 
				if (!value)
					Settings.Remove("HttpOnly");
				else
					Settings["HttpOnly"] = string.Empty; 
			}
		}

		/// <summary>
		/// Gets or sets the cookie SameSite attribute.
		/// Expected values are &quot;None&Quot;, &quot;Strict&quot;, or &quot;Lax&quot;.
		/// </summary>
		/// <value>The samesite value</value>
		public string SameSite
		{
			get { return Settings["SameSite"]; }
			set
			{
				if (value == null)
					Settings.Remove("SameSite");
				else
					Settings["SameSite"] = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Ceen.Httpd.ResponseCookie"/> class.
		/// </summary>
		/// <param name="name">The cookie name.</param>
		/// <param name="value">The cookie value.</param>
		public ResponseCookie(string name, string value)
		{
			Name = name;
			Value = value;
			Settings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase).WithDefaultValue(null);
		}
	}
}

