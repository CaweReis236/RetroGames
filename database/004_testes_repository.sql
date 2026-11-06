USE db2025108035;

-- Testes manuais das operacoes de jogo e de seus generos.
-- Todas as alteracoes ficam dentro de uma transacao e sao desfeitas no final.
-- Pre-requisitos: plataforma 1, condicao 2 e generos 1 e 2 existentes.

START TRANSACTION;

-- 1. INSERIR JOGO FICTICIO
INSERT INTO tb_jogo (
    no_jogo,
    ds_jogo,
    nu_ano_lancamento,
    vl_preco,
    qt_estoque,
    st_ativo,
    dt_cadastro,
    co_plataforma,
    co_condicao_jogo
)
VALUES (
    'Jogo Teste Repository',
    'Jogo ficticio para testar o Repository',
    1995,
    99.90,
    5,
    TRUE,
    NOW(),
    1,
    2
);

SET @jogoTeste = LAST_INSERT_ID();

-- 2. CONFERIR INSERCAO
SELECT *
FROM tb_jogo
WHERE co_seq_jogo = @jogoTeste;

-- 3. ALTERAR JOGO
UPDATE tb_jogo
SET no_jogo = 'Jogo Teste Alterado',
    vl_preco = 149.90,
    qt_estoque = 10
WHERE co_seq_jogo = @jogoTeste;

-- 4. CONFERIR ALTERACAO
SELECT *
FROM tb_jogo
WHERE co_seq_jogo = @jogoTeste;

-- 5. DESATIVAR JOGO
UPDATE tb_jogo
SET st_ativo = FALSE
WHERE co_seq_jogo = @jogoTeste;

-- 6. CONFERIR DESATIVACAO
SELECT co_seq_jogo, no_jogo, st_ativo
FROM tb_jogo
WHERE co_seq_jogo = @jogoTeste;

-- 7. LISTAR VINCULOS DE GENEROS
SELECT *
FROM tb_jogo_genero;

SELECT *
FROM tb_jogo_genero
WHERE co_jogo IN (1, 2, 3);

-- 8. INSERIR DOIS GENEROS NO JOGO FICTICIO
INSERT INTO tb_jogo_genero (co_jogo, co_genero)
VALUES (@jogoTeste, 1);

INSERT INTO tb_jogo_genero (co_jogo, co_genero)
VALUES (@jogoTeste, 2);

-- 9. CONFERIR VINCULOS INSERIDOS
SELECT *
FROM tb_jogo_genero
WHERE co_jogo = @jogoTeste;

-- 10. REMOVER OS VINCULOS DO JOGO FICTICIO
DELETE FROM tb_jogo_genero
WHERE co_jogo = @jogoTeste;

-- 11. CONFERIR REMOCAO DOS VINCULOS
SELECT *
FROM tb_jogo_genero
WHERE co_jogo = @jogoTeste;

-- 12. DESFAZER TODAS AS ALTERACOES DE TESTE
ROLLBACK;

-- 13. CONFERIR SE O JOGO FICTICIO FOI REMOVIDO
SELECT co_seq_jogo, no_jogo
FROM tb_jogo
WHERE co_seq_jogo = @jogoTeste;

-- 14. CONFERIR TOTAIS DO BANCO (DADOS DE EXEMPLO)
SELECT COUNT(*) AS total_jogos
FROM tb_jogo;

SELECT COUNT(*) AS total_vinculos
FROM tb_jogo_genero;

SELECT COUNT(*) AS jogos_ativos
FROM tb_jogo
WHERE st_ativo = TRUE;
