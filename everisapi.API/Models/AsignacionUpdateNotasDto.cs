using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace everisapi.API.Models
{
    public class AsignacionUpdateNotasDto
    {
        public int EvId { get; set; }

        public int Id { get; set; }

        public string Notas { get; set; }

    }
}
