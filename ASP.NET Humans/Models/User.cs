using System.Collections.Generic;
using ASP.NET_Humans;

namespace ASP.NET_Humans.Models
{
    public class User
    {

        public int Id { get; private set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public byte[] Salt { get; set; }
        public List<Company> Companies { get; set; }

        //public Company CreateCompany(string name, int budget = 1000)
        //{
        //    var company = new Company(name, budget) {User = this};
        //    return company;
        //}

        //public User(string login, string password, byte[] salt)
        //{
        //    Login = login;
        //    Password = password;
        //    Salt = salt;
        //    Companies = new List<Company>();
        //}
    }
}