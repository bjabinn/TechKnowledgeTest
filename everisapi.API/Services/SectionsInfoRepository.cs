using everisapi.API.Entities;
using everisapi.API.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace everisapi.API.Services
{
  public class SectionsInfoRepository : ISectionsInfoRepository
  {

    private AsignacionInfoContext _context;

    //Constructor genera el context
    public SectionsInfoRepository(AsignacionInfoContext context)
    {
      _context = context;
    }

    //Devolvemos el numero de preguntas de la seccion filtrada por proyecto
    public int GetNumPreguntasFromSection(int idSection, int idEvaluacion)
    {
      return _context.Respuestas.Where(r => r.EvaluacionId == idEvaluacion && r.PreguntaEntity.AsignacionEntity.SectionId == idSection).Count();
    }

    //Devolvemos el numero de preguntas respondidas de la seccion filtrada por proyecto
    public int GetRespuestasCorrectasFromSection(int idSection, int idEvaluacion)
    {
      return _context.Respuestas.Where(r => r.EvaluacionId == idEvaluacion && r.Estado == 1 && r.PreguntaEntity.AsignacionEntity.SectionId == idSection).Count();
    }

    //Devuelve un objeto estado con información extendida
    public IEnumerable<SectionInfoDto> GetSectionsInfoFromEval(int idEvaluacion)
    {
      //Recoge las respuestas de la evaluación
      List<SectionInfoDto> ListadoSectionInformacion = new List<SectionInfoDto>();
      var Respuestas = _context.Respuestas.
        Include(r => r.PreguntaEntity).
        ThenInclude(rp => rp.AsignacionEntity).
        ThenInclude(rpa => rpa.SectionEntity).
        Where(r => r.EvaluacionId == idEvaluacion).ToList();

      //Saca las en que secciones estuvo en ese momento
      var SectionsUtilizadas = Respuestas.Select(r => r.PreguntaEntity.AsignacionEntity.SectionEntity).Distinct().ToList();


      //Rellena los datos y los añade a la lista para cada sección
      foreach (var section in SectionsUtilizadas)
      {
        SectionInfoDto SectionAdd = new SectionInfoDto
        {
          Id = section.Id,
          Nombre = section.Nombre,
          Preguntas = Respuestas.Where(r => r.PreguntaEntity.AsignacionEntity.SectionEntity.Id == section.Id).Count(),
        };

        var notasSec = _context.NotasSections.Where(r => r.SectionId == section.Id && r.EvaluacionId == idEvaluacion).FirstOrDefault();

        if(notasSec == null)
        {
          notasSec = new NotasSectionsEntity
          {
            EvaluacionEntity = _context.Evaluaciones.Where(s => s.Id == idEvaluacion).FirstOrDefault(),
            SectionEntity = _context.Sections.Where(s => s.Id == section.Id).FirstOrDefault()
          };

          _context.NotasSections.Add(notasSec);
        }

        SectionAdd.Notas = notasSec.Notas;

        //Para calcular el progreso
        var listaAsignaciones = _context.Asignaciones.Where(r => r.SectionId == section.Id).ToList();

        int contestadas = 0;
        foreach(AsignacionEntity asig in listaAsignaciones)
        {
          var respuestasAsig = Respuestas.Where(p => p.PreguntaEntity.AsignacionEntity.Id == asig.Id).ToList();

          //Para ver si la primera es de las que habilitan a las demás o no
          //y si está contestada a NO (para contar las demás como contestadas
          bool flag = false;
          if(respuestasAsig[0].PreguntaEntity.Correcta == null && respuestasAsig[0].Estado == 2)
          {
            flag = true;
          }


          foreach(RespuestaEntity resp in respuestasAsig)
          {
            if(flag)
            {
              contestadas++;
            }
            else if (resp.Estado != 0)
            {
              contestadas++;
            }
          }

        }
        
        SectionAdd.Contestadas = contestadas;
        SectionAdd.Progreso = Math.Round( ((double)SectionAdd.Contestadas / SectionAdd.Preguntas) *100, 1);


        var listaRespuestas = Respuestas.Where(r => r.PreguntaEntity.AsignacionEntity.SectionEntity.Id == section.Id).ToList();
        double suma = 0;
        double puntosCorrectos = 0;

        foreach (var resp in listaRespuestas)
        {
          if (resp.PreguntaEntity.Correcta != null)
          {
            var maxPuntos = Respuestas.Where(r => r.PreguntaEntity.Correcta != null && r.PreguntaEntity.AsignacionId == resp.PreguntaEntity.AsignacionId).Count();
            var puntos = (double)resp.PreguntaEntity.AsignacionEntity.Peso / maxPuntos;

            puntosCorrectos += puntos;

            if (resp.Estado == 1 && resp.PreguntaEntity.Correcta.Equals("Si"))
            {
              suma += puntos;
            }

            else if (resp.Estado == 2 && resp.PreguntaEntity.Correcta.Equals("No"))
            {
              suma += puntos;
            }

          }
        }

        SectionAdd.RespuestasCorrectas = Math.Round(100 * suma/puntosCorrectos, 1);

        ListadoSectionInformacion.Add(SectionAdd);
      }

      return ListadoSectionInformacion;
    }

    //Devolvemos las asignaciones de una section
    IEnumerable<AsignacionEntity> ISectionsInfoRepository.GetAsignacionesFromSection(SectionEntity section)
    {
      var sectionSelected = _context.Sections.Where(p => p == section).FirstOrDefault();
      return sectionSelected.Asignaciones;
    }

    //Encuentra una asignacion filtrada por una section y la id de esa asignación
    AsignacionEntity ISectionsInfoRepository.GetAsignacionFromSection(SectionEntity section, int idAsignacion)
    {
      var sectionSelected = _context.Sections.Where(p => p == section).FirstOrDefault();
      return sectionSelected.Asignaciones.Where(a => a.Id == idAsignacion).FirstOrDefault();
    }


    //Recoge una sección por su id y puedes incluir las sections o no
    SectionEntity ISectionsInfoRepository.GetSection(int id, bool IncluirAsignaciones)
    {

      if (IncluirAsignaciones)
      {
        //Si se quiere incluir las asignaciones de la section entrara aquí
        //Incluimos las asignaciones de la section especificada (Include extiende de Microsoft.EntityFrameworkCore)
        return _context.Sections.Include(s => s.Asignaciones).
            Where(s => s.Id == id).FirstOrDefault();
      }
      else
      {
        //Si no es así devolveremos solo la section
        return _context.Sections.Where(p => p.Id == id).FirstOrDefault();
      }
    }

    //Recoge todas las sections
    IEnumerable<SectionEntity> ISectionsInfoRepository.GetSections()
    {
      return _context.Sections.ToList();
    }

    //Este metodo nos permite persistir los cambios en las entidades
    public bool SaveChanges()
    {
      return (_context.SaveChanges() >= 0);
    }

    //Aqui introducimos una nueva section
    public bool AddSection(SectionEntity section)
    {

      _context.Sections.Add(section);

      return SaveChanges();
    }

    //Nos permite modificar una section
    public bool AlterSection(SectionEntity section)
    {
      var SectionAlter = _context.Sections.Where(s => s.Id == section.Id).FirstOrDefault();

      SectionAlter.Nombre = section.Nombre;

      return SaveChanges();
    }

    //Nos permite modificar las notas una section
    public bool AddNotasSection(SectionWithNotasDto section)
    {
      var SectionAlter = _context.NotasSections.Where(s => s.SectionId == section.SectionId && s.EvaluacionId == section.EvaluacionId).FirstOrDefault();

      if(SectionAlter == null)
      {
        SectionAlter = new NotasSectionsEntity
        {
          EvaluacionEntity = _context.Evaluaciones.Where(s => s.Id == section.EvaluacionId).FirstOrDefault(),
          SectionEntity = _context.Sections.Where(s => s.Id == section.SectionId).FirstOrDefault()
        };

        _context.NotasSections.Add(SectionAlter);
      }

      SectionAlter.Notas = section.Notas;

      return SaveChanges();
    }

    //Elimina una section
    public bool DeleteSection(SectionEntity section)
    {

      _context.Sections.Remove(_context.Sections.Where(s => s == section).FirstOrDefault());

      return SaveChanges();
    }

  }
}
