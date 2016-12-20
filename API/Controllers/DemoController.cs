using API.Bindings;
using API.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Http.Routing;

namespace API.Controllers
{
    public class DemoController : ApiController
    {
        [HttpGet]
        [Route("demo/get_list")]
        public IHttpActionResult DemoGetList()
        {
            List<Movie> movies = new List<Movie>()
            {
                new Movie()
                {
                    Id = 1,
                    Title = "Up",
                    ReleaseDate = new DateTime(2009,5,29),
                    RunningTimeMinutes = 96
                },
                new Movie()
                {
                    Id = 2,
                    Title = "Toy Story",
                    ReleaseDate = new DateTime(1995, 11, 19),
                    RunningTimeMinutes = 81
                },
                new Movie()
                {
                    Id = 3,
                    Title = "Big Hero 6",
                    ReleaseDate = new DateTime(2014, 11, 7),
                    RunningTimeMinutes = 102
                }
            };

            return Ok(BaseResponse.MakeResponse(movies));
        }

        [HttpGet]
        [Route("demo/get_item/{id}")]
        public IHttpActionResult DemoGetItem(int id)
        {
            return Ok(BaseResponse.MakeResponse(new Movie()
            {
                Id = id,
                Title = "Big Hero 6",
                ReleaseDate = new DateTime(2014, 11, 7),
                RunningTimeMinutes = 102
            }));
        }


        [JSONParamBinding]
        [HttpPost]
        [Route("demo/submit_form_1")]
        public IHttpActionResult DemoSubmitForm1(RequestForm1 param)
        {
            Dictionary<string, string> errors = new Dictionary<string, string>();
            if (param.a.Equals("")) { errors.Add("a", "F001"); };
            if (param.b.Equals("")) { errors.Add("b", "F001"); };
            if (param.c == 0) { errors.Add("c", "F001"); };
            if (param.date.Equals("")) { errors.Add("date", "F001"); };

            DateTime? dateTime = null;
            String recvDate = "";

            try
            {
                dateTime = BuildDateTimeFromYAFormat(param.date);
                if (dateTime != null)
                {
                    recvDate = dateTime.GetValueOrDefault().ToString();
                }
            } catch (FormatException e)
            {

            }


            if (errors.Count > 0)
            {
                return Ok(BaseResponse.MakeResponse("F000", errors));
            }

            return Ok(BaseResponse.MakeResponse(new string[]
            {
                param.a,
                param.b,
                param.c + "",
                recvDate
            }));
        }





        private DateTime BuildDateTimeFromYAFormat(string dateString)
        {
            Regex r = new Regex(@"^\d{4}\d{2}\d{2}T\d{2}\d{2}\d{2}$");
            if (!r.IsMatch(dateString))
            {
                throw new FormatException(
                    string.Format("{0} is not the correct format. Should be yyyyMMddThhmmss", dateString));
            }

            DateTime dt = DateTime.ParseExact(dateString, "yyyyMMddThhmmss", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal);

            return dt;
        }
    }
}