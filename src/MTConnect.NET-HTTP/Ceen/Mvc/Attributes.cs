using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Ceen.Mvc
{
	/// <summary>
	/// Attribute for renaming an elements route
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)]
	public class RouteAttribute : Attribute
	{
		/// <summary>
		/// Gets or sets the route.
		/// </summary>
		/// <value>The route.</value>
		public string Route { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ceen.Mvc.RouteAttribute"/> class.
		/// </summary>
		/// <param name="route">The route expression to use.</param>
		public RouteAttribute(string route)
		{
			Route = route;
		}
	}

	/// <summary>
	/// Attribute for renaming an elements route
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Interface, Inherited = false, AllowMultiple = false)]
	public class NameAttribute : Attribute 
	{ 
		/// <summary>
		/// Gets or sets the name this routing entry uses
		/// </summary>
		/// <value>The path.</value>
		public string Name { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ceen.Mvc.NameAttribute"/> class.
		/// </summary>
		/// <param name="name">The name to use</param>
		public NameAttribute(string name)
		{
			Name = name;
		}
	}

	/// <summary>
	/// Flags that describe the allowed sources for arguments
	/// </summary>
	[Flags]
	public enum ParameterSource
	{
		None = 0x0,
		Url = 0x1,
		Query = 0x2,
		Form = 0x4,
		Header = 0x8,
        Body = 0xF,
		Default = Url | Query | Form | Body,
		Request = Query | Form,
		Any = Url | Query | Form | Request | Header | Body
	}

	[AttributeUsage(AttributeTargets.Parameter, AllowMultiple = false)]
	public class ParameterAttribute : Attribute
	{
		/// <summary>
		/// Gets or sets the allowed sources for the entry
		/// </summary>
		/// <value>The source.</value>
		public ParameterSource Source { get; set; }
        /// <summary>
        /// Gets or sets the name of the entry, defaults to the name or the method argument
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }
		/// <summary>
		/// Gets or sets a value indicating whether this argument is required.
		/// </summary>
		public bool Required { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ceen.Mvc.ParameterAttribute"/> class.
		/// </summary>
		/// <param name="source">Sets the allowed sources.</param>
		/// <param name="required">A flag indicating if the parameter is required.</param>
		public ParameterAttribute(ParameterSource source = ParameterSource.Default, bool required = true, string name = null)
		{
			Source = source;
			Required = required;
            Name = name;
		}
	}

	/// <summary>
	/// Base http verb filter attribute
	/// </summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class HttpVerbFilterAttribute : Attribute
	{
		/// <summary>
		/// Gets or sets the verb this filter represents.
		/// </summary>
		/// <value>The verb.</value>
		public string Verb { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Ceen.Mvc.HttpVerbFilterAttribute"/> class.
		/// </summary>
		/// <param name="verb">The verb to assign.</param>
		public HttpVerbFilterAttribute(string verb)
		{
			this.Verb = verb;
		}
	}

	/// <summary>
	/// Http GET verb filter attribute.
	/// </summary>
	public class HttpGetAttribute : HttpVerbFilterAttribute
	{
		public HttpGetAttribute() : base("GET") { }
	}

	/// <summary>
	/// Http POST verb filter attribute.
	/// </summary>
	public class HttpPostAttribute : HttpVerbFilterAttribute
	{
		public HttpPostAttribute() : base("POST") { }
	}

	/// <summary>
	/// Http PUT verb filter attribute.
	/// </summary>
	public class HttpPutAttribute : HttpVerbFilterAttribute
	{
		public HttpPutAttribute() : base("PUT") { }
	}

	/// <summary>
	/// Http DELETE verb filter attribute.
	/// </summary>
	public class HttpDeleteAttribute : HttpVerbFilterAttribute
	{
		public HttpDeleteAttribute() : base("DELETE") { }
	}

	/// <summary>
	/// Http HEAD verb filter attribute.
	/// </summary>
	public class HttpHeadAttribute : HttpVerbFilterAttribute
	{
		public HttpHeadAttribute() : base("HEAD") { }
	}

	/// <summary>
	/// Http PATCH verb filter attribute.
	/// </summary>
	public class HttpPatchAttribute : HttpVerbFilterAttribute
	{
		public HttpPatchAttribute() : base("PATCH") { }
	}

	/// <summary>
	/// Http MKCOL verb filter attribute.
	/// </summary>
	public class HttpMkColAttribute : HttpVerbFilterAttribute
	{
		public HttpMkColAttribute() : base("MKCOL") { }
	}

	/// <summary>
	/// Http PROPFIND verb filter attribute.
	/// </summary>
	public class HttpPropFindAttribute : HttpVerbFilterAttribute
	{
		public HttpPropFindAttribute() : base("PROPFIND") { }
	}

}
