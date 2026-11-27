import {useState} from 'react'
import {Link,useNavigate} from 'react-router-dom'
import useCarrinho from '../../hooks/useCarrinho.js'
import {pedidoApi} from '../../api/servicos.js'
import {obterMensagemErro} from '../../api/erros.js'
import {formatarPreco} from '../../lib/formato.js'
export default function Carrinho(){
  const {itens,alterar,remover,limpar,total}=useCarrinho(), navegar=useNavigate()
  const [ocupado,setOcupado]=useState(false),[erro,setErro]=useState('')
  async function finalizar(){if(ocupado||!itens.length||!window.confirm('Confirmar pedido no valor estimado de '+formatarPreco(total)+'?'))return
    setOcupado(true);setErro('')
    try{const pedido=await pedidoApi.finalizar(itens.map(({idJogo,quantidade})=>({idJogo,quantidade})));limpar();navegar('/meus-pedidos',{state:{confirmado:pedido.id}})}
    catch(e){setErro(obterMensagemErro(e))}finally{setOcupado(false)}}
  return <><h1>Seu carrinho</h1><Link to="/" className="btn btn-outline-secondary mb-3">← Continuar comprando</Link>
  {erro&&<div className="alert alert-danger" role="alert">{erro}</div>}
  {!itens.length?<div className="alert alert-secondary">Seu carrinho está vazio. Escolha um jogo no catálogo.</div>:<div className="row g-3"><div className="col-12 col-lg-8">{itens.map(i=><article key={i.idJogo} className="card p-3 mb-3"><div className="row g-2 align-items-center"><div className="col-12 col-md-5"><h2 className="h5"><Link to={'/jogos/'+i.idJogo}>{i.nome}</Link></h2><p className="mb-0">{formatarPreco(i.preco)} por unidade</p></div><div className="col-5 col-md-3"><label className="form-label" htmlFor={'q'+i.idJogo}>Quantidade</label><select className="form-select" disabled={ocupado} id={'q'+i.idJogo} value={i.quantidade} onChange={e=>alterar(i.idJogo,Number(e.target.value))}>{Array.from({length:10},(_,n)=><option key={n+1}>{n+1}</option>)}</select></div><div className="col-7 col-md-4 text-md-end"><p className="fw-bold mb-1">{formatarPreco(i.preco*i.quantidade)}</p><button className="btn btn-outline-danger" disabled={ocupado} onClick={()=>remover(i.idJogo)}>Remover</button></div></div></article>)}</div><aside className="col-12 col-lg-4"><div className="card p-3"><h2 className="h4">Resumo</h2><p className="fs-3 fw-bold">{formatarPreco(total)}</p><p className="small">Total estimado. O preço e estoque são confirmados pelo servidor na finalização.</p><button className="btn btn-primary" disabled={ocupado} onClick={finalizar}>{ocupado?'Finalizando...':'Finalizar compra'}</button></div></aside></div>}</>
}
