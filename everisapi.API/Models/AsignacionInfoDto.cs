using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace everisapi.API.Models
{
    public class AsignacionInfoDto
    {
        public int Id {get; set;}

        public string Nombre { get; set; }

        public string Notas { get; set; }


    public ICollection<PreguntaWithOneRespuestasDto> Preguntas { get; set; }
        = new List<PreguntaWithOneRespuestasDto>();
  }
}
