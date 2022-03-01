using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASP.NET_Humans.Models
{
    public class Worker
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Salary { get; set; }
        public Rarity Rarity { get; set; }
        public Byte[] ImageBytes { get; set; }
        public bool IsHired { get; set; }
        public User User { get; set; }  // нав свойство
        public string UserId { get; set; }  // внешний ключ
      

    }

    public enum Rarity
    {
        Legendary,
        Epic,
        Rare,
        Normal
    }
}