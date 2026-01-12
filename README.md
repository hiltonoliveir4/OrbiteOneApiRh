# 🧩 OrbiteOne API RH

API desenvolvida em **Node.js + Express + TypeScript**, utilizando **Prisma ORM** e **PostgreSQL**, com foco na integração de dados de **Colaboradores** e **Afastamentos**, seguindo boas práticas de arquitetura e tratamento de erros.

---

## 📌 Tecnologias Utilizadas

- Node.js
- Express
- TypeScript
- Prisma ORM (v7+)
- PostgreSQL
- Postman

---

## 📁 Estrutura do Projeto

```
src/
├── database/
├── modules/
│   ├── colaboradores/
│   └── afastamentos/
├── shared/
│   ├── errors/
│   └── middlewares/
└── server.ts
```

---

## ⚙️ Pré-requisitos

- Node.js >= 18
- PostgreSQL >= 13
- npm ou yarn

---

## 🛠️ Instalação

```bash
git clone <url-do-repositorio>
cd orbiteOneApiRH
npm install
```

---

## 🗄️ Banco de Dados

```sql
CREATE DATABASE orbiteone_rh;
CREATE SCHEMA IF NOT EXISTS integracao_sisponto;
```

---

## 🔐 Variáveis de Ambiente

Crie um arquivo `.env`:

```env
DATABASE_URL=postgresql://usuario:senha@localhost:5432/orbiteone_rh
SHADOW_DATABASE_URL=postgresql://usuario:senha@localhost:5432/orbiteone_rh_shadow
API_AUTH_TOKEN=SEU_TOKEN_AQUI
```

---

## 📦 Prisma

```bash
npx prisma migrate dev
npx prisma generate
```

---

## ▶️ Executar a API

```bash
npm run dev
```

Base URL:
```
http://localhost:3000
```

---

## 🔑 Autenticação

Todas as rotas exigem o header:

```
Authorization: Bearer SEU_TOKEN_AQUI
```

---

## 📚 Documentação da API

Documentação pública no Postman:

👉 https://documenter.getpostman.com/view/29692695/2sBXVfhqba

---

## ❗ Tratamento de Erros

A API retorna apenas mensagens controladas:

```json
{
  "message": "Colaborador já cadastrado com essa matrícula"
}
```

---

## 🧪 Testes

Os testes podem ser realizados via Postman utilizando a collection documentada.
