using System.ComponentModel.DataAnnotations.Schema;
using RandomNameGen;

namespace ASP.NET_Humans.Models
{
    //  public abstract class Worker
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
        public Company Company { get; set; } // навигационное свойство

        //public abstract void Work();

        //protected Worker()
        //{
        //}

        //protected Worker(string name, Sex sex, int age, int salary)
        //{
        //    Name = name;
        //    Age = age;
        //    Salary = salary;
        //    Sex = sex;
        //}
    }
}