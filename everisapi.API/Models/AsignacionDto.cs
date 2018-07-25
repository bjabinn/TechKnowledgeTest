using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace everisapi.API.Models
{
    public class AsignacionDto
    {
        public int Id {get; set;}

        public string Nombre { get; set; }

    public ICollection<PreguntaDto> PreguntasDeAsignacion { get; set; }
        = new List<PreguntaDto>();
  }
}
