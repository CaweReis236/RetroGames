import { useEffect, useState } from 'react'
import { Link, useNavigate, useParams } from 'react-router-dom'
import { jogoApi, plataformaApi, generoApi, condicaoJogoApi } from '../../api/servicos.js'
import { obterMensagemErro, obterErrosDosCampos } from '../../api/erros.js'
import { Carregando, MensagemErro } from '../../components/MensagensEstado.jsx'
const vazio={nome:'',descricao:'',anoLancamento:'',preco:'',estoque:'',idPlataforma:'',idCondicaoJogo:'',idsGeneros:[],ativo:true}
export default function FormularioJogo(){
  const {id}=useParams(), navegar=useNavigate()
  const [form,setForm]=useState(vazio), [apoio,setApoio]=useState(null), [erroCarga,setErroCarga]=useState(''), [versao,setVersao]=useState(0)
  const [erro,setErro]=useState(''), [erros,setErros]=useState({}), [ocupado,setOcupado]=useState(false)
  useEffect(()=>{let vigente=true
    Promise.all([plataformaApi.listar(),generoApi.listar(),condicaoJogoApi.listar(),id?jogoApi.buscarPorId(id):Promise.resolve(null)]).then(([plataformas,generos,condicoes,j])=>{if(!vigente)return;setForm(j?{nome:j.nome,descricao:j.descricao||'',anoLancamento:j.ano,preco:j.preco,estoque:j.estoque,idPlataforma:j.idPlataforma,idCondicaoJogo:j.idCondicaoJogo,idsGeneros:j.generos.map(g=>g.id),ativo:j.ativo}:vazio);setApoio({plataformas,generos,condicoes,id,versao});setErroCarga('')}).catch(e=>{if(vigente)setErroCarga(obterMensagemErro(e))})
    return()=>{vigente=false}},[id,versao])
  function mudar(campo,valor){setForm(f=>({...f,[campo]:valor}))}
  const classe=(campo,tipo='form-control')=>tipo+(erros[campo]?' is-invalid':'')
  const feedback=campo=><div className="invalid-feedback">{erros[campo]}</div>
  async function salvar(e){e.preventDefault();setOcupado(true);setErro('');setErros({})
    const modelo={...form};for(const campo of ['anoLancamento','preco','estoque','idPlataforma','idCondicaoJogo'])modelo[campo]=Number(form[campo])
    if(!id)delete modelo.ativo
    try{if(id)await jogoApi.alterar(id,modelo);else await jogoApi.inserir(modelo);navegar('/admin/jogos')}catch(e){setErro(obterMensagemErro(e));setErros(obterErrosDosCampos(e))}finally{setOcupado(false)}}
  return <><Link to="/admin/jogos" className="btn btn-outline-secondary mb-3">← Voltar à lista</Link><h1>{id?'Editar jogo':'Novo jogo'}</h1>
  {erroCarga?<MensagemErro mensagem={erroCarga} tentarNovamente={()=>{setErroCarga('');setVersao(v=>v+1)}}/>:!apoio||apoio.id!==id||apoio.versao!==versao?<Carregando/>:<form className="card p-3" noValidate onSubmit={salvar}>
    {erro&&<MensagemErro mensagem={erro}/>}
    <div className="row g-3">
    {[['nome','Nome','text'],['anoLancamento','Ano de lançamento','number'],['preco','Preço (R$)','number'],['estoque','Estoque','number']].map(([campo,rotulo,tipo])=><div key={campo} className={campo==='nome'?'col-12':'col-md-4'}><label htmlFor={campo} className="form-label">{rotulo}</label><input id={campo} type={tipo} step={campo==='preco'?'0.01':undefined} className={classe(campo)} value={form[campo]} onChange={e=>mudar(campo,e.target.value)}/>{feedback(campo)}</div>)}
    <div className="col-12"><label htmlFor="descricao" className="form-label">Descrição</label><textarea id="descricao" rows="4" className={classe('descricao')} value={form.descricao} onChange={e=>mudar('descricao',e.target.value)}/>{feedback('descricao')}</div>
    {[['idPlataforma','Plataforma',apoio.plataformas],['idCondicaoJogo','Condição',apoio.condicoes]].map(([campo,rotulo,opcoes])=><div className="col-md-6" key={campo}><label htmlFor={campo} className="form-label">{rotulo}</label><select id={campo} className={classe(campo,'form-select')} value={form[campo]} onChange={e=>mudar(campo,e.target.value)}><option value="">Selecione</option>{opcoes.map(o=><option key={o.id} value={o.id}>{o.nome||o.descricao}</option>)}</select>{feedback(campo)}</div>)}
    <fieldset className="col-12"><legend className="fs-5">Gêneros</legend><div className="row">{apoio.generos.map(g=><div className="col-6 col-md-4" key={g.id}><div className="form-check"><input type="checkbox" id={'genero'+g.id} className={'form-check-input'+(erros.idsGeneros?' is-invalid':'')} checked={form.idsGeneros.includes(g.id)} onChange={e=>setForm(f=>({...f,idsGeneros:e.target.checked?[...f.idsGeneros,g.id]:f.idsGeneros.filter(n=>n!==g.id)}))}/><label htmlFor={'genero'+g.id} className="form-check-label">{g.nome}</label></div></div>)}</div>{erros.idsGeneros&&<div className="text-danger small">{erros.idsGeneros}</div>}</fieldset>
    {id&&<div className="col-12 form-check ms-2"><input id="ativo" type="checkbox" className="form-check-input" checked={form.ativo} onChange={e=>mudar('ativo',e.target.checked)}/><label htmlFor="ativo" className="form-check-label">Jogo ativo</label></div>}
    </div><div className="mt-4"><button className="btn btn-primary me-2" disabled={ocupado}>{ocupado?'Salvando...':'Salvar'}</button><Link className="btn btn-outline-secondary" to="/admin/jogos">Cancelar</Link></div>
  </form>}</>
}
