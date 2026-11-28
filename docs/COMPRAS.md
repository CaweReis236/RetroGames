# Módulo de compras — versão básica (C01–C05)

Complemento à especificação técnica. Objetivo: o cliente compra e acompanha seus pedidos.
Entrega prevista: 29/11/2026. Apresentação: 01/12/2026.

## Banco e arquitetura

As tabelas tb_situacao_pedido, tb_pedido e tb_item_pedido já fazem parte de database/001_criar_tabelas.sql. Em banco existente, execute 005_compras.sql para semear as situações fixas (1 Pendente, 2 Pago, 3 Enviado, 4 Cancelado); 006_verificar_compras.sql é somente leitura. Não recrie tabelas nem apague dados existentes.

PedidoRepository, ItemPedidoRepository e SituacaoPedidoRepository leem uma tabela por consulta, com parâmetros. PedidoService combina dados com LINQ, define o preço e total, usa UTC e engloba pedido, itens e estoque em TransactionScope. A leitura do jogo durante a compra usa FOR UPDATE e a baixa é condicional ao estoque e ao jogo ativo. Nenhum preço vindo do cliente é aceito como fonte de verdade.

## Preparação e auditoria automáticas (sem Workbench)

Na raiz do repositório, com o .env local e conexão com o banco configurados:

```bash
dotnet run --project tools/BancoCompras -- preparar
dotnet run --project tools/BancoCompras -- verificar
```

A primeira operação verifica as tabelas e semeia apenas situações ausentes, com transação e sem sobrescrever dados. A segunda é somente leitura, mostra situações, contagem de pedidos/itens e estoque. A ferramenta aborta se o esquema necessário estiver ausente ou não for transacional. Os scripts SQL permanecem como referência. A ferramenta é separada da API e não expõe credenciais.

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


## Revisão da segurança (RS01–RS06)

- RS01 e RS02: cookie HttpOnly, SameSite Strict, duas horas com renovação deslizante; conforme a especificação acadêmica, sem token antiforgery adicional. Cookie Secure obrigatório fora de Development. Sessão não é persistida no armazenamento do React.
- RS03: FallbackPolicy exige login; cadastro e catálogo públicos declarados explicitamente; manutenção exige SomenteAdministrador. O histórico obtém o ID da sessão, não de parâmetro enviado pelo cliente.
- RS04: PasswordHasher para cadastro e login, sem senha em resposta ou log de aplicação. Os testes de cadastro verificam hash e perfil.
- RS05: .env local ignorado pelo Git; .env.example contém placeholders. Antes da entrega conferir arquivos versionados e histórico; não publicar a configuração real.
- RS06: parâmetros SQL para valores de entrada, inclusive IDs e baixa de estoque. Os nomes de parâmetros de listas são gerados pelo código, não pelo cliente.

A leitura do código foi realizada; a verificação dinâmica de autorização e isolamento deve ser executada no ambiente conectado. Verificar também npm audit; não executar npm audit fix --force sem avaliar impacto.

## Teste de integração automatizado

Com a API em execução no perfil HTTP (localhost:5080), a partir da raiz:

```bash
dotnet run --project tools/BancoCompras -- testar
```

Informe a conta de cliente que já realizou uma compra. A ferramenta solicita a senha sem eco, usa dois clientes HTTP independentes, cria uma conta de teste com endereço fictício e senha aleatória e verifica 401, 403, 400/3, 400/110, 409/106, 409/111, login automático, logout e isolamento. Compara a contagem de pedidos, itens e todos os estoques antes e depois das compras recusadas. Não efetua nova compra válida e não exclui registros. A conta fictícia criada fica no banco. Não executar junto a outras compras ou alterações de estoque. O teste unitário de transação abortada complementa a verificação real de compra recusada.

## Identidade visual e heurísticas para a demonstração

A identidade usa azul-marinho, âmbar, cartuchos e logotipo vetorial autoral, sem capas comerciais. Layout responsivo em Bootstrap. Validar manualmente em 360 e 1366 px.

| Heurística | Onde demonstrar |
| --- | --- |
| Visibilidade de status | Contador do carrinho, carregamento, pedido confirmado e situação Pendente |
| Correspondência com o mundo real | Valores em reais, data brasileira, disponibilidade |
| Controle e liberdade | Remover item, voltar ao catálogo, cancelar confirmação |
| Consistência | Botões de ação, cores e identidade compartilhada |
| Prevenção de erro | Quantidade de 1 a 10, bloqueio de produto esgotado e confirmação do pedido |
| Reconhecimento | Filtros, resumo do carrinho e gêneros exibidos |
| Recuperação de erros | Mensagem de estoque insuficiente, carrinho preservado |

## Revisão da sprint — registro de evidências

Confirmado pelo estudante em 27/11: Aulas 8 e 9, 33 testes unitários, cadastro, carrinho persistente, compra de duas unidades de Vector Search e histórico. Banco conferido em 28/11: um pedido, um item e estoque 8 para Vector Search. Front-end anterior ao acabamento compilado em casa.

Pendente de evidência: novo teste de integração, build após acabamento, revisão visual em ambas as larguras, auditoria de dependências e acesso do professor ao quadro. O merge só deve ocorrer após essas verificações. O percentual de progresso é estimativo, não substitui os critérios de aceite.
