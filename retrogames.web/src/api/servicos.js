import api from './api.js'
const dados = async (requisicao) => (await requisicao).data.data
export const jogoApi = { listar: (params, signal) => dados(api.get('/Jogo', { params, signal })), buscarPorId: (id, signal) => dados(api.get('/Jogo/' + id, { signal })) }
export const plataformaApi = { listar: () => dados(api.get('/Plataforma')) }
export const generoApi = { listar: () => dados(api.get('/Genero')) }
export const condicaoJogoApi = { listar: () => dados(api.get('/CondicaoJogo')) }
export const autenticacaoApi = { entrar: (dadosLogin) => dados(api.post('/Autenticacao/login', dadosLogin)), sair: () => dados(api.post('/Autenticacao/logout')), usuarioAtual: (signal) => dados(api.get('/Autenticacao/eu', {signal})) }
