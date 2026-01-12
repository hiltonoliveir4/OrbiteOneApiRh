import { Prisma } from '@prisma/client';
import { Colaborador } from '../domain/Colaborador';
import { ColaboradorRepository } from '../infra/ColaboradorRepository';
import { OrbiteOneError } from '@/shared/errors/OrbiteOneError';

export class ColaboradorService {
  constructor(
    private repository: ColaboradorRepository
  ) {}

  async criar(colaborador: Prisma.ColaboradorCreateInput) {
    const existente = await this.repository.buscarPorMatricula(
      colaborador.matricula
    );

    if (existente) {
      throw new OrbiteOneError(
        'Colaborador já cadastrado com essa matrícula',
        409
      );
    }

    return this.repository.criar(colaborador);
  }

  async listar() {
    return this.repository.listar();
  }

  async buscarPorMatricula(matricula: string) {
    const colaborador = await this.repository.buscarPorMatricula(matricula);

    if (!colaborador) {
      throw new OrbiteOneError('Colaborador não encontrado');
    }

    return colaborador;
  }

  async atualizar(matricula: string, dados: Partial<Colaborador>) {
    await this.buscarPorMatricula(matricula);
    return this.repository.atualizar(matricula, dados);
  }

  async remover(matricula: string) {
    await this.buscarPorMatricula(matricula);
    return this.repository.remover(matricula);
  }
}
