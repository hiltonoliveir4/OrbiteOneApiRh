import express from 'express';
import 'dotenv/config';
import { colaboradoresRoutes } from './modules/colaboradores/routes/colaboradores.routes';
import { errorHandler } from './shared/middlewares/errorHandler';
import { afastamentosRoutes } from './modules/afastamentos/routes/afastamentos.routes';

const app = express();

app.use(express.json());

app.use('/colaboradores', colaboradoresRoutes);
app.use('/afastamentos', afastamentosRoutes);

app.use(errorHandler);

app.listen(3000, () => {
  console.log('API RH rodando na porta 3000');
});
