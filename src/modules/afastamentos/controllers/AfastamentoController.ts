import { Request, Response, NextFunction } from 'express';
import { Prisma } from '@prisma/client';
import { AfastamentoRepository } from '../infra/AfastamentoRepository';
import { AfastamentoService } from '../services/AfastamentoService';
import { OrbiteOneError } from '@/shared/errors/OrbiteOneError';

const repository = new AfastamentoRepository();
const service = new AfastamentoService(repository);

const parseCsv = (
  body: string
): { linha: number; data: Prisma.AfastamentoCreateInput }[] => {
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

  return lines.slice(1).map((line, index) => {
    const values = line.split('|');

    if (values.length !== headers.length) {
      throw new OrbiteOneError('CSV inválido');
    }

    const item = headers.reduce<Prisma.AfastamentoCreateInput>(
      (acc, header, index) => {
        const rawValue = values[index].trim();
        (acc as Record<string, any>)[header] =
          rawValue === '' ? undefined : rawValue;
        return acc;
      },
      {} as Prisma.AfastamentoCreateInput
    );

    return {
      linha: index + 2,
      data: item,
    };
  });
};

export class AfastamentoController {
  async criar(req: Request, res: Response, next: NextFunction) {
    try {
      const afastamento = await service.criar(req.body);
      return res.status(201).json(afastamento);
    } catch (error) {
      next(error);
    }
  }

  async criarEmLoteCsv(req: Request, res: Response, next: NextFunction) {
    try {
      if (typeof req.body !== 'string') {
        throw new OrbiteOneError('CSV inválido');
      }

      const afastamentos = parseCsv(req.body);
      const resultado = await service.criarEmLote(afastamentos);
      return res.status(201).json(resultado);
    } catch (error) {
      next(error);
    }
  }

  async criarEmLoteJson(req: Request, res: Response, next: NextFunction) {
    try {
      if (!Array.isArray(req.body)) {
        throw new OrbiteOneError('Lista de afastamentos inválida');
      }

      const afastamentos = req.body.map(
        (data: Prisma.AfastamentoCreateInput, index: number) => ({
          linha: index + 1,
          data,
        })
      );
      const resultado = await service.criarEmLote(afastamentos);
      return res.status(201).json(resultado);
    } catch (error) {
      next(error);
    }
  }

  async listar(req: Request, res: Response, next: NextFunction) {
    try {
      const itens = await service.listar();
      return res.json(itens);
    } catch (error) {
      next(error);
    }
  }

  async buscarPorId(req: Request, res: Response, next: NextFunction) {
    try {
      const id = Number(req.params.id);
      const item = await service.buscarPorId(id);
      return res.json(item);
    } catch (error) {
      next(error);
    }
  }

  async listarPorMatricula(req: Request, res: Response, next: NextFunction) {
    try {
      const { matricula } = req.params;
      const itens = await service.listarPorMatricula(matricula);
      return res.json(itens);
    } catch (error) {
      next(error);
    }
  }

  async atualizar(req: Request, res: Response, next: NextFunction) {
    try {
      const id = Number(req.params.id);
      const item = await service.atualizar(id, req.body);
      return res.json(item);
    } catch (error) {
      next(error);
    }
  }

  async remover(req: Request, res: Response, next: NextFunction) {
    try {
      const id = Number(req.params.id);
      await service.remover(id);
      return res.status(204).send();
    } catch (error) {
      next(error);
    }
  }
}
