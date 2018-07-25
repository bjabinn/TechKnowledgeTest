export class Respuesta {
  constructor(
    public id: number,
    public estado: number,
    public preguntaid: number,
    public evaluacionId: number,
    public notas: string,
    public notasAdmin: string,
  ) { }
}
