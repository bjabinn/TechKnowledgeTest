using everisapi.API.Entities;
using everisapi.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace everisapi.API.Services
{
  public interface IEvaluacionInfoRepository
  {
    //Devuelve todas las evaluaciones
    IEnumerable<EvaluacionEntity> GetEvaluaciones();

    //Devuelve una evaluación
    EvaluacionEntity GetEvaluacion(int IdEvaluacion, Boolean IncluirRespuestas);

    //Devuelve información detallada de las evaluaciones
    List<EvaluacionInfoDto> GetEvaluationInfo(int IdProject);

    //Recoge una lista de evaluaciones con datos de información de muchas tablas filtrandola por paginado
    List<EvaluacionInfoDto> GetEvaluationInfoAndPage(int IdProject, int pageNumber);

    //Recoge una lista de evaluaciones con datos de información de muchas tablas filtrandola por paginado y los datos de una evaluación
    List<EvaluacionInfoDto> GetEvaluationInfoAndPageFiltered(int IdProject, int pageNumber, EvaluacionInfoPaginationDto Evaluation);

    //Devuelve todas las evaluaciones de un proyecto
    IEnumerable<EvaluacionEntity> GetEvaluacionesFromProject(int IdProject);

    //Devuelve una evaluacion si existiera una evaluacion inacabada en la base de datos filtrado por id de projecto
    EvaluacionEntity EvaluationIncompletaFromProject(int IdProject);

    //Añadir una evaluación en un proyecto
    void IncluirEvaluacion(EvaluacionEntity evaluacion);

    //Devuelve una lista de evaluaciones filtrada por una evaluacion de modelo y un paginado
    List<EvaluacionInfoDto> GetEvaluationInfoAndPageFilteredAdmin(int pageNumber, EvaluacionInfoPaginationDto Evaluacion);

    //Devuelve un número entero de las evaluaciones por proyecto o todas las evaluaciones
    int GetNumEval(int idProject);

    //Guardar cambio de las entidades
    bool SaveChanges();

    //Modifica una evaluación concreta de un proyecto
    bool ModificarEvaluacion(EvaluacionEntity evaluacion);

    //Elimina una evaluacion de la base de datos
    bool DeleteEvaluacion(EvaluacionEntity evaluacion);
  }
}
