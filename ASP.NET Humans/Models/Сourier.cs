using System;
using RandomNameGen;

namespace ASP.NET_Humans.Models
{
    public class Сourier : Worker
    {

        protected Сourier()
        {
        }
        public Сourier(string name, Sex sex, int salary, int age) : base(name, sex, age, salary)
        {
            Profession = "Сourier";
        }

        public double? DeliverySpeed { get; } = new Random().NextDouble() + 0.5;

        public override void Work()
        {
        //    Console.WriteLine($"I am working at {Company?.Name}. I will deliver your food!");
        }
    }
}