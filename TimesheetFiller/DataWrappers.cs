using RestSharp.Deserializers;
using System;
using System.Collections.Generic;

namespace TimesheetFiller
{
    class AttendanceRecord
    {
        [DeserializeAs(Name = "date")]
        public DateTime Date { get; set; }

        [DeserializeAs(Name = "intime")]
        public DateTime? InTime { get; set; }

        [DeserializeAs(Name = "outtime")]
        public DateTime? OutTime { get; set; }

        [DeserializeAs(Name = "totalWorkingHours")]
        public TimeSpan? WorkingHours { get; set; }

        [DeserializeAs(Name = "status")]
        public string Status { get; set; }
    }

    class AttendanceData
    {
        [DeserializeAs(Name = "data")]
        public List<AttendanceRecord> AtendanceEntries { get; set; }
    }

    class Record
    {
        [DeserializeAs(Name = "Id")]
        public int Id { get; set; }

        [DeserializeAs(Name = "Name")]
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            var compareWith = obj as Record;
            if (compareWith == null)
                return false;

            return Id.Equals(compareWith.Id);
        }
    }

    class TimesheetRecord: Record
    {
        [DeserializeAs(Name = "AddedDateEdit")]
        public DateTime Date { get; set; }

        [DeserializeAs(Name = "WorkHoursEdit")]
        public TimeSpan Time { get; set; }
    }

    class Data
    {
        [DeserializeAs(Name = "ProjectList")]
        public List<Record> Projects { get; set; }


        [DeserializeAs(Name = "DeveloperList")]
        public List<Record> Developers { get; set; }

        [DeserializeAs(Name = "TimeSheetList")]
        public List<TimesheetRecord> TimeEntries { get; set; }
    }
}
