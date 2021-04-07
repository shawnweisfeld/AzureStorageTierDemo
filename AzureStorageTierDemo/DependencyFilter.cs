﻿using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.ApplicationInsights.Extensibility;

namespace AzureStorageTierDemo
{
    public class DependencyFilter : ITelemetryProcessor
    {
        private ITelemetryProcessor Next { get; set; }

        // next will point to the next TelemetryProcessor in the chain.
        public DependencyFilter(ITelemetryProcessor next)
        {
            Next = next;
        }

        public void Process(ITelemetry item)
        {
            var dependencyTelemetry = item as DependencyTelemetry;

            if (dependencyTelemetry != null
                && dependencyTelemetry.Type != null
                && (dependencyTelemetry.Type == "Azure blob"
                    || dependencyTelemetry.Type == "Http"
                    || dependencyTelemetry.Type == "InProc | Microsoft.Storage"))
            {
                return;
            }

            Next.Process(item);
        }
    }
}