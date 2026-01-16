import { NextFunction, Request, Response } from 'express';
import { Prisma } from '@prisma/client';
import { ColaboradorRepository } from '../infra/ColaboradorRepository';
import { ColaboradorService } from '../services/ColaboradorService';
import { OrbiteOneError } from '@/shared/errors/OrbiteOneError';

const repository = new ColaboradorRepository();
const service = new ColaboradorService(repository);

const parseCsv = (body: string): Prisma.ColaboradorCreateInput[] => {
  if (!body || !body.trim()) {
    throw new OrbiteOneError('CSV vazio');
  }

  const lines = body
    .split(/\r?\n/)
    .map((line) => line.trim())
    .filter(Boolean);

  if (lines.length < 2) {
    throw new OrbiteOneError('CSV inválido');
  }

  const headers = lines[0].split('|').map((header) => header.trim());

  if (headers.some((header) => !header)) {
    throw new OrbiteOneError('CSV inválido');
  }

  return lines.slice(1).map((line) => {
    const values = line.split('|');

    if (values.length !== headers.length) {
      throw new OrbiteOneError('CSV inválido');
    }

    const item = headers.reduce<Prisma.ColaboradorCreateInput>(
      (acc, header, index) => {
        const rawValue = values[index].trim();
        (acc as Record<string, any>)[header] =
          rawValue === '' ? undefined : rawValue;
        return acc;
      },
      {} as Prisma.ColaboradorCreateInput
    );

    return item;
  });
};

export class ColaboradorController {
  async criar(req: Request, res: Response, next: NextFunction) {
    try {
      const colaborador = await service.criar(req.body);
      return res.status(201).json(colaborador);
    } catch (error: any) {
      next(error);
    }
  }

  async listar(req: Request, res: Response) {
    const colaboradores = await service.listar();
    return res.json(colaboradores);
  }

  async criarEmLoteCsv(req: Request, res: Response, next: NextFunction) {
    try {
      if (typeof req.body !== 'string') {
        throw new OrbiteOneError('CSV inválido');
      }

      const colaboradores = parseCsv(req.body);
      const criados = await service.criarEmLote(colaboradores);
      return res.status(201).json(criados);
    } catch (error: any) {
      next(error);
    }
  }

  async criarEmLoteJson(req: Request, res: Response, next: NextFunction) {
    try {
      if (!Array.isArray(req.body)) {
        throw new OrbiteOneError('Lista de colaboradores inválida');
      }

      const criados = await service.criarEmLote(req.body);
      return res.status(201).json(criados);
    } catch (error: any) {
      next(error);
    }
  }

  async buscar(req: Request, res: Response, next: NextFunction) {
    try {
      const { matricula } = req.params;
      const colaborador = await service.buscarPorMatricula(matricula);
      return res.json(colaborador);
    } catch (error: any) {
      next(error);
    }
  }

  async atualizar(req: Request, res: Response, next: NextFunction) {
    try {
      const { matricula } = req.params;
      const colaborador = await service.atualizar(matricula, req.body);
      return res.json(colaborador);
    } catch (error: any) {
      next(error);
    }
  }

  async remover(req: Request, res: Response, next: NextFunction) {
    try {
      const { matricula } = req.params;
      await service.remover(matricula);
      return res.status(204).send();
    } catch (error: any) {
      next(error);
    }
  }
}
