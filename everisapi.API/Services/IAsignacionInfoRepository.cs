using everisapi.API.Entities;
using everisapi.API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace everisapi.API.Services
{
  public interface IAsignacionInfoRepository
  {
    //Devuelve todas las asignaciones
    IEnumerable<AsignacionEntity> GetAsignaciones();

    //Devuelve todas las asignaciones con datos extendidos filtrado por evaluación
    IEnumerable<AsignacionInfoDto> GetAsignFromEval(int idEval);

    //Devuelve una asignación con datos extendidos filtrado por evaluación y asignación
    AsignacionInfoDto GetAsignFromEvalAndAsig(int idEval, int idAsig);

    //Devuelve una asignación con datos extendidos filtrado por evaluación y su sección
    IEnumerable<AsignacionInfoDto> GetAsignFromEvalAndSection(int idEval, int idSection);

    //Devuelve una asignación
    AsignacionEntity GetAsignacion(int AsignacionId, Boolean IncluirPreguntas);

    //Devuelve todas las preguntas de una asignación
    IEnumerable<PreguntaEntity> GetPreguntaPorAsignacion(int AsignacionId);

    //Devuelve una pregunta concreta de una asignación
    PreguntaEntity GetPreguntaDeAsignacion(int AsignacionId, int PreguntaId);

    //Devuelve si una asignación existe o no
    bool AsignacionExiste(int AsignacionId);

    //Añadir una pregunta a una ciudad
    void IncluirPreguntaParaAsignacion(int asignacionId, PreguntaEntity pregunta);

    //Guardar cambio de las entidades
    bool SaveChanges();

    //Eliminar preguntas de una asignación
    void EliminarPreguntaDeAsignacion(PreguntaEntity pregunta);

    //Aqui introducimos una nueva asignación
    bool AddAsig(AsignacionEntity asignacion);

    //Nos permite modificar una asignación
    bool AlterAsig(AsignacionEntity asignacion);

    //Elimina una asignación
    bool DeleteAsig(AsignacionEntity asignacion);

    //Este metodo nos permite añadir notas para una asignación
    bool AddNotas(AsignacionUpdateNotasDto asignacion);

    //Devuelve todas las asignaciones con notas de esa evaluacion
    IEnumerable<AsignacionConNotasDto> GetAsignConNotas(int idEval);
  }
}
