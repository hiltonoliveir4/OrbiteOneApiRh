export interface Afastamento {
  matricula: string;
  descricao: string;
  dataInicio: Date;
  dataFinal?: Date;
  cnpjUnidade?: string;
  codigoSituacao?: string;
}
