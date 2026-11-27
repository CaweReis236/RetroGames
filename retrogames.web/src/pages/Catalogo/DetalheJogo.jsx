import { useEffect, useState } from 'react'
import { Link, useParams } from 'react-router-dom'
import { jogoApi } from '../../api/servicos.js'
import { obterMensagemErro } from '../../api/erros.js'
import { formatarPreco } from '../../lib/formato.js'
import { Carregando, MensagemErro } from '../../components/MensagensEstado.jsx'
import useCarrinho from '../../hooks/useCarrinho.js'
export default function DetalheJogo() {
  const {adicionar,itens}=useCarrinho()
  const [aviso,setAviso]=useState('')
  const { id } = useParams()
  const [estado, setEstado] = useState(null)
  const [tentativa, setTentativa] = useState(0)
  useEffect(()=>{
    const abort = new AbortController()
    jogoApi.buscarPorId(id, abort.signal).then(jogo=>{if(!abort.signal.aborted)setEstado({id,tentativa,jogo})}).catch(e=>{if(!abort.signal.aborted)setEstado({id,tentativa,erro:obterMensagemErro(e)})})
    return ()=>abort.abort()
  },[id,tentativa])
  const jogo=estado?.jogo
  return <><Link className="btn btn-outline-secondary mb-4" to="/">← Voltar ao catálogo</Link>
    {estado?.id !== id || estado?.tentativa !== tentativa ? <Carregando/> : estado.erro ? <MensagemErro mensagem={estado.erro} tentarNovamente={()=>setTentativa(n=>n+1)}/> : <article className="card p-4">
      <p className="text-secondary">{jogo.nomePlataforma} · {jogo.ano}</p><h1>{jogo.nome}</h1><p>{jogo.descricao || 'Sem descrição.'}</p>
      <dl><dt>Condição</dt><dd>{jogo.descricaoCondicao}</dd><dt>Gêneros</dt><dd>{jogo.generos.map(g=>g.nome).join(', ')}</dd><dt>Estoque</dt><dd>{jogo.estoque ? jogo.estoque+' disponíveis' : 'Esgotado'}</dd></dl><p className="fs-3 fw-bold">{formatarPreco(jogo.preco)}</p>
          <div className="d-flex flex-wrap gap-2"><button className="btn btn-primary" disabled={!jogo.ativo||jogo.estoque<1||(itens.find(i=>i.idJogo===jogo.id)?.quantidade||0)>=Math.min(10,jogo.estoque)} onClick={()=>{adicionar(jogo);setAviso('Jogo adicionado ao carrinho.')}}>{jogo.estoque<1?'Esgotado':'Adicionar ao carrinho'}</button><Link className="btn btn-outline-secondary" to="/carrinho">Ver carrinho</Link></div>{aviso&&<p role="status" className="mt-2">{aviso}</p>}
    </article>}
  </>
}
