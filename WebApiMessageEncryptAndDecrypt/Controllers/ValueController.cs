using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using ServiceStack;

namespace WebApiMessageEncryptAndDecrypt.Controllers
{
    public class ValueController : ApiController
    {
        private IList<ValueModel> values;

        public ValueController()
        {
            values = new List<ValueModel>();

            for (int i = 0; i < 10; i++)
            {
                values.Add(new ValueModel()
                {
                    Id = i,
                    Data = $"Value {i}"
                });
            }
            
        }

        [System.Web.Http.Route("api/Value")]
        [HttpGet]
        public IHttpActionResult GetValues()
        {
            return Ok(values.ToJson());
        }


        [System.Web.Http.Route("api/Value/{id}")]
        [HttpGet]
        public IHttpActionResult GetValue(int id)
        {
            var value = values.Where(it => it.Id == id);
            return Ok(value.ToJson());
        }

        [System.Web.Http.Route("api/Value")]
        [HttpPost]
        public IHttpActionResult PostValue(ValueModel model)
        {
            values.Add(model);
            return Ok(values.ToJson());
        }
    }

    public class ValueModel
    {
        public Int32 Id { get; set; }
        public String Data { get; set; }
    }
}