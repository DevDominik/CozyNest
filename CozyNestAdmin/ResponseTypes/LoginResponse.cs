using CozyNestAdmin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CozyNestAdmin.ResponseTypes
{
    public class LoginResponse : TokenResponse
    {
        public UserDataResponse UserData { get; set; }
    }
}
