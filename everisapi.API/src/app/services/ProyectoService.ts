
import {throwError as observableThrowError,  Observable } from 'rxjs';

import {catchError, map} from 'rxjs/operators';
import { Injectable, Inject } from '@angular/core';
import { Http, Response, Headers, RequestOptions } from '@angular/http';
import { Router } from '@angular/router';



import { AppComponent } from '../app.component';

@Injectable()
export class ProyectoService {
  public url: string;
  public UsuarioLogeado: string;

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

  //Este metdo nos permite verificar si el usuario ya esta logeado en la web
  //Puede estar recordado o puede estar iniciado solo para esta sesión
  //Si no esta logeado de ninguna manera enviara false
  public verificarUsuario() {
    var local = localStorage.getItem("user");
    var storage = this._appComponent._storageDataService.UserData;
    if (local != null && local != undefined) {
      this.UsuarioLogeado = localStorage.getItem("user");
      return true;
    } else if (storage != undefined && storage != null) {
      this.UsuarioLogeado = this._appComponent._storageDataService.UserData.nombre;
      return true;
    } else {
      return false;
    }
  }

  //Este metodo devuelve todos los proyectos de todos los usuarios
  getAllProyectos() {
    let Token = this._appComponent.ComprobarUserYToken();
    let headers = new Headers({
      'Authorization': Token
    });
    return this._http.get(this.url + 'users/fullproyectos', { headers: headers }).pipe(
      map((response: Response) => response.json()),
      catchError(this.errorHandler),);
  }

  //Este metodo recoge todos los proyectos de un usuario de la base de datos
  getProyectosDeUsuario() {
    let Token = this._appComponent.ComprobarUserYToken();
    let headers = new Headers({
      'Authorization': Token
    });
    return this._http.get(this.url + 'users/' + this.UsuarioLogeado + "/proyectos", { headers: headers }).pipe(
      map((response: Response) => response.json()),
      catchError(this.errorHandler),);
  }

  //Este metodo recoge un proyecto de un usuario si existe mediante un nombre de usuario y su id de proyecto
  getOneProjecto(idProyecto) {
    let Token = this._appComponent.ComprobarUserYToken();
    let headers = new Headers({
      'Authorization': Token
    });
    return this._http.get(this.url + 'users/' + this.UsuarioLogeado + '/proyecto/' + idProyecto, { headers: headers }).pipe(
      map((response: Response) => response.json()),
      catchError(this.errorHandler),);
  }

  //Este metodo devuelve todos los permisos de un usuario
  getRolesUsuario() {
    let Token = this._appComponent.ComprobarUserYToken();
    let headers = new Headers({
      'Authorization': Token
    });
    return this._http.get(this.url + 'users/' + this.UsuarioLogeado + '/roles', { headers: headers }).pipe(
      map((response: Response) => response.json()),
      catchError(this.errorHandler),);
  }

  //Implementamos este metodo para permitir la recogida de los errores y su gestión
  errorHandler(error: Response) {
    return observableThrowError(error.status);
  }

}
