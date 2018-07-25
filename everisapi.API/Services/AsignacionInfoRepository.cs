using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using everisapi.API.Entities;
using everisapi.API.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;

namespace everisapi.API.Services
{
  public class AsignacionInfoRepository : IAsignacionInfoRepository
  {

    private AsignacionInfoContext _context;

    //Le damos un contexto en el constructor
    public AsignacionInfoRepository(AsignacionInfoContext context)
    {
      _context = context;
    }

    //Comprobamos si una asignación concreta existe
    public bool AsignacionExiste(int AsignacionId)
    {
      return _context.Asignaciones.Any(a => a.Id == AsignacionId);
    }

    //Recogemos una asignación con o sin preguntas incluidas
    public AsignacionEntity GetAsignacion(int AsignacionId, Boolean IncluirPreguntas)
    {

      if (IncluirPreguntas)
      {
        //Si se quiere incluir las preguntas de la asignación entrara aquí
        //Incluimos las preguntas de la asignación especificada (Include extiende de Microsoft.EntityFrameworkCore)
        return _context.Asignaciones.Include(c => c.PreguntasDeAsignacion).
            Where(c => c.Id == AsignacionId).FirstOrDefault();
      }
      else
      {
        //Si no es así devolveremos solo la asignación
        return _context.Asignaciones.Where(c => c.Id == AsignacionId).FirstOrDefault();
      }
    }

    //Recogemos una lista completa de asignaciones
    public IEnumerable<AsignacionEntity> GetAsignaciones()
    {
      //Devolvemos todas las asignaciones ordenadas por Nombre
      return _context.Asignaciones.OrderBy(c => c.Nombre).ToList();
    }

    //Devuelve todas las asignaciones con datos extendidos filtrado por evaluacion
    public IEnumerable<AsignacionInfoDto> GetAsignFromEval(int idEval)
    {
      List<AsignacionInfoDto> AsignacionesInfo = new List<AsignacionInfoDto>();
      List<RespuestaDto> ListaRespuestas = Mapper.Map<List<RespuestaDto>>(_context.Respuestas.Where(r => r.EvaluacionId == idEval).ToList());



      var asignaciones = (from res in _context.Respuestas
                          where res.EvaluacionId == idEval
                          select new { res.Id, res.PreguntaId, res.Estado, res.EvaluacionId } into respuestas
                          join p in _context.Preguntas on respuestas.PreguntaId equals p.Id
                          select new { p.Id, p.AsignacionId, p.Pregunta, } into preguntas
                          join asig in _context.Asignaciones on preguntas.AsignacionId equals asig.Id
                          select new { asig.Id, asig.Nombre, asig.PreguntasDeAsignacion } into asignacionesEntity
                          select asignacionesEntity).Distinct().ToList();
      foreach (var asignacion in asignaciones)
      {
        var introducirasig = new AsignacionInfoDto
        {
          Id = asignacion.Id,
          Nombre = asignacion.Nombre,
          Preguntas = ChangePregunta(asignacion.PreguntasDeAsignacion, ListaRespuestas)
        };

        var nota = _context.NotasAsignaciones.Where(r => r.EvaluacionId == idEval && r.AsignacionId == asignacion.Id).FirstOrDefault();

        if (nota != null)
        {
          introducirasig.Notas = nota.Notas;
        }

        AsignacionesInfo.Add(introducirasig);
      }

      return AsignacionesInfo;
    }

    //Filtra por evaluación y sección devolviendo una lista de asignaciones
    public IEnumerable<AsignacionInfoDto> GetAsignFromEvalAndSection(int idEval, int idSection)
    {
      List<AsignacionInfoDto> AsignacionesInfo = new List<AsignacionInfoDto>();
      List<RespuestaDto> ListaRespuestas = Mapper.Map<List<RespuestaDto>>(_context.Respuestas.Where(r => r.EvaluacionId == idEval).ToList());

      var asignaciones = (from res in _context.Respuestas
                          where res.EvaluacionId == idEval
                          select new { res.Id, res.PreguntaId, res.Estado, res.EvaluacionId } into respuestas
                          join p in _context.Preguntas on respuestas.PreguntaId equals p.Id
                          select new { p.Id, p.AsignacionId, p.Pregunta, } into preguntas
                          join asig in _context.Asignaciones on preguntas.AsignacionId equals asig.Id
                          where asig.SectionId == idSection
                          select new { asig.Id, asig.Nombre, asig.PreguntasDeAsignacion } into asignacionesEntity
                          select asignacionesEntity).Distinct().ToList();

      foreach (var asignacion in asignaciones)
      {
        var introducirasig = new AsignacionInfoDto
        {
          Id = asignacion.Id,
          Nombre = asignacion.Nombre,
          Preguntas = ChangePregunta(asignacion.PreguntasDeAsignacion, ListaRespuestas),
        };

        var nota = _context.NotasAsignaciones.Where(r => r.EvaluacionId == idEval && r.AsignacionId == asignacion.Id).FirstOrDefault();

        if(nota != null)
        {
          introducirasig.Notas = nota.Notas;
        }

        AsignacionesInfo.Add(introducirasig);
      }

      return AsignacionesInfo;
    }

    //Devuelve todas las asignaciones con datos extendidos filtrado por evaluacion
    public AsignacionInfoDto GetAsignFromEvalAndAsig(int idEval, int idAsig)
    {

      AsignacionInfoDto AsignacionesInfo = new AsignacionInfoDto();
      List<RespuestaDto> ListaRespuestas = Mapper.Map<List<RespuestaDto>>(_context.Respuestas.Where(r => r.EvaluacionId == idEval).ToList());

      var asignacionBD = (from res in _context.Respuestas
                          where res.EvaluacionId == idEval
                          select new { res.Id, res.PreguntaId, res.Estado, res.EvaluacionId } into respuestas
                          join p in _context.Preguntas on respuestas.PreguntaId equals p.Id
                          select new { p.Id, p.AsignacionId, p.Pregunta, p.Correcta} into preguntas
                          join asig in _context.Asignaciones on preguntas.AsignacionId equals asig.Id
                          where asig.Id == idAsig
                          select new { asig.Id, asig.Nombre, asig.PreguntasDeAsignacion } into asignacionesEntity
                          select asignacionesEntity).Distinct().FirstOrDefault();


      AsignacionesInfo.Id = asignacionBD.Id;
      AsignacionesInfo.Nombre = asignacionBD.Nombre;
      AsignacionesInfo.Preguntas = ChangePregunta(asignacionBD.PreguntasDeAsignacion, ListaRespuestas);

      var nota = _context.NotasAsignaciones.Where(r => r.EvaluacionId == idEval && r.AsignacionId == idAsig).FirstOrDefault();

      if (nota != null)
      {
        AsignacionesInfo.Notas = nota.Notas;
      }

      return AsignacionesInfo;
    }

    /*Metodo que convierte una pregunta y una respuesta en una Dto devolviendola
    public PreguntaWithOneRespuestasDto changePregunta( PreguntaEntity Pregunta,RespuestaEntity Respuesta) {
      var PreguntaConRespuesta = new PreguntaWithOneRespuestasDto { Id = Pregunta.Id, Pregunta = Pregunta.Pregunta, Respuesta = Mapper.Map<RespuestaDto>(Respuesta) };
      return PreguntaConRespuesta;
    }*/

    // Metodo que devolvia una lista
    public List<PreguntaWithOneRespuestasDto> ChangePregunta(IEnumerable<PreguntaEntity> Preguntas, IEnumerable<RespuestaDto> Respuestas)
    {
      List<PreguntaWithOneRespuestasDto> PreguntasConRespuestas = new List<PreguntaWithOneRespuestasDto>();
      foreach (var pregunta in Preguntas)
      {
        var RespuestaParaPregunta = Respuestas.Where(r => r.PreguntaId == pregunta.Id).FirstOrDefault();

        var PreguntaAdd = new PreguntaWithOneRespuestasDto { Id = pregunta.Id, Pregunta = pregunta.Pregunta, Correcta = pregunta.Correcta,
          Respuesta = RespuestaParaPregunta };

        PreguntasConRespuestas.Add(PreguntaAdd);
      }
      PreguntasConRespuestas = PreguntasConRespuestas.OrderBy(p => p.Id).ToList();
      return PreguntasConRespuestas;

    }

    //Recogemos una pregunta de una asignación
    public PreguntaEntity GetPreguntaDeAsignacion(int AsignacionId, int PreguntaId)
    {
      //Devolvemos una pregunta especifica de una Asignación
      return _context.Preguntas.Where(p => p.AsignacionId == AsignacionId && p.Id == PreguntaId).FirstOrDefault();
    }

    //Recogemos la lista de preguntas de una asignación
    //En el caso de que no exista devolvera un array vacio e igualmente si la lista de la ciudad esta vacia
    public IEnumerable<PreguntaEntity> GetPreguntaPorAsignacion(int AsignacionId)
    {
      //Devolvemos una lista de preguntas de una asignación especifica
      return _context.Preguntas.Where(p => p.AsignacionId == AsignacionId).ToList();
    }

    /*INTRODUCIR DATOS*/

    //Aqui encontramos la asignación y le incluimos la pregunta
    public void IncluirPreguntaParaAsignacion(int asignacionId, PreguntaEntity pregunta)
    {
      var asignacion = GetAsignacion(asignacionId, false);
      asignacion.PreguntasDeAsignacion.Add(pregunta);
    }

    //Este metodo nos permite añadir una nueva asignación
    public bool AddAsig(AsignacionEntity asignacion)
    {

      _context.Asignaciones.Add(asignacion);

      return SaveChanges();
    }


    //Este metodo nos permite añadir notas para una asignación
    public bool AddNotas(AsignacionUpdateNotasDto asignacion)
    {

      var asig = _context.NotasAsignaciones.Where(r => r.EvaluacionId == asignacion.EvId && r.AsignacionId == asignacion.Id).FirstOrDefault();


      if(asig == null)
      {
        asig = new NotasAsignacionesEntity
        {
          AsignacionId = asignacion.Id,
          EvaluacionId = asignacion.EvId,
        };
        _context.NotasAsignaciones.Add(asig);

      }

      asig.Notas = asignacion.Notas;

      return SaveChanges();
    }

    //Este metodo nos permite persistir los cambios en las entidades
    public bool SaveChanges()
    {
      return (_context.SaveChanges() >= 0);
    }

    /*MODIFICAR DATOS*/
    //Este metodo nos permite alterar la información de una asignación
    public bool AlterAsig(AsignacionEntity asignacion)
    {
      var AlterAsig = _context.Asignaciones.Include(a => a.PreguntasDeAsignacion).Include(a => a.SectionEntity).Where(a => a.Id == asignacion.Id).FirstOrDefault();
      if (AlterAsig == null)
      {
        return false;
      }
      AlterAsig.Nombre = asignacion.Nombre;
      AlterAsig.SectionId = asignacion.SectionId;
      /*
      //Elimina las preguntas previas
      foreach (var pregunta in AlterAsig.PreguntasDeAsignacion)
      {
        _context.Preguntas.Remove(pregunta);
      }

      //Introduce las preguntas nuevas
      foreach (var pregunta in asignacion.PreguntasDeAsignacion)
      {
        _context.Preguntas.Add(pregunta);
      }*/

      return SaveChanges();
    }

    /*ELIMINAR DATOS*/
    //Elimina una pregunta concreta de una asignación
    public void EliminarPreguntaDeAsignacion(PreguntaEntity pregunta)
    {
      _context.Preguntas.Remove(pregunta);
    }

    //Devuelve todas las asignaciones con notas de esa evaluacion
    public IEnumerable<AsignacionConNotasDto> GetAsignConNotas(int idEval)
    {
      List<AsignacionConNotasDto> AsignacionesInfo = new List<AsignacionConNotasDto>();

      var asignaciones = _context.NotasAsignaciones.Include(r => r.AsignacionEntity).ThenInclude(a => a.SectionEntity).
        Where(n => n.EvaluacionId == idEval).ToList();

      foreach(NotasAsignacionesEntity asig in asignaciones)
      {
        var nuevaAsignacion = new AsignacionConNotasDto
        {
          Asignacion = asig.AsignacionEntity.Nombre,
          Section = asig.AsignacionEntity.SectionEntity.Nombre,
          Notas = asig.Notas
        };

        AsignacionesInfo.Add(nuevaAsignacion);
      }

      return AsignacionesInfo;

    }

      //Este metodo elimina una asignación
      public bool DeleteAsig(AsignacionEntity asignacion)
    {

      _context.Asignaciones.Remove(asignacion);

      return SaveChanges();
    }
  }
}
