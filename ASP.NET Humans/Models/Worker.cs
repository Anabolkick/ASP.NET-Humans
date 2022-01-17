using System.ComponentModel.DataAnnotations.Schema;
using RandomNameGen;

namespace ASP.NET_Humans.Models
{
    public class Worker
    {
        public int Id { get;  set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Profession { get; set; }   // = "Without profession"; //TODO ????
        public int Salary { get; set; }

        [NotMapped]
        public Sex Sex { get; set; }
        public int? CompanyId { get; set; } // внешний ключ

    }
}