import { NextFunction, Request, Response } from 'express';
import { ColaboradorRepository } from '../infra/ColaboradorRepository';
import { ColaboradorService } from '../services/ColaboradorService';

const repository = new ColaboradorRepository();
const service = new ColaboradorService(repository);

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
