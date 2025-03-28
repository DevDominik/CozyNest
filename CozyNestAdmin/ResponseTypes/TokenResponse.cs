using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CozyNestAdmin.ResponseTypes
{
    public class TokenResponse : MessageResponse
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }

}
