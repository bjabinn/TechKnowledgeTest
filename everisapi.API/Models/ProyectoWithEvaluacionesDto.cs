using everisapi.API.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace everisapi.API.Models
{
  public class ProyectoWithEvaluacionesDto
  {
    public int Id { get; set; }

    public string Nombre { get; set; }

    public DateTime Fecha { get; set; }

    public ICollection<EvaluacionDto> Evaluaciones { get; set; }
    = new List<EvaluacionDto>();
  }
}
