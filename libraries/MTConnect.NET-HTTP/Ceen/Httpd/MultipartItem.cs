using System;
using System.Collections.Generic;
using System.IO;

namespace Ceen.Httpd
{
	/// <summary>
	/// Interface for a multipart item
	/// </summary>
	public class MultipartItem : IMultipartItem
	{
		/// <summary>
		/// The headers associated with the item
		/// </summary>
		/// <value>The headers.</value>
		public IDictionary<string, string> Headers { get; private set; }

		/// <summary>
		/// Gets or sets the form name.
		/// </summary>
		/// <value>The name.</value>
		public string Name { get; set; }
		/// <summary>
		/// Gets or sets the filename.
		/// </summary>
		/// <value>The filename.</value>
		public string Filename { get; set; }
		/// <summary>
		/// Gets the Content-Type header value.
		/// </summary>
		/// <value>The type of the content.</value>
		public string ContentType { get { return Headers["Content-Type"]; } }
		/// <summary>
		/// The data for this entry
		/// </summary>
		/// <value>The data.</value>
		public Stream Data { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="Ceen.Httpd.MultipartItem"/> class.
		/// </summary>
		public MultipartItem()
		{
			this.Headers = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase).WithDefaultValue(null);
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Ceen.Httpd.MultipartItem"/> class.
		/// </summary>
		/// <param name="headers">The headers present on the multipart entry.</param>
		public MultipartItem(IDictionary<string, string> headers)
		{
			this.Headers = headers;
		}
	}
}

