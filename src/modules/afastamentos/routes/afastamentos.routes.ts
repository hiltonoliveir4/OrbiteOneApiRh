import express, { Router } from 'express';
import { AfastamentoController } from '../controllers/AfastamentoController';
import { authMiddleware } from '@/shared/middlewares/authMiddleware';

const router = Router();
const controller = new AfastamentoController();

router.use(authMiddleware);

// CRUD
router.post('/', controller.criar.bind(controller));
router.post(
  '/import/csv',
  express.text({ type: ['text/csv', 'text/plain'] }),
  controller.criarEmLoteCsv.bind(controller)
);
router.post('/import/json', controller.criarEmLoteJson.bind(controller));
router.get('/', controller.listar.bind(controller));
// Extra útil
router.get('/matricula/:matricula', controller.listarPorMatricula.bind(controller));
router.get('/:id', controller.buscarPorId.bind(controller));
router.put('/:id', controller.atualizar.bind(controller));
router.delete('/:id', controller.remover.bind(controller));

export { router as afastamentosRoutes };
