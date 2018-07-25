using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace everisapi.API.Models
{
    public class UsersWithRolesDto
    {
        public string Nombre { get; set; }

        public string Password { get; set; }

        public ICollection<User_RoleCreateDto> User_Role { get; set; }
        = new List<User_RoleCreateDto>();
    }
}
