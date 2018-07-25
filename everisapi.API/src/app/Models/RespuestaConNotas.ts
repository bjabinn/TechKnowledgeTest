export class RespuestaConNotas {
  constructor(
    public id: number,
    public estado: number,
    public pregunta: string,
    public correcta: string,
    public notas: string,
    public notasAdmin: string,
    public section: string,
    public asignacion: string
  ) { }
}
