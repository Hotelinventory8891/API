using Application.Web.Models;
using Core.Lookup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace WebApi.Controllers
{
    [RoutePrefix("api/Values")]
    //[Authorize]
    public class ValuesController : ApiController
    {
        private ILookupRepository _LookUpRepo;
        private ILookupTypeRepository _LookUpTypeRepo;
        public ValuesController(ILookupTypeRepository LookUpTypeRepo, ILookupRepository LookUpRepo)
        {
            _LookUpTypeRepo = LookUpTypeRepo;
            _LookUpRepo = LookUpRepo;
        }
        [Route("GetLookUpType")]
        [HttpGet]
        public IEnumerable<LookupTypeSnapshot> GetLookUpType()
        {
            IEnumerable<LookupTypeSnapshot> res = null;
            try
            {
                res = _LookUpTypeRepo.GetAllLookupTypeDetails();

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return res;
        }
        //[Route("GetCountrie")]
        //[HttpGet]
        //public IHttpActionResult GetCountrie()
        //{
        //    IEnumerable<comboBindingModel> res = null;
        //    try
        //    {
        //        res = _CountryRepo.GetAllCountryDetails()
        //            .Select(s => new comboBindingModel { ID = s.COUNTRYID.ToString(), Name = s.COUNTRYNAME });
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //    return Ok(res);
        //}

        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
