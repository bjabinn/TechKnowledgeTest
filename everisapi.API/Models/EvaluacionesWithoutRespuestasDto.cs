using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace everisapi.API.Models
{
    public class EvaluacionesWithoutRespuestasDto
    {

    public int Id { get; set; }

    public DateTime Fecha { get; set; }

    public bool Estado { get; set; }

    public int ProyectoId { get; set; }

  }
}
