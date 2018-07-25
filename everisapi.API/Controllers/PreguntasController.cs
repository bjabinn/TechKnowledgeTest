using everisapi.API.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using everisapi.API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;

namespace everisapi.API.Controllers
{

  [Authorize]
  [Route("api/asignaciones")]
  public class PreguntasController : Controller
  {
    //Inyectamos un logger
    private ILogger<PreguntasController> _logger;
    private IAsignacionInfoRepository _asignacionInfoRepository;

    //Utilizamos el constructor para inicializar el logger
    public PreguntasController(ILogger<PreguntasController> logger, IAsignacionInfoRepository asignacionInfoRepository)
    {
      _logger = logger;
      _asignacionInfoRepository = asignacionInfoRepository;
    }

    /*METODOS GET DE PREGUNTAS*/

    //Recoge todas las preguntas de una asignación mediante la id
    [HttpGet("{asignacionid}/preguntas")]
    public IActionResult GetPreguntasAsignacion(int asignacionId)
    {
      try
      {
        //Comprueba si existe asignación y si existe manda un json con la información
        //si no existe mandara un error 404 el error 500 aparecera si el servidor falla
        if (!_asignacionInfoRepository.AsignacionExiste(asignacionId))
        {
          _logger.LogInformation($"La asignación con id {asignacionId} no pudo ser encontrado.");
          return NotFound();
        }

        //Recogemos una lista de preguntas de la asignacion
        var preguntasDeAsignacion = _asignacionInfoRepository.GetPreguntaPorAsignacion(asignacionId);

        //Transformamos la lista anterior en una nueva con los datos que necesitamos
        //Ya que otros son relevantes
        var preguntaDeAsignacionResult = Mapper.Map<IEnumerable<PreguntaDto>>(preguntasDeAsignacion);
        return Ok(preguntaDeAsignacionResult);
      }
      catch (Exception ex)
      {
        _logger.LogCritical("Se recogio un error al recibir la petición de asignación preguntas concreta con id " + asignacionId + ": " + ex);
        return StatusCode(500, "Un error ha ocurrido mientras se procesaba su petición.");
      }
    }


    //Recoge una pregunta concreta de una asignación
    [HttpGet("{asignacionid}/pregunta/{id}", Name = "GetPregunta")]
    public IActionResult GetPreguntaAsignacion(int asignacionId, int id)
    {
      try
      {

        //Comprobamos si la asignacion existe
        if (!_asignacionInfoRepository.AsignacionExiste(asignacionId))
        {
          _logger.LogInformation("La asignación con id " + asignacionId + " no pudo ser encontrado.");
          return NotFound();
        }

        //Comprobamos si la pregunta existe
        var preguntasDeAsignacion = _asignacionInfoRepository.GetPreguntaDeAsignacion(asignacionId, id);

        if (preguntasDeAsignacion == null)
        {
          _logger.LogInformation("La asignación con id " + asignacionId + " no pudo ser encontrado.");
          return NotFound();
        }

        //Si existe creamos una nueva pregunta con los datos estrictamente necesarios

        var preguntasDeAsignacionResult = Mapper.Map<PreguntaDto>(preguntasDeAsignacion);

        return Ok(preguntasDeAsignacionResult);
      }
      catch (Exception ex)
      {
        _logger.LogCritical("Se recogio un error al recibir la petición de pregunta sobre asignación concreta con id de pregunta " + id + " en asignacion " + asignacionId + ": " + ex);
        return StatusCode(500, "Un error ha ocurrido mientras se procesaba su petición.");
      }
    }

    /*METODOS POST DE PREGUNTAS*/
    //Este metodo nos permite introducir una pregunta a una asignación
    [HttpPost("{asignacionId}/pregunta")]
    public IActionResult CreatePregunta(int asignacionId,
        [FromBody] PreguntaCreateDto PreguntaRecogida)
    {
      try
      {
        //Comprueba que el body del json es correcto sino devolvera null
        //Si esto ocurre devolveremos un error
        if (PreguntaRecogida == null)
        {
          return BadRequest();
        }

        //Si no cumple con el modelo de creación devuelve error
        if (!ModelState.IsValid)
        {
          return BadRequest(ModelState);
        }

        //Comprueba si la asignación existe sino dara una respuesta 404
        if (!_asignacionInfoRepository.AsignacionExiste(asignacionId))
        {
          _logger.LogInformation("La asignación con id " + asignacionId + " no pudo ser encontrado.");
          return NotFound();
        }

        //Hacemos un mapeo de la pregunta que recogimos
        var IngresarPregunta = Mapper.Map<Entities.PreguntaEntity>(PreguntaRecogida);

        //La incluimos en la asignación
        _asignacionInfoRepository.IncluirPreguntaParaAsignacion(asignacionId, IngresarPregunta);

        //Guardamos los cambios a la entidad y esta debera devolver si es correcta o no
        if (!_asignacionInfoRepository.SaveChanges())
        {
          _logger.LogCritical("Ocurrio un error al guardar los cambios cuando intentamos incluir una pregunta a la asignación: " + asignacionId);
          return StatusCode(500, "Ocurrio un problema en la petición.");
        }

        //Si todo a salido bien recogemos la pregunta y la mostramos en el return siguiente
        var PreguntaCreadaDevuelta = Mapper.Map<Models.PreguntaDto>(IngresarPregunta);

        return CreatedAtRoute("GetPregunta", new { asignacionId = asignacionId, id = PreguntaCreadaDevuelta.Id }, PreguntaCreadaDevuelta);
      }
      catch (Exception ex)
      {
        _logger.LogCritical("Se recogio un error al recibir la petición de creación de pregunta: " + ex);
        return StatusCode(500, "Un error ha ocurrido mientras se procesaba su petición.");
      }
    }

    /*METODOS UPDATE DE PREGUNTAS*/
    //Con este metodo modificamos una pregunta de una asignación 
    [HttpPut("{asignacionId}/pregunta/{id}")]
    public IActionResult UpdatePregunta(int asignacionId, int id, [FromBody] PreguntaUpdateDto PreguntaCambiar)
    {
      try
      {
        //Hace todas las comprobaciones de si las preguntas estan bien formadas
        //Tambien comprueba si existe la asignación y la pregunta y lo modifican si es asi
        if (PreguntaCambiar == null)
        {
          return BadRequest();
        }

        if (!ModelState.IsValid)
        {
          return BadRequest(ModelState);
        }

        if (!_asignacionInfoRepository.AsignacionExiste(asignacionId))
        {
          _logger.LogInformation($"La asignación con id {asignacionId} no pudo ser encontrado.");
          return NotFound();
        }

        var PreguntaEncontrada = _asignacionInfoRepository.GetPreguntaDeAsignacion(asignacionId, id);

        if (PreguntaEncontrada == null)
        {
          return NotFound();
        }

        Mapper.Map(PreguntaCambiar, PreguntaEncontrada);

        if (!_asignacionInfoRepository.SaveChanges())
        {
          _logger.LogCritical("Ocurrio un error al guardar los cambios cuando intentamos actualizar una pregunta de asignación: " + asignacionId + ", id de pregunta: " + id);
          return StatusCode(500, "Ocurrio un problema en la petición.");
        }

        //Si todo salio bien dara un mensaje 200 con todo correcto
        return Ok("Actualización correcta.");
      }
      catch (Exception ex)
      {
        _logger.LogCritical("Se recogio un error al recibir la petición de modificación de pregunta con id " + id + " y la asignación con id " + asignacionId + ": " + ex);
        return StatusCode(500, "Un error ha ocurrido mientras se procesaba su petición.");
      }
    }

    /*METODOS DELETE DE PREGUNTAS*/
    //Aqui podremos eliminar una pregunta de una asignación
    [HttpDelete("{asignacionId}/pregunta/{id}")]
    public IActionResult DeletePregunta(int asignacionId, int id)
    {
      try
      {
        //Comprueba que todo existe y si es asi elimina la pregunta de la asignación
        if (!_asignacionInfoRepository.AsignacionExiste(asignacionId))
        {
          _logger.LogInformation("La asignación con id " + asignacionId + " no pudo ser encontrado.");
          return NotFound();
        }

        var PreguntaEncontrada = _asignacionInfoRepository.GetPreguntaDeAsignacion(asignacionId, id);

        if (PreguntaEncontrada == null)
        {
          return NotFound();
        }

        _asignacionInfoRepository.EliminarPreguntaDeAsignacion(PreguntaEncontrada);

        if (!_asignacionInfoRepository.SaveChanges())
        {
          _logger.LogCritical("Ocurrio un error al guardar los cambios cuando intentamos eliminar una pregunta de asignación: " + asignacionId + ", id de pregunta: " + id);
          return StatusCode(500, "Ocurrio un problema en la petición.");
        }

        //Si todo salio bien devuelve el mensaje con codigo 200
        return Ok("La pregunta fue eliminada");
      }
      catch (Exception ex)
      {
        _logger.LogCritical("Se recogio un error al recibir la petición de eliminación de pregunta con id " + id + " de la asignación con id " + asignacionId + ": " + ex);
        return StatusCode(500, "Un error ha ocurrido mientras se procesaba su petición.");
      }
    }
  }
}
