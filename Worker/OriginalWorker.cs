using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace OriginalWorker
{
    public class OriginalWorker
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int Salary { get; set; }
        public Rarity Rarity { get; set; }

        [NotMapped]
        public Byte[] ImageBytes { get; set; }
    }

    public enum Rarity
    {
        Legendary,
        Epic,
        Rare,
        Normal
    }
}
