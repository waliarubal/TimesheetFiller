using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TimesheetFiller.Controls
{
    partial class Attendance : UserControl
    {
        public Attendance()
        {
            InitializeComponent();
        }

        public async Task Populate()
        {
            var data = await ApiClient.Instance.GetAttendanceData(12, 2018);

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
                }
            }

            dgvAttendance.ResumeLayout();
        }
    }
}
