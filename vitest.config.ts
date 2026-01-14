import { defineConfig } from 'vitest/config';
import path from 'path';

export default defineConfig({
  test: {
    environment: 'node',
    coverage: {
      provider: 'v8',
      reporter: ['text', 'json', 'html'],
      all: true,
      include: [
        'src/modules/colaboradores/**/*.ts',
        'src/modules/afastamentos/**/*.ts',
        'src/shared/middlewares/**/*.ts',
        'src/shared/errors/OrbiteOneError.ts',
      ],
    },
  },
  resolve: {
    alias: {
      '@': path.resolve(__dirname, 'src'),
    },
  },
});
