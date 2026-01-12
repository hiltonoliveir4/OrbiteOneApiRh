import { Router } from 'express';
import { ColaboradorController } from '../controllers/ColaboradorController';
import { authMiddleware } from '@/shared/middlewares/authMiddleware';

const router = Router();
const controller = new ColaboradorController();

router.use(authMiddleware);

router.post('/', controller.criar);
router.get('/', controller.listar);
router.get('/:matricula', controller.buscar);
router.put('/:matricula', controller.atualizar);
router.delete('/:matricula', controller.remover);

export { router as colaboradoresRoutes };
