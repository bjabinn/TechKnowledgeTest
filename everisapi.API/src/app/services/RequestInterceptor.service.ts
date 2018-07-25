
import {throwError as observableThrowError,  Observable ,  BehaviorSubject } from 'rxjs';

import {take, filter, catchError, switchMap, finalize} from 'rxjs/operators';
import { Injectable, Injector } from "@angular/core";
import { HttpInterceptor, HttpRequest, HttpHandler, HttpSentEvent, HttpHeaderResponse, HttpProgressEvent, HttpResponse, HttpUserEvent, HttpErrorResponse } from "@angular/common/http";







import { UserService } from "app/services/UserService";
import { AppComponent } from "app/app.component";
import { HttpClient } from "@angular/common/http/src/client";

@Injectable()
export class RequestInterceptorService implements HttpInterceptor {

  isRefreshingToken: boolean = false;
  tokenSubject: BehaviorSubject<string> = new BehaviorSubject<string>(null);

  constructor(private _userService: UserService,
    private injector: Injector,
    private _appComponent: AppComponent) { }

  addToken(req: HttpRequest<any>, token: string): HttpRequest<any> {
    return req.clone({ setHeaders: { Authorization: 'Bearer ' + token } })
  }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpSentEvent | HttpHeaderResponse | HttpProgressEvent | HttpResponse<any> | HttpUserEvent<any>> {
    return next.handle(this.addToken(req, this._appComponent._storageDataService.GetToken())).pipe(
      catchError(error => {
        if (error instanceof HttpErrorResponse) {
          switch ((<HttpErrorResponse>error).status) {
            case 400:
              return this.handle400Error(error);
            case 401:
              return this.handle401Error(req, next);
          }
        } else {
          return observableThrowError(error);
        }
      }));
  }

  handle400Error(error) {
    if (error && error.status === 400 && error.error && error.error.error === 'invalid_grant') {
      // If we get a 400 and the error message is 'invalid_grant', the token is no longer valid so logout.
      return this.logoutUser();
    }

    return observableThrowError(error);
  }

  handle401Error(req: HttpRequest<any>, next: HttpHandler) {
    if (!this.isRefreshingToken) {
      this.isRefreshingToken = true;

      // Reset here so that the following requests wait until the token
      // comes back from the refreshToken call.
      this.tokenSubject.next(null);

      return this._userService.SignUpMe(this._appComponent._storageDataService.GetUser()).pipe(
        switchMap((newToken: string) => {
          if (newToken) {
            this.tokenSubject.next(newToken);
            this._appComponent._storageDataService.RefreshToken(newToken);
            return next.handle(this.addToken(this.getNewRequest(req), newToken));
          }

          // If we don't get a new token, we are in trouble so logout.
          return this.logoutUser();
        }),
        catchError(error => {
          // If there is an exception calling 'refreshToken', bad news so logout.
          return this.logoutUser();
        }),
        finalize(() => {
          this.isRefreshingToken = false;
        }),);
    } else {
      return this.tokenSubject.pipe(
        filter(token => token != null),
        take(1),
        switchMap(token => {
          return next.handle(this.addToken(this.getNewRequest(req), token));
        }),);
    }
  }

  /*
      This method is only here so the example works.
      Do not include in your code, just use 'req' instead of 'this.getNewRequest(req)'.
  */
  getNewRequest(req: HttpRequest<any>): HttpRequest<any> {
    if (req.url.indexOf('getData') > 0) {
      return new HttpRequest('GET', 'http://private-4002d-testerrorresponses.apiary-mock.com/getData');
    }

    return new HttpRequest('GET', 'http://private-4002d-testerrorresponses.apiary-mock.com/getLookup');
  }

  logoutUser() {
    // Route to the login page (implementation up to you)
    return observableThrowError("");
  }
}
