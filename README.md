# WorkCalendar
Application to filter calendar events based on the attendees. This application can be used to filter down your work calendar to only show events that you need to attend.

## Setup
1. Open `Web` project
2. Copy appsettings.json to appsettings.Development.json
3. Configure `CalenderFilter` to filter calenders connected to your account by description and summary to speed up lookups. 
4. Configure `AccessToken` to a secret token to secure your read-only calendar endpoint 
5. Go to the default endpoint `/{token}/{email}` and login with the google account that contains the calendars you want to filter.

## Example config
```json
{
  "AccessToken": "3e6fbc64-201e-4c9d-9377-bd0a0bf5b888",
  "CalenderFilter": "CompanyInc",
}
```

### Tips
1. Use `"CalenderFilter": ""` to search through all calendars