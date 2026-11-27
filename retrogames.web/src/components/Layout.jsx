import { Link, NavLink, Outlet } from 'react-router-dom'
export default function Layout() {
  return <div className="d-flex flex-column min-vh-100">
    <a className="visually-hidden-focusable p-2" href="#conteudo">Ir ao conteúdo</a>
    <header><nav className="navbar navbar-expand-md navbar-dark border-bottom border-secondary"><div className="container">
      <Link className="navbar-brand fw-bold" to="/">RETRO<span className="marca">GAMES</span></Link>
      <button className="navbar-toggler" type="button" data-bs-toggle="collapse" data-bs-target="#menu" aria-controls="menu" aria-expanded="false" aria-label="Abrir menu"><span className="navbar-toggler-icon" /></button>
      <div id="menu" className="collapse navbar-collapse"><div className="navbar-nav ms-auto"><NavLink className="nav-link" to="/" end>Catálogo</NavLink></div></div>
    </div></nav></header>
    <main id="conteudo" className="container py-4 flex-grow-1"><Outlet /></main>
    <footer className="border-top border-secondary py-4"><div className="container">RetroGames · Jogos clássicos, novas partidas.</div></footer>
  </div>
}
