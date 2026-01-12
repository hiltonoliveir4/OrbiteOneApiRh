import { Router } from 'express';
import { AfastamentoController } from '../controllers/AfastamentoController';
import { authMiddleware } from '@/shared/middlewares/authMiddleware';

const router = Router();
const controller = new AfastamentoController();

// Protege tudo
router.use(authMiddleware);

// CRUD
router.post('/', controller.criar.bind(controller));
router.get('/', controller.listar.bind(controller));
router.get('/:id', controller.buscarPorId.bind(controller));
router.put('/:id', controller.atualizar.bind(controller));
router.delete('/:id', controller.remover.bind(controller));

// Extra útil
router.get('/matricula/:matricula', controller.listarPorMatricula.bind(controller));

export { router as afastamentosRoutes };
