export function obterMensagemErro(erro) {
  if (!erro.response) return 'Não foi possível conectar ao servidor.'
  return erro.response.data?.errorMessage || 'Não foi possível concluir a operação.'
}
export function obterErrosDosCampos(erro) {
  const resposta = erro.response?.data
  if(resposta?.responseCode !== 3 || !resposta.data) return {}
  return Object.fromEntries(Object.entries(resposta.data).map(([campo,mensagem])=>[campo, Array.isArray(mensagem)?mensagem.join(' '):String(mensagem)]))
}
