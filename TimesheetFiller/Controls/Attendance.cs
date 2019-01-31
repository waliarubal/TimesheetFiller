using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmsTool.Controls
{
    partial class Attendance : UserControl
    {
        public Attendance()
        {
            InitializeComponent();

            var now = DateTime.Now;
            var thisMonth = new DateTime(now.Year, now.Month, 1);
            var lastMonth = thisMonth.AddMonths(-1);
            var lastToLastMonth = lastMonth.AddMonths(-1);

            cboMonth.Items.Add(new DateWrapper(thisMonth));
            cboMonth.Items.Add(new DateWrapper(lastMonth));
            cboMonth.Items.Add(new DateWrapper(lastToLastMonth));
            cboMonth.SelectedIndex = 0;
        }

        public DateWrapper SelectedMonth
        {
            get => cboMonth.SelectedItem as DateWrapper;
        }

        public async Task Populate()
        {
            var data = await ApiClient.Instance.GetAttendanceData(SelectedMonth.Month, SelectedMonth.Year);

            dgvAttendance.SuspendLayout();

            dgvAttendance.Rows.Clear();
            foreach (var entry in data.AtendanceEntries)
            {
                var row = dgvAttendance.Rows[dgvAttendance.Rows.Add()];
                if (entry.Date.DayOfWeek == DayOfWeek.Saturday || entry.Date.DayOfWeek == DayOfWeek.Sunday)
                    row.DefaultCellStyle.BackColor = Color.LightYellow;

                var cells = row.Cells;
                cells[0].Value = entry.Date.ToShortDateString();
                if (entry.InTime.HasValue)
                    cells[1].Value = entry.InTime.Value.TimeOfDay;
                if (entry.OutTime.HasValue)
                    cells[2].Value = entry.OutTime.Value.TimeOfDay;
                if (entry.WorkingHours.HasValue)
                    cells[3].Value = entry.WorkingHours.Value;
                cells[4].Value = entry.Status;
                switch(entry.Status)
                {
                    case "In Process":
                        cells[4].Style.ForeColor = Color.Gray;
                        break;

                    case "Present":
                        cells[4].Style.ForeColor = Color.Green;
                        break;

                    case "Half Day":
                        break;

                    case "Absent":
                        cells[4].Style.ForeColor = Color.Red;
                        break;

                    default:
                        if (entry.Status.StartsWith("Holiday"))
                            cells[4].Style.ForeColor = Color.Orange;
                        break;
                }
            }

            dgvAttendance.ResumeLayout();
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            btnRefresh.Enabled = false;
            await Populate();
            btnRefresh.Enabled = true;
        }
    }
}
