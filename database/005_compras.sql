-- C04/C05: dados de dominio de compras. Execute somente no seu schema RetroGames.
-- As tres tabelas de compras ja constam no 001_criar_tabelas.sql do repositorio.
-- Nao sobrescreve dados existentes. Se houver divergencia de IDs, confira a saida.
START TRANSACTION;
INSERT INTO tb_situacao_pedido (co_situacao_pedido, ds_situacao_pedido)
SELECT 1, 'Pendente' FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM tb_situacao_pedido WHERE co_situacao_pedido=1);
INSERT INTO tb_situacao_pedido (co_situacao_pedido, ds_situacao_pedido)
SELECT 2, 'Pago' FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM tb_situacao_pedido WHERE co_situacao_pedido=2);
INSERT INTO tb_situacao_pedido (co_situacao_pedido, ds_situacao_pedido)
SELECT 3, 'Enviado' FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM tb_situacao_pedido WHERE co_situacao_pedido=3);
INSERT INTO tb_situacao_pedido (co_situacao_pedido, ds_situacao_pedido)
SELECT 4, 'Cancelado' FROM DUAL WHERE NOT EXISTS (SELECT 1 FROM tb_situacao_pedido WHERE co_situacao_pedido=4);
COMMIT;
SELECT co_situacao_pedido, ds_situacao_pedido FROM tb_situacao_pedido ORDER BY co_situacao_pedido;
