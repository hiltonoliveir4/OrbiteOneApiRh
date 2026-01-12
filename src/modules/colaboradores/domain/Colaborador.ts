export interface Colaborador {
  matricula: string;
  nome: string;
  pis: string;
  cpf: string;
  dataAdmissao: Date;
  dataDemissao?: Date;
  nomeSetor?: string;
  nomeCargo?: string;
  nomeDepartamento?: string;
  nomeUnidade: string;
  nomeLotacao: string;
  situacao: string;
  cnpjUnidade: string;
  ctps?: string;
  serie?: string;
}
