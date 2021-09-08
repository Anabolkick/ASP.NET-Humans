using System.ComponentModel.DataAnnotations.Schema;
using RandomNameGen;

namespace ASP.NET_Humans.Models
{
    public abstract class Worker
    {
        protected Worker()
        {
        }

        protected Worker(string name, Sex sex, int age, int salary)
        {
            Name = name;
            Age = age;
            Salary = salary;
            Sex = sex;
        }

        public int Id { get; private set; }
        public string Name { get; }
        public int Age { get; }
        public string Profession { get; set; } = "Without profession"; //TODO ????

        public int Salary { get; }

        [NotMapped]
        public Sex Sex { get; set; }

        public int? CompanyId { get; private set; } // внешний ключ
        public Company Company { get; private set; } // навигационное свойство

        public abstract void Work();
    }
}