# MIGRATION_NOTES

Data: 2026-01-28

## Endpoints (origem → destino)

### Colaboradores
- `POST /colaboradores` → `POST /colaboradores`
- `POST /colaboradores/import/csv` → `POST /colaboradores/import/csv`
- `POST /colaboradores/import/json` → `POST /colaboradores/import/json`
- `GET /colaboradores` → `GET /colaboradores`
- `GET /colaboradores/:matricula` → `GET /colaboradores/{matricula}`
- `PUT /colaboradores/:matricula` → `PUT /colaboradores/{matricula}`
- `DELETE /colaboradores/:matricula` → `DELETE /colaboradores/{matricula}`

### Afastamentos
- `POST /afastamentos` → `POST /afastamentos`
- `POST /afastamentos/import/csv` → `POST /afastamentos/import/csv`
- `POST /afastamentos/import/json` → `POST /afastamentos/import/json`
- `GET /afastamentos` → `GET /afastamentos`
- `GET /afastamentos/matricula/:matricula` → `GET /afastamentos/matricula/{matricula}`
- `GET /afastamentos/:id` → `GET /afastamentos/{id}`
- `PUT /afastamentos/:id` → `PUT /afastamentos/{id}`
- `DELETE /afastamentos/:id` → `DELETE /afastamentos/{id}`

### Extras
- `GET /health` (novo)

## Diferenças de contrato

### Autenticação
- **Antes:** Header `Authorization` com token completo
- **Depois:** Header `X-API-KEY` com o secret configurado em `Security:StaticSecret`

### Validações
- **Create de Colaboradores/Afastamentos:**
  - **Antes:** Sem validação explícita; erros do banco podiam virar 500.
  - **Depois:** Validação básica via DataAnnotations; retorna `400` com `message: "Payload inválido"` quando faltam campos obrigatórios.

### Atualizações parciais
- **Antes (Node):** `null` em payload podia limpar campos opcionais.
- **Depois (.NET):** Campos `null` são tratados como "não informados" (não alteram o valor atual). Para limpar, usar rotinas específicas ou enviar atualização via banco.

## Diferenças de status codes
- `POST /colaboradores` e `POST /afastamentos`:
  - **Antes:** Possível `500` para payload inválido.
  - **Depois:** `400` (Payload inválido) quando faltam campos obrigatórios.

## Mudanças de validações e mensagens de erro
- Mensagens de CSV e listas inválidas mantidas:
  - `CSV vazio`
  - `CSV inválido`
  - `Lista de colaboradores inválida`
  - `Lista de afastamentos inválida`
- Autenticação alterada:
  - `X-API-KEY não informado`
  - `X-API-KEY inválido`

## Decisões técnicas importantes
- Clean Architecture simplificada: `Api`, `Application`, `Domain`, `Infrastructure`.
- EF Core + Npgsql com schema `integracao_sisponto` e tabelas com nomes originais.
- JSON em `snake_case` para manter compatibilidade com payloads existentes.
- Middleware global de erro retornando `{ "message": "..." }` (sem stack trace).
- **Migrations EF Core:** não geradas automaticamente (ambiente sem `dotnet`/`dotnet-ef`).
  - Recomenda-se gerar via `dotnet ef migrations add Initial` no projeto `Infrastructure`.
- **Testes:** smoke tests foram implementados no nível de services (sem `WebApplicationFactory`) devido a indisponibilidade de pacotes `Microsoft.AspNetCore.Mvc.Testing` no feed atual.
