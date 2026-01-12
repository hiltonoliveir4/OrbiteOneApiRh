import { Request, Response, NextFunction } from 'express';

export function authMiddleware(
  req: Request,
  res: Response,
  next: NextFunction
) {
  const authHeader = req.headers.authorization;

  if (!authHeader) {
    return res.status(401).json({
      message: 'Authorization não informado',
    });
  }

  if (authHeader !== process.env.API_AUTH_TOKEN) {
    return res.status(401).json({
      message: 'Authorization inválido',
    });
  }

  next();
}
