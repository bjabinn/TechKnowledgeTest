using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace everisapi.API.Models
{
    public class PreguntaWithOneRespuestasDto
    {
        public int Id {get; set;}

        public string Pregunta { get; set; }

        public string Correcta { get; set; }

        public RespuestaDto Respuesta { get; set; }
    }
}
