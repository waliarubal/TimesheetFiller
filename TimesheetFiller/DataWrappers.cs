using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace TimesheetFiller
{
    class DateWrapper
    {
        readonly DateTime _date;

        public DateWrapper(DateTime date)
        {
            _date = date;
        }

        public int Year
        {
            get => _date.Year;
        }

        public int Month
        {
            get => _date.Month;
        }

        public override string ToString()
        {
            return _date.ToString("MMMMM yyyy");
        }
    }

    class AttendanceRecord
    {
        [JsonProperty(PropertyName = "date")]
        public DateTime Date { get; set; }

        [JsonProperty(PropertyName = "intime")]
        public DateTime? InTime { get; set; }

        [JsonProperty(PropertyName = "outtime")]
        public DateTime? OutTime { get; set; }

        [JsonProperty(PropertyName = "totalWorkingHours")]
        public TimeSpan? WorkingHours { get; set; }

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
    }

    class AttendanceData
    {
        [JsonProperty(PropertyName = "data")]
        public List<AttendanceRecord> AtendanceEntries { get; set; }
    }

    class Record
    {
        [JsonProperty(PropertyName = "Id")]
        public int Id { get; set; }

        [JsonProperty(PropertyName = "Name")]
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

    class TimesheetRecord : Record
    {
        [JsonProperty(PropertyName = "AddedDateEdit")]
        public DateTime Date { get; set; }

        [JsonProperty(PropertyName = "WorkHoursEdit")]
        public TimeSpan Time { get; set; }
    }

    class Data
    {
        [JsonProperty(PropertyName = "ProjectList")]
        public List<Record> Projects { get; set; }

        [JsonProperty(PropertyName = "DeveloperList")]
        public List<Record> Developers { get; set; }

        [JsonProperty(PropertyName = "TimeSheetList")]
        public List<TimesheetRecord> TimeEntries { get; set; }
    }
}
