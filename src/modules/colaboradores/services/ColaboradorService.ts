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

  async criarEmLote(
    colaboradores: { linha: number; data: Prisma.ColaboradorCreateInput }[]
  ) {
    const linhasComErro: { linha: number; erro: string }[] = [];
    let criados = 0;
    let atualizados = 0;

    for (const { linha, data } of colaboradores) {
      try {
        const existente = await this.repository.buscarPorMatricula(
          data.matricula
        );

        if (existente) {
          await this.repository.atualizar(data.matricula, data);
          atualizados += 1;
        } else {
          await this.repository.criar(data);
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

    const total = colaboradores.length;
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
