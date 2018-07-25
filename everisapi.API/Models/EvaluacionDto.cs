using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace everisapi.API.Models
{
    public class EvaluacionDto
    {

    public int Id { get; set; }

    public DateTime Fecha { get; set; }

    public bool Estado { get; set; }

    public ICollection<RespuestaDto> Respuestas { get; set; }
    = new List<RespuestaDto>();

    public int ProyectoId { get; set; }
  }
}
