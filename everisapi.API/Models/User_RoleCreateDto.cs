using everisapi.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace everisapi.API.Models
{
    public class User_RoleCreateDto
    {
        public string UserNombre { get; set; }

        public int RoleId { get; set; }
    }
}
