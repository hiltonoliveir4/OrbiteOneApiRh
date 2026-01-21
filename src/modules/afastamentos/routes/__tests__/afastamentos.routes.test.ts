import { afterAll, beforeAll, beforeEach, describe, expect, it, vi } from 'vitest';
import request from 'supertest';
import express from 'express';

const prismaMock = vi.hoisted(() => ({
  afastamento: {
    create: vi.fn(),
    findMany: vi.fn(),
    findUnique: vi.fn(),
    update: vi.fn(),
    delete: vi.fn(),
  },
}));

vi.mock('@/database', () => ({
  prisma: prismaMock,
}));

import { afastamentosRoutes } from '../afastamentos.routes';
import { errorHandler } from '@/shared/middlewares/errorHandler';

const authToken = 'test-token';

const makeApp = () => {
  const app = express();
  app.use(express.json());
  app.use('/afastamentos', afastamentosRoutes);
  app.use(errorHandler);
  return app;
};

beforeAll(() => {
  process.env.API_AUTH_TOKEN = authToken;
});

afterAll(() => {
  delete process.env.API_AUTH_TOKEN;
});

beforeEach(() => {
  vi.clearAllMocks();
});

describe('afastamentos routes', () => {
  it('rejects missing authorization header', async () => {
    const app = makeApp();
    const res = await request(app).get('/afastamentos');

    expect(res.status).toBe(401);
    expect(res.body).toEqual({ message: 'Authorization não informado' });
  });

  it('rejects invalid authorization header', async () => {
    const app = makeApp();
    const res = await request(app)
      .get('/afastamentos')
      .set('Authorization', 'invalid-token');

    expect(res.status).toBe(401);
    expect(res.body).toEqual({ message: 'Authorization inválido' });
  });

  it('creates an afastamento without data_final', async () => {
    const app = makeApp();
    prismaMock.afastamento.create.mockResolvedValue({
      id: 1,
      matricula: '123',
    });

    const res = await request(app)
      .post('/afastamentos')
      .set('Authorization', authToken)
      .send({
        matricula: '123',
        descricao: 'Licenca medica',
        data_inicio: '2024-01-10',
      });

    expect(res.status).toBe(201);
    expect(res.body).toEqual({ id: 1, matricula: '123' });

    const createArgs = prismaMock.afastamento.create.mock.calls[0][0];
    expect(createArgs.data.data_inicio).toEqual(expect.any(Date));
    expect(createArgs.data.data_final).toBeUndefined();
  });

  it('creates an afastamento with data_final', async () => {
    const app = makeApp();
    prismaMock.afastamento.create.mockResolvedValue({
      id: 2,
      matricula: '124',
    });

    const res = await request(app)
      .post('/afastamentos')
      .set('Authorization', authToken)
      .send({
        matricula: '124',
        descricao: 'Ferias',
        data_inicio: '2024-01-10',
        data_final: '2024-02-10',
      });

    expect(res.status).toBe(201);
    expect(res.body).toEqual({ id: 2, matricula: '124' });

    const createArgs = prismaMock.afastamento.create.mock.calls[0][0];
    expect(createArgs.data.data_final).toEqual(expect.any(Date));
  });

  it('creates afastamentos from csv upload', async () => {
    const app = makeApp();
    prismaMock.afastamento.findMany
      .mockResolvedValueOnce([{ id: 10, matricula: '123' }])
      .mockResolvedValueOnce([]);
    prismaMock.afastamento.update.mockResolvedValueOnce({
      id: 10,
      matricula: '123',
    });
    prismaMock.afastamento.create.mockResolvedValueOnce({
      id: 11,
      matricula: '124',
    });

    const csv = [
      'matricula|descricao|data_inicio|data_final',
      '123|Licenca|2024-01-10|',
      '124|Ferias|2024-02-10|2024-03-10',
    ].join('\n');

    const res = await request(app)
      .post('/afastamentos/import/csv')
      .set('Authorization', authToken)
      .set('Content-Type', 'text/csv')
      .send(csv);

    expect(res.status).toBe(201);
    expect(res.body).toEqual({
      total: 2,
      criados: 1,
      atualizados: 1,
      sucesso: 2,
      erros: 0,
      linhas_com_erro: [],
    });
    expect(prismaMock.afastamento.create).toHaveBeenCalledTimes(1);
    expect(prismaMock.afastamento.update).toHaveBeenCalledTimes(1);

    const updateArgs = prismaMock.afastamento.update.mock.calls[0][0];
    expect(updateArgs.data.data_inicio).toEqual(expect.any(Date));
    expect(updateArgs.data.data_final).toBeUndefined();

    const createArgs = prismaMock.afastamento.create.mock.calls[0][0];
    expect(createArgs.data.data_inicio).toEqual(expect.any(Date));
    expect(createArgs.data.data_final).toEqual(expect.any(Date));
  });

  it('returns 400 for empty csv upload', async () => {
    const app = makeApp();

    const res = await request(app)
      .post('/afastamentos/import/csv')
      .set('Authorization', authToken)
      .set('Content-Type', 'text/csv')
      .send('');

    expect(res.status).toBe(400);
    expect(res.body).toEqual({ message: 'CSV vazio' });
  });

  it('returns 400 for csv without data rows', async () => {
    const app = makeApp();

    const res = await request(app)
      .post('/afastamentos/import/csv')
      .set('Authorization', authToken)
      .set('Content-Type', 'text/csv')
      .send('matricula|descricao|data_inicio');

    expect(res.status).toBe(400);
    expect(res.body).toEqual({ message: 'CSV inválido' });
  });

  it('returns 400 for invalid csv upload', async () => {
    const app = makeApp();

    const res = await request(app)
      .post('/afastamentos/import/csv')
      .set('Authorization', authToken)
      .set('Content-Type', 'text/csv')
      .send('matricula|descricao|data_inicio\n123|Licenca');

    expect(res.status).toBe(400);
    expect(res.body).toEqual({ message: 'CSV inválido' });
  });

  it('returns 400 for csv with empty headers', async () => {
    const app = makeApp();

    const res = await request(app)
      .post('/afastamentos/import/csv')
      .set('Authorization', authToken)
      .set('Content-Type', 'text/csv')
      .send('matricula||descricao\n123||Licenca');

    expect(res.status).toBe(400);
    expect(res.body).toEqual({ message: 'CSV inválido' });
  });

  it('returns 400 when csv body is not text', async () => {
    const app = makeApp();

    const res = await request(app)
      .post('/afastamentos/import/csv')
      .set('Authorization', authToken)
      .send({ matricula: '123' });

    expect(res.status).toBe(400);
    expect(res.body).toEqual({ message: 'CSV inválido' });
  });

  it('creates afastamentos from json list', async () => {
    const app = makeApp();
    prismaMock.afastamento.findMany
      .mockResolvedValueOnce([{ id: 20, matricula: '123' }])
      .mockResolvedValueOnce([]);
    prismaMock.afastamento.update.mockResolvedValueOnce({
      id: 20,
      matricula: '123',
    });
    prismaMock.afastamento.create.mockResolvedValueOnce({
      id: 21,
      matricula: '124',
    });

    const res = await request(app)
      .post('/afastamentos/import/json')
      .set('Authorization', authToken)
      .send([
        {
          id: 20,
          matricula: '123',
          descricao: 'Licenca',
          data_inicio: '2024-01-10',
        },
        {
          id: 21,
          matricula: '124',
          descricao: 'Ferias',
          data_inicio: '2024-02-10',
        },
      ]);

    expect(res.status).toBe(201);
    expect(res.body).toEqual({
      total: 2,
      criados: 1,
      atualizados: 1,
      sucesso: 2,
      erros: 0,
      linhas_com_erro: [],
    });
    expect(prismaMock.afastamento.create).toHaveBeenCalledTimes(1);
    expect(prismaMock.afastamento.update).toHaveBeenCalledTimes(1);
  });

  it('returns 400 for invalid json list', async () => {
    const app = makeApp();

    const res = await request(app)
      .post('/afastamentos/import/json')
      .set('Authorization', authToken)
      .send({ matricula: '123' });

    expect(res.status).toBe(400);
    expect(res.body).toEqual({ message: 'Lista de afastamentos inválida' });
  });

  it('returns error lines when import has failures', async () => {
    const app = makeApp();
    prismaMock.afastamento.findMany
      .mockResolvedValueOnce([])
      .mockResolvedValueOnce([]);
    prismaMock.afastamento.create
      .mockRejectedValueOnce(new Error('Falha ao criar'))
      .mockResolvedValueOnce({ id: 12, matricula: '124' });

    const res = await request(app)
      .post('/afastamentos/import/json')
      .set('Authorization', authToken)
      .send([
        {
          matricula: '123',
          descricao: 'Licenca',
          data_inicio: '2024-01-10',
        },
        {
          matricula: '124',
          descricao: 'Ferias',
          data_inicio: '2024-02-10',
        },
      ]);

    expect(res.status).toBe(201);
    expect(res.body).toEqual({
      total: 2,
      criados: 1,
      atualizados: 0,
      sucesso: 1,
      erros: 1,
      linhas_com_erro: [
        {
          linha: 1,
          erro: 'Erro ao processar linha',
        },
      ],
    });
  });

  it('returns 500 when creating an afastamento fails', async () => {
    const app = makeApp();
    const errorSpy = vi.spyOn(console, 'error').mockImplementation(() => {});
    prismaMock.afastamento.create.mockRejectedValue(new Error('boom'));

    const res = await request(app)
      .post('/afastamentos')
      .set('Authorization', authToken)
      .send({
        matricula: '123',
        descricao: 'Licenca medica',
        data_inicio: '2024-01-10',
      });

    expect(res.status).toBe(500);
    expect(res.body).toEqual({
      message: 'Erro interno ao processar a requisição',
    });

    errorSpy.mockRestore();
  });

  it('lists afastamentos', async () => {
    const app = makeApp();
    prismaMock.afastamento.findMany.mockResolvedValue([
      { id: 1 },
      { id: 2 },
    ]);

    const res = await request(app)
      .get('/afastamentos')
      .set('Authorization', authToken);

    expect(res.status).toBe(200);
    expect(res.body).toEqual([{ id: 1 }, { id: 2 }]);
  });

  it('returns 500 on unexpected errors', async () => {
    const app = makeApp();
    const errorSpy = vi.spyOn(console, 'error').mockImplementation(() => {});
    prismaMock.afastamento.findMany.mockRejectedValue(new Error('boom'));

    const res = await request(app)
      .get('/afastamentos')
      .set('Authorization', authToken);

    expect(res.status).toBe(500);
    expect(res.body).toEqual({
      message: 'Erro interno ao processar a requisição',
    });

    errorSpy.mockRestore();
  });

  it('fetches afastamento by id', async () => {
    const app = makeApp();
    prismaMock.afastamento.findUnique.mockResolvedValue({
      id: 1,
      matricula: '123',
    });

    const res = await request(app)
      .get('/afastamentos/1')
      .set('Authorization', authToken);

    expect(res.status).toBe(200);
    expect(res.body).toEqual({ id: 1, matricula: '123' });
  });

  it('returns 404 when afastamento is not found', async () => {
    const app = makeApp();
    prismaMock.afastamento.findUnique.mockResolvedValue(null);

    const res = await request(app)
      .get('/afastamentos/999')
      .set('Authorization', authToken);

    expect(res.status).toBe(404);
    expect(res.body).toEqual({ message: 'Afastamento não encontrado' });
  });

  it('lists afastamentos by matricula', async () => {
    const app = makeApp();
    prismaMock.afastamento.findMany.mockResolvedValue([
      { id: 1, matricula: '123' },
    ]);

    const res = await request(app)
      .get('/afastamentos/matricula/123')
      .set('Authorization', authToken);

    expect(res.status).toBe(200);
    expect(res.body).toEqual([{ id: 1, matricula: '123' }]);
  });

  it('returns 500 when listing afastamentos by matricula fails', async () => {
    const app = makeApp();
    const errorSpy = vi.spyOn(console, 'error').mockImplementation(() => {});
    prismaMock.afastamento.findMany.mockRejectedValue(new Error('boom'));

    const res = await request(app)
      .get('/afastamentos/matricula/123')
      .set('Authorization', authToken);

    expect(res.status).toBe(500);
    expect(res.body).toEqual({
      message: 'Erro interno ao processar a requisição',
    });

    errorSpy.mockRestore();
  });

  it('updates an afastamento with date strings', async () => {
    const app = makeApp();
    prismaMock.afastamento.findUnique.mockResolvedValue({
      id: 1,
      matricula: '123',
    });
    prismaMock.afastamento.update.mockResolvedValue({
      id: 1,
      descricao: 'Atualizado',
    });

    const res = await request(app)
      .put('/afastamentos/1')
      .set('Authorization', authToken)
      .send({
        descricao: 'Atualizado',
        data_inicio: '2024-01-01',
        data_final: '2024-01-10',
      });

    expect(res.status).toBe(200);
    expect(res.body).toEqual({ id: 1, descricao: 'Atualizado' });

    const updateArgs = prismaMock.afastamento.update.mock.calls[0][0];
    expect(updateArgs.data.data_inicio).toEqual(expect.any(Date));
    expect(updateArgs.data.data_final).toEqual(expect.any(Date));
  });

  it('returns 404 when updating a missing afastamento', async () => {
    const app = makeApp();
    prismaMock.afastamento.findUnique.mockResolvedValue(null);

    const res = await request(app)
      .put('/afastamentos/999')
      .set('Authorization', authToken)
      .send({ descricao: 'Atualizado' });

    expect(res.status).toBe(404);
    expect(res.body).toEqual({ message: 'Afastamento não encontrado' });
  });

  it('removes an afastamento', async () => {
    const app = makeApp();
    prismaMock.afastamento.findUnique.mockResolvedValue({
      id: 1,
    });
    prismaMock.afastamento.delete.mockResolvedValue({
      id: 1,
    });

    const res = await request(app)
      .delete('/afastamentos/1')
      .set('Authorization', authToken);

    expect(res.status).toBe(204);
    expect(res.body).toEqual({});
  });

  it('returns 404 when removing a missing afastamento', async () => {
    const app = makeApp();
    prismaMock.afastamento.findUnique.mockResolvedValue(null);

    const res = await request(app)
      .delete('/afastamentos/999')
      .set('Authorization', authToken);

    expect(res.status).toBe(404);
    expect(res.body).toEqual({ message: 'Afastamento não encontrado' });
  });
});
