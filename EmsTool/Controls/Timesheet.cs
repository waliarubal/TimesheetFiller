using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmsTool.Controls
{
    partial class Timesheet : UserControl
    {
        public Timesheet()
        {
            InitializeComponent();
        }

        #region properties

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

            btnAdd.Enabled = false;

            var missedRecordDates = await ApiClient.Instance.AddTimesheetData(Project, Developer, StartDate, EndDate, Description);
            if (missedRecordDates.Count > 0)
                MessageBox.Show("Failed to add timesheet records.");
            else
                MessageBox.Show("Timesheet records added successfully.");

            btnAdd.Enabled = true;
        }

        public async Task Populate()
        {
            var data = await ApiClient.Instance.GetTimesheetData();

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
    }
}
