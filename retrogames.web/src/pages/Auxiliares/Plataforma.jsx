import { useEffect, useState } from 'react'
import { plataformaApi } from '../../api/servicos.js'
import { obterMensagemErro, obterErrosDosCampos } from '../../api/erros.js'
import { Carregando, MensagemErro, EstadoVazio } from '../../components/MensagensEstado.jsx'
const vazio = {nome:'',fabricante:'',anoLancamento:''}
export default function Plataforma(){
  const [lista,setLista]=useState(null), [form,setForm]=useState(vazio), [id,setId]=useState(null)
  const [erro,setErro]=useState(''), [erros,setErros]=useState({}), [ocupado,setOcupado]=useState(false), [aviso,setAviso]=useState('')
  const [erroLista,setErroLista]=useState(''), [versao,setVersao]=useState(0)
  useEffect(()=>{let vigente=true; plataformaApi.listar().then(l=>{if(vigente){setLista(l);setErroLista('')}}).catch(e=>{if(vigente)setErroLista(obterMensagemErro(e))});return()=>{vigente=false}},[versao])
  function limpar(){setId(null);setForm(vazio);setErros({});setErro('')}
  async function salvar(e){e.preventDefault();setOcupado(true);setErro('');setErros({});setAviso('')
    try{const dados={...form,anoLancamento:Number(form.anoLancamento)};if(id===null)await plataformaApi.inserir(dados);else await plataformaApi.alterar(id,dados);limpar();setAviso('Plataforma salva.');setVersao(v=>v+1)}
    catch(e){setErro(obterMensagemErro(e));setErros(obterErrosDosCampos(e))}finally{setOcupado(false)}
  }
  async function excluir(p){if(!window.confirm('Excluir '+p.nome+'?'))return;setOcupado(true);setErro('');setAviso('');try{await plataformaApi.excluir(p.id);if(id===p.id)limpar();setAviso('Plataforma excluída.');setVersao(v=>v+1)}catch(e){setErro(obterMensagemErro(e))}finally{setOcupado(false)}}
  return <><h1>Plataformas</h1>{erro&&<MensagemErro mensagem={erro}/>} {aviso&&<div role="status" className="alert alert-success">{aviso}</div>}
    <form className="card p-3 mb-4" noValidate onSubmit={salvar}><h2 className="h4">{id===null?'Nova plataforma':'Editar plataforma'}</h2><div className="row g-3">
    {[['nome','Nome','text'],['fabricante','Fabricante','text'],['anoLancamento','Ano de lançamento','number']].map(([campo,rotulo,tipo])=><div className="col-md-4" key={campo}><label className="form-label" htmlFor={campo}>{rotulo}</label><input id={campo} type={tipo} value={form[campo]} className={'form-control'+(erros[campo]?' is-invalid':'')} onChange={e=>setForm(f=>({...f,[campo]:e.target.value}))}/><div className="invalid-feedback">{erros[campo]}</div></div>)}</div>
    <div className="mt-3"><button className="btn btn-primary me-2" disabled={ocupado}>Salvar</button><button type="button" className="btn btn-outline-secondary" onClick={limpar} disabled={ocupado}>Cancelar</button></div></form>
    {erroLista?<MensagemErro mensagem={erroLista} tentarNovamente={()=>setVersao(v=>v+1)}/>:!lista?<Carregando/>:lista.length===0?<EstadoVazio mensagem="Nenhuma plataforma cadastrada."/>:<div className="table-responsive"><table className="table align-middle"><thead><tr><th>Nome</th><th>Fabricante</th><th>Ano</th><th>Ações</th></tr></thead><tbody>{lista.map(p=><tr key={p.id}><td>{p.nome}</td><td>{p.fabricante}</td><td>{p.anoLancamento}</td><td><button disabled={ocupado} className="btn btn-sm btn-outline-primary me-2" onClick={()=>{setId(p.id);setForm({nome:p.nome,fabricante:p.fabricante,anoLancamento:p.anoLancamento});setErro('');setErros({})}}>Editar</button><button disabled={ocupado} className="btn btn-sm btn-outline-danger" onClick={()=>excluir(p)}>Excluir</button></td></tr>)}</tbody></table></div>}</>
}
