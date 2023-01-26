// Copyright (c) 2023 TrakHound Inc., All Rights Reserved.
// TrakHound Inc. licenses this file to you under the MIT license.

using MTConnect.Configurations;
using MTConnect.Observations.Input;
using MTConnect.Shdr;
using System.Linq;

namespace MTConnect.Adapters.Shdr
{
    /// <summary>
    /// An Adapter class for communicating with an MTConnect Agent using the SHDR protocol.
    /// Supports multiple concurrent Agent connections.
    /// Uses a queue to collect changes to Observations and sends all of the buffered items on demand
    /// </summary>
    public class ShdrQueueAdapter : ShdrAdapter
    {
        private readonly ItemQueue<ShdrDataItem> _dataItemsBuffer = new ItemQueue<ShdrDataItem>();
        private readonly ItemQueue<ShdrMessage> _messagesBuffer = new ItemQueue<ShdrMessage>();
        private readonly ItemQueue<ShdrCondition> _conditionsBuffer = new ItemQueue<ShdrCondition>();
        private readonly ItemQueue<ShdrTimeSeries> _timeSeriesBuffer = new ItemQueue<ShdrTimeSeries>();
        private readonly ItemQueue<ShdrDataSet> _dataSetsBuffer = new ItemQueue<ShdrDataSet>();
        private readonly ItemQueue<ShdrTable> _tablesBuffer = new ItemQueue<ShdrTable>();


        public ShdrQueueAdapter(int port = 7878, int heartbeat = 10000) : base(port, heartbeat) 
        {
            FilterDuplicates = false;
        }

        public ShdrQueueAdapter(string deviceKey, int port = 7878, int heartbeat = 10000) : base(deviceKey, port, heartbeat)
        {
            FilterDuplicates = false;
        }

        public ShdrQueueAdapter(ShdrAdapterConfiguration configuration) : base(configuration)
        {
            FilterDuplicates = false;
        }


        /// <summary>
        /// Sends the buffered items to the Agent
        /// </summary>
        public void SendBuffer()
        {
            WriteBufferDataItems();
            WriteBufferMessages();
            WriteBufferConditions();
            WriteBufferTimeSeries();
            WriteBufferDataSets();
            WriteBufferTables();
        }


        protected override void OnChangedSent()
        {
            // Clear Buffer (to prevent duplicates)
            _dataItemsBuffer.Clear();
            _messagesBuffer.Clear();
            _conditionsBuffer.Clear();
            _timeSeriesBuffer.Clear();
            _dataSetsBuffer.Clear();
            _tablesBuffer.Clear();
        }


        #region "DataItems"

        protected override void OnDataItemAdd(ShdrDataItem dataItem)
        {
            // Add to Buffer
            var key = CreateUniqueId(dataItem);
            _dataItemsBuffer.Add(key, dataItem);
        }

        private bool WriteBufferDataItems(int count = 1000)
        {
            var dataItems = _dataItemsBuffer.Take(count);
            if (!dataItems.IsNullOrEmpty())
            {
                // Create SHDR string to send
                var shdrLine = ShdrDataItem.ToString(dataItems);

                var success = WriteLine(shdrLine);
                if (success)
                {
                    // Update Last Sent DataItems
                    UpdateLastDataItems(dataItems);
                }
            }

            return false;
        }

        #endregion

        #region "Messages"

        protected override void OnMessageAdd(ShdrMessage message)
        {
            // Add to Buffer
            var key = CreateUniqueId(message);
            _messagesBuffer.Add(key, message);
        }

        private bool WriteBufferMessages(int count = 1000)
        {
            var messages = _messagesBuffer.Take(count);
            if (!messages.IsNullOrEmpty())
            {
                var success = false;

                foreach (var message in messages)
                {
                    // Create SHDR string to send
                    var shdrLine = message.ToString();
                    success = WriteLine(shdrLine);
                    if (!success) break;
                }

                if (success)
                {
                    // Update Last Sent Messages
                    UpdateLastMessages(messages);
                }
            }

            return false;
        }

        #endregion

        #region "Conditions"

        protected override void OnConditionAdd(ShdrCondition condition)
        {
            // Add to Buffer
            var key = CreateUniqueId(condition);
            _conditionsBuffer.Add(key, condition);
        }

        private bool WriteBufferConditions(int count = 1000)
        {
            var conditions = _conditionsBuffer.Take(count);
            if (!conditions.IsNullOrEmpty())
            {
                var success = false;

                foreach (var condition in conditions)
                {
                    // Create SHDR string to send
                    var shdrLine = condition.ToString();
                    success = WriteLine(shdrLine);
                    if (!success) break;
                }

                if (success)
                {
                    // Update Last Sent Conditions
                    UpdateLastConditions(conditions);
                }
            }

            return false;
        }

        #endregion

        #region "TimeSeries"

        protected override void OnTimeSeriesAdd(ShdrTimeSeries timeSeries)
        {
            // Add to Buffer
            var key = CreateUniqueId(timeSeries);
            _timeSeriesBuffer.Add(key, timeSeries);
        }

        private bool WriteBufferTimeSeries(int count = 1000)
        {
            var timeSeries = _timeSeriesBuffer.Take(count);
            if (!timeSeries.IsNullOrEmpty())
            {
                var success = false;

                foreach (var item in timeSeries)
                {
                    // Create SHDR string to send
                    var shdrLine = item.ToString();
                    success = WriteLine(shdrLine);
                    if (!success) break;
                }

                if (success)
                {
                    // Update Last Sent TimeSeries
                    UpdateLastTimeSeries(timeSeries);
                }
            }

            return false;
        }

        #endregion

        #region "DataSet"

        protected override void OnDataSetAdd(ShdrDataSet dataSet)
        {
            // Add to Buffer
            var key = CreateUniqueId(dataSet);
            _dataSetsBuffer.Add(key, dataSet);
        }

        private bool WriteBufferDataSets(int count = 1000)
        {
            var dataSets = _dataSetsBuffer.Take(count);
            if (!dataSets.IsNullOrEmpty())
            {
                var success = false;

                foreach (var item in dataSets)
                {
                    // Create SHDR string to send
                    var shdrLine = item.ToString();
                    success = WriteLine(shdrLine);
                    if (!success) break;
                }

                if (success)
                {
                    // Update Last Sent DataSet
                    UpdateLastDataSets(dataSets);
                }
            }

            return false;
        }

        #endregion

        #region "Table"

        protected override void OnTableAdd(ShdrTable table)
        {
            // Add to Buffer
            var key = CreateUniqueId(table);
            _tablesBuffer.Add(key, table);
        }

        private bool WriteBufferTables(int count = 1000)
        {
            var tables = _tablesBuffer.Take(count);
            if (!tables.IsNullOrEmpty())
            {
                var success = false;

                foreach (var item in tables)
                {
                    // Create SHDR string to send
                    var shdrLine = item.ToString();
                    success = WriteLine(shdrLine);
                    if (!success) break;
                }

                if (success)
                {
                    // Update Last Sent Table
                    UpdateLastTables(tables);
                }
            }

            return false;
        }

        #endregion



        private static ulong CreateUniqueId(IObservationInput observationInput)
        {
            if (observationInput != null)
            {
                var hashBytes = observationInput.ChangeIdWithTimestamp;
                var hash = string.Concat(hashBytes.Select(b => b.ToString("x2")));
                return hash.GetUInt64Hash();
            }

            return 0;
        }

        private static ulong CreateUniqueId(ShdrCondition condition)
        {
            if (condition != null)
            {
                var hashBytes = condition.ChangeIdWithTimestamp;
                var hash = string.Concat(hashBytes.Select(b => b.ToString("x2")));
                return hash.GetUInt64Hash();
            }

            return 0;
        }
    }
}