using RestSharp.Deserializers;
using System.Collections.Generic;

namespace TimesheetFiller
{
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

    class Data
    {
        [DeserializeAs(Name = "ProjectList")]
        public List<Record> Projects { get; set; }


        [DeserializeAs(Name = "DeveloperList")]
        public List<Record> Developer { get; set; }
    }
}
