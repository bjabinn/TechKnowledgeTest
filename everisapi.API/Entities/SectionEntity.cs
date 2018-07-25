using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace everisapi.API.Entities
{
    public class SectionEntity
    {

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(120)]
    public string Nombre { get; set; }

    [Required]
    public ICollection<AsignacionEntity> Asignaciones { get; set; }
    = new List<AsignacionEntity>();
  }
}
