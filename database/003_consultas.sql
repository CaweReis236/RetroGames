-- RetroGames - Aula 1, Etapa 5: consultas e testes de integridade
-- Banco da UNIFAL. Nao apaga tabelas nem dados existentes.
USE db2025108035;

-- =============================================================
-- CONSULTAS (execute normalmente)
-- =============================================================

-- 1. Catalogo: jogos ativos, plataforma e condicao, por nome.
SELECT j.co_seq_jogo, j.no_jogo, p.no_plataforma,
       c.ds_condicao_jogo, j.vl_preco, j.qt_estoque
FROM tb_jogo AS j
JOIN tb_plataforma AS p ON p.co_seq_plataforma = j.co_plataforma
JOIN tb_condicao_jogo AS c ON c.co_condicao_jogo = j.co_condicao_jogo
WHERE j.st_ativo = TRUE
ORDER BY j.no_jogo, j.co_seq_jogo;

-- 2. Generos de um jogo informado: altere o nome abaixo.
SET @nome_jogo = 'Super Mario World';
SELECT j.no_jogo, g.no_genero
FROM tb_jogo AS j
JOIN tb_jogo_genero AS jg ON jg.co_jogo = j.co_seq_jogo
JOIN tb_genero AS g ON g.co_seq_genero = jg.co_genero
WHERE j.no_jogo COLLATE utf8mb4_unicode_ci =
      @nome_jogo COLLATE utf8mb4_unicode_ci
ORDER BY g.no_genero;

-- 3. Jogos de um genero informado: altere o genero abaixo.
SET @nome_genero = 'Plataforma';
SELECT g.no_genero, j.no_jogo, p.no_plataforma
FROM tb_genero AS g
JOIN tb_jogo_genero AS jg ON jg.co_genero = g.co_seq_genero
JOIN tb_jogo AS j ON j.co_seq_jogo = jg.co_jogo
JOIN tb_plataforma AS p ON p.co_seq_plataforma = j.co_plataforma
WHERE g.no_genero COLLATE utf8mb4_unicode_ci =
      @nome_genero COLLATE utf8mb4_unicode_ci
ORDER BY j.no_jogo;

-- 4. Contagem por plataforma, incluindo plataformas sem jogos.
SELECT p.co_seq_plataforma, p.no_plataforma,
       COUNT(j.co_seq_jogo) AS quantidade_jogos
FROM tb_plataforma AS p
LEFT JOIN tb_jogo AS j ON j.co_plataforma = p.co_seq_plataforma
GROUP BY p.co_seq_plataforma, p.no_plataforma
ORDER BY p.no_plataforma;

-- 5. Busca por parte do nome, cinco resultados por pagina.
-- Pagina 1: @pagina = 1; pagina 2: @pagina = 2.
-- 5. Busca de jogos por nome com paginação

SET @busca = 'a';
SET @pagina = 1;
SET @tamanho_pagina = 5;

SELECT
    j.co_seq_jogo,
    j.no_jogo,
    j.vl_preco
FROM tb_jogo AS j
WHERE j.no_jogo COLLATE utf8mb4_unicode_ci
      LIKE CONCAT('%', @busca, '%') COLLATE utf8mb4_unicode_ci
ORDER BY j.no_jogo, j.co_seq_jogo
LIMIT 5 OFFSET 5;

-- =============================================================
-- TESTES DE INTEGRIDADE - EXECUTAR UM POR VEZ, MANUALMENTE
-- =============================================================
-- ATENCAO: os comandos de teste abaixo estao COMENTADOS para
-- impedir alteracoes acidentais ao executar o arquivo inteiro.
-- Para cada teste: execute START TRANSACTION; descomente e execute
-- SOMENTE a instrucao do teste; registre o erro real no comentario;
-- execute ROLLBACK; antes de iniciar o proximo teste.
-- Se o Workbench parar no erro, execute ROLLBACK separadamente.
-- Use a mesma aba/conexao para START TRANSACTION e ROLLBACK.
-- Nao use COMMIT nos testes.

-- TESTE 1: FK inexistente em tb_jogo.
-- Esperado: erro 1452 (Cannot add or update a child row).
-- Real: erro 1452. Chave estrangeira funcionando corretamente.
 START TRANSACTION;
INSERT INTO tb_jogo
 (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque,
  st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo)
 SELECT '__RG_TESTE_FK__', NULL, 1990, 10.00, 1,
        TRUE, UTC_TIMESTAMP(),
        (SELECT COALESCE(MAX(co_seq_plataforma), 0) + 100000
        FROM tb_plataforma),
      (SELECT MIN(co_condicao_jogo) FROM tb_condicao_jogo);
ROLLBACK;

-- TESTE 2: plataforma com nome ja existente.
-- Esperado: erro 1062 (Duplicate entry, UNIQUE).
-- Real: erro 1062. Restrição UNIQUE funcionando corretamente.
 START TRANSACTION;
 INSERT INTO tb_plataforma (no_plataforma, no_fabricante, nu_ano_lancamento)
 SELECT no_plataforma, 'Teste', 1990
 FROM tb_plataforma ORDER BY co_seq_plataforma LIMIT 1;
 ROLLBACK;

-- TESTE 3: excluir plataforma referenciada por algum jogo.
-- Esperado: erro 1451 (Cannot delete or update a parent row).
-- Real: erro 1451. Chave estrangeira impediu a exclusão.
 START TRANSACTION;
 DELETE FROM tb_plataforma
 WHERE co_seq_plataforma =
(SELECT co_plataforma FROM tb_jogo ORDER BY co_seq_jogo LIMIT 1);
 ROLLBACK;

-- TESTE 4: jogo com no_jogo NULL.
-- Esperado: erro 1048 (Column 'no_jogo' cannot be null).
-- Real: PREENCHER APOS EXECUTAR.
 START TRANSACTION;
 INSERT INTO tb_jogo
 (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque,
  st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo)
 SELECT NULL, NULL, 1990, 10.00, 1, TRUE, UTC_TIMESTAMP(),
        (SELECT MIN(co_seq_plataforma) FROM tb_plataforma),
       (SELECT MIN(co_condicao_jogo) FROM tb_condicao_jogo);
 ROLLBACK;

-- TESTE 5: preco igual a zero (validacao fica no Service, RN06).
-- Esperado: INSERT aceito (1 row affected), sem erro SQL.
-- Real: PREENCHER APOS EXECUTAR.
 START TRANSACTION;
 INSERT INTO tb_jogo
 (no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque,
  st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo)
 SELECT '__RG_TESTE_PRECO_ZERO__', 'Registro temporario', 1990,
        0.00, 1, TRUE, UTC_TIMESTAMP(),
        (SELECT MIN(co_seq_plataforma) FROM tb_plataforma),
        (SELECT MIN(co_condicao_jogo) FROM tb_condicao_jogo);
 ROLLBACK;

-- =============================================================
-- LIMPEZA / CONFERENCIA
-- =============================================================
-- ROLLBACK; -- execute na MESMA conexao apos cada teste.
-- Os testes 1 a 4 falham sem inserir/apagar nada; o teste 5
-- insere temporariamente, mas ROLLBACK remove o registro.
-- Conferir que nao sobraram registros de teste:
SELECT co_seq_jogo, no_jogo
FROM tb_jogo
WHERE no_jogo IN ('__RG_TESTE_FK__', '__RG_TESTE_PRECO_ZERO__');
-- Esperado: 0 linhas.
