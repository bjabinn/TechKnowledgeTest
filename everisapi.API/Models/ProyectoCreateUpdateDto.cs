using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace everisapi.API.Models
{
  public class ProyectoCreateUpdateDto
  {
    public int Id { get; set; }

    public string Nombre { get; set; }

    public DateTime Fecha { get; set; }

    public string UserNombre { get; set; }
  }
}
