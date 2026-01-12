import { Prisma } from '@prisma/client';
import { OrbiteOneError } from '@/shared/errors/OrbiteOneError';
import { AfastamentoRepository } from '../infra/AfastamentoRepository';

export class AfastamentoService {
  constructor(private repository: AfastamentoRepository) {}

  async criar(data: Prisma.AfastamentoCreateInput) {
    return this.repository.criar(data);
  }

  async listar() {
    return this.repository.listar();
  }

  async buscarPorId(id: number) {
    const item = await this.repository.buscarPorId(id);
    if (!item) throw new OrbiteOneError('Afastamento não encontrado', 404);
    return item;
  }

  async listarPorMatricula(matricula: string) {
    return this.repository.listarPorMatricula(matricula);
  }

  async atualizar(id: number, data: Prisma.AfastamentoUpdateInput) {
    await this.buscarPorId(id);
    return this.repository.atualizar(id, data);
  }

  async remover(id: number) {
    await this.buscarPorId(id);
    return this.repository.remover(id);
  }
}
