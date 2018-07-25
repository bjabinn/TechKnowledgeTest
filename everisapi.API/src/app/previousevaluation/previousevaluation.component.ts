import { Component, OnInit, ViewChild } from '@angular/core';
import { EvaluacionInfo } from 'app/Models/EvaluacionInfo';
import { EvaluacionFilterInfo } from 'app/Models/EvaluacionFilterInfo';
import { Proyecto } from 'app/Models/Proyecto';
import { AppComponent } from 'app/app.component';
import { Router } from '@angular/router';
import { EvaluacionService } from '../services/EvaluacionService';
import { Evaluacion } from 'app/Models/Evaluacion';
import { Observable } from 'rxjs/Rx';
import { interval ,  Subscription } from 'rxjs';
import { ProyectoService } from 'app/services/ProyectoService';
import { Role } from 'app/Models/Role';
import { NgbModal, ModalDismissReasons } from '@ng-bootstrap/ng-bootstrap';
import { NgbDate } from '@ng-bootstrap/ng-bootstrap/datepicker/ngb-date';
import { ChartsModule, BaseChartDirective } from 'ng2-charts/ng2-charts';
import { DatePipe } from '@angular/common';
import { setTimeout } from 'timers';

@Component({
  selector: 'app-previousevaluation',
  templateUrl: './previousevaluation.component.html',
  styleUrls: ['./previousevaluation.component.scss'],
  providers: [EvaluacionService, ProyectoService]
})
export class PreviousevaluationComponent implements OnInit {
  public clicked: boolean = true;
  public EvaluacionFiltrar: EvaluacionFilterInfo = { 'nombre': '', 'estado': '', 'fecha': '', 'userNombre': '', 'puntuacion': '' };
  public Typing: boolean = false;
  public permisosDeUsuario: Array<Role> = [];
  public ListaDeEvaluacionesPaginada: Array<EvaluacionInfo>;
  public nEvaluaciones: number = 0;
  public UserName: string = "";
  public Project: Proyecto = { 'id': null, 'nombre': '', 'fecha': null };
  public Mostrar = false;
  public PageNow = 1;
  public NumMax = 0;
  public ErrorMessage: string = null;
  public NumEspera = 750;
  public MostrarInfo = false;
  public Timeout: Subscription;
  public textoModal: string;
  public anadeNota = null;
  public fechaPicker: NgbDate;


  public Admin: boolean = false;
  public ListaDeProyectos: Array<Proyecto> = [];
  public ProyectoSeleccionado: boolean = false;


  //Datos de la barras
  public barChartType: string = 'line';
  public barChartLegend: boolean = false;
  public AgileComplianceTotal: number = 100;
  public ListaSeccionesAgileCompliance: number[] = [];
  public ListaPuntuacion: { label: string, backgroundColor: string, borderColor: string, data: Array<any> }[] = [];
  public ListaNombres: string[] = [];

  //Para actualizar la grafica
  @ViewChild(BaseChartDirective) public chart: BaseChartDirective;


  constructor(
    private _appComponent: AppComponent,
    private _router: Router,
    private _evaluacionService: EvaluacionService,
    private _proyectoService: ProyectoService,
    private modalService: NgbModal
  ) { }

  ngOnInit() {
    //Recogemos los proyectos y realizamos comprobaciones
    var Role;
    this.Project = this._appComponent._storageDataService.UserProjectSelected;
    if (!this._proyectoService.verificarUsuario()) {
      this._router.navigate(['/login']);
    }
    this.UserName = this._proyectoService.UsuarioLogeado;

    //Recogemos el rol del usuario
    this._proyectoService.getRolesUsuario().subscribe(
      res => {

        this.permisosDeUsuario = res;
        //Si no hay errores y son recogidos busca si tienes permisos de usuario
        for (let num = 0; num < this.permisosDeUsuario.length; num++) {
          if (this.permisosDeUsuario[num].role == "Administrador") {
            if (this.Project == null || this.Project == undefined || this.Project.id == -1) {
              this.Project = { id: 0, nombre: '', fecha: null };
              this.Admin = true;
            }
          }
        }

        //Comprueba que tenga  un proyecto seleccionado y si no es asi lo devuelve a home
        if (this.Project == null || this.Project == undefined) {
          this._router.navigate(['/home']);
        } else if (this.Project.id == -1 && !this.Admin) {
          this._router.navigate(['/home']);
        } else {
          this.MostrarInfo = true;
        }

        this.GetPaginacion();

      },
      error => {
        //Si el servidor tiene algún tipo de problema mostraremos este error
        if (error == 404) {
          this.ErrorMessage = "Error: " + error + "No pudimos recoger los datos del usuario.";
        } else if (error == 500) {
          this.ErrorMessage = "Error: " + error + " Ocurrio un error en el servidor, contacte con el servicio técnico.";
        } else if (error == 401) {
          this.ErrorMessage = "Error: " + error + " El usuario es incorrecto o no tiene permisos, intente introducir su usuario nuevamente.";
        } else {
          this.ErrorMessage = "Error: " + error + " Ocurrio un error en el servidor, contacte con el servicio técnico.";
        }
      },
      () => {
        this.Restablecer();
      });

    if (this.Project.fecha != null) {
      //Para que no de error en modo development
      setTimeout(() => {
        this._appComponent.anadirUserProyecto(this.UserName, this.Project.nombre);
      },0);
    } else {
      //Para que no de error en modo development
      setTimeout(() => {
        this._appComponent.anadirUserProyecto(this.UserName, "Todos");
      },0);

    }
  }

  //Este metodo devuelve el número de paginas máximo que hay
  public CalcularPaginas() {
    var NumeroDePaginas = Math.floor((this.nEvaluaciones / 10) * 100) / 100;
    if (NumeroDePaginas % 1 != 0) {
      this.NumMax = Math.floor(NumeroDePaginas) + 1;
    } else {
      this.NumMax = NumeroDePaginas;
    }
  }

  //Restablece los datos de la busqueda
  public Restablecer() {
    if (this.clicked) {
      this.EvaluacionFiltrar.fecha = "";
      this.EvaluacionFiltrar.nombre = "";
      this.EvaluacionFiltrar.userNombre = "";
      this.EvaluacionFiltrar.estado = "";
      this.EvaluacionFiltrar.puntuacion = "";
      this.ProyectoSeleccionado = false;

      this.GetPaginacion();
      this.clicked = false;
    }
    else {
      this.clicked = true;

      if (this.ListaDeProyectos.length == 0) {

        //Segun el tipo de rol que tengas te permitira tener todos los proyectos o solo los tuyos
        //El servicio se encargara de enviar una respuesta con el listado de proyecto
        //El usuario necesario ya tendria que haber sido cargado en el logueo
        if (!this.Admin) {
          //Aqui se entra solo si no tienes permisos de administrador dandote los proyectos que te tocan
          this._proyectoService.getProyectosDeUsuario().subscribe(
            res => {
              this.ListaDeProyectos = res;
            },
            error => {
              //Si el servidor tiene algún tipo de problema mostraremos este error
              if (error == 404) {
                this.ErrorMessage = "Error: " + error + " El usuario o proyecto autenticado no existe.";
              } else if (error == 500) {
                this.ErrorMessage = "Error: " + error + " Ocurrio un error en el servidor, contacte con el servicio técnico.";
              } else if (error == 401) {
                this.ErrorMessage = "Error: " + error + " El usuario es incorrecto o no tiene permisos, intente introducir su usuario nuevamente.";
              } else {
                this.ErrorMessage = "Error: " + error + " Ocurrio un error en el servidor, contacte con el servicio técnico.";
              }
            });
        } else {
          //Aqui entra si eres administrador dandote todos los proyectos
          this._proyectoService.getAllProyectos().subscribe(
            res => {
              this.ListaDeProyectos = res;

            },
            error => {
              //Si el servidor tiene algún tipo de problema mostraremos este error
              if (error == 404) {
                this.ErrorMessage = "Error: " + error + " El usuario o proyecto autenticado no existe.";
              } else if (error == 500) {
                this.ErrorMessage = "Error: " + error + " Ocurrio un error en el servidor, contacte con el servicio técnico.";
              } else if (error == 401) {
                this.ErrorMessage = "Error: " + error + " El usuario es incorrecto o no tiene permisos, intente introducir su usuario nuevamente.";
              } else {
                this.ErrorMessage = "Error: " + error + " Ocurrio un error en el servidor, contacte con el servicio técnico.";
              }
            });
        }

      }

    }
  }

  //Para el desplegable de elegir proyecto
  public SeleccionDeProyecto(index: number) {

    //Ningun proyecto elegido
    if (isNaN(index)) {
      this.ProyectoSeleccionado = false;
      this.EvaluacionFiltrar.nombre = "";
    }
    //Elegido algun proyecto
    else {
      this.ProyectoSeleccionado = true;
      this.EvaluacionFiltrar.nombre = this.ListaDeProyectos[index].nombre;
    }

    this.TryHttpRequest(false);
  }

  //Este metodo devuelve la transforma la lista de evaluaciones dada en una lista paginada
  public paginacionLista(pageNumber: number) {
    var Skip = pageNumber * 10;
    var ListaPaginada = new Array<EvaluacionInfo>();
    var contador = Skip;
    while (ListaPaginada.length != 10 && contador < this.ListaDeEvaluacionesPaginada.length) {
      ListaPaginada.push(this.ListaDeEvaluacionesPaginada[contador]);
      contador++;
    }
    this.ListaDeEvaluacionesPaginada = ListaPaginada;
  }

 
  //Guarda los datos en el storage y cambia de ruta hacia la generación de grafica
  public SaveDataToPDF(evaluacion: EvaluacionInfo) {
    this._appComponent._storageDataService.EvaluacionToPDF = evaluacion;
    this._router.navigate(['/pdfgenerator']);
  }

  //Filtra por evaluaciones completas completas o ninguna
  public ChangeFiltro(estado: string) {
    this.PageNow = 1;
    this.EvaluacionFiltrar.estado = estado;
    this.GetPaginacion();
  }

  //Al presionar el boton va avanzado y retrocediendo
  public NextPreviousButton(Option: boolean) {
    if (Option && this.PageNow < this.NumMax) {
      this.paginacionLista(this.PageNow++);
      this.GetPaginacion();
    } else if (!Option && this.PageNow > 1) {
      this.PageNow--;
      var CualToca = this.PageNow - 1;
      this.paginacionLista(CualToca);
      this.GetPaginacion();
    }
  }

  //Este metodo es llamado cuando cambias un valor de filtrado y en 500 milisegundos te manda a la primera pagina y recarga el componente con
  //los nuevos elementos
  public TryHttpRequest(timeout: boolean) {
    if (this.Timeout != null && !this.Timeout != undefined) {
      this.Timeout.unsubscribe();
    }

    //Cuando se escribe nombre
    if (timeout) {
      this.Timeout = interval(500)
        .subscribe(i => {
          this.PageNow = 1, this.GetPaginacion(),
            this.Timeout.unsubscribe()
        });
    }

    //Cuando se cambia de proyecto
    else {

      this.PageNow = 1;
      this.GetPaginacion();
    }

  }

  //Recarga los elementos en la pagina en la que se encuentra 
  public GetPaginacion() {
    this.Mostrar = false;
    this._evaluacionService.getEvaluacionInfoFiltered(this.PageNow - 1, this.Project.id, this.EvaluacionFiltrar)
      .subscribe(
        res => {
          this.nEvaluaciones = res.numEvals;
          this.ListaDeEvaluacionesPaginada = res.evaluacionesResult;
          this.CalcularPaginas();
          this.shareDataToChart();
          this.Mostrar = true;
        },
        error => {
          if (error == 404) {
            this.ErrorMessage = "Error: " + error + " No pudimos recoger la información de las evaluaciones, lo sentimos.";
          } else if (error == 500) {
            this.ErrorMessage = "Error: " + error + " Ocurrio un error en el servidor, contacte con el servicio técnico.";
          } else if (error == 401) {
            this.ErrorMessage = "Error: " + error + " El usuario es incorrecto o no tiene permisos, intente introducir su usuario nuevamente.";
          } else {
            this.ErrorMessage = "Error: " + error + " Ocurrio un error en el servidor, contacte con el servicio técnico.";
          }
        });

  }

  //Modal de notas evaluacion y objetivos
  //tipo=0 -> Evaluacion
  //tipo=1 -> Objetivos
  public AbrirModal(content, numeroEv, tipo) {

    this.anadeNota = null;

    if (tipo == 0) {
      if (this.ListaDeEvaluacionesPaginada[numeroEv].notasEv != null) {
        this.textoModal = this.ListaDeEvaluacionesPaginada[numeroEv].notasEv;
      } else {
        this.textoModal = "";
      }
    } else if (tipo == 1) {
      if (this.ListaDeEvaluacionesPaginada[numeroEv].notasOb != null) {
        this.textoModal = this.ListaDeEvaluacionesPaginada[numeroEv].notasOb;
      } else {
        this.textoModal = "";
      }
    }

    this.modalService.open(content).result.then(
      (closeResult) => {
        //Si cierra, no se guarda

      }, (dismissReason) => {
        if (dismissReason == 'Guardar') {

          this.Mostrar = false;

          if (tipo == 0) {
            if (this.textoModal != "") {
              this.ListaDeEvaluacionesPaginada[numeroEv].notasEv = this.textoModal;
            } else {
              this.ListaDeEvaluacionesPaginada[numeroEv].notasEv = null;
            }
          } else {
            if (this.textoModal != "") {
              this.ListaDeEvaluacionesPaginada[numeroEv].notasOb = this.textoModal;
            } else {
              this.ListaDeEvaluacionesPaginada[numeroEv].notasOb = null;
            }
          }

          var evalu = new Evaluacion(
            this.ListaDeEvaluacionesPaginada[numeroEv].id,
            this.ListaDeEvaluacionesPaginada[numeroEv].fecha,
            this.ListaDeEvaluacionesPaginada[numeroEv].estado,
            this.ListaDeEvaluacionesPaginada[numeroEv].notasOb,
            this.ListaDeEvaluacionesPaginada[numeroEv].notasEv,
            this.ListaDeEvaluacionesPaginada[numeroEv].puntuacion,
          );

          this._evaluacionService.updateEvaluacion(evalu)
            .subscribe(
              res => {
                this.anadeNota = "Se guardó la nota correctamente";
              },
              error => {
                if (error == 404) {
                  this.ErrorMessage = "Error: " + error + " No pudimos recoger la información de las evaluaciones, lo sentimos.";
                } else if (error == 500) {
                  this.ErrorMessage = "Error: " + error + " Ocurrio un error en el servidor, contacte con el servicio técnico.";
                } else if (error == 401) {
                  this.ErrorMessage = "Error: " + error + " El usuario es incorrecto o no tiene permisos, intente introducir su usuario nuevamente.";
                } else {
                  this.ErrorMessage = "Error: " + error + " Ocurrio un error en el servidor, contacte con el servicio técnico.";
                }
              },
              () => {
                this.Mostrar = true;
              });


        }
        //Else, Click fuera, no se guarda
      })
  }

  public VolverInicio() {
    this._router.navigate(['/home']);
  }

  //Para cambiar la fecha y que aparezca en el formato correcto en la caja tenemos que liar to esto
  public changeDate() {
    if (this.fechaPicker.day < 10) {
      this.EvaluacionFiltrar.fecha = "0" + this.fechaPicker.day + "/";
    } else {
      this.EvaluacionFiltrar.fecha = this.fechaPicker.day + "/";
    }

    if (this.fechaPicker.month < 10) {
      this.EvaluacionFiltrar.fecha = this.EvaluacionFiltrar.fecha + "0" + this.fechaPicker.month + "/" + this.fechaPicker.year;
    } else {
      this.EvaluacionFiltrar.fecha = this.EvaluacionFiltrar.fecha + this.fechaPicker.month + "/" + this.fechaPicker.year;
    }

    this.PageNow = 1;
    this.GetPaginacion();

  }

  //Limpiamos la caja de fecha
  public limpiar() {
    this.EvaluacionFiltrar.fecha = "";

    this.PageNow = 1;
    this.GetPaginacion();
  }

  //Da los datos a las diferentes listas que usaremos para las graficas
  public shareDataToChart() {

    this.ListaPuntuacion = [];
    this.ListaNombres = [];
    var listaPunt = [];


    //Cogemos los datoa a añadir
    for (var i = this.ListaDeEvaluacionesPaginada.length - 1; i >= 0; i--) {
      var pipe = new DatePipe('en-US');
      listaPunt.push(this.ListaDeEvaluacionesPaginada[i].puntuacion);
      this.ListaNombres.push(pipe.transform(this.ListaDeEvaluacionesPaginada[i].fecha, 'dd/MM/yyyy, HH:mm'));
    }

    this.ListaPuntuacion.push({
      data: listaPunt, label: 'Puntuación', backgroundColor: 'rgba(92, 183, 92, 0.5)',
      borderColor: 'rgba(92, 183, 92, 0.5)', });


    //Para actualizar la grafica una vez esté visible
    setTimeout(() => {

      if (this.chart && this.chart.chart && this.chart.chart.config) {
        this.chart.chart.config.data.labels = this.ListaNombres;
        this.chart.chart.config.data.datasets = this.ListaPuntuacion;
        this.chart.chart.update();
      }
    }, 300);

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
    }];

  //Estos son los datos introducidos en la grafica para que represente sus formas
  public barChartData: any[] = [
    { data: this.ListaPuntuacion, label: 'Puntuación' }
  ];


}
