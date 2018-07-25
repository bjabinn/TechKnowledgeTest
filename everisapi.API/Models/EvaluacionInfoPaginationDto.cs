using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace everisapi.API.Models
{
    public class EvaluacionInfoPaginationDto
    {

    public string Nombre { get; set; }

    public string UserNombre { get; set; }

    public string Puntuacion { get; set; }

    public string Fecha { get; set; }

    public string Estado { get; set; }

  }
}
