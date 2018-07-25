export class EvaluacionInfo {
  constructor(
    public id: number,
    public nombre: string,
    public userNombre: string,
    public puntuacion: number,
    public media: number,
    public fecha: string,
    public notasEv: string,
    public notasOb: string,
    public estado: boolean,
    public flagNotasSec: boolean,
    public flagNotasAsig: boolean
  ) { }
}
