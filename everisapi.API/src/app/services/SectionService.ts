
import {throwError as observableThrowError,  Observable } from 'rxjs';

import {map, catchError} from 'rxjs/operators';
import { Injectable, Inject } from '@angular/core';
import { Http, Response, Headers, RequestOptions } from '@angular/http';
import { Router } from '@angular/router';



import { SectionModify } from 'app/Models/SectionModify';
import { AppComponent } from 'app/app.component';

@Injectable()
export class SectionService {
  public identity;
  public token;
  public url: string;

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

  //Este metodo recoge todos los usuarios de la base de datos
  getSections() {
    let Token = this._appComponent.ComprobarUserYToken();
    let headers = new Headers({
      'Authorization': Token
    });
    return this._http.get(this.url + 'sections', { headers: headers }).pipe(
      map((response: Response) => response.json()),
      catchError(this.errorHandler),);
  }

  //Este metodo recoge un usuario si existe mediante un nombre de usuario
  getOneSection(id) {
    let Token = this._appComponent.ComprobarUserYToken();
    let headers = new Headers({
      'Authorization': Token
    });
    return this._http.get(this.url + 'sections/' + id, { headers: headers }).pipe(
      map((response: Response) => response.json()),
      catchError(this.errorHandler),);
  }

  //Este metodo recoge un usuario si existe mediante un nombre de usuario
  getAsignacionesSection(id) {
    let Token = this._appComponent.ComprobarUserYToken();
    let headers = new Headers({
      'Authorization': Token
    });
    return this._http.get(this.url + 'sections/' + id + '/asignaciones', { headers: headers }).pipe(
      map((response: Response) => response.json()),
      catchError(this.errorHandler),);
  }

  //Devuelve el numero de preguntas para cada sección segun que proyecto selecciones
  getPreguntasSection(idSection, idProject) {
    let Token = this._appComponent.ComprobarUserYToken();
    let headers = new Headers({
      'Authorization': Token
    });
    return this._http.get(this.url + 'sections/' + idSection + '/evaluacion/' + idProject + "/preguntas", { headers: headers }).pipe(
      map((response: Response) => response.json()),
      catchError(this.errorHandler),);
  }

  //Devuelve el numero de respuestas correctas para cada sección segun que proyecto selecciones
  getRespuestasSection(idSection, idProject) {
    let Token = this._appComponent.ComprobarUserYToken();
    let headers = new Headers({
      'Authorization': Token
    });
    return this._http.get(this.url + 'sections/' + idSection + '/evaluacion/' + idProject + "/respuestas", { headers: headers }).pipe(
      map((response: Response) => response.json()),
      catchError(this.errorHandler),);
  }

  //Recoge todos los datos extendidos de una evaluación
  getSectionInfo(idEvaluacion) {
    let Token = this._appComponent.ComprobarUserYToken();
    let headers = new Headers({
      'Authorization': Token
    });
    return this._http.get(this.url + 'sections/evaluacion/' + idEvaluacion, { headers: headers }).pipe(
      map((response: Response) => response.json()),
      catchError(this.errorHandler),);
  }

  //Devuelve las preguntas para una asignación en especifico
  getPreguntasArea(id) {
    let Token = this._appComponent.ComprobarUserYToken();
    let headers = new Headers({
      'Authorization': Token
    });
    return this._http.get(this.url + 'asignaciones/' + id + '/preguntas', { headers: headers }).pipe(
      map((response: Response) => response.json()),
      catchError(this.errorHandler),);
  }

  //Añade una nota para la seccion indicada a la base de datos
  addNota(SeccionUpdate: SectionModify) {
    let Token = this._appComponent.ComprobarUserYToken();
    let params = JSON.stringify(SeccionUpdate);
    let headers = new Headers({
      'Authorization': Token,
      'Content-Type': 'application/json'
    });

    return this._http.put(this.url + 'sections/addNotas/', params, { headers: headers }).pipe(
      map(res => res));
  }

  //Obtiene todas las respuestas con notas para esta evaluacion
  getRespuestasConNotas(id) {
    let Token = this._appComponent.ComprobarUserYToken();
    let headers = new Headers({
      'Authorization': Token
    });

    return this._http.get(this.url + 'respuestas/evaluacion/' + id, { headers: headers }).pipe(
      map((response: Response) => response.json()),
      catchError(this.errorHandler),);
  }

  //Obtiene todas las asignaciones con notas para esta evaluacion
  getAsignConNotas(id) {
    let Token = this._appComponent.ComprobarUserYToken();
    let headers = new Headers({
      'Authorization': Token
    });

    return this._http.get(this.url + 'asignaciones/evaluacion/' + id + '/notas', { headers: headers }).pipe(
      map((response: Response) => response.json()),
      catchError(this.errorHandler),);
  }


  //Implementamos este metodo para permitir la recogida de los errores y su gestión
  errorHandler(error: Response) {
    return observableThrowError(error.status);
  }

}
