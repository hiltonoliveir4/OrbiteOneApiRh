import { Request, Response, NextFunction } from 'express';
import { OrbiteOneError } from '@/shared/errors/OrbiteOneError';

export function errorHandler(
  err: any,
  _req: Request,
  res: Response,
  _next: NextFunction
) {
  // Erro conhecido da aplicação
  if (err instanceof OrbiteOneError) {
    return res.status(err.statusCode).json({
      message: err.message,
    });
  }

  // Loga erro inesperado
  console.error(err);

  // Erro genérico (não vaza detalhe)
  return res.status(500).json({
    message: 'Erro interno ao processar a requisição',
  });
}
