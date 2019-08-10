using System.Collections.Generic;
using Google.Apis.Calendar.v3.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Web.Services;

namespace Web.Controllers
{
    [Route("/")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public CalendarController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        
        [HttpGet("{token}/{email}")]
        public ActionResult<string> Get(string token, string email)
        {
            var formatter = new CalendarEventFormatter();
            var expectedToken = _configuration["AccessToken"];

            if (!token.Equals(expectedToken))
            {
                return formatter.GetAsICalFormat(new List<Event>());
            }

            var calendarNameFilter = _configuration["CalenderFilter"];
            var calendarService = new CalenderFilterService();
            var events = calendarService.GetEventsForEmailForCalendarsMatchingName(calendarNameFilter, email);


            return formatter.GetAsICalFormat(events);
        }
    }
}