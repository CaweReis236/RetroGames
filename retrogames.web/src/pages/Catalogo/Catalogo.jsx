import { useEffect, useState } from 'react'
import { useSearchParams } from 'react-router-dom'
import { jogoApi, plataformaApi, generoApi } from '../../api/servicos.js'
import { obterMensagemErro } from '../../api/erros.js'
import CardJogo from '../../components/CardJogo.jsx'
import Paginacao from '../../components/Paginacao.jsx'
import { Carregando, MensagemErro, EstadoVazio } from '../../components/MensagensEstado.jsx'
export default function Catalogo() {
  const [params, setParams] = useSearchParams()
  const chave = params.toString()
  const [resultado, setResultado] = useState(null)
  const [apoio, setApoio] = useState({ plataformas: [], generos: [] })
  const [erroApoio, setErroApoio] = useState('')
  const [erro, setErro] = useState('')
  const [tentativa, setTentativa] = useState(0)
  const [tentativaApoio, setTentativaApoio] = useState(0)
  const busca = params.get('busca') || ''
  const pagina = Math.min(1000000, Math.max(1, parseInt(params.get('pagina'), 10) || 1))
  useEffect(() => {
    let vigente = true
    Promise.all([plataformaApi.listar(), generoApi.listar()]).then(([plataformas, generos]) => { if(vigente){setApoio({ plataformas, generos });setErroApoio('')} }).catch(e => {if(vigente)setErroApoio(obterMensagemErro(e))})
    return () => { vigente = false }
  }, [tentativaApoio])
  useEffect(() => {
    const abort = new AbortController()
    const filtros = new URLSearchParams(chave)
    jogoApi.listar({ busca: filtros.get('busca') || undefined, idPlataforma: filtros.get('idPlataforma') || undefined, idGenero: filtros.get('idGenero') || undefined, pagina, tamanhoPagina: 12 }, abort.signal)
      .then(data => { if(!abort.signal.aborted){setResultado({chave,tentativa,data});setErro('')} })
      .catch(e => { if(!abort.signal.aborted){setErro(obterMensagemErro(e));setResultado({chave,tentativa,data:null})} })
    return () => abort.abort()
  }, [chave, pagina, tentativa])
  function mudar(nome, valor) {
    const novos = new URLSearchParams(params)
    if(valor) novos.set(nome, valor); else novos.delete(nome)
    if(nome !== 'pagina') novos.delete('pagina')
    setParams(novos)
  }
  const pronto = resultado?.chave === chave && resultado?.tentativa === tentativa
  return <>
    <section className="hero-retro p-4 p-md-5 mb-4"><div className="rg-hero-texto"><p className="marca fw-bold mb-2 rg-eyebrow">★  ESCOLHA SUA PRÓXIMA PARTIDA</p><h1 className="display-5 fw-bold">Catálogo de jogos</h1><p className="mb-0">Clássicos físicos para jogar de novo. Encontre sua próxima aventura.</p></div><div className="rg-hero-arte" aria-hidden="true"><svg viewBox="0 0 260 150"><rect x="24" y="13" width="212" height="119" rx="12" fill="#0d1728" stroke="#52698b" strokeWidth="4"/><rect x="39" y="26" width="182" height="84" rx="5" fill="#263d57"/><path d="M39 96l42-39 27 26 31-32 82 59H39z" fill="#5d9d98"/><circle cx="177" cy="49" r="15" fill="#ffc66d"/><path d="M100 132v7h60v-7" stroke="#ffc66d" strokeWidth="4"/><rect x="105" y="117" width="50" height="5" rx="2" fill="#ffc66d"/></svg></div></section>
    <form className="row g-3 mb-4 rg-filtros" key={busca} onSubmit={e => { e.preventDefault(); mudar('busca', new FormData(e.currentTarget).get('busca').trim()) }}>
      <div className="col-12 col-md-6"><label htmlFor="busca" className="form-label">Buscar pelo nome</label><div className="input-group"><input id="busca" name="busca" className="form-control" defaultValue={busca} placeholder="Ex.: Vector Search" maxLength={150}/><button className="btn btn-primary">Buscar</button></div></div>
      <div className="col-12 col-sm-6 col-md-3"><label htmlFor="plataforma" className="form-label">Plataforma</label><select id="plataforma" className="form-select" value={params.get('idPlataforma') || ''} onChange={e=>mudar('idPlataforma',e.target.value)}><option value="">Todas</option>{apoio.plataformas.map(p=><option key={p.id} value={p.id}>{p.nome}</option>)}</select></div>
      <div className="col-6 col-md-3"><label htmlFor="genero" className="form-label">Gênero</label><select id="genero" className="form-select" value={params.get('idGenero') || ''} onChange={e=>mudar('idGenero',e.target.value)}><option value="">Todos</option>{apoio.generos.map(g=><option key={g.id} value={g.id}>{g.nome}</option>)}</select></div>
      <div><button type="button" className="btn btn-outline-secondary" onClick={()=>setParams({})}>Limpar filtros</button></div>
    </form>
    {erroApoio && <MensagemErro mensagem={erroApoio} tentarNovamente={()=>setTentativaApoio(n=>n+1)}/>}
    {!pronto ? <Carregando/> : erro ? <MensagemErro mensagem={erro} tentarNovamente={()=>setTentativa(n=>n+1)}/> : <>
      <p className="rg-resultados" role="status">{resultado.data.totalItens} jogos encontrados</p>
      {resultado.data.itens.length === 0 ? <EstadoVazio/> : <div className="row g-3">{resultado.data.itens.map(j=><div className="col-12 col-md-6 col-lg-4" key={j.id}><CardJogo jogo={j}/></div>)}</div>}
      <Paginacao pagina={pagina} totalPaginas={resultado.data.totalPaginas} aoMudar={n=>mudar('pagina',String(n))}/>
    </>}
  </>
}
