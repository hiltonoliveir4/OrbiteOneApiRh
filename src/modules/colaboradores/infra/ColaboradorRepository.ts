import { prisma } from '@/database';
import { Colaborador } from '../domain/Colaborador';
import { Prisma } from '@prisma/client';

export class ColaboradorRepository {
  async criar(data: Prisma.ColaboradorCreateInput) {
    return prisma.colaborador.create({
      data: {
        ...data,
        data_admissao: new Date(data.data_admissao as any),
        data_demissao: data.data_demissao
          ? new Date(data.data_demissao as any)
          : undefined,
      },
    });
  }

  async listar() {
    return prisma.colaborador.findMany();
  }

  async buscarPorMatricula(matricula: string) {
    return prisma.colaborador.findUnique({
      where: { matricula }
    });
  }

  async atualizar(matricula: string, data: Partial<Colaborador>) {
    return prisma.colaborador.update({
      where: { matricula },
      data
    });
  }

  async remover(matricula: string) {
    return prisma.colaborador.delete({
      where: { matricula }
    });
  }
}
