import {useEffect,useState} from 'react'
import {Link,useLocation} from 'react-router-dom'
import {pedidoApi} from '../../api/servicos.js'
import {obterMensagemErro} from '../../api/erros.js'
import {formatarPreco} from '../../lib/formato.js'
import {Carregando,MensagemErro,EstadoVazio} from '../../components/MensagensEstado.jsx'
export default function MeusPedidos(){
  const {state}=useLocation(),[dados,setDados]=useState(null),[erro,setErro]=useState(''),[versao,setVersao]=useState(0)
  useEffect(()=>{const abort=new AbortController();pedidoApi.meus(abort.signal).then(p=>{if(!abort.signal.aborted){setDados(p);setErro('')}}).catch(e=>{if(!abort.signal.aborted)setErro(obterMensagemErro(e))});return()=>abort.abort()},[versao])
  return <><h1>Meus pedidos</h1><Link to="/" className="btn btn-outline-secondary mb-3">← Voltar ao catálogo</Link>
  {state?.confirmado&&<div className="alert alert-success" role="status">Pedido #{state.confirmado} realizado com sucesso!</div>}
  {erro?<MensagemErro mensagem={erro} tentarNovamente={()=>{setErro('');setVersao(v=>v+1)}}/>:dados===null?<Carregando/>:!dados.length?<EstadoVazio mensagem="Você ainda não tem pedidos. Explore o catálogo."/>:dados.map(p=><article key={p.id} className="card p-3 mb-3"><div className="d-flex flex-wrap justify-content-between gap-2"><h2 className="h5">Pedido #{p.id}</h2><span className="badge text-bg-secondary align-self-start">{p.situacao}</span></div><p>{new Date(p.dataPedido).toLocaleString('pt-BR')}</p><div className="table-responsive"><table className="table"><thead><tr><th>Jogo</th><th>Qtd.</th><th>Preço pago/un.</th><th>Subtotal</th></tr></thead><tbody>{p.itens.map(i=><tr key={i.idJogo}><td>{i.nomeJogo}</td><td>{i.quantidade}</td><td>{formatarPreco(i.valorUnitario)}</td><td>{formatarPreco(i.valorUnitario*i.quantidade)}</td></tr>)}</tbody></table></div><p className="fw-bold text-end mb-0">Total: {formatarPreco(p.total)}</p></article>)}</>
}
