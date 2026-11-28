import { Link } from 'react-router-dom'
import { formatarPreco } from '../lib/formato.js'
function ArteCartucho({jogo}){
  const tom = ['#5e83b9','#b36552','#4c938b','#8066a8','#a17c46'][Math.abs(jogo.id)%5]
  return <div className="rg-cartucho" style={{'--tom-cartucho':tom}} aria-hidden="true">
    <span className="rg-cartucho-grade"/>
    <svg viewBox="0 0 180 100" width="180" height="100">
      <path d="M34 12h112v9h8v68H26V21h8z" fill="#141d30" stroke="#ffffff" strokeOpacity=".25" strokeWidth="2"/>
      <rect x="43" y="24" width="94" height="46" rx="4" fill={tom}/>
      <path d="M43 60l29-27 20 20 13-12 32 29H43z" fill="#fff" opacity=".21"/>
      <path d="M49 77h82M51 82h78" stroke="#66768d" strokeWidth="2"/>
      <rect x="111" y="30" width="18" height="4" fill="#ffe09b"/>
    </svg>
    <span className="rg-cartucho-legenda">{jogo.nomePlataforma}</span>
  </div>
}
export default function CardJogo({ jogo }) {
  return <article className="card h-100 jogo-card"><ArteCartucho jogo={jogo}/><div className="card-body d-flex flex-column">
    <div className="d-flex flex-wrap justify-content-between gap-2 align-items-center"><span className="text-secondary small">{jogo.ano} · {jogo.descricaoCondicao}</span><span className={'rg-estoque '+(jogo.estoque===0?'rg-esgotado':'')}>{jogo.estoque===0?'Esgotado':jogo.estoque<=2?'Últimas unidades':'Disponível'}</span></div>
    <h2 className="h5 mt-3">{jogo.nome}</h2><p className="small text-secondary">{jogo.generos.join(' · ')}</p>
    <p className="fs-4 fw-bold mt-auto">{formatarPreco(jogo.preco)}</p>
    <Link to={'/jogos/' + jogo.id} className="btn btn-primary">Ver detalhes</Link>
  </div></article>
}
