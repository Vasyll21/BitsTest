using CsvHelper.Configuration.Attributes;

namespace BitsOrchestra.Models
{
    public class CsvEmployeeModel
    {
        [Name("Name")]
        public string Name { get; set; }
        [Name("Date of birth")]
        public DateTime DateOfBirth { get; set; }
        [Name("Married")]
        [BooleanTrueValues("yes")]
        [BooleanFalseValues("no")]
        public bool Married { get; set; }
        [Name("Phone")]
        public string Phone { get; set; }
        [Name("Salary")]
        public decimal Salary { get; set; }
    }
}
