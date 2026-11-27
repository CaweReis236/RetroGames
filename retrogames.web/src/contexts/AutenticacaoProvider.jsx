import { useEffect, useState } from 'react'
import { autenticacaoContext } from './autenticacaoContext.js'
import { autenticacaoApi } from '../api/servicos.js'
export default function AutenticacaoProvider({children}) {
  const [usuario, setUsuario] = useState(null)
  const [carregando, setCarregando] = useState(true)
  useEffect(() => {
    const abort = new AbortController()
    const expirar = () => {setUsuario(null); setCarregando(false)}
    window.addEventListener('sessao:expirada', expirar)
    autenticacaoApi.usuarioAtual(abort.signal).then(u => {if(!abort.signal.aborted)setUsuario(u)}).catch(() => {if(!abort.signal.aborted)setUsuario(null)}).finally(() => {if(!abort.signal.aborted)setCarregando(false)})
    return () => {abort.abort(); window.removeEventListener('sessao:expirada', expirar)}
  }, [])
  async function entrar(credenciais) { const u = await autenticacaoApi.entrar(credenciais); setUsuario(u); return u }
  async function sair() { await autenticacaoApi.sair(); setUsuario(null) }
  return <autenticacaoContext.Provider value={{usuario, carregando, estaAutenticado:!!usuario, ehAdministrador:usuario?.perfil === 'Administrador', entrar, sair}}>{children}</autenticacaoContext.Provider>
}
