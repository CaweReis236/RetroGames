import { Link } from 'react-router-dom'
import { formatarPreco } from '../lib/formato.js'
export default function CardJogo({ jogo }) {
  return <article className="card h-100 jogo-card"><div className="card-body d-flex flex-column">
    <span className="text-secondary small text-uppercase">{jogo.nomePlataforma} · {jogo.ano}</span>
    <h2 className="h5 mt-2">{jogo.nome}</h2><p className="small">{jogo.descricaoCondicao} · {jogo.generos.join(', ')}</p>
    <p className="fs-4 fw-bold mt-auto">{formatarPreco(jogo.preco)}</p>
    {jogo.estoque === 0 && <p className="text-danger fw-bold">Esgotado</p>}
    <Link to={'/jogos/' + jogo.id} className="btn btn-primary">Ver detalhes</Link>
  </div></article>
}
