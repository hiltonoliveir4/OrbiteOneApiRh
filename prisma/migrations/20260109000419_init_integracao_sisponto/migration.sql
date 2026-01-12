-- CreateTable
CREATE TABLE "colaboradores" (
    "id" SERIAL NOT NULL,
    "matricula" VARCHAR(13) NOT NULL,
    "nome" VARCHAR(50) NOT NULL,
    "pis" VARCHAR(11) NOT NULL,
    "cpf" VARCHAR(11) NOT NULL,
    "data_admissao" TIMESTAMP(3) NOT NULL,
    "data_demissao" TIMESTAMP(3),
    "nome_setor" VARCHAR(40),
    "nome_cargo" VARCHAR(40),
    "nome_departamento" VARCHAR(40),
    "nome_unidade" VARCHAR(40) NOT NULL,
    "nome_lotacao" VARCHAR(40) NOT NULL,
    "situacao" VARCHAR(10) NOT NULL,
    "cnpj_unidade" VARCHAR(14) NOT NULL,
    "ctps" VARCHAR(15),
    "serie" VARCHAR(15),

    CONSTRAINT "colaboradores_pkey" PRIMARY KEY ("id")
);

-- CreateTable
CREATE TABLE "afastamentos" (
    "id" SERIAL NOT NULL,
    "matricula" VARCHAR(13) NOT NULL,
    "descricao" VARCHAR(50) NOT NULL,
    "data_inicio" TIMESTAMP(3) NOT NULL,
    "data_final" TIMESTAMP(3),
    "cnpj_unidade" VARCHAR(14),
    "codigo_situacao" VARCHAR(10),

    CONSTRAINT "afastamentos_pkey" PRIMARY KEY ("id")
);

-- CreateIndex
CREATE UNIQUE INDEX "colaboradores_matricula_key" ON "colaboradores"("matricula");

-- AddForeignKey
ALTER TABLE "afastamentos" ADD CONSTRAINT "afastamentos_matricula_fkey" FOREIGN KEY ("matricula") REFERENCES "colaboradores"("matricula") ON DELETE RESTRICT ON UPDATE CASCADE;
