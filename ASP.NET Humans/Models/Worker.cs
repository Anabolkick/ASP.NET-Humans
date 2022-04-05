namespace ASP.NET_Humans.Models
{
    public class Worker : OriginalWorker.OriginalWorker
    {
        public bool IsHired { get; set; }
        public User User { get; set; }  // нав свойство
        public string UserId { get; set; }  // внешний ключ
    }
}