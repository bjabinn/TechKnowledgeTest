using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace everisapi.API.Models
{
    public class RespuestaDto
    {
    public int Id { get; set; }

    public int Estado { get; set; }

    public int PreguntaId { get; set; }

    public int EvaluacionId { get; set; }

    public string Notas { get; set; }

    public string NotasAdmin { get; set; }

  }
}
