using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonGeneratorApi.Models
{
    public class Worker
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Salary { get; set; }
        public Rarity Rarity { get; set; }
        public Byte[] ImageBytes { get; set; }
        public Gender Gender { get; set; }

    }


    public enum Rarity
    {
        Legendary,
        Epic,
        Rare,
        Normal
    }
}
