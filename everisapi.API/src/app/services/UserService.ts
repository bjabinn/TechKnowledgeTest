
import {throwError as observableThrowError,  Observable } from 'rxjs';

import {catchError, map} from 'rxjs/operators';
import { Injectable, Inject } from '@angular/core';
import { Http, Response, Headers, RequestOptions } from '@angular/http';
import { Router } from '@angular/router';



import { User } from 'app/Models/User';
import { AppComponent } from 'app/app.component';

@Injectable()
export class UserService {
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
  /*getUsers(){
      return this._http.get(this.url + 'users')  
          .map((response: Response) => response.json())  
          .catch(this.errorHandler);
  }*/

  //Este metodo recoge un usuario si existe mediante un nombre de usuario
  //Metodo de prueba
  /*SignUpMe(nombreUsuario) {
    return this._http.get(this.url + 'users/' + nombreUsuario)
      .map((response: Response) => response.json())
      .catch(this.errorHandler);
  }*/

  //Nos permite recoger un token si existe el usuario
  SignUpMe(Usuario: User) {
    let params = JSON.stringify(Usuario);
    let headers = new Headers({
      'Content-Type': 'application/json'
    });
    return this._http.post(this.url + 'Token', params, { headers: headers }).pipe(
      map(res => res.json()),
      catchError(this.errorHandler),);
  }

  //Implementamos este metodo para permitir la recogida de los errores y su gestión
  errorHandler(error: Response) {
    return observableThrowError(error.status);
  }

}
