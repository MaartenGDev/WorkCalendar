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
            var calendar = new Calendar {Method = "PUBLISH", Properties =
            {
                new CalendarProperty("X-PUBLISHED-TTL", "PT15M"),
                new CalendarProperty("X-WR-TIMEZONE", "Europe/Paris"),
                new CalendarProperty("X-WR-CALNAME", calendarName),
                new CalendarProperty("X-WR-CALDESC", calenderDescription),
            }};


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
                    Start = new CalDateTime(@event.Start.DateTime.Value),
                    End = new CalDateTime(@event.End.DateTime.Value),
                });
            }

            var serializer = new CalendarSerializer();
            return serializer.SerializeToString(calendar);
        }
    }
}