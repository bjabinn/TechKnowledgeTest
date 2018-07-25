
import {throwError as observableThrowError,  Observable } from 'rxjs';

import {catchError, map} from 'rxjs/operators';
import { Injectable, Inject } from '@angular/core';
import { Http, Response, Headers, RequestOptions } from '@angular/http';
import { HttpParams } from '@angular/common/http';
import { Router } from '@angular/router';



import { AppComponent } from '../app.component';
import { AsignacionUpdate } from 'app/Models/AsignacionUpdate';
import { Respuesta } from 'app/Models/Respuesta';

@Injectable()
export class RespuestasService {

  private url: string;

  constructor(private _http: Http,
    private _appComponent: AppComponent) {
    /*//DESCOMENTAR
    var loc = window.location.href;
    var index = 0;
    for (var i = 0; i < 3; i++) {
      index = loc.indexOf("/", index + 1);
    }

    this.url = loc.substring(0, index) + "/api/";
    */

    //COMENTAR
    this.url = "http://localhost:60406/api/";
  }


  //Este metodo devuelve todas las respuestas de una asignacion en un proyecto
  getRespuestasAsigProy(idEvaluacion: number, idAsignacion: number) {
    let Token = this._appComponent.ComprobarUserYToken();
    let headers = new Headers({
      'Authorization': Token
    });
    return this._http.get(this.url + 'respuestas/evaluacion/' + idEvaluacion + '/asignacion/' + idAsignacion, { headers: headers }).pipe(
      map((response: Response) => response.json()),
      catchError(this.errorHandler),);
  }

  //Este metodo devuelve un listado con sus preguntas y respuestas de una evaluación y su asignación
  getRespuestasAsig(idEvaluacion: number, idAsig: number) {
    let Token = this._appComponent.ComprobarUserYToken();
    let headers = new Headers({
      'Authorization': Token
    });

    return this._http.get(this.url + 'asignaciones/evaluacion/' + idEvaluacion + '/asignacion/' + idAsig, { headers: headers }).pipe(
      map((response: Response) => response.json()),
      catchError(this.errorHandler),);
  }

  //Este metodo se usa cuando se quiere poner todas las respuestas de una asignacion a No Contestado
  //Excepto la primera, que se pone a No
  updateRespuestasAsig(idEvaluacion: number, idAsig: number) {
    let Token = this._appComponent.ComprobarUserYToken();
    let headers = new Headers({
      'Authorization': Token
    });


    return this._http.get(this.url + 'respuestas/evaluacion/' + idEvaluacion + '/asignacion/' + idAsig + '/update', { headers: headers }).pipe(
      map(res => res));
  }

  //Este metodo altera el valor de la respuesta en la base de datos
  AlterRespuesta(RespuestaForUpdate: Respuesta) {
    let Token = this._appComponent.ComprobarUserYToken();
    let params = JSON.stringify(RespuestaForUpdate);
    let headers = new Headers({
      'Authorization': Token,
      'Content-Type': 'application/json'
    });

    return this._http.put(this.url + 'respuestas/update/', params, { headers: headers }).pipe(
      map(res => res));
  }

  //Este metodo altera la nota de dicha asignacion
  AddNotaAsig(AsigConNota: AsignacionUpdate) {
    let Token = this._appComponent.ComprobarUserYToken();
    let params = JSON.stringify(AsigConNota);
    let headers = new Headers({
      'Authorization': Token,
      'Content-Type': 'application/json'
    });

    return this._http.put(this.url + 'asignaciones/addNotas/', params, { headers: headers }).pipe(
      map(res => res));
  }

  //Implementamos este metodo para permitir la recogida de los errores y su gestión
  errorHandler(error: Response) {
    return observableThrowError(error.status);
  }

}
