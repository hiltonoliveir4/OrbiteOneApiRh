import { Request, Response, NextFunction } from 'express';
import { AfastamentoRepository } from '../infra/AfastamentoRepository';
import { AfastamentoService } from '../services/AfastamentoService';

const repository = new AfastamentoRepository();
const service = new AfastamentoService(repository);

export class AfastamentoController {
  async criar(req: Request, res: Response, next: NextFunction) {
    try {
      const afastamento = await service.criar(req.body);
      return res.status(201).json(afastamento);
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
