-- RetroGames - Etapa 3: estrutura relacional (MySQL 8)
-- Execute em um schema vazio previamente selecionado no Workbench.
-- As tabelas de compras complementam o modulo de catalogo da especificacao.

CREATE TABLE tb_condicao_jogo (
    co_condicao_jogo INT NOT NULL,
    ds_condicao_jogo VARCHAR(50) NOT NULL,
    PRIMARY KEY (co_condicao_jogo),
    UNIQUE KEY uq_condicao_jogo_descricao (ds_condicao_jogo)
);

CREATE TABLE tb_perfil (
    co_perfil INT NOT NULL,
    ds_perfil VARCHAR(50) NOT NULL,
    PRIMARY KEY (co_perfil),
    UNIQUE KEY uq_perfil_descricao (ds_perfil)
);

CREATE TABLE tb_situacao_pedido (
    co_situacao_pedido INT NOT NULL,
    ds_situacao_pedido VARCHAR(50) NOT NULL,
    PRIMARY KEY (co_situacao_pedido),
    UNIQUE KEY uq_situacao_pedido_descricao (ds_situacao_pedido)
);

CREATE TABLE tb_plataforma (
    co_seq_plataforma INT NOT NULL AUTO_INCREMENT,
    no_plataforma VARCHAR(80) NOT NULL,
    no_fabricante VARCHAR(80) NOT NULL,
    nu_ano_lancamento INT NOT NULL,
    PRIMARY KEY (co_seq_plataforma),
    UNIQUE KEY uq_plataforma_nome (no_plataforma)
);

CREATE TABLE tb_genero (
    co_seq_genero INT NOT NULL AUTO_INCREMENT,
    no_genero VARCHAR(50) NOT NULL,
    PRIMARY KEY (co_seq_genero),
    UNIQUE KEY uq_genero_nome (no_genero)
);

CREATE TABLE tb_jogo (
    co_seq_jogo INT NOT NULL AUTO_INCREMENT,
    no_jogo VARCHAR(150) NOT NULL,
    ds_jogo VARCHAR(1000) NULL,
    nu_ano_lancamento INT NOT NULL,
    vl_preco DECIMAL(10,2) NOT NULL,
    qt_estoque INT NOT NULL,
    st_ativo BOOLEAN NOT NULL DEFAULT TRUE,
    dt_cadastro DATETIME NOT NULL,
    co_plataforma INT NOT NULL,
    co_condicao_jogo INT NOT NULL,
    PRIMARY KEY (co_seq_jogo),
    INDEX ix_jogo_nome (no_jogo),
    CONSTRAINT fk_jogo_plataforma FOREIGN KEY (co_plataforma)
        REFERENCES tb_plataforma (co_seq_plataforma),
    CONSTRAINT fk_jogo_condicao FOREIGN KEY (co_condicao_jogo)
        REFERENCES tb_condicao_jogo (co_condicao_jogo)
);

CREATE TABLE tb_jogo_genero (
    co_jogo INT NOT NULL,
    co_genero INT NOT NULL,
    PRIMARY KEY (co_jogo, co_genero),
    CONSTRAINT fk_jogo_genero_jogo FOREIGN KEY (co_jogo)
        REFERENCES tb_jogo (co_seq_jogo),
    CONSTRAINT fk_jogo_genero_genero FOREIGN KEY (co_genero)
        REFERENCES tb_genero (co_seq_genero)
);

CREATE TABLE tb_usuario (
    co_seq_usuario INT NOT NULL AUTO_INCREMENT,
    no_usuario VARCHAR(150) NOT NULL,
    ds_email VARCHAR(256) NOT NULL,
    ds_senha_hash VARCHAR(500) NOT NULL,
    st_ativo BOOLEAN NOT NULL DEFAULT TRUE,
    dt_cadastro DATETIME NOT NULL,
    co_perfil INT NOT NULL,
    PRIMARY KEY (co_seq_usuario),
    UNIQUE KEY uq_usuario_email (ds_email),
    CONSTRAINT fk_usuario_perfil FOREIGN KEY (co_perfil)
        REFERENCES tb_perfil (co_perfil)
);

CREATE TABLE tb_pedido (
    co_seq_pedido INT NOT NULL AUTO_INCREMENT,
    dt_pedido DATETIME NOT NULL,
    vl_total DECIMAL(10,2) NOT NULL,
    co_usuario INT NOT NULL,
    co_situacao_pedido INT NOT NULL,
    PRIMARY KEY (co_seq_pedido),
    CONSTRAINT fk_pedido_usuario FOREIGN KEY (co_usuario)
        REFERENCES tb_usuario (co_seq_usuario),
    CONSTRAINT fk_pedido_situacao FOREIGN KEY (co_situacao_pedido)
        REFERENCES tb_situacao_pedido (co_situacao_pedido)
);

CREATE TABLE tb_item_pedido (
    co_seq_item_pedido INT NOT NULL AUTO_INCREMENT,
    qt_item INT NOT NULL,
    vl_unitario DECIMAL(10,2) NOT NULL,
    co_pedido INT NOT NULL,
    co_jogo INT NOT NULL,
    PRIMARY KEY (co_seq_item_pedido),
    UNIQUE KEY uq_item_pedido_jogo (co_pedido, co_jogo),
    CONSTRAINT fk_rg_item_pedido_pedido FOREIGN KEY (co_pedido)
        REFERENCES tb_pedido (co_seq_pedido),
    CONSTRAINT fk_rg_item_pedido_jogo FOREIGN KEY (co_jogo)
        REFERENCES tb_jogo (co_seq_jogo)
);

-- Verificacao da estrutura:
SHOW TABLES;
