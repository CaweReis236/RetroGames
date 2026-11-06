-- RetroGames - Etapa 4: dados de exemplo
-- Executar UMA VEZ depois de 001_criar_tabelas.sql, com o schema correto selecionado.
-- Nao apaga nem altera tabelas de outros trabalhos. Nao insere usuarios.
-- Os IDs explicitos tornam as associacoes jogo-genero verificaveis.

START TRANSACTION;

INSERT INTO tb_condicao_jogo (co_condicao_jogo, ds_condicao_jogo) VALUES
(1, 'Novo'), (2, 'Usado'), (3, 'Recondicionado');

INSERT INTO tb_perfil (co_perfil, ds_perfil) VALUES
(1, 'Administrador'), (2, 'Cliente');

INSERT INTO tb_plataforma (co_seq_plataforma, no_plataforma, no_fabricante, nu_ano_lancamento) VALUES
(1, 'Nintendo Entertainment System', 'Nintendo', 1983),
(2, 'Super Nintendo', 'Nintendo', 1990),
(3, 'Nintendo 64', 'Nintendo', 1996),
(4, 'Mega Drive', 'Sega', 1988),
(5, 'PlayStation', 'Sony', 1994),
(6, 'Game Boy', 'Nintendo', 1989);

INSERT INTO tb_genero (co_seq_genero, no_genero) VALUES
(1, 'Plataforma'),
(2, 'Acao'),
(3, 'Aventura'),
(4, 'RPG'),
(5, 'Corrida'),
(6, 'Luta'),
(7, 'Puzzle'),
(8, 'Esporte');

-- st_ativo = FALSE no jogo 19; qt_estoque = 0 no jogo 20.
-- dt_cadastro em UTC; descricao pode ser NULL segundo a especificacao.
INSERT INTO tb_jogo
(co_seq_jogo, no_jogo, ds_jogo, nu_ano_lancamento, vl_preco, qt_estoque, st_ativo, dt_cadastro, co_plataforma, co_condicao_jogo) VALUES
(1, 'Super Mario Bros.', 'Classico de plataforma do NES.', 1985, 89.90, 4, TRUE, UTC_TIMESTAMP(), 1, 2),
(2, 'The Legend of Zelda', 'Aventura de exploracao no NES.', 1986, 159.90, 2, TRUE, UTC_TIMESTAMP(), 1, 2),
(3, 'Metroid', 'Exploracao e acao no NES.', 1986, 129.90, 3, TRUE, UTC_TIMESTAMP(), 1, 3),
(4, 'Mega Man 2', 'Plataforma e acao no NES.', 1988, 119.90, 5, TRUE, UTC_TIMESTAMP(), 1, 2),
(5, 'Super Mario World', 'Plataforma do Super Nintendo.', 1990, 149.90, 6, TRUE, UTC_TIMESTAMP(), 2, 2),
(6, 'The Legend of Zelda: A Link to the Past', 'Aventura do Super Nintendo.', 1991, 219.90, 2, TRUE, UTC_TIMESTAMP(), 2, 2),
(7, 'Super Metroid', 'Acao e exploracao no Super Nintendo.', 1994, 249.90, 2, TRUE, UTC_TIMESTAMP(), 2, 3),
(8, 'Chrono Trigger', 'RPG do Super Nintendo.', 1995, 399.90, 1, TRUE, UTC_TIMESTAMP(), 2, 2),
(9, 'Super Mario 64', 'Plataforma 3D do Nintendo 64.', 1996, 199.90, 4, TRUE, UTC_TIMESTAMP(), 3, 2),
(10, 'Mario Kart 64', 'Corrida de karts do Nintendo 64.', 1996, 179.90, 3, TRUE, UTC_TIMESTAMP(), 3, 2),
(11, 'The Legend of Zelda: Ocarina of Time', 'Aventura do Nintendo 64.', 1998, 299.90, 2, TRUE, UTC_TIMESTAMP(), 3, 2),
(12, 'Super Smash Bros.', 'Luta com personagens da Nintendo.', 1999, 189.90, 2, TRUE, UTC_TIMESTAMP(), 3, 3),
(13, 'Sonic the Hedgehog', 'Plataforma veloz do Mega Drive.', 1991, 99.90, 5, TRUE, UTC_TIMESTAMP(), 4, 2),
(14, 'Streets of Rage 2', 'Acao cooperativa do Mega Drive.', 1992, 149.90, 3, TRUE, UTC_TIMESTAMP(), 4, 2),
(15, 'Golden Axe', 'Acao e aventura do Mega Drive.', 1989, 109.90, 2, TRUE, UTC_TIMESTAMP(), 4, 3),
(16, 'Crash Bandicoot', 'Plataforma do PlayStation.', 1996, 119.90, 4, TRUE, UTC_TIMESTAMP(), 5, 2),
(17, 'Final Fantasy VII', 'RPG do PlayStation.', 1997, 249.90, 2, TRUE, UTC_TIMESTAMP(), 5, 2),
(18, 'Tekken 3', 'Jogo de luta do PlayStation.', 1998, 129.90, 3, TRUE, UTC_TIMESTAMP(), 5, 2),
(19, 'Tetris', 'Puzzle classico do Game Boy; anuncio inativo.', 1989, 79.90, 1, FALSE, UTC_TIMESTAMP(), 6, 2),
(20, 'Pokemon Red', 'RPG do Game Boy; sem estoque.', 1996, 199.90, 0, TRUE, UTC_TIMESTAMP(), 6, 2);

-- Cada jogo tem entre 1 e 3 generos.
INSERT INTO tb_jogo_genero (co_jogo, co_genero) VALUES
(1,1), (1,2),
(2,3), (2,2),
(3,2), (3,3), (3,1),
(4,1), (4,2),
(5,1), (5,2),
(6,3), (6,4),
(7,2), (7,3), (7,1),
(8,4), (8,3),
(9,1), (9,3),
(10,5), (10,8),
(11,3), (11,2), (11,4),
(12,6), (12,2),
(13,1), (13,2),
(14,2), (14,6),
(15,2), (15,3),
(16,1), (16,2),
(17,4), (17,3),
(18,6), (18,8),
(19,7),
(20,4), (20,3);

COMMIT;

-- Conferencia da Etapa 4:
SELECT 'condicoes' AS tabela, COUNT(*) AS quantidade FROM tb_condicao_jogo
UNION ALL SELECT 'perfis', COUNT(*) FROM tb_perfil
UNION ALL SELECT 'plataformas', COUNT(*) FROM tb_plataforma
UNION ALL SELECT 'generos', COUNT(*) FROM tb_genero
UNION ALL SELECT 'jogos', COUNT(*) FROM tb_jogo
UNION ALL SELECT 'associacoes_jogo_genero', COUNT(*) FROM tb_jogo_genero
UNION ALL SELECT 'usuarios', COUNT(*) FROM tb_usuario;

SELECT co_seq_jogo, no_jogo, qt_estoque, st_ativo
FROM tb_jogo WHERE st_ativo = FALSE OR qt_estoque = 0;

SELECT j.co_seq_jogo, j.no_jogo, COUNT(jg.co_genero) AS quantidade_generos
FROM tb_jogo AS j
LEFT JOIN tb_jogo_genero AS jg ON jg.co_jogo = j.co_seq_jogo
GROUP BY j.co_seq_jogo, j.no_jogo
HAVING COUNT(jg.co_genero) NOT BETWEEN 1 AND 3;
-- A ultima consulta deve retornar zero linhas.

ROLLBACK;
