using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models
{
    public class APIResponseModel<T>
    {
        public T Data { get; set; }
        public string Msg { get; set; }
    }
}