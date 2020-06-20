using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    public class PropertyDetailsModel
    {
        public long PropertyId { get; set; }
        public int PropertyTypeId { get; set; }
        public string PropertyType { get; set; }
        public string PropertyName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public int StateId { get; set; }
        public int CountryId { get; set; }
        public string ZipCode { get; set; }
        public string Area { get; set; }
        public int No_Of_Rooms { get; set; }
        public string Check_In_Time { get; set; }
        public string Check_Out_Time { get; set; }
        public List<FacilityModel> PropertyFacilities { get; set; }
        public List<RoomDetailsModel> RoomDetails { get; set; }
    }
    public class RoomDetailsModel
    {
        public long RoomId { get; set; }
        public int RoomTypeId { get; set; }
        public string RoomType { get; set; }
        public List<FacilityModel> RoomFacilities { get; set; }
        public List<RoomTariffModel> RoomTariffs { get; set; }
    }

    public class RoomTariffModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal Tariff { get; set; }
        public decimal Discount_Percent { get; set; }
    }
}