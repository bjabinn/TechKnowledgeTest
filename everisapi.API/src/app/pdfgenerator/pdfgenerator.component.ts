import { Component, OnInit, ElementRef } from '@angular/core';
import { ChartsModule } from 'ng2-charts/ng2-charts';
import { EvaluacionInfo } from 'app/Models/EvaluacionInfo';
import { Proyecto } from 'app/Models/Proyecto';
import { SectionInfo } from 'app/Models/SectionInfo';
import { RespuestaConNotas } from 'app/Models/RespuestaConNotas';
import { AsignacionConNotas } from 'app/Models/AsignacionConNotas';
import { AppComponent } from 'app/app.component';
import { Router } from '@angular/router';
import { SectionService } from 'app/services/SectionService';
import { DatePipe } from '@angular/common';
import { ProyectoService } from 'app/services/ProyectoService';
import { Http } from '@angular/http';
import { NgbModal, ModalDismissReasons } from '@ng-bootstrap/ng-bootstrap';
import { map } from 'rxjs/operators';
import { RespuestasService } from '../services/RespuestasService';
import { Respuesta } from '../Models/Respuesta';

import * as jsPDF from 'jspdf';
import * as html2canvas from 'html2canvas';
import * as imgTransform from 'img-transform';

import { forEach } from '@angular/router/src/utils/collection';
import { concat } from 'rxjs-compat/operator/concat';

@Component({
  selector: 'app-pdfgenerator',
  templateUrl: './pdfgenerator.component.html',
  styleUrls: ['./pdfgenerator.component.scss'],
  providers: [SectionService, ProyectoService, DatePipe, RespuestasService]
})
export class PdfgeneratorComponent implements OnInit {

  public ListaDeDatos: Array<SectionInfo> = [];
  public UserName: string = "";
  public Project: Proyecto = null;
  public Evaluacion: EvaluacionInfo;
  public Mostrar = false;
  public ErrorMessage = null;
  public AdminOn: boolean = false;

  //Datos de la barras
  public barChartType: string = 'bar';
  public barChartLegend: boolean = true;
  public AgileComplianceTotal: number = 100;
  public ListaSeccionesAgileCompliance: number[] = [];
  public ListaPuntuacion: number[] = [];
  public ListaNombres: string[] = [];

  //Para las notas
  public mostrarCheckboxes: boolean = true;
  public mostrarNotasEv: boolean = false;
  public mostrarNotasOb: boolean = false;
  public mostrarNotasSec: boolean = false;
  public mostrarNotasAsig: boolean = false;
  public mostrarNotasPreg: boolean = false;
  public ListaDeRespuestas: Array<RespuestaConNotas> = [];
  public ListaDeAsignaciones: Array<AsignacionConNotas> = [];
  public cargandoNotas: boolean = false;
  public textoModal: string;
  public anadeNota: string = null;


  //PDF
  public cargandoPDF: boolean = false;
  public total: number = -1;
  public totalCompletado: number = -2;
  public resultados = [];
  public continuar: boolean = true;
  public primeraVez: boolean = true;
  public doc = null;
  public iteracionResultados: number = 0;

  constructor(
    private _proyectoService: ProyectoService,
    private _appComponent: AppComponent,
    private _respuestasService: RespuestasService,
    private _router: Router,
    private _sectionService: SectionService,
    private http: Http,
    private datePipe: DatePipe,
    private modalService: NgbModal) {

    //Recupera los datos y los comprueba
    this.Project = this._appComponent._storageDataService.UserProjectSelected;
    this.Evaluacion = this._appComponent._storageDataService.EvaluacionToPDF;
    this._proyectoService.verificarUsuario();
    this.UserName = this._proyectoService.UsuarioLogeado;

    if (this.Evaluacion == null || this.Evaluacion == undefined || this.Project == null || this.Project == undefined) {

      this._router.navigate(['/home']);

    } else if (this.Evaluacion.id == null) {

      this._router.navigate(['/home']);

    }

    var ArrayRoles = [];
    this._proyectoService.getRolesUsuario().subscribe(
      res => {
        this.AdminOn = false;
        ArrayRoles = res;
        //Si no hay errores y son recogidos busca si tienes permisos de usuario
        for (let num = 0; num < ArrayRoles.length; num++) {
          if (ArrayRoles[num].role == "Administrador") {
            this.AdminOn = true;
          }
        }
        if (this.Project.id == null && !this.AdminOn) {
          this._router.navigate(['/home']);
        }
      },
      error => {
        this._router.navigate(['/home']);
      });

    //Notas para aprobados
    this.http.get('assets/notas_aprobados.json').pipe(
      map(res => res.json()))
      .toPromise()
      .then((notas) => {
        this.ListaSeccionesAgileCompliance.push(parseInt(notas.ceremonias));
        this.ListaSeccionesAgileCompliance.push(parseInt(notas.roles));
        this.ListaSeccionesAgileCompliance.push(parseInt(notas.artefactos));

        this.AgileComplianceTotal = notas.total;
      });

  }

  ngOnInit() {

    //Recoge los datos de las secciones
    if (this.Evaluacion != null && this.Evaluacion != undefined) {
      this._sectionService.getSectionInfo(this.Evaluacion.id).subscribe(
        res => {
          this.ListaDeDatos = res;
          this.shareDataToChart();
        },
        error => {
          if (error == 404) {
            this.ErrorMessage = "Error: " + error + "No pudimos recoger los datos de la sección lo sentimos.";
          } else if (error == 500) {
            this.ErrorMessage = "Error: " + error + " Ocurrio un error en el servidor, contacte con el servicio técnico.";
          } else if (error == 401) {
            this.ErrorMessage = "Error: " + error + " El usuario es incorrecto o no tiene permisos, intente introducir su usuario nuevamente.";
          } else {
            this.ErrorMessage = "Error: " + error + " Ocurrio un error en el servidor, contacte con el servicio técnico.";
          }
        }
      );
    } else {
      this._router.navigate(['/home']);
    }




    //Para que no de error en modo development
    setTimeout(() => {
    this._appComponent.anadirUserProyecto(null, this.Evaluacion.nombre);
    });

  }

  //Da los datos a las diferentes listas que usaremos para las graficas
  public shareDataToChart() {
    for (var i = 0; i < this.ListaDeDatos.length; i++) {
      this.ListaNombres.push(this.ListaDeDatos[i].nombre);
      this.ListaPuntuacion.push(this.ListaDeDatos[i].respuestasCorrectas);
    }
    this.Mostrar = true;
  }

  //Opciones para la grafica
  public barChartOptions: any = {
    scaleShowVerticalLines: true,
    scales: {
      yAxes: [{
        ticks: {
          steps: 10,
          stepValue: 10,
          max: 100,
          min: 0,
        }
      }]
    }
  };

  //Colores para la grafica
  public chartColors: Array<any> = [
    { // first color
      backgroundColor: 'rgba(92, 183, 92, 0.5)',
      borderColor: 'rgba(92, 183, 92, 0.5)',
      pointBackgroundColor: 'rgba(92, 183, 92, 0.5)',
      pointBorderColor: '#fff',
      pointHoverBackgroundColor: '#fff',
      pointHoverBorderColor: 'rgba(92, 183, 92, 0.5)'
    },
    { // second color
      backgroundColor: 'rgba(52, 122, 183, 0.5)',
      borderColor: 'rgba(52, 122, 183, 0.5)',
      pointBackgroundColor: 'rgba(52, 122, 183, 0.5)',
      pointBorderColor: '#fff',
      pointHoverBackgroundColor: '#fff',
      pointHoverBorderColor: 'rgba(52, 122, 183, 0.5)'
    }];

  //Estos son los datos introducidos en la grafica para que represente sus formas
  public barChartData: any[] = [
    { data: this.ListaSeccionesAgileCompliance, label: 'Agile Compliance' },
    { data: this.ListaPuntuacion, label: 'Puntuación' }
  ];

  //Genera un pdf a partir de una captura de pantalla
  //Esta funcion se encarga de coger todo lo que se quiera de pantalla y guardarlo en un vector
  //Este proceso no es instantaneo, así que se va llamando a la funcion createPDF cada vez hasta que están todos completados
  public downloadPDF() {
    this.anadeNota = null;

    this.cargandoPDF = true;

    var cajas = ["tablaPuntuaciones", "Grafica", "notasEvaluacion", "notasObjetivos", "notasSecciones", "notasAsignaciones", "notasPreguntas"];

    this.resultados = [];

    var referencia = this;

    this.total = 0;

    this.totalCompletado = 0;

    cajas.forEach((caja, index) => {

      var elemento = document.getElementById(caja);
      this.resultados.push(null);

      if (elemento != null) {
        setTimeout(function () {

          html2canvas(elemento).then(canvas => {

            referencia.resultados[index] = canvas;

            referencia.totalCompletado++;

            referencia.createPDF();

          });

        }, 500);


        this.total++;
      }

    });


  }

  //Funcion para coger los elementos que se han guardado en un vector y crear con ellos un pdf
  public createPDF() {

    //Cuando se han cargado todos los elementos en el vector
    if (this.total === this.totalCompletado) {

      //Cuando se quiere crear un nuevo pdf
      if (this.primeraVez) {

        this.doc = new jsPDF('p', 'mm', 'A4');

        this.primeraVez = false;
      }

      //Suma de la altura
      var alturaTotal = 0;

      //Dimensiones de la pagina
      var tamanioPag = 297;

      var tamanioRestante = tamanioPag;

      //Usa dimensiones en mm, asi que tenemos que calcular cuanto mide
      var unMmEnPx = parseInt(window.getComputedStyle(document.getElementById("my_mm")).height.toString().split('px')[0]);


      //Por cada imagen
      //Esta funcion vuelve a ser llamada al dividir imágenes, por eso tendremos que llevar la cuenta de por donde nos quedamos
      for (var j = this.iteracionResultados; j < this.resultados.length && this.continuar; j++) {

        var imagen = this.resultados[j];

        this.iteracionResultados++;

        if (imagen != null) {

          var tamanioImagen = imagen.height / unMmEnPx;

          //Si la imagen es mas grande que la pagina se llama a otra funcion que la dividira
          if (tamanioImagen >= tamanioPag) {
            
            this.continuar = false;

            this.imagenGrande(imagen, 0, tamanioPag, tamanioImagen);


          } else {

            //Añadimos la imagen a la pagina existente o creamos una nueva página
            tamanioRestante -= tamanioImagen + 20;

            if (tamanioRestante < 0) {
              this.doc.addPage();
              alturaTotal = 0;
              tamanioRestante = tamanioPag;
            }

            this.doc.addImage(imagen.toDataURL("image/png", 1.0), 'PNG', 15, alturaTotal + 20, 0, 0, '', 'FAST');

            alturaTotal += tamanioImagen;
          }


        }

      }

      //Si se ha terminado de crear el pdf
      if (this.continuar) {
        var date = this.datePipe.transform(this.Evaluacion.fecha, 'dd-MM-yyyy');
        var nombre = this.Evaluacion.nombre;
        this.doc.save(nombre + '.' + date + '.' + 'AgileMeter.pdf');


        this.totalCompletado = -1;
        this.total = -2;

        this.cargandoPDF = false;
        this.continuar = true;
        this.primeraVez = true;
        this.iteracionResultados = 0;
      }

    }
  }

  //Cuando hay una imagen que ocupa mas de una pagina se llama a esta funcion, que divide el pdf y la imagen en varias páginas
  //Es una funcion recursiva que se llama a si misma para ir cortando la imagen
  //Esto se hace porque es la unica forma de conservar el orden al ser un proceso asíncrono
  public imagenGrande(imagen, iteracion, tamanioPag, tamanioImg) {

    //Las dimensiones para añadir imagenes son muy raras
    //El tamaño de la imagen lo da mal
    //Todo lo que se ve ha sido sacado por prueba y error (hay que mejorarlo)
    var sumar = 1;

    if (iteracion > 0) {
      sumar = -40;
    }


    var relacion = tamanioImg / tamanioPag;
    var totalIteraciones = Math.floor(tamanioImg / tamanioPag);

    //Numero de iteraciones
    if (totalIteraciones == 1 && relacion >= 1.1) {
      totalIteraciones++;
    }

    //Cuanto sumar a la imagen por arriba para continuar
    var top = 950;

    if (iteracion == totalIteraciones - 1 && iteracion != 0) {

      top = Math.ceil((((tamanioImg % tamanioPag) * 950 / tamanioPag) + 30 * totalIteraciones) % 950);

    }
    
    //Transformamos la i
    imgTransform(imagen.toDataURL("image/png", 1.0)).crop(2000, top, 0, (950 * iteracion) + sumar).done(dataUrl => {

      this.doc.addPage();

      this.doc.addImage(dataUrl, 'PNG', 15, 20, 0, 0, '', 'FAST');

      //Si se ha terminado, se llama a la funcion anterior para continuar con la creacion del pdf
      if (iteracion == totalIteraciones - 1) {

        this.continuar = true;
        this.createPDF();

      } else {

        this.imagenGrande(imagen, iteracion + 1, tamanioPag, tamanioImg);
      }

    });

  }



  //Para no mostrar la pantalla de cargando
  public apagarCargar() {
    this.cargandoPDF = false;
  }

  //Para mostrar o no las notas de evaluacion
  public cambiarMostrarNotasEv() {
    if (this.Evaluacion.notasEv != null && this.Evaluacion.notasEv != "") {
      this.mostrarNotasEv = !this.mostrarNotasEv;
    }
  }

  //Para mostrar o no las notas de objetivos
  public cambiarMostrarNotasOb() {
    if (this.Evaluacion.notasOb != null && this.Evaluacion.notasOb != "") {
      this.mostrarNotasOb = !this.mostrarNotasOb;
    }
  }

  //Para mostrar o no las notas de seccion
  public cambiarMostrarNotasSec() {
    if (this.Evaluacion.flagNotasSec) {
      this.mostrarNotasSec = !this.mostrarNotasSec;
    }
  }

  //Para mostrar o no las notas de preguntas
  public cambiarMostrarNotasPreg() {

      //No se ha hecho la peticion al servidor aun
      if (!this.mostrarNotasPreg && this.ListaDeRespuestas.length == 0) {
        this.cargandoNotas = true;

        this._sectionService.getRespuestasConNotas(this.Evaluacion.id).subscribe(
          res => {
            this.ListaDeRespuestas = res;
            this.cargandoNotas = false;
            this.mostrarNotasPreg = true;
          },
          error => {
            if (error == 404) {
              this.ErrorMessage = "Error: " + error + "No pudimos recoger los datos de las preguntas.";
            } else if (error == 500) {
              this.ErrorMessage = "Error: " + error + " Ocurrio un error en el servidor, contacte con el servicio técnico.";
            } else if (error == 401) {
              this.ErrorMessage = "Error: " + error + " El usuario es incorrecto o no tiene permisos, intente introducir su usuario nuevamente.";
            } else {
              this.ErrorMessage = "Error: " + error + " Ocurrio un error en el servidor, contacte con el servicio técnico.";
            }
          }
        );
      }
      else {
        this.mostrarNotasPreg = !this.mostrarNotasPreg;
      }
  }


  //Para mostrar o no las notas de asignacion
  public cambiarMostrarNotasAsig() {
    if (this.Evaluacion.flagNotasAsig) {

      //No se ha hecho la peticion al servidor aun
      if (!this.mostrarNotasAsig && this.ListaDeAsignaciones.length == 0) {
        this.cargandoNotas = true;

        this._sectionService.getAsignConNotas(this.Evaluacion.id).subscribe(
          res => {
            this.ListaDeAsignaciones = res;
            this.cargandoNotas = false;
            this.mostrarNotasAsig = true;
          },
          error => {
            if (error == 404) {
              this.ErrorMessage = "Error: " + error + "No pudimos recoger los datos de las preguntas.";
            } else if (error == 500) {
              this.ErrorMessage = "Error: " + error + " Ocurrio un error en el servidor, contacte con el servicio técnico.";
            } else if (error == 401) {
              this.ErrorMessage = "Error: " + error + " El usuario es incorrecto o no tiene permisos, intente introducir su usuario nuevamente.";
            } else {
              this.ErrorMessage = "Error: " + error + " Ocurrio un error en el servidor, contacte con el servicio técnico.";
            }
          }
        );

      }
      else {
        this.mostrarNotasAsig = !this.mostrarNotasAsig;
      }
    }
  }

  //Para volver a la pantalla de evaluaciones
  public Volver(lugar) {
    this._router.navigate([lugar]);
  }

  //Para abrir el modal de notas de admin
  public AbrirModal(content, i) {

    this.anadeNota = null;

    if (this.ListaDeRespuestas[i].notasAdmin != null) {
      this.textoModal = this.ListaDeRespuestas[i].notasAdmin;
    } else {
      this.textoModal = "";
    }

    this.modalService.open(content).result.then(
      (closeResult) => {
        //Si cierra, no se guarda

      }, (dismissReason) => {
        if (dismissReason == 'Guardar') {

          if (this.textoModal != "") {
            this.ListaDeRespuestas[i].notasAdmin = this.textoModal;
          } else {
            this.ListaDeRespuestas[i].notasAdmin = null;
          }

          var resp = new Respuesta(this.ListaDeRespuestas[i].id, this.ListaDeRespuestas[i].estado,
            1, 1, this.ListaDeRespuestas[i].notas, this.ListaDeRespuestas[i].notasAdmin);


          this._respuestasService.AlterRespuesta(resp).subscribe(
            res => {

              this.anadeNota = "Nota añadida correctamente";
            },
            error => {

              if (error == 404) {
                this.ErrorMessage = "Error: " + error + "No pudimos realizar la actualización de la respuesta, lo sentimos.";
              } else if (error == 500) {
                this.ErrorMessage = "Error: " + error + " Ocurrio un error en el servidor, contacte con el servicio técnico.";
              } else if (error == 401) {
                this.ErrorMessage = "Error: " + error + " El usuario es incorrecto o no tiene permisos, intente introducir su usuario nuevamente.";
              } else {
                this.ErrorMessage = "Error: " + error + " Ocurrio un error en el servidor, contacte con el servicio técnico.";
              }
            });

        }
        //Else, Click fuera, no se guarda
      })
  }

}
