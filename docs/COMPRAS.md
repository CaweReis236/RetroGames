# Módulo de compras — versão básica (C01–C05)

Complemento à especificação técnica. Objetivo: o cliente compra e acompanha seus pedidos.
Entrega prevista: 29/11/2026. Apresentação: 01/12/2026.

## Banco e arquitetura

As tabelas tb_situacao_pedido, tb_pedido e tb_item_pedido já fazem parte de database/001_criar_tabelas.sql. Em banco existente, execute 005_compras.sql para semear as situações fixas (1 Pendente, 2 Pago, 3 Enviado, 4 Cancelado); 006_verificar_compras.sql é somente leitura. Não recrie tabelas nem apague dados existentes.

PedidoRepository, ItemPedidoRepository e SituacaoPedidoRepository leem uma tabela por consulta, com parâmetros. PedidoService combina dados com LINQ, define o preço e total, usa UTC e engloba pedido, itens e estoque em TransactionScope. A leitura do jogo durante a compra usa FOR UPDATE e a baixa é condicional ao estoque e ao jogo ativo. Nenhum preço vindo do cliente é aceito como fonte de verdade.

## Endpoints, aceite e rastreabilidade

| História | Rota / tela | Critérios de aceite e erros | Código / testes |
| --- | --- | --- | --- |
| C01 Como visitante, quero criar conta para comprar. | POST /api/Autenticacao/cadastrar; /cadastro | 201, cliente ativo, senha com hash e cookie; 400/3 modelo inválido, 409/106 e-mail duplicado. | AutenticacaoService, CadastroRequestValidator, AutenticacaoServiceTests, Cadastro.jsx |
| C02 Como cliente, quero menu de compras para acessar meus pedidos. | Layout | Carrinho e Meus pedidos; sem Administração. | Layout.jsx, RotaProtegida.jsx; teste manual |
| C03 Como cliente, quero montar carrinho para escolher quantidades. | /jogos/:id, /carrinho | Não adicionar esgotado; quantidade 1–10, remover, total estimado e persistência local; visitante redirecionado ao login. | CarrinhoProvider.jsx, DetalheJogo.jsx, Carrinho.jsx; teste manual |
| C04 Como usuário logado, quero finalizar a compra para registrar o pedido. | POST /api/Pedido | 201, Pendente, UTC, preço do banco, transação; 401/4, 400/3, 400/110, 409/111. | PedidoService, PedidoRequestValidator, PedidoController, PedidoServiceTests |
| C05 Como usuário logado, quero consultar meus pedidos para acompanhar compras. | GET /api/Pedido/meus; /meus-pedidos | 200, só os próprios pedidos, ordem decrescente, situação, total, itens e preço pago; 401/4. | PedidoService, MeusPedidos.jsx, PedidoServiceTests |

Senhas e sessão não são armazenadas no localStorage; apenas o carrinho. A API continua protegida por cookie HttpOnly/SameSite Strict e autorização. Cadastro usa senha de 8–128 caracteres (decisão do projeto). O checkout não processa pagamentos reais.

## Validação antes da entrega (a executar)

- dotnet test RetroGames.sln; dotnet build RetroGames.sln; npm run build.
- Cadastro, duplicidade, login, persistência do carrinho no F5, quantidade, remoção e total.
- Compra com dois jogos: conferir pedido, itens, preço do banco e baixa de estoque.
- Compra com estoque insuficiente: comparar contagem de pedidos, itens e estoque antes/depois; nada deve mudar.
- Segunda conta: não deve ver pedidos da primeira.
- Testar visitante, cliente e administrador. Verificar 401/403 na API.
- Testar 360px e 1366px. Demonstrar pelo menos cinco heurísticas: status do sistema, linguagem familiar, controle (remover/cancelar), prevenção (limite de quantidade e confirmação), reconhecimento (resumo do carrinho), mensagens de erro.
- Atualizar o quadro compartilhado com o professor, responsáveis e situação real; registrar revisão da sprint. Não marcar como aprovado o que ainda não foi testado.
