// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

namespace MTConnect.Devices.Configurations.ImageFiles
{
	/// <summary>
	/// Reference to a file containing an image of the Component
	/// </summary>
	public class ImageFile : IImageFile
	{
		/// <summary>
		/// Unique identifier of the image file
		/// </summary>
		public string Id { get; set; }

		/// <summary>
		/// URL giving the location of the image file.
		/// </summary>
		public string Href { get; set; }

		/// <summary>
		/// Mime type of the image file.
		/// </summary>
		public string MediaType { get; set; }

		/// <summary>
		/// Description of the image file.
		/// </summary>
		public string Name { get; set; }
	}
}
