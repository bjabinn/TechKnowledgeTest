using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using everisapi.API.Entities;
using everisapi.API.Models;
using Microsoft.EntityFrameworkCore;

namespace everisapi.API.Services
{
  public class RespuestasInfoRepository : IRespuestasInfoRepository
  {

    private AsignacionInfoContext _context;

    //Le damos un contexto en el constructor
    public RespuestasInfoRepository(AsignacionInfoContext context)
    {
      _context = context;
    }

    //Recoge una unica respuesta filtrada por id
    public RespuestaEntity GetRespuesta(int RespuestaId)
    {
      return _context.Respuestas.Where(r => r.Id == RespuestaId).FirstOrDefault();
    }

    //Recoge todas las respuestas existentes
    public IEnumerable<RespuestaEntity> GetRespuestas()
    {
      return _context.Respuestas.ToList();
    }

    //Introduciendo el id de evaluacion y el id de pregunta te da la lista de respuestas
    public IEnumerable<RespuestaEntity> GetRespuestasFromPregEval(int idEvaluacion, int IdPregunta)
    {
      return _context.Respuestas.Where(r => r.PreguntaId == IdPregunta && r.EvaluacionId == idEvaluacion).ToList();
    }

    //Introduciendo la id de asignacion y la id de evaluacion sacaremos una lista con todas las respuestas
    public IEnumerable<RespuestaEntity> GetRespuestasFromAsigEval(int idEvaluacion, int IdAsignacion)
    {
      return _context.Respuestas.Where(r => r.PreguntaEntity.AsignacionId == IdAsignacion && r.EvaluacionId == idEvaluacion).ToList();
    }

    //Guarda todos los cambios en la base de datos
    public bool SaveChanges()
    {
      return (_context.SaveChanges() >= 0);
    }

    //Realiza un update de la respuesta por el id de la respuesta y el estado que se desea cambiar
    public bool UpdateRespuesta(RespuestaDto Respuesta)
    {
      RespuestaEntity respuestaAnterior = _context.Respuestas.Where(r => r.Id == Respuesta.Id).FirstOrDefault();
      if (respuestaAnterior != null)
      {
        respuestaAnterior.Estado = Respuesta.Estado;
        respuestaAnterior.Notas = Respuesta.Notas;
        respuestaAnterior.NotasAdmin = Respuesta.NotasAdmin;

        return SaveChanges();
      }
      else
      {
        return false;
      }
    }

    //Este metodo se usa cuando se quiere poner todas las respuestas de una asignacion a No Contestado
    //Excepto la primera, que se pone a No
    public bool UpdateRespuestasAsignacion(int idEvaluacion, int IdAsignacion)
    {
      var respuestas = _context.Respuestas.Where(r => r.PreguntaEntity.AsignacionId == IdAsignacion && r.EvaluacionId == idEvaluacion).ToList();
      if (respuestas != null)
      {
        int cont = 0;
        foreach(var resp in respuestas){
          if(cont == 0)
          {
            resp.Estado = 2;
          }
          else
          {
            resp.Estado = 0;
          }

          cont++;
        }

        return SaveChanges();
      }
      else
      {
        return false;
      }
    }

    //Introduciendo la id de evaluacion sacaremos una lista con todas las respuestas 
    public IEnumerable<RespuestaConNotasDto> GetRespuestasConNotas(int idEvaluacion)
    {
      var respuestas = _context.Respuestas.
        Include(r => r.PreguntaEntity).
        ThenInclude(p => p.AsignacionEntity).
        ThenInclude(p => p.SectionEntity).
        Where(r => r.EvaluacionId == idEvaluacion).ToList();


      List<RespuestaConNotasDto> lista = new List<RespuestaConNotasDto>();

      foreach (RespuestaEntity resp in respuestas) {
        lista.Add(new RespuestaConNotasDto
        {
          Id = resp.Id,
          Estado = resp.Estado,
          Correcta = resp.PreguntaEntity.Correcta,
          Notas = resp.Notas,
          NotasAdmin = resp.NotasAdmin,
          Asignacion = resp.PreguntaEntity.AsignacionEntity.Nombre,
          Section = resp.PreguntaEntity.AsignacionEntity.SectionEntity.Nombre,
          Pregunta = resp.PreguntaEntity.Pregunta
        });
      }

      return lista;
    }

    //Aqui introducimos una nueva respuesta
    public bool AddRespuesta(RespuestaEntity respuesta)
    {
      _context.Respuestas.Add(respuesta);
      return SaveChanges();
    }

    //Elimina una respuesta
    public bool DeleteRespuesta(RespuestaEntity respuesta)
    {
      _context.Respuestas.Remove(_context.Respuestas.Where(r => r == respuesta).FirstOrDefault());
      return SaveChanges();
    }

    //Muestra si existe la respuesta
    public bool ExiteRespuesta(int idRespuesta)
    {
      return _context.Respuestas.Any(r => r.Id == idRespuesta);
    }
  }
}
