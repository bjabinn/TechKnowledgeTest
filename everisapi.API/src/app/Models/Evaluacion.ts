import { Respuesta } from "./Respuesta";

export class Evaluacion {
  constructor(
    public id: number,
    public fecha: string,
    public estado: boolean,
    public notasObjetivos: string,
    public notasEvaluacion: string,
    public puntuacion: number,
  ) { }
}
