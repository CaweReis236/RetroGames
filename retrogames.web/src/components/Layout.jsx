import { Link, NavLink, Outlet, useNavigate } from 'react-router-dom'
import { useState } from 'react'
import useAutenticacao from '../hooks/useAutenticacao.js'
import { obterMensagemErro } from '../api/erros.js'
import useCarrinho from '../hooks/useCarrinho.js'
import Marca from './Marca.jsx'
export default function Layout() {
  const {usuario, carregando, ehAdministrador, sair} = useAutenticacao()
  const {quantidade}=useCarrinho()
  const navegar = useNavigate()
  const [erro,setErro] = useState('')
  async function encerrar(){setErro('');try{await sair();navegar('/')}catch(e){setErro(obterMensagemErro(e))}}
  return <div className="d-flex flex-column min-vh-100">
    <a className="visually-hidden-focusable p-2" href="#conteudo">Ir ao conteúdo</a>
    <header><nav className="navbar navbar-expand-md navbar-dark border-bottom border-secondary"><div className="container">
      <Link className="navbar-brand fw-bold" to="/"><Marca/></Link>
      <button className="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#menu" aria-controls="menu" aria-expanded="false" aria-label="Abrir menu"><span className="navbar-toggler-icon" /></button>
      <div id="menu" className="collapse navbar-collapse"><div className="navbar-nav ms-auto align-items-md-center gap-md-2"><NavLink className="nav-link" to="/" end>Catálogo</NavLink><NavLink className="nav-link" to="/carrinho">Carrinho <span className="rg-contador">{quantidade}</span></NavLink>{!carregando && (usuario ? <><span className="navbar-text px-md-2">Olá, {usuario.nome}</span>{!ehAdministrador && <NavLink className="nav-link" to="/meus-pedidos">Meus pedidos</NavLink>}{ehAdministrador && <div className="nav-item dropdown"><button className="nav-link dropdown-toggle" data-bs-toggle="dropdown" aria-expanded="false">Administração</button><div className="dropdown-menu"><NavLink className="dropdown-item" to="/admin/jogos">Jogos</NavLink><NavLink className="dropdown-item" to="/admin/plataformas">Plataformas</NavLink></div></div>}<button className="btn btn-outline-secondary ms-md-2" onClick={encerrar}>Sair</button></> : <NavLink className="nav-link" to="/login">Entrar</NavLink>)}</div></div>
    </div></nav></header>
    <main id="conteudo" className="container py-4 flex-grow-1">{erro && <div className="alert alert-danger" role="alert">{erro}</div>}<Outlet /></main>
    <footer className="border-top border-secondary py-4"><div className="container d-flex flex-wrap justify-content-between gap-2"><span>RetroGames · Jogos clássicos, novas partidas.</span><span className="small">Projeto acadêmico · Sem pagamentos reais</span></div></footer>
  </div>
}
