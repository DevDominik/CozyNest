using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CozyNestAdmin.ResponseTypes
{
    public class UserDataResponse
    {
        public string Username { get; set; }
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public bool Closed { get; set; }
        public string Email { get; set; }
        public DateTime JoinDate { get; set; }
        public string RoleName { get; set; }
    }
}
