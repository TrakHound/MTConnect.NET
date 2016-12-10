// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using System.Xml;

namespace MTConnect
{
    internal static partial class Tools
    {
        public static class XML
        {
            public static string GetAttribute(XmlNode node, string attributeName)
            {
                if (node.Attributes != null)
                {

                    string attrName = attributeName;
                    if (attrName.Length > 0) attrName = attrName.Remove(0, 1).Insert(0, attrName[0].ToString().ToLower());

                    var nameAttribute = node.Attributes[attrName];
                    if (nameAttribute != null)
                        return nameAttribute.Value;
                    else
                        return "";
                }
                else
                    return "";
            }

            public static void AssignProperties(Object obj, XmlNode node)
            {
                foreach (System.Reflection.PropertyInfo info in obj.GetType().GetProperties())
                {
                    string value = GetAttribute(node, info.Name);
                    if (value != "")
                    {
                        Type t = info.PropertyType;

                        // Make sure DateTime gets set as UTC
                        if (t == typeof(DateTime))
                        {
                            DateTime dt = UTC.FromString(value);
                            info.SetValue(obj, dt, null);
                        }
                        else if (t.IsEnum)
                        {
                            try
                            {
                                var val = Enum.Parse(t, value, true);
                                info.SetValue(obj, val, null);
                            }
                            catch (Exception ex) { Log.Write("Exception :: " + ex.Message); }
                        }
                        else
                        {
                            info.SetValue(obj, Convert.ChangeType(value, t), null);
                        }
                    }
                }
            }

            public static T ParseEnum<T>(string value)
            {
                return (T)Enum.Parse(typeof(T), value, true);
            }

        }
    }
}
