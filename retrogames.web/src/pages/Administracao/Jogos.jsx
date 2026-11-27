import { useEffect, useState } from 'react'
import { Link, useSearchParams } from 'react-router-dom'
import { jogoApi } from '../../api/servicos.js'
import { obterMensagemErro } from '../../api/erros.js'
import { Carregando, MensagemErro, EstadoVazio } from '../../components/MensagensEstado.jsx'
import Paginacao from '../../components/Paginacao.jsx'
import { formatarPreco } from '../../lib/formato.js'
export default function Jogos(){
  const [params,setParams]=useSearchParams()
  const busca=params.get('busca')||'', pagina=Math.max(1,Math.min(1000000,parseInt(params.get('pagina'),10)||1))
  const [dados,setDados]=useState(null), [erro,setErro]=useState(''), [erroAcao,setErroAcao]=useState(''), [versao,setVersao]=useState(0), [ocupado,setOcupado]=useState(false)
  useEffect(()=>{const abort=new AbortController();jogoApi.listar({busca,pagina,tamanhoPagina:12,incluirInativos:true},abort.signal).then(d=>{if(!abort.signal.aborted){setDados({d,busca,pagina,versao});setErro('')}}).catch(e=>{if(!abort.signal.aborted)setErro(obterMensagemErro(e))});return()=>abort.abort()},[busca,pagina,versao])
  async function desativar(j){if(!window.confirm('Desativar '+j.nome+'?'))return;setOcupado(true);setErroAcao('');try{await jogoApi.desativar(j.id);setVersao(v=>v+1)}catch(e){setErroAcao(obterMensagemErro(e))}finally{setOcupado(false)}}
  const pronto=dados?.busca===busca&&dados?.pagina===pagina&&dados?.versao===versao
  return <><div className="d-flex flex-wrap justify-content-between align-items-center gap-2 mb-3"><h1>Administrar jogos</h1><Link to="/admin/jogos/novo" className="btn btn-primary">Novo jogo</Link></div>
  <form key={busca} className="mb-3" onSubmit={e=>{e.preventDefault();setErro('');setParams({busca:new FormData(e.currentTarget).get('busca')})}}><label htmlFor="buscaAdmin" className="form-label">Buscar jogo</label><div className="input-group"><input className="form-control" id="buscaAdmin" name="busca" defaultValue={busca} maxLength={150}/><button className="btn btn-primary">Buscar</button><button className="btn btn-outline-secondary" type="button" onClick={()=>{setErro('');setParams({})}}>Limpar</button></div></form>
  {erroAcao&&<MensagemErro mensagem={erroAcao}/>}
  {erro?<MensagemErro mensagem={erro} tentarNovamente={()=>{setErro('');setVersao(v=>v+1)}}/>:!pronto?<Carregando/>:dados.d.itens.length===0?<EstadoVazio mensagem="Nenhum jogo encontrado."/>:<><p role="status">{dados.d.totalItens} jogos</p><div className="table-responsive"><table className="table align-middle"><thead><tr><th>Nome</th><th>Preço</th><th>Estoque</th><th>Situação</th><th>Ações</th></tr></thead><tbody>{dados.d.itens.map(j=><tr key={j.id}><td>{j.nome}</td><td>{formatarPreco(j.preco)}</td><td>{j.estoque}</td><td><span className={'badge '+(j.ativo?'text-bg-success':'text-bg-secondary')}>{j.ativo?'Ativo':'Inativo'}</span></td><td><Link to={'/admin/jogos/'+j.id+'/editar'} className="btn btn-sm btn-outline-primary me-2">Editar</Link>{j.ativo&&<button className="btn btn-sm btn-outline-danger" disabled={ocupado} onClick={()=>desativar(j)}>Desativar</button>}</td></tr>)}</tbody></table></div><Paginacao pagina={pagina} totalPaginas={dados.d.totalPaginas} aoMudar={n=>{setErro('');setParams({busca,pagina:String(n)})}}/></>}
  </>
}
