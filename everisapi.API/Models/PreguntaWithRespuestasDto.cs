using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace everisapi.API.Models
{
    public class PreguntaWithRespuestasDto
    {
        public int Id {get; set;}

        public string Pregunta { get; set; }

        public ICollection<RespuestaDto> Respuestas { get; set; }
        = new List<RespuestaDto>();
    }
}
