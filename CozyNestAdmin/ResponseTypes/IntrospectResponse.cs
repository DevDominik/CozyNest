using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CozyNestAdmin.ResponseTypes
{
    class IntrospectResponse
    {
        public bool Active { get; set; }
        public UserDataResponse UserData { get; set; }
    }
}
