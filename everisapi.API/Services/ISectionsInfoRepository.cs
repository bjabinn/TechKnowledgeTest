using everisapi.API.Entities;
using everisapi.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace everisapi.API.Services
{
  public interface ISectionsInfoRepository
  {
    //Devuelve todos los sections
    IEnumerable<SectionEntity> GetSections();

    //Devuelve un section
    SectionEntity GetSection(int id, bool IncluirAsignaciones);

    //Devuelve todos los sections con sus preguntas y respuestas para un proyecto
    IEnumerable<SectionInfoDto> GetSectionsInfoFromEval(int idEvaluacion);

    //Devuelve una de las asignaciones del section
    AsignacionEntity GetAsignacionFromSection(SectionEntity section, int idAsignacion);

    //Devuelve el numero de preguntas de cada seccion y en un evaluacion por id
    int GetNumPreguntasFromSection(int idSection, int idEvaluacion);

    //Devuelve el numero de preguntas respondidas de cada seccion y en un evaluacion por id
    int GetRespuestasCorrectasFromSection(int idSection, int idEvaluacion);

    //Devuelve todas las asignaciones de un sector
    IEnumerable<AsignacionEntity> GetAsignacionesFromSection(SectionEntity section);

    //Guardar cambio de las entidades
    bool SaveChanges();

    //Aqui introducimos una nueva section
    bool AddSection(SectionEntity section);

    //Nos permite modificar una section
    bool AlterSection(SectionEntity section);

    //Elimina una section
    bool DeleteSection(SectionEntity section);

    //Nos permite modificar las notas una section
    bool AddNotasSection(SectionWithNotasDto section);
  }
}
