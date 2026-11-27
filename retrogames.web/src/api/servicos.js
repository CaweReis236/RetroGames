import api from './api.js'
const dados = async (requisicao) => (await requisicao).data.data
export const jogoApi = { listar: (params, signal) => dados(api.get('/Jogo', { params, signal })), buscarPorId: (id, signal) => dados(api.get('/Jogo/' + id, { signal })), inserir: modelo => dados(api.post('/Jogo',modelo)), alterar: (id,modelo) => dados(api.put('/Jogo/'+id,modelo)), desativar: id => dados(api.delete('/Jogo/'+id)) }
export const plataformaApi = { listar: () => dados(api.get('/Plataforma')), inserir: modelo => dados(api.post('/Plataforma',modelo)), alterar: (id,modelo) => dados(api.put('/Plataforma/'+id,modelo)), excluir: id => dados(api.delete('/Plataforma/'+id)) }
export const generoApi = { listar: () => dados(api.get('/Genero')) }
export const condicaoJogoApi = { listar: () => dados(api.get('/CondicaoJogo')) }
export const autenticacaoApi = { entrar: (dadosLogin) => dados(api.post('/Autenticacao/login', dadosLogin)), sair: () => dados(api.post('/Autenticacao/logout')), usuarioAtual: (signal) => dados(api.get('/Autenticacao/eu', {signal})) }
