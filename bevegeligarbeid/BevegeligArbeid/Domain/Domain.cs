
// =====================================================
// AUTHOR: Jørgen Nyborg & Eirik Stub Mo 
// NOTES: Based on Triona AS' REST API model
//======================================================
using Newtonsoft.Json;
using System.Collections.Generic;

namespace BevegeligArbeid.Domain
{
    public class Location
    {
        public string roadnr { get; set; }
        public int from_hp { get; set; }
        public int to_hp { get; set; }
        public int from_km { get; set; }
        public int to_km { get; set; }
        public string place { get; set; }
        public string stretch { get; set; }
    }

    public class Responsible
    {
        public string email { get; set; }
        public string name { get; set; }
        public object phone { get; set; }
    }

    public class ResponsibleAtLocation
    {
        public string email { get; set; }
        public string name { get; set; }
        public object phone { get; set; }
    }

    public class Address
    {
        public object street { get; set; }
        public int postal_code { get; set; }
    }

    public class Contractor
    {
        public string name { get; set; }
        public Address address { get; set; }
        public string email { get; set; }
    }

    public class Attachment
    {
        public string name { get; set; }
        public string planNr { get; set; }
        public string content_type { get; set; }
    }

    public class WorkPeriod
    {
        public long start { get; set; }
        public long stop { get; set; }
    }

    public class PlanListing
    {
        public string Id { get; set; }
        public int Status { get; set; }
    }

    public class PlanListingList
    {
        [JsonProperty("plans")]
        public ICollection<PlanListing> Plans { get; set; }

        public PlanListingList(ICollection<PlanListing> Plans)
        {
            this.Plans = Plans;
        }
    }

    public class Plan
    {
        public string planNr { get; set; }
        public string sveisNr { get; set; }
        public string decisionNr { get; set; }
        public string description { get; set; }
        public int status { get; set; }
        public Location location { get; set; }
        public Responsible responsible { get; set; }
        public List<ResponsibleAtLocation> responsibleAtLocation { get; set; }
        public string sendTo { get; set; }
        public Contractor contractor { get; set; }
        public List<Attachment> attachments { get; set; }
        public WorkPeriod workPeriod { get; set; }
        public List<string> equipment { get; set; }
    }


    public class PointListingList
    {
        [JsonProperty("points")]
        public ICollection<PointListing> PointList { get; set; }

        public PointListingList(ICollection<PointListing> PointList)
        {
            this.PointList = PointList;
            //App._notificationService.Notify(0, "PointListingList constructed", "" + PointList.Count);
            foreach (PointListing point in PointList)
            {
                //App._notificationService.Notify(0, "Point:", point.id + " " + point.status);
            }
        }
    }

    public class PointListing
    {
        
        public string id { get; set; }
        public int status { get; set; }
     
    }

    public class RoadRef
    {
        public int county { get; set; }
        public int municipality { get; set; }
        public int roadnr { get; set; }
        public string roadStatus { get; set; }
        public string roadCategory { get; set; }
        public int from_hp { get; set; }
        public int to_hp { get; set; }
        public int from_km { get; set; }
        public int to_km { get; set; }
    }

    public class LocationLongLat
    {
        public double longitude { get; set; }
        public double latitude { get; set; }
    }

    public class LogEntry
    {
        public object insertion_time { get; set; }
        public RoadRef roadRef { get; set; }
        [JsonProperty("location")]
        public LocationLongLat location { get; set; }
        public object start_time { get; set; }
        public object stop_time { get; set; }
        public object control_time { get; set; }
        public string equipment { get; set; }
        public string description { get; set; }
        public string sign { get; set; }
        public string setup { get; set; }
    }

	public class Settings
	{
		public int maxSpeed{ get; set;}
		public int timeToLog { get; set;}
    }

    public class LogEntryList
    {
        [JsonProperty("log")]
        public ICollection<LogEntry> LogEntries { get; set; }

        public LogEntryList(ICollection<LogEntry> LogEntries)
        {
            this.LogEntries = LogEntries;
            //App._notificationService.Notify(0, "LogEntryList constructed", "" + LogEntries.Count);
            foreach (LogEntry logEntry in LogEntries)
            {
               //App._notificationService.Notify(0, "LogEntry:", ""+logEntry.location.latitude);
            }
        }
    }
}
