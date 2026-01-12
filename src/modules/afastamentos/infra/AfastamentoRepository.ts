import { prisma } from '@/database';
import { Prisma } from '@prisma/client';

export class AfastamentoRepository {
  async criar(data: Prisma.AfastamentoCreateInput) {
    return prisma.afastamento.create({
      data: {
        ...data,
        data_inicio: new Date(data.data_inicio as any),
        data_final: data.data_final ? new Date(data.data_final as any) : undefined,
      },
    });
  }

  async listar() {
    return prisma.afastamento.findMany();
  }

  async buscarPorId(id: number) {
    return prisma.afastamento.findUnique({
      where: { id },
    });
  }

  async listarPorMatricula(matricula: string) {
    return prisma.afastamento.findMany({
      where: { matricula },
    });
  }

  async atualizar(id: number, data: Prisma.AfastamentoUpdateInput) {
    // Converte datas se vierem como string
    const patched: Prisma.AfastamentoUpdateInput = { ...data };

    if (typeof (patched as any).data_inicio === 'string') {
      (patched as any).data_inicio = new Date((patched as any).data_inicio);
    }
    if (typeof (patched as any).data_final === 'string') {
      (patched as any).data_final = new Date((patched as any).data_final);
    }

    return prisma.afastamento.update({
      where: { id },
      data: patched,
    });
  }

  async remover(id: number) {
    return prisma.afastamento.delete({
      where: { id },
    });
  }
}
