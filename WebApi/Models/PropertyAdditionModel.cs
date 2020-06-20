using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    public class PropertyAdditionModel
    {
        public string PropertyName { get; set; }
        public int PropertyType { get; set; }
        public List<int> PropertyFacilities { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string Area { get; set; }
        public int LocationId { get; set; }
        public LocationDetails Location { get; set; }
        public int Country { get; set; }//Add CountryId, will come from list of countries
        public int State { get; set; }//Add StateId, will come from list of States
        public string ZipCode { get; set; }
        public int No_Of_Rooms { get; set; }
        public List<RoomAdditionModel> Rooms { get; set; }
        public string Check_In_Time { get; set; }
        public string Check_Out_Time { get; set; }
    }
    public class RoomAdditionModel
    {
        public int RoomType { get; set; }
        public int Max_Adult { get; set; }
        public int Max_Child { get; set; }
        public int Max_Occupant { get; set; }
        public int Floor { get; set; }
        public List<int> RoomFacilities { get; set; }
        public decimal RoomDefaultTariff { get; set; }
    }
    public class LocationDetails
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
        
}