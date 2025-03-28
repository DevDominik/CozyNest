using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CozyNestAdmin.ResponseTypes
{
    class GetUsersResponse : MessageResponse
    {
        public List<UserDataResponse> Users { get; set; }
    }
}
