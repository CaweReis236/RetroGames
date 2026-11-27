-- Somente leitura. Conferencia do schema e dos dados, sem expor hashes ou e-mails.
SELECT DATABASE() AS banco, VERSION() AS versao;
SELECT TABLE_NAME, ENGINE FROM information_schema.TABLES
WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME IN ('tb_condicao_jogo','tb_perfil','tb_plataforma','tb_genero','tb_jogo','tb_jogo_genero','tb_usuario','tb_situacao_pedido','tb_pedido','tb_item_pedido') ORDER BY TABLE_NAME;
SHOW CREATE TABLE tb_pedido;
SHOW CREATE TABLE tb_item_pedido;
SHOW CREATE TABLE tb_situacao_pedido;
SELECT co_perfil, ds_perfil FROM tb_perfil ORDER BY co_perfil;
SELECT co_situacao_pedido, ds_situacao_pedido FROM tb_situacao_pedido ORDER BY co_situacao_pedido;
SELECT co_seq_jogo, no_jogo, vl_preco, qt_estoque, st_ativo FROM tb_jogo ORDER BY co_seq_jogo;
SELECT COUNT(*) AS pedidos FROM tb_pedido;
SELECT COUNT(*) AS itens FROM tb_item_pedido;
