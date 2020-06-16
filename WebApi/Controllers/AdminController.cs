using Core.Lookup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApi.Controllers
{
    [RoutePrefix("api/Admin")]
    [Authorize]
    public class AdminController : ApiController
    {
        private ILookupRepository _lookupRepo;
        private ILookupTypeRepository _lookuptypeRepo;

        public AdminController(ILookupRepository lookupRepo, ILookupTypeRepository lookuptypeRepo)
        {
            _lookupRepo = lookupRepo;
            _lookuptypeRepo = lookuptypeRepo;
        }
        [Route("ListProperty")]
        [HttpPost]
        public IHttpActionResult ListProperty()
        {
            try
            {

            }
            catch (Exception ex)
            {

            }
            return Ok();
        }
    }
}
