using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MDBA.ConsoleApp
{


    public class FewsPiRoot
    {
        [JsonPropertyName("version")]
        public string Version { get; set; }

        [JsonPropertyName("timeZone")]
        public string TimeZone { get; set; }

        [JsonPropertyName("timeSeries")]
        public List<TimeSeriesItem> TimeSeries { get; set; }
    }

    public class TimeSeriesItem
    {
        [JsonPropertyName("header")]
        public Header Header { get; set; }

        [JsonPropertyName("events")]
        public List<EventItem> Events { get; set; }
    }

    public class Header
    {
        public string Type { get; set; }
        public string ModuleInstanceId { get; set; }
        public string LocationId { get; set; }
        public string ParameterId { get; set; }
        public List<string> QualifierId { get; set; }
        public TimeStep TimeStep { get; set; }
        public DateHolder StartDate { get; set; }
        public DateHolder EndDate { get; set; }
        public string MissVal { get; set; }
        public string StationName { get; set; }
        public string Units { get; set; }

        // Geographical coordinates
        public string Lat { get; set; }
        public string Lon { get; set; }
    }

    public class TimeStep
    {
        public string Unit { get; set; }
        public string Multiplier { get; set; }
    }

    public class DateHolder
    {
        public string Date { get; set; }
        public string Time { get; set; }
    }

    public class EventItem
    {
        [JsonPropertyName("date")]
        public string Date { get; set; }

        [JsonPropertyName("time")]
        public string Time { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("flag")]
        public string Flag { get; set; }
    }
}
