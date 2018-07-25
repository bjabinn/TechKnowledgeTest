export class SectionInfo {
  constructor(
    public id: number,
    public nombre: string,
    public preguntas: number,
    public contestadas: number,
    public respuestasCorrectas: number,
    public progreso: number,
    public notas: string
  ) { }
}
