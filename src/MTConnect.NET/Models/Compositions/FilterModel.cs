// Copyright (c) 2022 TrakHound Inc., All Rights Reserved.

// This file is subject to the terms and conditions defined in
// file 'LICENSE.txt', which is part of this source code package.

using MTConnect.Devices.Compositions;
using MTConnect.Devices.Events;
using MTConnect.Devices.Samples;
using MTConnect.Models.DataItems;
using MTConnect.Streams.Samples;
using System;

namespace MTConnect.Models.Compositions
{
    /// <summary>
    /// Any substance or structure through which liquids or gases are passed to remove suspended impurities or to recover solids.
    /// </summary>
    public class FilterModel : CompositionModel, IFilterModel
    {
        /// <summary>
        /// The measurement of accumulated time for an activity or event.
        /// </summary>
        public AccumulatedTimeValue AccumulatedTime
        {
            get => GetSampleValue<AccumulatedTimeValue>(Devices.Samples.AccumulatedTimeDataItem.NameId);
            set => AddDataItem(new AccumulatedTimeDataItem(Id), value);
        }
        public IDataItemModel AccumulatedTimeDataItem => GetDataItem(Devices.Samples.AccumulatedTimeDataItem.NameId);


        /// <summary>
        /// The time and date code associated with a material or other physical item.
        /// </summary>
        public DateCodeModel DateCode
        {
            get => GetDateCode();
            set => SetDateCode(value);
        }


        public FilterModel() 
        {
            Type = FilterComposition.TypeId;
        }

        public FilterModel(string compositionId)
        {
            Id = compositionId;
            Type = FilterComposition.TypeId;
        }


        protected DateCodeModel GetDateCode()
        {
            var x = new DateCodeModel();

            // Manufacture
            x.Manufacture = GetStringValue(DateCodeDataItem.NameId, DateCodeDataItem.GetSubTypeId(DateCodeDataItem.SubTypes.MANUFACTURE)).ToDateTime();
            x.ManufactureDataItem = GetDataItem(DateCodeDataItem.NameId, DateCodeDataItem.GetSubTypeId(DateCodeDataItem.SubTypes.MANUFACTURE));

            // Expiration
            x.Expiration = GetStringValue(DateCodeDataItem.NameId, DateCodeDataItem.GetSubTypeId(DateCodeDataItem.SubTypes.EXPIRATION)).ToDateTime();
            x.ExpirationDataItem = GetDataItem(DateCodeDataItem.NameId, DateCodeDataItem.GetSubTypeId(DateCodeDataItem.SubTypes.EXPIRATION));

            // First Use
            x.FirstUse = GetStringValue(DateCodeDataItem.NameId, DateCodeDataItem.GetSubTypeId(DateCodeDataItem.SubTypes.FIRST_USE)).ToDateTime();
            x.FirstUseDataItem = GetDataItem(DateCodeDataItem.NameId, DateCodeDataItem.GetSubTypeId(DateCodeDataItem.SubTypes.FIRST_USE));

            return x;
        }

        protected void SetDateCode(DateCodeModel model)
        {
            if (model != null)
            {
                if (model.Manufacture > DateTime.MinValue)
                {
                    // Manufacture
                    AddDataItem(new DateCodeDataItem(Id, DateCodeDataItem.SubTypes.MANUFACTURE), model.Manufacture.ToString("o"));
                }

                if (model.Expiration > DateTime.MinValue)
                {
                    // Expiration
                    AddDataItem(new DateCodeDataItem(Id, DateCodeDataItem.SubTypes.EXPIRATION), model.Expiration.ToString("o"));
                }

                if (model.FirstUse > DateTime.MinValue)
                {
                    // First Use
                    AddDataItem(new DateCodeDataItem(Id, DateCodeDataItem.SubTypes.FIRST_USE), model.FirstUse.ToString("o"));
                }
            }
        }
    }
}
