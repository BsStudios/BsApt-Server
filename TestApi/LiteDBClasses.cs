using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestApi
{
    public static class LiteDBClasses
    {
        public class Users
        {
            public int Id { get; set; }
            public string username { get; set; }
            public string password { get; set; }
        }

    }
}
