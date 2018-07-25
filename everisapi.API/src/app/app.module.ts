import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import {FormsModule} from '@angular/forms';
import { HttpModule } from '@angular/http';

import { AppComponent } from './app.component';
import { LoginComponent } from './login/login.component';
import { LoadingComponent } from './loading/loading.component';
import { AppRoutingModule, routing } from './app-routing.module';
import { HomeComponent } from './home/home.component';
import { NewevaluationComponent } from './newevaluation/newevaluation.component';
import { PreviousevaluationComponent } from './previousevaluation/previousevaluation.component';
import { MenunewevaluationComponent } from './menunewevaluation/menunewevaluation.component';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { PdfgeneratorComponent } from './pdfgenerator/pdfgenerator.component';
import { ChartsModule } from 'ng2-charts/ng2-charts';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { RequestInterceptorService } from './services/RequestInterceptor.service';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    HomeComponent,
    NewevaluationComponent,
    PreviousevaluationComponent,
    MenunewevaluationComponent,
    PdfgeneratorComponent,
    LoadingComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    routing,
    HttpModule,
    ChartsModule,
    NgbModule.forRoot()
  ],
  providers: [{
    provide: HTTP_INTERCEPTORS,
    useClass: RequestInterceptorService,
    multi: true
  }],
  bootstrap: [AppComponent]
})
export class AppModule { }

