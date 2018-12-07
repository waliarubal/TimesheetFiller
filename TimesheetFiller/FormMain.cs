using RestSharp;
using System;
using System.Diagnostics;
using System.Net;
using System.Windows.Forms;

namespace TimesheetFiller
{
    public partial class FormMain : Form
    {
        RestClient _client;

        const string
            BASE_URL = "http://ems.dotsquares.com",
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

        private void btnAdd_Click(object sender, EventArgs e)
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

            var startDateMinusOne = StartDate.Subtract(new TimeSpan(1, 0, 0, 0));
            var endDate = EndDate;
            while (startDateMinusOne.Date <= endDate.Date)
            {
                startDateMinusOne = startDateMinusOne.AddDays(1);
                if (startDateMinusOne.DayOfWeek == DayOfWeek.Saturday || startDateMinusOne.DayOfWeek == DayOfWeek.Sunday)
                    continue;

                var request = new RestRequest(TIMESHEET_ADD_DATA_URL, Method.POST, DataFormat.Json);
                request.AddParameter("AddedDate", string.Format("{0}/{1}/{2}", startDateMinusOne.Day, startDateMinusOne.Month, startDateMinusOne.Year));
                request.AddParameter("Description", Description);
                request.AddParameter("DeveloperId", Developer.Id);
                request.AddParameter("Id", 0);
                request.AddParameter("ProjectId", Project.Id);
                request.AddParameter("WorkHours", "08:00");
                var response = Client.Execute(request);
                if (response.StatusCode != HttpStatusCode.OK)
                    continue;
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
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

            var request = new RestRequest("/", Method.POST, DataFormat.Json);
            request.AddParameter("Email", UserName);
            request.AddParameter("Password", Password);
            request.AddParameter("ReturnUrl", string.Empty);

            var response = Client.Execute(request);
            if (response.StatusCode != HttpStatusCode.Found && response.StatusCode != HttpStatusCode.OK)
            {
                MessageBox.Show("Invalid user name or password.");
                txtUserName.Focus();
                return;
            }

            request = new RestRequest(TIMESHEET_GET_DATA_URL, Method.POST, DataFormat.Json);
            request.AddParameter("DateFrom", string.Empty);
            request.AddParameter("DateTo", string.Empty);
            request.AddParameter("pageNumber", 1);
            request.AddParameter("pageSize", 25);
            IRestResponse<Data> data = Client.Execute<Data>(request);
            if (!(data.StatusCode == HttpStatusCode.OK || data.StatusCode == HttpStatusCode.Found))
            {
                MessageBox.Show("Failed to get data from EMS.");
                txtUserName.Focus();
                return;
            }

            cboProject.BeginUpdate();
            cboProject.Items.Clear();
            foreach (var project in data.Data.Projects)
                cboProject.Items.Add(project);
            cboProject.EndUpdate();

            cboDeveloper.BeginUpdate();
            cboDeveloper.Items.Clear();
            foreach (var developer in data.Data.Developer)
                cboDeveloper.Items.Add(developer);
            cboDeveloper.EndUpdate();
        }
    }
}
