﻿//
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
//

#if NETSTANDARD2_0
using System;
#endif
using System.Diagnostics.Tracing;
using Microsoft.SqlTools.Utility;

namespace Microsoft.SqlTools.ServiceLayer.Utility
{

    /// <summary>
    /// This listener class will listen for events from the SqlClientEventSource class
    /// and forward them to the logger.
    /// </summary>
    public class SqlClientListener : EventListener
    {
        protected override void OnEventSourceCreated(EventSource eventSource)
        {
            // Only enable events from SqlClientEventSource.
            if (eventSource.Name.Equals("Microsoft.Data.SqlClient.EventSource"))
            {
                // Use EventKeyWord 2 to capture basic application flow events.
                // See https://docs.microsoft.com/sql/connect/ado-net/enable-eventsource-tracing for all available keywords.
                EnableEvents(eventSource, EventLevel.Informational, (EventKeywords)2);
            }
        }

        /// <summary>
        /// This callback runs whenever an event is written by SqlClientEventSource.
        /// Event data is accessed through the EventWrittenEventArgs parameter.
        /// </summary>
        /// <param name="eventData">The data for the event</param>
        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            // Skip EventCounters as they can come from any event source and will pollute traces captured.
            if (eventData.Payload == null || eventData.EventName.Equals(nameof(EventCounter)))
            {
                return;
            }

            foreach (object payload in eventData.Payload)
            {
                if (payload != null)
                {
#if NETSTANDARD2_0_OR_GREATER
                    Logger.Verbose($"eventTID:{Environment.CurrentManagedThreadId} {payload.ToString()}");
#else
                    Logger.Verbose($"eventTID:{eventData.OSThreadId} {payload.ToString()}");
#endif
                }
            }
        }
    }
}
