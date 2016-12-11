using System;
using System.Collections.Generic;
using System.Text;

namespace MTConnect.Serialization
{
    /// <summary>
    /// Associates this class as an available extra Type for serialization. Use with TripleI.Framework.Xml.SerializableExtraTypes
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public sealed class SerializableExtraTypeAttribute : Attribute
    {
        /// <summary>
        /// Associates the adorned class with the given Type.  Any derived classes will also be associated.
        /// </summary>
        /// <param name="type">The Type that the class will be associated with. Type of interface is not supported.</param>
        public SerializableExtraTypeAttribute(Type type)
            : base()
        { _Types.Add(type); }

        /// <summary>
        /// Associates the adorned class with the given Types.  Any derived classes will also be associated.
        /// </summary>
        /// <param name="types">The Types that the class will be associated with. Type of interface is not supported.</param>
        public SerializableExtraTypeAttribute(Type[] types)
            : base()
        { _Types.AddRange(types); }

        private List<Type> _Types = new List<Type>();

        /// <summary>
        /// The list of Types that the adorned class is associated with.
        /// </summary>
        public Type[] Types { get { return _Types.ToArray(); } }
    }
}

