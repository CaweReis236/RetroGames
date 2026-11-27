import { useState } from 'react'
import { useNavigate, useSearchParams } from 'react-router-dom'
import useAutenticacao from '../../hooks/useAutenticacao.js'
import { obterMensagemErro } from '../../api/erros.js'
export default function Login() {
  const { entrar } = useAutenticacao()
  const [params] = useSearchParams()
  const navegar = useNavigate()
  const [email,setEmail] = useState(''), [senha,setSenha] = useState(''), [erro,setErro] = useState(''), [enviando,setEnviando] = useState(false)
  async function enviar(e) {
    e.preventDefault(); setEnviando(true); setErro('')
    try {
      await entrar({email,senha})
      const destino = params.get('voltarPara') || '/'
      navegar(destino.startsWith('/') && !destino.startsWith('//') && !destino.includes('\\') ? destino : '/', {replace:true})
    } catch(e) { setErro(obterMensagemErro(e)) } finally {setEnviando(false)}
  }
  return <section className="card p-4 mx-auto" style={{maxWidth:480}}><h1>Entrar</h1>
    {erro && <div className="alert alert-danger" role="alert">{erro}</div>}
    <form onSubmit={enviar}><label htmlFor="email" className="form-label">E-mail</label><input id="email" type="email" autoComplete="username" className="form-control mb-3" required value={email} onChange={e=>setEmail(e.target.value)}/>
    <label htmlFor="senha" className="form-label">Senha</label><input id="senha" type="password" autoComplete="current-password" className="form-control mb-3" required value={senha} onChange={e=>setSenha(e.target.value)}/>
    <button className="btn btn-primary" disabled={enviando}>{enviando?'Entrando...':'Entrar'}</button></form></section>
}
