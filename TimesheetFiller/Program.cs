using System;
using System.Windows.Forms;

namespace TimesheetFiller
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.Run(new FormMain());

            //var cookieContainer = new CookieContainer();

            //var client = new RestClient("http://ems.projectstatus.in");
            //client.CookieContainer = cookieContainer;
            
            //var request = new RestRequest("/", Method.POST, DataFormat.Json);
            //request.AddParameter("Email", "rubal.walia@dotsquares.com");
            //request.AddParameter("Password", "dots@123");
            //request.AddParameter("ReturnUrl", string.Empty);

            //var response = client.Execute(request);
            //if (!(response.StatusCode == HttpStatusCode.Found || response.StatusCode == HttpStatusCode.OK))
            //    return;

            //var startDateMinusOne = new DateTime(2018, 10, 26);
            //var endDate = DateTime.Now;
            //while(startDateMinusOne.Date <= endDate.Date)
            //{
            //    startDateMinusOne = startDateMinusOne.AddDays(1);
            //    if (startDateMinusOne.DayOfWeek == DayOfWeek.Saturday || startDateMinusOne.DayOfWeek == DayOfWeek.Sunday)
            //        continue;

            //    request = new RestRequest("/timesheet/AddTimesheetData", Method.POST, DataFormat.Json);
            //    request.AddParameter("AddedDate", string.Format("{0}/{1}/{2}", startDateMinusOne.Day, startDateMinusOne.Month, startDateMinusOne.Year));
            //    request.AddParameter("Description", "Ticket Search work.");
            //    request.AddParameter("DeveloperId", 657);
            //    request.AddParameter("Id", 0);
            //    request.AddParameter("ProjectId", 2226);
            //    request.AddParameter("WorkHours", "08:00");
            //    response = client.Execute(request);
            //    if (response.StatusCode != HttpStatusCode.OK)
            //        return;
            //}

            //request = new RestRequest("/timesheet/GetData", Method.POST, DataFormat.Json);
            //response = client.Execute(request);
            //if (response.StatusCode != HttpStatusCode.OK)
            //    return;
        }
    }
}
