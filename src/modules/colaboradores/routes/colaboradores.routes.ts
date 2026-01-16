import express, { Router } from 'express';
import { ColaboradorController } from '../controllers/ColaboradorController';
import { authMiddleware } from '@/shared/middlewares/authMiddleware';

const router = Router();
const controller = new ColaboradorController();

router.use(authMiddleware);

router.post('/', controller.criar);
router.post(
  '/import/csv',
  express.text({ type: ['text/csv', 'text/plain'] }),
  controller.criarEmLoteCsv
);
router.post('/import/json', controller.criarEmLoteJson);
router.get('/', controller.listar);
router.get('/:matricula', controller.buscar);
router.put('/:matricula', controller.atualizar);
router.delete('/:matricula', controller.remover);

export { router as colaboradoresRoutes };
