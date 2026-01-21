import { Prisma } from '@prisma/client';
import { OrbiteOneError } from '@/shared/errors/OrbiteOneError';
import { AfastamentoRepository } from '../infra/AfastamentoRepository';

export class AfastamentoService {
  constructor(private repository: AfastamentoRepository) {}

  async criar(data: Prisma.AfastamentoCreateInput) {
    return this.repository.criar(data);
  }

  async criarEmLote(
    afastamentos: {
      linha: number;
      data: Prisma.AfastamentoCreateInput & { id?: number };
    }[]
  ) {
    const linhasComErro: { linha: number; erro: string }[] = [];
    let criados = 0;
    let atualizados = 0;

    for (const { linha, data } of afastamentos) {
      try {
        const existentes = await this.repository.listarPorMatricula(
          data.matricula
        );

        if (existentes.length > 0) {
          const { id: _id, ...updateData } = data;
          await this.repository.atualizar(existentes[0].id, updateData);
          atualizados += 1;
        } else {
          const { id: _id, ...createData } = data;
          await this.repository.criar(createData);
          criados += 1;
        }
      } catch (error: any) {
        linhasComErro.push({
          linha,
          erro:
            error instanceof OrbiteOneError
              ? error.message
              : 'Erro ao processar linha',
        });
      }
    }

    const total = afastamentos.length;
    const sucesso = criados + atualizados;
    const erros = linhasComErro.length;

    return {
      total,
      criados,
      atualizados,
      sucesso,
      erros,
      linhas_com_erro: linhasComErro,
    };
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
