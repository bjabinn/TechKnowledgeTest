import { Component, OnInit } from '@angular/core';
import { User } from 'app/Models/User';
import { Role } from 'app/Models/Role';
import { Proyecto } from 'app/Models/Proyecto';
import { ProyectoService } from '../services/ProyectoService';
import { EvaluacionService } from '../services/EvaluacionService';
import { Router } from "@angular/router";
import { AppComponent } from '../app.component';
import { Evaluacion } from 'app/Models/Evaluacion';
import { EvaluacionCreate } from 'app/Models/EvaluacionCreate';
import { NgbModal, ModalDismissReasons } from '@ng-bootstrap/ng-bootstrap';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.scss'],
  providers: [ProyectoService, EvaluacionService]
})
export class HomeComponent implements OnInit {
  public ErrorMessage: string = null;
  public ListaDeProyectos: Array<Proyecto> = [];
  public permisosDeUsuario: Array<Role> = [];
  public AdminOn = false;
  public ProyectoSeleccionado: Proyecto;
  public NombreDeUsuario: string;
  public Deshabilitar = false;
  public MostrarInfo = false;
  public SendingInfo = false;
  public existeRepetida = false;

  constructor(
    private _proyectoService: ProyectoService,
    private _evaluacionService: EvaluacionService,
    private modalService: NgbModal,
    private _router: Router,
    private _appComponent: AppComponent) { }

  ngOnInit() {
    //Empezamos cargando el usuario en el componente mientras verificamos si esta logueado
    //En casao de no estar logeado nos enviara devuelta al login
    if (!this._proyectoService.verificarUsuario()) {
      this._router.navigate(['/login']);
    }

    //Recogemos el nombre del usuario con el que nos logueamos
    this.NombreDeUsuario = this._proyectoService.UsuarioLogeado;

    //Reiniciamos los proyectos seleccionados en el servicio
    this._appComponent._storageDataService.UserProjectSelected = { id: -1, nombre: '', fecha: null };

    //Intentamos recoger los roles de los usuarios
    this._proyectoService.getRolesUsuario().subscribe(
      res => {
        this.permisosDeUsuario = res;
        //Si no hay errores y son recogidos busca si tienes permisos de usuario
        for (let num = 0; num < this.permisosDeUsuario.length; num++) {
          if (this.permisosDeUsuario[num].role == "Administrador") {
            this.AdminOn = true;
          }
        }
        //Llamamos al metodo para asignar proyectos
        this.MostrarInfo = true;
        this.RecogerProyectos();

      },
      error => {
        //Si el servidor tiene algún tipo de problema mostraremos este error
        if (error == 404) {
          this.ErrorMessage = "Error: " + error + " El usuario autenticado no existe.";
        } else if (error == 500) {
          this.ErrorMessage = "Error: " + error + " Ocurrio un error en el servidor, contacte con el servicio técnico.";
        } else if (error == 401) {
          this.ErrorMessage = "Error: " + error + " El usuario es incorrecto o no tiene permisos, intente introducir su usuario nuevamente.";
        } else {
          this.ErrorMessage = "Error: " + error + " Ocurrio un error en el servidor, contacte con el servicio técnico.";
        }
      });

    //Para que no de error en modo development
    setTimeout(() => {
      this._appComponent.anadirUserProyecto(this.NombreDeUsuario, null)
    });

  }

  //Metodo que asigna los proyectos por permisos y usuario
  public RecogerProyectos() {

    //Segun el tipo de rol que tengas te permitira tener todos los proyectos o solo los tuyos
    //El servicio se encargara de enviar una respuesta con el listado de proyecto
    //El usuario necesario ya tendria que haber sido cargado en el logueo
    if (!this.AdminOn) {
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

  //Este metodo guarda el proyecto que a sido seleccionado en el front
  public SeleccionDeProyecto(index: number) {
    this.ProyectoSeleccionado = this.ListaDeProyectos[index];
    this._appComponent._storageDataService.UserProjectSelected = this.ProyectoSeleccionado;
    this.existeRepetida = false;


    //Comprueba que no esta vacia el proyecto elegido
    if (this.ProyectoSeleccionado != null && this.ProyectoSeleccionado != undefined) {
      //Comprueba si ya termino de enviarse la información desde la api
      if (!this.SendingInfo) {
        this.SendingInfo = true;
        this._evaluacionService.getIncompleteEvaluacionFromProject(this.ProyectoSeleccionado.id).subscribe(
          res => {
            //Lo guarda en el storage
            this._appComponent._storageDataService.Evaluacion = res;
            //Si hay un proyecto sin finalizar
            if (res != null) {
              this.existeRepetida = true;
            } 
          },
          error => {
            //Habilitamos la pagina nuevamente
            this.Deshabilitar = false;
            if (error == 404) {
              this.ErrorMessage = "Error: " + error + " No se puede completar la comprobación en la evaluación lo sentimos.";
            } else if (error == 500) {
              this.ErrorMessage = "Error: " + error + " Ocurrio un error en el servidor, contacte con el servicio técnico.";
            } else if (error == 401) {
              this.ErrorMessage = "Error: " + error + " El usuario es incorrecto o no tiene permisos, intente introducir su usuario nuevamente.";
            } else {
              this.ErrorMessage = "Error: " + error + " Ocurrio un error en el servidor, contacte con el servicio técnico.";
            }
          },
          () => {
            this.SendingInfo = false;
          });
      }
    }

  }

  //Este metodo crea una nueva evaluación y la manda para guardarla en la base de datos
  public GuardarEvaluacion() {

    var NuevaEvaluacion: EvaluacionCreate = { 'estado': false, 'proyectoid': this.ProyectoSeleccionado.id };

    this._evaluacionService.addEvaluacion(NuevaEvaluacion).subscribe(
      res => {
        this._appComponent._storageDataService.Evaluacion = res;
        this.SendingInfo = false;
        this._router.navigate(['/menunuevaevaluacion']);
      },
      error => {
        if (error == 404) {
          this.ErrorMessage = "Error: " + error + " La petición de crear una evaluación es incorrecta.";
        } else if (error == 500) {
          this.ErrorMessage = "Error: " + error + " Ocurrio un error en el servidor, contacte con el servicio técnico.";
        } else if (error == 401) {
          this.ErrorMessage = "Error: " + error + " El usuario es incorrecto o no tiene permisos, intente introducir su usuario nuevamente.";
        } else {
          this.ErrorMessage = "Error: " + error + " Ocurrio un error en el servidor, contacte con el servicio técnico.";
        }
        this.SendingInfo = false;
      });
  }

  //Este metodo consulta las evaluaciones anteriores de este proyecto si esta seleccionado y existe
  public EvaluacionesAnteriores() {
    if (this.AdminOn || this.ProyectoSeleccionado != null && this.ProyectoSeleccionado != undefined) {
      this._router.navigate(['/evaluacionprevia']);
    } else {
      this.ErrorMessage = "Seleccione un proyecto para realizar esta acción.";
    }
  }

  //Este metodo guarda la evaluacion y cambia su estado como finalizado
  public FinishEvaluation() {
    //Recoge la evaluación
    var Evaluacion = this._appComponent._storageDataService.Evaluacion;

    //La cambia a temrinada
    Evaluacion.estado = true;

    //La envia a la base de datos ya terminada
    this._evaluacionService.updateEvaluacion(Evaluacion).subscribe(
      res => {
        //Una vez terminado guarda una nueva evaluación
        this.GuardarEvaluacion();
        this.SendingInfo = false;
      },
      error => {
        if (error == 404) {
          this.ErrorMessage = "Error: " + error + " La petición de modificación de evaluación no puede ser realizada.";
        } else if (error == 500) {
          this.ErrorMessage = "Error: " + error + " Ocurrio un error en el servidor, contacte con el servicio técnico.";
        } else if (error == 401) {
          this.ErrorMessage = "Error: " + error + " El usuario es incorrecto o no tiene permisos, intente introducir su usuario nuevamente.";
        } else {
          this.ErrorMessage = "Error: " + error + " Ocurrio un error en el servidor, contacte con el servicio técnico.";
        }
        this.SendingInfo = false;
      });
  }

  //Muestra un modal con lo que se debe hacer en cada caso
  showModal(content) {
    //Comprueba que no esta vacia el proyecto elegido
    if (this.ProyectoSeleccionado != null && this.ProyectoSeleccionado != undefined) {
      //Comprueba si ya termino de enviarse la información desde la api
      if (!this.SendingInfo) {
        if (this.existeRepetida) {
                this.modalService.open(content).result.then(
                  (closeResult) => {
                  }, (dismissReason) => {
                    //Si selecciona continuar cargara la valuación que no termino
                    if (dismissReason == 'Continuar') {
                      this._router.navigate(['/menunuevaevaluacion']);
                    } else if (dismissReason == 'Nueva') {
                      //Si selecciona nuevo que es la otra opción cogera la evaluación anterior lo finalizara
                      //cargara una nueva y lo mostrara
                      this.FinishEvaluation();
                    } 
                  })
              } else {
                //Si no encuentra ninguna repetida directamente te crea una nueva evaluación
                this.GuardarEvaluacion();
              }
            }
    } else {
      this.ErrorMessage = "Seleccione un proyecto para realizar esta acción.";
    }
  }

  //Para seguir con la evaluacion seleccionada
  public continuarEvaluacion() {
    this._router.navigate(['/menunuevaevaluacion']);
  }
}
