using System;
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
            var calendarNameFilter = _configuration["CalenderFilter"];

            var calenderName = "WorkCalendar for " + email;
            var calendarDescription = "All events for " + email + " from calendars matching: " + calendarNameFilter.Trim();
            
            if (!token.Equals(expectedToken))
            {
                return formatter.GetAsICalFormat(new List<Event>(), calenderName, calendarDescription);
            }

            var calendarService = new CalenderFilterService();
            var events = calendarService.GetEventsForEmailForCalendarsMatchingName(calendarNameFilter, email);


            return formatter.GetAsICalFormat(events, calenderName, calendarDescription);
        }
    }
}