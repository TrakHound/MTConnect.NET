// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using System;
using MTConnect.Devices;
using MTConnect.Devices.DataItems;

namespace MTConnect.Models
{
    public class DataItemModel : DataItem, IDataItemModel
    {
        public DataItemCategory DataItemCategory
        {
            get => Category;
            set => Category = value;
        }

        public string DataItemId
        {
            get => Id;
            set => Id = value;
        }

        public string DataItemName
        {
            get => Name;
            set => Name = value;
        }

        public new double? NativeScale
        {
            get 
            {
                if (base.NativeScale > 0) return base.NativeScale;
                return null;
            }
            set
            {
                if (value.HasValue) base.NativeScale = value.Value;
            }
        }

        public new int? SignificantDigits
        {
            get
            {
                if (base.SignificantDigits > 0) return base.SignificantDigits;
                return null;
            }
            set
            {
                if (value.HasValue) base.SignificantDigits = value.Value;
            }
        }

        public new double? SampleRate
        {
            get
            {
                if (base.SampleRate > 0) return base.SampleRate;
                return null;
            }
            set
            {
                if (value.HasValue) base.SampleRate = value.Value;
            }
        }


        public DataItemModel() { }

        public DataItemModel(IDataItem dataItem)
        {
            if (dataItem != null)
            {
                Category = dataItem.Category;
                Id = dataItem.Id;
                Name = dataItem.Name;
                Type = dataItem.Type;
                SubType = dataItem.SubType;
                NativeUnits = dataItem.NativeUnits;
                NativeScale = dataItem.NativeScale;
                SampleRate = dataItem.SampleRate;
                Source = dataItem.Source;
                Relationships = dataItem.Relationships;
                Representation = dataItem.Representation;
                ResetTrigger = dataItem.ResetTrigger;
                CoordinateSystem = dataItem.CoordinateSystem;
                CoordinateSystemIdRef = dataItem.CoordinateSystemIdRef;
                CompositionId = dataItem.CompositionId;
                Constraints = dataItem.Constraints;
                Definition = dataItem.Definition;
                Units = dataItem.Units;
                Statistic = dataItem.Statistic;
                SignificantDigits = dataItem.SignificantDigits;
                Filters = dataItem.Filters;
                InitialValue = dataItem.InitialValue;
            }
        }

        public static string GetValueString(IDataItemModel dataItem, object value, object nativeValue = null)
        {
            if (dataItem != null)
            {
                if (value.IsNumeric())
                {
                    if (dataItem.SignificantDigits > 0)
                    {
                        return value.ToDouble().ToString("N" + dataItem.SignificantDigits);
                    }
                    else
                    {
                        var val = value.ToDouble();

                        if (nativeValue != null)
                        {
                            var nativeVal = nativeValue.ToDouble();
                            var nativeDigits = Math.Max(nativeVal.GetDigitCountAfterDecimal(), 0);

                            return val.ToString("N" + nativeDigits);
                        }
                    }
                }
            }

            return value?.ToString();
        }
    }
}
