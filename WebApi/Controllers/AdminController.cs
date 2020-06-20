using Core.Lookup;
using Core.Property;
using Repository.Property;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi.Models;

namespace WebApi.Controllers
{
    [RoutePrefix("api/Admin")]
    //[Authorize]
    public class AdminController : ApiController
    {
        private ILookupViewRepository _lookupviewRepo;
        private IPropertyUserRepository _propertyUserRepo;
        private IPropertyRepository _propertyRepo;
        private IPropertyFacilityRepository _propertyFacilityRepo;
        private IPropertyDetailsViewRepository _propertyDetailsViewRepo;
        private IRoomRepository _roomRepo;
        private IRoomFacilityRepository _roomFacilityRepo;
        private IRoomTariffRepository _roomTariffRepo;
        private IRoomDetailsViewRepository _roomDetailsViewRepo;

        public AdminController(ILookupViewRepository lookupviewRepo, IPropertyUserRepository propertyUserRepo, IPropertyRepository propertyRepo, 
            IPropertyFacilityRepository propertyFacilityRepo, IPropertyDetailsViewRepository propertyDetailsViewRepo, IRoomRepository roomRepo, 
            IRoomFacilityRepository roomFacilityRepo, IRoomTariffRepository roomTariffRepo, IRoomDetailsViewRepository roomDetailsViewRepo)
        {
            _lookupviewRepo = lookupviewRepo;
            _propertyUserRepo = propertyUserRepo;
            _propertyRepo = propertyRepo;
            _propertyFacilityRepo = propertyFacilityRepo;
            _propertyDetailsViewRepo = propertyDetailsViewRepo;
            _roomRepo = roomRepo;
            _roomFacilityRepo = roomFacilityRepo;
            _roomTariffRepo = roomTariffRepo;
            _roomDetailsViewRepo = roomDetailsViewRepo;
        }
        [Route("GetPropertyTypes")]
        [HttpGet]
        public IHttpActionResult GetPropertyTypes()
        {
            IEnumerable<LookUpViewSnapshot> res;
            try
            {
                Expression<Func<LookUpViewSnapshot, bool>> propertyexpr = s => s.LookUpType.Trim().ToUpper() == "PROPERTY TYPE";
                res = _lookupviewRepo.GetFilteredLookupViewDetails(propertyexpr);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Ok(new APIResponseModel<IEnumerable<LookUpViewSnapshot>> { Data = res, Msg = "Successfully retreived property types!!" });
        }        

        [Route("AddProperty")]
        [HttpPost]
        public IHttpActionResult AddProperty([FromBody]IEnumerable<PropertyAdditionModel> properties)
        {
            bool res=false;
            string Msg = string.Empty;
            DateTime now = DateTime.Now;
            var UserId = 1;//To be replaced by logged in user
            try
            {
                foreach(PropertyAdditionModel prop in properties)
                {
                    Expression<Func<PropertyDetailsViewSnapshot, bool>> propexpr = s => s.Ownerid == UserId && s.PropertyName.Trim().ToUpper() == prop.PropertyName.Trim().ToUpper();
                    bool isexisitngProp = _propertyDetailsViewRepo.GetFilteredPropertyDetailsViewDetails(propexpr).Count() > 0 ? true : false;
                    if (isexisitngProp)
                    {
                        Msg += "Property "+ prop.PropertyName +" is already registered by you. Please change the name or edit the existing property details!!";
                    }
                    else
                    {
                        //Adding the property
                        PropertySnapshot newProp = new PropertySnapshot
                        {
                            PropertyName = prop.PropertyName,
                            PropertyType = prop.PropertyType,
                            AddressLine1 = prop.AddressLine1,
                            AddressLine2 = prop.AddressLine2,
                            AddressLine3 = prop.AddressLine3,
                            Area = prop.Area,
                            ZipCode = prop.ZipCode,
                            StateId = prop.State,
                            CountryId = prop.Country,
                            IsActive = true,
                            No_Of_Rooms = prop.No_Of_Rooms,
                            Check_In_Time = prop.Check_In_Time,
                            Check_Out_Time = prop.Check_Out_Time,
                            CreatedBy = UserId,
                            CreatedOn = now,
                            LastUpdatedBy = UserId,
                            LastUpdatedOn = now
                        };
                        newProp.PropertyId = _propertyRepo.AddProperty(newProp);
                        //Adding Property for Owner
                        PropertyUserSnapshot newpropuser = new PropertyUserSnapshot
                        {
                            PropertyId = newProp.PropertyId,
                            OwnerId = UserId,
                            IsActive = true,
                            CreatedBy = UserId,
                            CreatedOn = now,
                            LastUpdatedBy = UserId,
                            LastUpdatedOn = now
                        };
                        newpropuser.Id = _propertyUserRepo.AddPropertyUser(newpropuser);
                        //Adding property facilities
                        foreach(int propfacility in prop.PropertyFacilities)
                        {
                            PropertyFacilitySnapshot newpropfacility = new PropertyFacilitySnapshot
                            {
                                PropertyId = newProp.PropertyId,
                                FacilityId = propfacility,
                                IsActive = true,
                                CreatedBy = UserId,
                                CreatedOn = now,
                                LastUpdatedBy = UserId,
                                LastUpdatedOn = now
                            };
                            newpropfacility.Id = _propertyFacilityRepo.AddPropertyFacility(newpropfacility);
                        }
                        Msg += "Successfully created new property with its facilities!!\n";

                        //Adding Rooms for the property
                        foreach (RoomAdditionModel room in prop.Rooms)
                        {
                            RoomSnapshot newroom = new RoomSnapshot
                            {
                                PropertyId = newProp.PropertyId,
                                RoomType = room.RoomType,
                                Floor = room.Floor,
                                Max_Adult = room.Max_Adult,
                                Max_Children = room.Max_Child,
                                Max_Occupant = room.Max_Occupant,
                                IsActive = true,
                                CreatedBy = UserId,
                                CreatedOn = now,
                                LastUpdatedBy = UserId,
                                LastUpdatedOn = now
                            };
                            newroom.RoomId = _roomRepo.AddRoom(newroom);
                            //Adding Rooms for the property
                            foreach(int roomfacility in room.RoomFacilities)
                            {
                                RoomFacilitySnapshot newroomfacility = new RoomFacilitySnapshot
                                {
                                    RoomId = newroom.RoomId,
                                    FacilityId = roomfacility,
                                    IsActive = true,
                                    CreatedBy = UserId,
                                    CreatedOn = now,
                                    LastUpdatedBy = UserId,
                                    LastUpdatedOn = now
                                };
                                newroomfacility.Id = _roomFacilityRepo.AddRoomFacility(newroomfacility);
                            }
                            //Adding Room default tariff
                            RoomTariffSnapshot roomTariff = new RoomTariffSnapshot
                            {
                                RoomId = newroom.RoomId,
                                StartDate = now,
                                EndDate = DateTime.MaxValue,
                                Tariff = room.RoomDefaultTariff,
                                Discount_Percent = 0,
                                IsDefault = true,
                                CreatedBy = UserId,
                                CreatedOn = now,
                                LastUpdatedBy = UserId,
                                LastUpdatedOn = now                               
                            };
                            roomTariff.Id = _roomTariffRepo.AddRoomTariff(roomTariff);
                        }
                        Msg += "Successfully added " + prop.Rooms.Count.ToString() + " new room(s) to the property with the room facilities and default tariff!!\n";                        
                    }
                }
                
                res = true;
            }
            catch (Exception ex)
            {
                res = false;
                throw ex;
            }
            return Ok(new APIResponseModel<bool> { Data = res, Msg = Msg});
        }

        [Route("GetPropertyDetails/{PropertyId:long?}")]
        [HttpGet]
        public IHttpActionResult GetPropertyDetails([FromUri]long PropertyId = 0)
        {
            List<PropertyDetailsModel> res = new List<PropertyDetailsModel>();
            var user = 1;
            Expression<Func<PropertyDetailsViewSnapshot, bool>> propExpr = null;
            try
            {
                if (PropertyId == 0)
                {
                    propExpr = s => s.Ownerid == user;
                }
                else
                {
                    propExpr = s => s.Ownerid == user && s.PropertyId == PropertyId;
                }

                res = _propertyDetailsViewRepo.GetFilteredPropertyDetailsViewDetails(propExpr).Select(s => new PropertyDetailsModel
                {
                    PropertyId = s.PropertyId,
                    PropertyName = s.PropertyName,
                    PropertyTypeId = s.PropertyTypeId,
                    PropertyType = s.PropertyType,
                    PropertyFacilities = GetFacilitiesList(s.PropertyFacilities),
                    AddressLine1 = s.AddressLine1,
                    AddressLine2 = s.AddressLine2,
                    AddressLine3 = s.AddressLine3,
                    Area = s.Area,
                    StateId = s.StateId,
                    CountryId = s.CountryId,
                    ZipCode = s.ZipCode,
                    No_Of_Rooms = s.No_Of_Rooms,
                    RoomDetails = GetRoomDetails(s.PropertyId),
                    Check_In_Time = s.Check_In_Time,
                    Check_Out_Time = s.Check_Out_Time
                }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Ok(new APIResponseModel<IEnumerable<PropertyDetailsModel>> { Data = res, Msg = "Successfully retreived property details!!" });
        }

        private List<RoomDetailsModel> GetRoomDetails(long propertyId)
        {
            List<RoomDetailsModel> res = new List<RoomDetailsModel>();
            Expression<Func<RoomDetailsViewSnapshot, bool>> roomexpr = s => s.PropertyId == propertyId;
            try
            {
                res = _roomDetailsViewRepo.GetFilteredRoomDetailsViewDetails(roomexpr).Select(s => new RoomDetailsModel
                {
                    RoomId = s.RoomId,
                    RoomTypeId = s.RoomTypeId,
                    RoomType = s.RoomType,
                    RoomFacilities = GetFacilitiesList(s.RoomFacilities),
                    RoomTariffs = GetRoomTariffs(s.RoomId)
                }).ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return res;
        }

        private List<RoomTariffModel> GetRoomTariffs(long roomId)
        {
            List<RoomTariffModel> res = new List<RoomTariffModel>();
            Expression<Func<RoomTariffSnapshot, bool>> roomtariffexpr = s => s.RoomId == roomId;
            try
            {
                RoomTariffModel rtm = null;
                var roomtariff = _roomTariffRepo.GetFilteredRoomTariffDetails(roomtariffexpr);
                var definedtariffs = roomtariff.Where(s => s.IsDefault == false).Select(s => new RoomTariffModel
                {
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    Tariff = s.Tariff,
                    Discount_Percent = s.Discount_Percent
                }).OrderBy(s => s.StartDate).ToList();

                var defaulttariffs = roomtariff.Where(s => s.IsDefault == true).Select(s => new RoomTariffModel
                {
                    StartDate = s.StartDate,
                    EndDate = s.EndDate,
                    Tariff = s.Tariff,
                    Discount_Percent = s.Discount_Percent
                }).FirstOrDefault();

                if (definedtariffs.Count() == 0)
                {
                    res.Add(defaulttariffs);
                }
                else
                {
                    for (int i = 0; i < definedtariffs.Count(); i++)
                    {
                        if (i == 0 && definedtariffs[i].StartDate > defaulttariffs.StartDate)
                        {
                            rtm = new RoomTariffModel
                            {
                                StartDate = defaulttariffs.StartDate,
                                EndDate = definedtariffs[i].StartDate.AddDays(-1),
                                Tariff = defaulttariffs.Tariff,
                                Discount_Percent = defaulttariffs.Discount_Percent
                            };
                            res.Add(rtm);
                        }
                        else if (i > 0)
                        {
                            if (definedtariffs[i].StartDate > definedtariffs[i - 1].EndDate && (definedtariffs[i].StartDate - definedtariffs[i - 1].EndDate).Days > 1)
                            {
                                rtm = new RoomTariffModel
                                {
                                    StartDate = definedtariffs[i - 1].EndDate.AddDays(1),
                                    EndDate = definedtariffs[i].StartDate.AddDays(-1),
                                    Tariff = defaulttariffs.Tariff,
                                    Discount_Percent = defaulttariffs.Discount_Percent
                                };
                                res.Add(rtm);
                            }
                        }
                    }
                    res.AddRange(definedtariffs);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return res.OrderBy(s => s.StartDate).ToList();
        }

        private List<FacilityModel> GetFacilitiesList(string Facilities)
        {
            return Facilities.Split(';').Select(s => new FacilityModel
            {
                FacilityId = Int32.Parse(s.Split(':')[0]),
                FacilityName = s.Split(':')[1]
            }).ToList();
        }
    }
}
