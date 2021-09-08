using System;
using RandomNameGen;

namespace ASP.NET_Humans.Models
{
    public class Cook : Worker
    {
        protected Cook()
        {
        }

        public Cook(string name, Sex sex, int salary, int age) : base(name, sex, age, salary)
        {
            Profession = "Cook";
        }

        public double? WorkingSpeed { get; } = (new Random().NextDouble() + 1) * 2;

        public override void Work()
        {
           // Console.WriteLine($"I am working at {Company?.Name}. I will cook your food."); //TODO No company
        }
    }
}