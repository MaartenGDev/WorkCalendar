using System;
using System.Collections.Generic;
using System.Linq;
using Google.Apis.Calendar.v3.Data;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using Calendar = Ical.Net.Calendar;

namespace Web.Services
{
    public class CalendarEventFormatter
    {
        public string GetAsICalFormat(IEnumerable<Event> events, string calendarName, string calenderDescription)
        {
            var calendar = new Calendar {
                Method = "PUBLISH", 
                TimeZones = { new VTimeZone("Europe/Amsterdam") },
                Properties =
            {
                new CalendarProperty("X-PUBLISHED-TTL", "PT15M"),
                new CalendarProperty("X-WR-TIMEZONE", "Europe/Amsterdam"),
                new CalendarProperty("X-WR-CALNAME", calendarName),
                new CalendarProperty("X-WR-CALDESC", calenderDescription),
            }};

            calendar.AddTimeZone("Europe/Amsterdam");


            foreach (var @event in events.Where(x => x.Start.DateTime != null && x.End.DateTime != null))
            {
                if (!@event.Start.DateTime.HasValue || !@event.End.DateTime.HasValue)
                {
                    continue;
                }
                calendar.Events.Add(new CalendarEvent
                {
                    Description = @event.Description,
                    Summary = @event.Summary,
                    Location = @event.Location,
                    Attendees = @event.Attendees == null 
                        ? new List<Attendee>() 
                        : @event.Attendees.Select(x => new Attendee { CommonName = x.DisplayName, Value = new Uri($"mailto:{x.Email}")}).ToList(),
                    Start = new CalDateTime(@event.Start.DateTime.Value),
                    End = new CalDateTime(@event.End.DateTime.Value),
                });
            }

            var serializer = new CalendarSerializer();
            return serializer.SerializeToString(calendar);
        }
    }
}