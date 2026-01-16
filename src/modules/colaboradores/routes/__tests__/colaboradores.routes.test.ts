import { afterAll, beforeAll, beforeEach, describe, expect, it, vi } from 'vitest';
import request from 'supertest';
import express from 'express';

const prismaMock = vi.hoisted(() => ({
  colaborador: {
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

import { colaboradoresRoutes } from '../colaboradores.routes';
import { errorHandler } from '@/shared/middlewares/errorHandler';

const authToken = 'test-token';

const makeApp = () => {
  const app = express();
  app.use(express.json());
  app.use('/colaboradores', colaboradoresRoutes);
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

describe('colaboradores routes', () => {
  it('rejects missing authorization header', async () => {
    const app = makeApp();
    const res = await request(app).get('/colaboradores');

    expect(res.status).toBe(401);
    expect(res.body).toEqual({ message: 'Authorization não informado' });
  });

  it('rejects invalid authorization header', async () => {
    const app = makeApp();
    const res = await request(app)
      .get('/colaboradores')
      .set('Authorization', 'invalid-token');

    expect(res.status).toBe(401);
    expect(res.body).toEqual({ message: 'Authorization inválido' });
  });

  it('creates a colaborador without data_demissao', async () => {
    const app = makeApp();
    prismaMock.colaborador.findUnique.mockResolvedValue(null);
    prismaMock.colaborador.create.mockResolvedValue({
      matricula: '123',
      nome: 'Ana',
    });

    const res = await request(app)
      .post('/colaboradores')
      .set('Authorization', authToken)
      .send({
        matricula: '123',
        nome: 'Ana',
        data_admissao: '2024-01-01',
      });

    expect(res.status).toBe(201);
    expect(res.body).toEqual({ matricula: '123', nome: 'Ana' });
    expect(prismaMock.colaborador.create).toHaveBeenCalledTimes(1);

    const createArgs = prismaMock.colaborador.create.mock.calls[0][0];
    expect(createArgs.data.data_admissao).toEqual(expect.any(Date));
    expect(createArgs.data.data_demissao).toBeUndefined();
  });

  it('creates a colaborador with data_demissao', async () => {
    const app = makeApp();
    prismaMock.colaborador.findUnique.mockResolvedValue(null);
    prismaMock.colaborador.create.mockResolvedValue({
      matricula: '124',
      nome: 'Bea',
    });

    const res = await request(app)
      .post('/colaboradores')
      .set('Authorization', authToken)
      .send({
        matricula: '124',
        nome: 'Bea',
        data_admissao: '2024-01-01',
        data_demissao: '2024-02-01',
      });

    expect(res.status).toBe(201);
    expect(res.body).toEqual({ matricula: '124', nome: 'Bea' });

    const createArgs = prismaMock.colaborador.create.mock.calls[0][0];
    expect(createArgs.data.data_demissao).toEqual(expect.any(Date));
  });

  it('returns 409 when colaborador already exists', async () => {
    const app = makeApp();
    prismaMock.colaborador.findUnique.mockResolvedValue({
      matricula: '123',
    });

    const res = await request(app)
      .post('/colaboradores')
      .set('Authorization', authToken)
      .send({
        matricula: '123',
        nome: 'Ana',
        data_admissao: '2024-01-01',
      });

    expect(res.status).toBe(409);
    expect(res.body).toEqual({
      message: 'Colaborador já cadastrado com essa matrícula',
    });
  });

  it('lists colaboradores', async () => {
    const app = makeApp();
    prismaMock.colaborador.findMany.mockResolvedValue([
      { matricula: '123' },
      { matricula: '124' },
    ]);

    const res = await request(app)
      .get('/colaboradores')
      .set('Authorization', authToken);

    expect(res.status).toBe(200);
    expect(res.body).toEqual([{ matricula: '123' }, { matricula: '124' }]);
  });

  it('creates colaboradores from csv upload', async () => {
    const app = makeApp();
    prismaMock.colaborador.findUnique
      .mockResolvedValueOnce(null)
      .mockResolvedValueOnce(null);
    prismaMock.colaborador.create
      .mockResolvedValueOnce({ matricula: '123', nome: 'Ana' })
      .mockResolvedValueOnce({ matricula: '124', nome: 'Bea' });

    const csv = [
      'matricula|nome|pis|cpf|data_admissao|data_demissao|nome_unidade|nome_lotacao|situacao|cnpj_unidade',
      '123|Ana|111|222|2024-01-01||Unidade A|Lotacao A|ATIVO|12345678901234',
      '124|Bea|333|444|2024-02-01|2024-03-01|Unidade A|Lotacao A|ATIVO|12345678901234',
    ].join('\n');

    const res = await request(app)
      .post('/colaboradores/import/csv')
      .set('Authorization', authToken)
      .set('Content-Type', 'text/csv')
      .send(csv);

    expect(res.status).toBe(201);
    expect(res.body).toEqual([
      {
        status: 'cadastrado',
        message: 'funcionario 123 foi cadastrado',
        colaborador: { matricula: '123', nome: 'Ana' },
      },
      {
        status: 'cadastrado',
        message: 'funcionario 124 foi cadastrado',
        colaborador: { matricula: '124', nome: 'Bea' },
      },
    ]);
    expect(prismaMock.colaborador.create).toHaveBeenCalledTimes(2);

    const firstCreateArgs = prismaMock.colaborador.create.mock.calls[0][0];
    expect(firstCreateArgs.data.data_admissao).toEqual(expect.any(Date));
    expect(firstCreateArgs.data.data_demissao).toBeUndefined();
  });

  it('returns 400 for empty csv upload', async () => {
    const app = makeApp();

    const res = await request(app)
      .post('/colaboradores/import/csv')
      .set('Authorization', authToken)
      .set('Content-Type', 'text/csv')
      .send('');

    expect(res.status).toBe(400);
    expect(res.body).toEqual({ message: 'CSV vazio' });
  });

  it('returns 400 for csv without data rows', async () => {
    const app = makeApp();

    const res = await request(app)
      .post('/colaboradores/import/csv')
      .set('Authorization', authToken)
      .set('Content-Type', 'text/csv')
      .send('matricula,nome');

    expect(res.status).toBe(400);
    expect(res.body).toEqual({ message: 'CSV inválido' });
  });

  it('returns 400 for invalid csv upload', async () => {
    const app = makeApp();

    const res = await request(app)
      .post('/colaboradores/import/csv')
      .set('Authorization', authToken)
      .set('Content-Type', 'text/csv')
      .send('matricula|nome\n123');

    expect(res.status).toBe(400);
    expect(res.body).toEqual({ message: 'CSV inválido' });
  });

  it('returns 400 for csv with empty headers', async () => {
    const app = makeApp();

    const res = await request(app)
      .post('/colaboradores/import/csv')
      .set('Authorization', authToken)
      .set('Content-Type', 'text/csv')
      .send('matricula||nome\n123||Ana');

    expect(res.status).toBe(400);
    expect(res.body).toEqual({ message: 'CSV inválido' });
  });

  it('returns 400 when csv body is not text', async () => {
    const app = makeApp();

    const res = await request(app)
      .post('/colaboradores/import/csv')
      .set('Authorization', authToken)
      .send({ matricula: '123' });

    expect(res.status).toBe(400);
    expect(res.body).toEqual({ message: 'CSV inválido' });
  });

  it('creates colaboradores from json list', async () => {
    const app = makeApp();
    prismaMock.colaborador.findUnique
      .mockResolvedValueOnce(null)
      .mockResolvedValueOnce({ matricula: '126', nome: 'Dani' });
    prismaMock.colaborador.create
      .mockResolvedValueOnce({ matricula: '125', nome: 'Cris' });

    const res = await request(app)
      .post('/colaboradores/import/json')
      .set('Authorization', authToken)
      .send([
        {
          matricula: '125',
          nome: 'Cris',
          pis: '555',
          cpf: '666',
          data_admissao: '2024-03-01',
          nome_unidade: 'Unidade B',
          nome_lotacao: 'Lotacao B',
          situacao: 'ATIVO',
          cnpj_unidade: '12345678901234',
        },
        {
          matricula: '126',
          nome: 'Dani',
          pis: '777',
          cpf: '888',
          data_admissao: '2024-04-01',
          nome_unidade: 'Unidade B',
          nome_lotacao: 'Lotacao B',
          situacao: 'ATIVO',
          cnpj_unidade: '12345678901234',
        },
      ]);

    expect(res.status).toBe(201);
    expect(res.body).toEqual([
      {
        status: 'cadastrado',
        message: 'funcionario 125 foi cadastrado',
        colaborador: { matricula: '125', nome: 'Cris' },
      },
      {
        status: 'existente',
        message: 'funcionario 126 ja esta cadastrado',
      },
    ]);
    expect(prismaMock.colaborador.create).toHaveBeenCalledTimes(1);
  });

  it('returns 400 for invalid json list', async () => {
    const app = makeApp();

    const res = await request(app)
      .post('/colaboradores/import/json')
      .set('Authorization', authToken)
      .send({ matricula: '125' });

    expect(res.status).toBe(400);
    expect(res.body).toEqual({ message: 'Lista de colaboradores inválida' });
  });

  it('returns 500 on unexpected errors', async () => {
    const app = makeApp();
    const errorSpy = vi.spyOn(console, 'error').mockImplementation(() => {});
    prismaMock.colaborador.findMany.mockRejectedValue(new Error('boom'));

    const res = await request(app)
      .get('/colaboradores')
      .set('Authorization', authToken);

    expect(res.status).toBe(500);
    expect(res.body).toEqual({
      message: 'Erro interno ao processar a requisição',
    });

    errorSpy.mockRestore();
  });

  it('fetches colaborador by matricula', async () => {
    const app = makeApp();
    prismaMock.colaborador.findUnique.mockResolvedValue({
      matricula: '123',
      nome: 'Ana',
    });

    const res = await request(app)
      .get('/colaboradores/123')
      .set('Authorization', authToken);

    expect(res.status).toBe(200);
    expect(res.body).toEqual({ matricula: '123', nome: 'Ana' });
  });

  it('returns 400 when colaborador is not found', async () => {
    const app = makeApp();
    prismaMock.colaborador.findUnique.mockResolvedValue(null);

    const res = await request(app)
      .get('/colaboradores/999')
      .set('Authorization', authToken);

    expect(res.status).toBe(400);
    expect(res.body).toEqual({ message: 'Colaborador não encontrado' });
  });

  it('updates a colaborador', async () => {
    const app = makeApp();
    prismaMock.colaborador.findUnique.mockResolvedValue({
      matricula: '123',
    });
    prismaMock.colaborador.update.mockResolvedValue({
      matricula: '123',
      nome: 'Ana Maria',
    });

    const res = await request(app)
      .put('/colaboradores/123')
      .set('Authorization', authToken)
      .send({ nome: 'Ana Maria' });

    expect(res.status).toBe(200);
    expect(res.body).toEqual({ matricula: '123', nome: 'Ana Maria' });
  });

  it('returns 400 when updating a missing colaborador', async () => {
    const app = makeApp();
    prismaMock.colaborador.findUnique.mockResolvedValue(null);

    const res = await request(app)
      .put('/colaboradores/999')
      .set('Authorization', authToken)
      .send({ nome: 'Ana Maria' });

    expect(res.status).toBe(400);
    expect(res.body).toEqual({ message: 'Colaborador não encontrado' });
  });

  it('removes a colaborador', async () => {
    const app = makeApp();
    prismaMock.colaborador.findUnique.mockResolvedValue({
      matricula: '123',
    });
    prismaMock.colaborador.delete.mockResolvedValue({
      matricula: '123',
    });

    const res = await request(app)
      .delete('/colaboradores/123')
      .set('Authorization', authToken);

    expect(res.status).toBe(204);
    expect(res.body).toEqual({});
  });

  it('returns 400 when removing a missing colaborador', async () => {
    const app = makeApp();
    prismaMock.colaborador.findUnique.mockResolvedValue(null);

    const res = await request(app)
      .delete('/colaboradores/999')
      .set('Authorization', authToken);

    expect(res.status).toBe(400);
    expect(res.body).toEqual({ message: 'Colaborador não encontrado' });
  });
});
