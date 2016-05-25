// Copyright (c) 2016 Feenux LLC, All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System.Collections.Generic;
using System.Data;
using System.Xml;

namespace MTConnect
{
    internal static partial class Tools
    {
        public static class Tables
        {
            public static void CreateColumns(object obj, DataTable table, string prefix)
            {
                System.Reflection.PropertyInfo[] info = obj.GetType().GetProperties();
                string[] ColumnNames = new string[info.Length];
                for (int x = 0; x <= info.Length - 1; x++)
                {
                    ColumnNames[x] = prefix + info[x].Name;
                }
                for (int x = 0; x <= ColumnNames.Length - 1; x++)
                {
                    var col = new DataColumn();
                    col.ColumnName = ColumnNames[x];
                    if (!table.Columns.Contains(col.ColumnName)) table.Columns.Add(col);
                }
            }

            public static DataTable GetEventTypes()
            {
                XmlDocument xml = new XmlDocument();
                xml.LoadXml(Properties.Resources.EventDataValues);

                List<TableInfo> infos = GetData(xml);

                return CreateTable(infos);
            }

            public class TableInfo
            {
                public TableInfo() { }

                public string Name { get; set; }
                public string Value { get; set; }
            }

            static List<TableInfo> GetData(XmlNode xml)
            {
                List<TableInfo> result = new List<TableInfo>();

                foreach (XmlNode node in xml.ChildNodes)
                {
                    if (node.Name.ToLower() == "eventvalues" && node.NodeType == XmlNodeType.Element)
                    {
                        foreach (XmlNode child in node.ChildNodes)
                        {
                            if (child.NodeType == XmlNodeType.Element)
                            {
                                string name = child.Name;

                                foreach (XmlNode valueNode in child.ChildNodes)
                                {
                                    var info = new TableInfo();
                                    info.Name = name;
                                    info.Value = valueNode.Name;
                                    result.Add(info);
                                }
                            }
                        }
                    }
                }

                return result;
            }

            static DataTable CreateTable(List<TableInfo> infos)
            {
                DataTable result = new DataTable();

                result.Columns.Add("NAME");
                result.Columns.Add("VALUE");

                foreach (TableInfo info in infos)
                {
                    DataRow row = result.NewRow();
                    row["NAME"] = info.Name;
                    row["VALUE"] = info.Value;
                    result.Rows.Add(row);
                }

                return result;
            }

        }
    }
}
