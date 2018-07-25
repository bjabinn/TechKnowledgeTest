using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace everisapi.API.Models
{
    public class SectionDto
    {
    public int Id { get; set; }

    public string Nombre { get; set; }

    public ICollection<AsignacionDto> Asignaciones { get; set; }
    = new List<AsignacionDto>();
  }
}
