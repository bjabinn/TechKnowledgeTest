using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace everisapi.API.Models
{
    public class SectionInfoDto
    {
    public int Id { get; set; }

    public string Nombre { get; set; }

    public int Preguntas { get; set; }

    public int Contestadas { get; set; }

    public double RespuestasCorrectas { get; set; }

    public double Progreso { get; set; }

    public string Notas { get; set; }
  }
}
