using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace MTConnect.Serialization
{
    /// <summary>
    /// Utility service for serialization.
    /// </summary>
    public sealed class SerializableExtraTypes
    {
        /// <summary>
        /// Serialize an object
        /// </summary>
        /// <param name="obj">obj to serialize</param>
        /// <param name="root">the root type for extraTypes reference</param>
        /// <returns></returns>
        public static string Serialize(object obj, Type root)
        {
            return Serialize(obj, Formatting.None, root);
        }

        /// <summary>
        /// Serialize an object
        /// </summary>
        /// <param name="obj">object to serialize</param>
        /// <param name="format">Format to serialize with</param>
        /// <param name="root">the root type for an extraTypes reference</param>
        /// <returns></returns>
        public static string Serialize(object obj, Formatting format, Type root)
        {
            XmlSerializer s = new XmlSerializer(obj.GetType(), SerializableExtraTypes.GetTypes(root));
            Stream stream = new MemoryStream();
            XmlTextWriter xw = new XmlTextWriter(stream, null);
            xw.Formatting = format;
            s.Serialize(xw, obj);
            stream.Position = 0;
            StreamReader rdr = new StreamReader(stream);
            return rdr.ReadToEnd();
        }

        /// <summary>
        /// Deserialize an object
        /// </summary>
        /// <param name="xml">xml of the object to deserialize</param>
        /// <param name="objType">type of object to deserialize</param>
        /// <param name="root">the root type for extraTypes reference</param>
        /// <returns></returns>
        public static object Deserialize(string xml, Type objType, Type root)
        {
            XmlSerializer serializer = new XmlSerializer(objType, SerializableExtraTypes.GetTypes(root));
            StringReader reader = new StringReader(xml);
            return serializer.Deserialize(reader);
        }

        /// <summary>
        /// Register a Type a related to another type for serialization.
        /// </summary>
        /// <param name="root">the Type to be related to.</param>
        /// <param name="relatedType">the Type that will be related</param>
        public static void RegisterType(Type root, Type relatedType)
        {
            // validate that type is not an interface
            if (root.IsInterface || relatedType.IsInterface)
                throw new NotSupportedException("TripleI.Framework.Xml.SerializationRegister.RegisterType(Type root, Type relatedType):: root and relatedType do not allow Interface Type definitions");

            // get registration node
            RegistrationNode node = Instance.InnerList.GetNode(root);
            if (node == null)
            {
                node = new RegistrationNode(root);
                Instance.InnerList.Add(node);
            }
            // add current type
            if (!relatedType.IsAbstract)
                if (!node.Types.Contains(relatedType))
                    node.Types.Add(relatedType);

            // go through asseblies and add children.
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
                foreach (Type t in assembly.GetTypes())
                    if (t.IsSubclassOf(relatedType) && !t.IsAbstract && !node.Types.Contains(t) && !t.IsGenericTypeDefinition) node.Types.Add(t);
        }
        /// <summary>
        /// Get types that have been related to the root Type
        /// </summary>
        /// <param name="root">the root type</param>
        /// <returns>related types</returns>
        public static Type[] GetTypes(Type root)
        {
            List<Type> returner = new List<Type>(Instance.InnerList.GetRelatedTypes(root));
            for (int i = 0; i < returner.Count; ++i)
                AddRelatedTypes(returner, returner[i]);
            return returner.ToArray();
        }

        /// <summary>
        /// private empty constructor for singleton creation
        /// </summary>
        private SerializableExtraTypes() { InitializeService(); }

        /// <summary>
        /// private instance Singlton
        /// </summary>
        private static SerializableExtraTypes Instance = new SerializableExtraTypes();

        /// <summary>
        /// Initializes the instance to discover types and related types.
        /// </summary>
        private void InitializeService()
        {
            AppDomain.CurrentDomain.AssemblyLoad += new AssemblyLoadEventHandler(CurrentDomain_AssemblyLoad);
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
                this.InitialzeParseAssembly(assembly);
        }
        /// <summary>
        /// Internal register method.
        /// </summary>
        /// <param name="root">the Type to be related to.</param>
        /// <param name="relatedType">the Type that will be related</param>
        private void InitialzeRegisterType(Type root, Type relatedType)
        {
            // validate that type is not an interface
            if (root.IsInterface || relatedType.IsInterface)
                throw new NotSupportedException("TripleI.Framework.Xml.SerializationRegister.RegisterType(Type root, Type relatedType):: root and relatedType do not allow Interface Type definitions");

            // get registration node
            RegistrationNode node = InnerList.GetNode(root);
            if (node == null)
            {
                node = new RegistrationNode(root);
                InnerList.Add(node);
            }

            // add current type
            if (!relatedType.IsAbstract)
                if (!node.Types.Contains(relatedType))
                    node.Types.Add(relatedType);

            // go through asseblies and add children.
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (Assembly assembly in assemblies)
                foreach (Type t in assembly.GetTypes())
                    if (t.IsSubclassOf(relatedType) && !t.IsAbstract && !node.Types.Contains(t) && !t.IsGenericTypeDefinition) node.Types.Add(t);
        }
        /// <summary>
        /// Searches an assembly for registerable types durring initialization.
        /// </summary>
        /// <param name="assembly">assembly to be searched</param>
        private void InitialzeParseAssembly(Assembly assembly)
        {
            foreach (Type t in assembly.GetTypes())
            {
                object[] objects = t.GetCustomAttributes(typeof(SerializableExtraTypeAttribute), false);
                if (objects != null)
                    foreach (object o in objects)
                    {
                        SerializableExtraTypeAttribute attr = (SerializableExtraTypeAttribute)o;
                        foreach (Type tp in attr.Types)
                            this.InitialzeRegisterType(tp, t);
                    }
            }
        }

        /// <summary>
        /// Searches an assembly for registerable types.
        /// </summary>
        /// <param name="assembly">assembly to be searched</param>
        private static void ParseAssembly(Assembly assembly)
        {
            foreach (Type t in assembly.GetTypes())
            {
                object[] objects = t.GetCustomAttributes(typeof(SerializableExtraTypeAttribute), false);
                if (objects != null)
                    foreach (object o in objects)
                    {
                        SerializableExtraTypeAttribute attr = (SerializableExtraTypeAttribute)o;
                        foreach (Type tp in attr.Types)
                            RegisterType(tp, t);
                    }
            }
        }
        /// <summary>
        /// Event handler
        /// </summary>
        static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            ParseAssembly(args.LoadedAssembly);
        }

        /// <summary>
        /// Collection of registered types and their related types.
        /// </summary>
        private RegistrationCollection InnerList = new RegistrationCollection();

        /// <summary>
        /// Adds a list of types to be related to the root type.
        /// </summary>
        /// <param name="list">Types to be related</param>
        /// <param name="root">Type to be related to.</param>
        private static void AddRelatedTypes(List<Type> list, Type root)
        {
            List<Type> related = new List<Type>(Instance.InnerList.GetRelatedTypes(root));
            foreach (Type t in related)
                if (!list.Contains(t)) list.Add(t);
        }

        /// <summary>
        /// Holder for registerd types.
        /// </summary>
        private class RegistrationNode
        {
            public RegistrationNode(Type type) { _Key = type; }
            private Type _Key;
            public Type Key { get { return _Key; } }
            private List<Type> _Types = new List<Type>();
            public List<Type> Types { get { return _Types; } }
        }

        /// <summary>
        /// Collection for registered types.
        /// </summary>
        private class RegistrationCollection : List<RegistrationNode>
        {
            public RegistrationCollection() { }
            public RegistrationNode GetNode(Type type)
            {
                foreach (RegistrationNode rn in this)
                    if (rn.Key.Equals(type)) return rn;
                return null;

            }

            public Type[] GetRelatedTypes(Type type)
            {
                foreach (RegistrationNode rn in this)
                    if (rn.Key.Equals(type)) return rn.Types.ToArray();
                return Type.EmptyTypes;
            }

            public bool ContainsNode(Type type)
            {
                foreach (RegistrationNode rn in this)
                    if (rn.Key.Equals(type)) return true;
                return false;
            }
        }
    }
}

