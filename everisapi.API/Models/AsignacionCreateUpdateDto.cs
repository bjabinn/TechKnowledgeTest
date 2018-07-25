using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace everisapi.API.Models
{
  public class AsignacionCreateUpdateDto
  {
    public int Id { get; set; }

    [Required]
    public string Nombre { get; set; }

    [Required]
    public int SectionId { get; set; }
  }
}
