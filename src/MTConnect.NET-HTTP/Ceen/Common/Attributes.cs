using System;
namespace Ceen
{
	/// <summary>
	/// An attribute used to require a dependency on a particular handler.
	/// This can be used to force an authentication or cookie module to
	/// be loaded before a method can be called
	/// </summary>
	public class RequireHandlerAttribute : Attribute
	{
		/// <summary>
		/// Gets or sets a value indicating whether derived handlers are allowed.
		/// </summary>
		public bool AllowDerived { get; set; } = true;
		/// <summary>
		/// Gets or sets the type of the required module.
		/// </summary>
		public Type RequiredType { get; set; }

		/// <summary>
		/// Creates a new handler attribute
		/// </summary>
		/// <value>The type of the required module.</value>
		public RequireHandlerAttribute(Type requiredtype)
		{
			RequiredType = requiredtype;
		}
	}
}
