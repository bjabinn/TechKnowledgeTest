using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace everisapi.API.Entities
{
    public class UserEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Nombre { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public ICollection<ProyectoEntity> ProyectosDeUsuario { get; set; }
        = new List<ProyectoEntity>();

        [Required]
        public ICollection<User_RoleEntity> User_Role { get; set; }
        = new List<User_RoleEntity>();
    }
}
