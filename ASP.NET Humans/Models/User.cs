using System;
using System.Collections.Generic;
using ASP.NET_Humans;
using Microsoft.AspNetCore.Identity;

namespace ASP.NET_Humans.Models
{
    public class User : IdentityUser
    {
        public IEnumerable<Worker> Workers { get; private set; }
    }
}