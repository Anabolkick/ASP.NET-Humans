using System;
using System.Collections.Generic;
using ASP.NET_Humans.Models;

namespace ASP.NET_Humans
{
    public class Company
    {
        //internal Company()
        //{
        //}
        //internal Company(string name, int budget)
        //{
        //    Name = name;
        //    Budget = budget;
        //    Workers = new List<Worker>();
        //}

        public int Id { get; private set; }
        public string Name { get; }
        public int Budget { get; private set; }
        public List<Worker> Workers { get; }

        public int UserId { get; private set; }
        public User User { get; set; }

        //public void AddWorker(Worker worker)
        //{
        //    if (Budget - worker.Salary >= 0)
        //    {
        //        Workers.Add(worker);
        //        Budget -= worker.Salary;

        //        Console.ForegroundColor = ConsoleColor.Green;
        //        Console.WriteLine(
        //            $"{worker.Name} successful added to your team. Salary: {worker.Salary}. Current balance: {Budget}");
        //    }
        //    else
        //    {
        //        Console.ForegroundColor = ConsoleColor.DarkRed;
        //        Console.WriteLine(
        //            $"You need more money to hire {worker.Name}. Budget: {Budget}. Salary: {worker.Salary}"); //TODO Delegate
        //    }

        //    Console.ResetColor();
        //}
    }
}