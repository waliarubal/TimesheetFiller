using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TimesheetFiller
{
    public partial class FormMain : Form
    {
        RestClient _client;

        const string
            BASE_URL = "https://ems.dotsquares.com",
            TIMESHEET_URL = "timesheet/index",
            TIMESHEET_GET_DATA_URL = "timesheet/GetData",
            TIMESHEET_ADD_DATA_URL = "timesheet/AddTimesheetData";

        public FormMain()
        {
            InitializeComponent();

            if (Debugger.IsAttached)
            {
                txtUserName.Text = "rubal.walia@dotsquares.com";
                txtPassword.Text = "dots@123";
            }
        }

        #region properties

        RestClient Client
        {
            get
            {
                if (_client == null)
                {
                    _client = new RestClient
                    {
                        BaseUrl = new Uri(BASE_URL, UriKind.Absolute),
                        CookieContainer = new CookieContainer()

                    };
                }

                return _client;
            }
        }

        string UserName
        {
            get => txtUserName.Text;
        }

        string Password
        {
            get => txtPassword.Text;
        }

        Record Project
        {
            get => cboProject.SelectedIndex >= 0 ? cboProject.SelectedItem as Record : null;
        }

        Record Developer
        {
            get => cboDeveloper.SelectedIndex >= 0 ? cboDeveloper.SelectedItem as Record : null;
        }

        DateTime StartDate
        {
            get => dtpStartDate.Value;
        }

        DateTime EndDate
        {
            get => dtpEndDate.Value;
        }

        string Description
        {
            get => txtDescription.Text;
        }

        #endregion

        private async void btnAdd_Click(object sender, EventArgs e)
        {
            if (Project == null)
            {
                MessageBox.Show("Project not selected.");
                cboProject.Focus();
                return;
            }
            if (Developer == null)
            {
                MessageBox.Show("Developer not selected.");
                cboDeveloper.Focus();
                return;
            }
            if (StartDate.Date > EndDate.Date)
            {
                MessageBox.Show("Start date should not be greater than end date.");
                dtpStartDate.Focus();
                return;
            }
            if (string.IsNullOrEmpty(Description))
            {
                MessageBox.Show("Description not entered.");
                txtDescription.Focus();
                return;
            }

            var missedRecordDates = await AddTimesheetData(Client, Project, Developer, StartDate, EndDate, Description);
            if (missedRecordDates.Count > 0)
                MessageBox.Show("Failed to add timesheet records.");
        }

        private async void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(UserName))
            {
                MessageBox.Show("User name not entered.");
                txtUserName.Focus();
                return;
            }
            if (string.IsNullOrEmpty(Password))
            {
                MessageBox.Show("Password not entered.");
                txtUserName.Focus();
                return;
            }

            var data = await Authenticate(Client, UserName, Password);
            if (data.Key != null)
            {
                MessageBox.Show(data.Key);
                txtUserName.Focus();
                return;
            }

            cboProject.BeginUpdate();
            cboProject.Items.Clear();
            foreach (var project in data.Value.Projects)
                cboProject.Items.Add(project);
            cboProject.EndUpdate();

            cboDeveloper.BeginUpdate();
            cboDeveloper.Items.Clear();
            foreach (var developer in data.Value.Developers)
                cboDeveloper.Items.Add(developer);
            cboDeveloper.EndUpdate();

            dtpStartDate.Value = data.Value.TimeEntries[0].Date.AddDays(1);
        }

        #region private methods

        async Task<IList<DateTime>> AddTimesheetData(RestClient client, Record project, Record developer, DateTime startDate, DateTime endDate, string description)
        {
            var tokenSource = new CancellationTokenSource();

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
                var response = await client.ExecuteTaskAsync(request, tokenSource.Token);
                if (response.StatusCode != HttpStatusCode.OK)
                {
                    missedRecordDates.Add(startDateMinusOne);
                    continue;
                }
            }

            return missedRecordDates;
        }

        async Task<KeyValuePair<string, Data>> Authenticate(RestClient client, string userName, string password)
        {
            var tokenSource = new CancellationTokenSource();

            var request = new RestRequest("/", Method.POST, DataFormat.Json);
            request.AddParameter("Email", UserName);
            request.AddParameter("Password", Password);
            request.AddParameter("ReturnUrl", string.Empty);

            var response = await client.ExecuteTaskAsync(request, tokenSource.Token);
            if (response.StatusCode != HttpStatusCode.Found && response.StatusCode != HttpStatusCode.OK)
                return new KeyValuePair<string, Data>("Invalid user name or password.", null);

            request = new RestRequest(TIMESHEET_GET_DATA_URL, Method.POST, DataFormat.Json);
            request.DateFormat = "dd/MM/yyyy";
            request.AddParameter("DateFrom", string.Empty);
            request.AddParameter("DateTo", string.Empty);
            request.AddParameter("pageNumber", 1);
            request.AddParameter("pageSize", 25);
            IRestResponse<Data> data = await client.ExecuteTaskAsync<Data>(request, tokenSource.Token);
            if (!(data.StatusCode == HttpStatusCode.OK || data.StatusCode == HttpStatusCode.Found))
                return new KeyValuePair<string, Data>("Failed to get data from EMS.", null);

            return new KeyValuePair<string, Data>(null, data.Data);
        }

        #endregion
    }
}
