using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLayer.Dtos.Account
{
    public class UserCreateDto
    {
        public string? Fullname { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? Number { get; set; }
        public string? Password { get; set; }
        public string? ConfrimPassword { get; set; }
    }
}
