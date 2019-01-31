using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace TimesheetFiller
{
    class ApiClient
    {
        const string
            BASE_URL = "https://ems.dotsquares.com",
            TIMESHEET_URL = "timesheet/index",
            TIMESHEET_GET_DATA_URL = "timesheet/GetData",
            TIMESHEET_ADD_DATA_URL = "timesheet/AddTimesheetData",
            ATTENDANCE_GET_DATA_URL = "attendance/GetAttendance";

        static ApiClient _instance;
        readonly CookieContainer _cookieContainer;
        readonly Uri _baseUrl;

        private ApiClient()
        {
            _baseUrl = new Uri(BASE_URL, UriKind.Absolute);
            _cookieContainer = new CookieContainer();

            Client = new RestClient
            {
                BaseUrl = _baseUrl,
                CookieContainer = _cookieContainer

            };

            TokenSource = new CancellationTokenSource();
        }

        #region properties

        public static ApiClient Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ApiClient();

                return _instance;
            }
        }

        RestClient Client { get; }

        CancellationTokenSource TokenSource { get; }

        #endregion

        string GetCookie(string key)
        {
            var cookies = _cookieContainer.GetCookies(_baseUrl);
            foreach (Cookie cookie in cookies)
                if (cookie.Name.Equals(key))
                    return cookie.Value;

            return null;
        }

        T Deserialize<T>(string json)
        {
            var settings = new JsonSerializerSettings { DateFormatString = "dd/MM/yyyy" };
            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        public async Task<KeyValuePair<string, bool>> Authenticate(string userName, string password)
        {
            var request = new RestRequest("/", Method.POST, DataFormat.Json);
            request.AddParameter("Email", userName);
            request.AddParameter("Password", password);
            request.AddParameter("ReturnUrl", string.Empty);

            var response = await Client.ExecuteTaskAsync(request, TokenSource.Token);
            if (response.StatusCode != HttpStatusCode.Found && response.StatusCode != HttpStatusCode.OK)
                return new KeyValuePair<string, bool>("Invalid user name or password.", false);

            return new KeyValuePair<string, bool>(null, true);
        }

        public async Task<KeyValuePair<string, Data>> GetTimesheetData()
        {
            var request = new RestRequest(TIMESHEET_GET_DATA_URL, Method.POST, DataFormat.Json);
            request.AddParameter("DateFrom", string.Empty);
            request.AddParameter("DateTo", string.Empty);
            request.AddParameter("pageNumber", 1);
            request.AddParameter("pageSize", 25);
            IRestResponse response = await Client.ExecuteTaskAsync(request, TokenSource.Token);
            if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Found)
                return new KeyValuePair<string, Data>("Failed to get data from EMS.", null);

            var data = Deserialize<Data>(response.Content); 
            return new KeyValuePair<string, Data>(null, data);
        }

        public async Task<IList<DateTime>> AddTimesheetData(Record project, Record developer, DateTime startDate, DateTime endDate, string description)
        {
            var missedRecordDates = new List<DateTime>();
            var startDateMinusOne = startDate.Subtract(new TimeSpan(1, 0, 0, 0));
            while (startDateMinusOne.Date < endDate.Date)
            {
                startDateMinusOne = startDateMinusOne.AddDays(1);
                if (startDateMinusOne.DayOfWeek == DayOfWeek.Saturday || startDateMinusOne.DayOfWeek == DayOfWeek.Sunday)
                    continue;

                var request = new RestRequest(TIMESHEET_ADD_DATA_URL, Method.POST, DataFormat.Json);
                request.AddParameter("AddedDate", string.Format("{0}/{1}/{2}", startDateMinusOne.Day, startDateMinusOne.Month, startDateMinusOne.Year));
                request.AddParameter("Description", description);
                request.AddParameter("DeveloperId", developer.Id);
                request.AddParameter("Id", 0);
                request.AddParameter("ProjectId", project.Id);
                request.AddParameter("WorkHours", "08:00");
                var response = await Client.ExecuteTaskAsync(request, TokenSource.Token);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    missedRecordDates.Add(startDateMinusOne);
                    continue;
                }
            }

            return missedRecordDates;
        }

        public async Task<AttendanceData> GetAttendanceData(int month, int year)
        {
            var userId = long.Parse(GetCookie("UserSessionCookies").Replace("{\"Uid\":", string.Empty).Replace("}", string.Empty));

            var request = new RestRequest(ATTENDANCE_GET_DATA_URL, Method.POST, DataFormat.Json);
            request.AddParameter("month", string.Format("{0}-{1}", month, year));
            request.AddParameter("userid", userId);
            request.AddParameter("draw", 1);
            request.AddParameter("start", 0);

            var response = await Client.ExecuteTaskAsync(request, TokenSource.Token);
            if (response.StatusCode != HttpStatusCode.OK)
                return null;

            var data = Deserialize<AttendanceData>(response.Content);
            return data;
        }

    }
}
