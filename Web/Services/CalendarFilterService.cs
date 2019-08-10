using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;

namespace Web.Services
{
    public class CalenderFilterService
    {
        private static readonly string[] Scopes = {CalendarService.Scope.CalendarReadonly};
        private readonly CalendarService _service;

        public CalenderFilterService()
        {
            UserCredential credential;

            using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore("token.json", true)).Result;
            }

            _service = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "Work calender",
            });
        }

        public IEnumerable<Event> GetEventsForEmailForCalendarsMatchingName(string name, string email)
        {
            var calendars = _service.CalendarList.List().Execute();

            return calendars.Items
                .Where(x => string.IsNullOrEmpty(name) ||
                            (x.Description != null && x.Description.ToLower().Contains(name) ||
                             x.Summary != null && x.Summary.ToLower().Contains(name)))
                .Select(x => x.Id)
                .SelectMany(calendarId => GetEventsForEmailFromCalendar(calendarId, email));
        }

        private IEnumerable<Event> GetEventsForEmailFromCalendar(string calendarId, string email)
        {
            EventsResource.ListRequest request = _service.Events.List(calendarId);
            request.TimeMin = DateTime.Now;
            request.SingleEvents = true;
            request.OrderBy = EventsResource.ListRequest.OrderByEnum.StartTime;

            var events = request.Execute();
            return events.Items.Where(eventItem =>
            {
                if (eventItem.Attendees == null)
                {
                    return false;
                }

                return eventItem.Attendees.Any(x => x.Email == email);
            });
        }
    }
}