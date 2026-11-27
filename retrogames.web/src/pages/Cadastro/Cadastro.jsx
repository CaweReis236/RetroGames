import { useState } from 'react'
import { Link,useNavigate,useSearchParams } from 'react-router-dom'
import useAutenticacao from '../../hooks/useAutenticacao.js'
import { obterMensagemErro,obterErrosDosCampos } from '../../api/erros.js'
export default function Cadastro(){
  const {cadastrar}=useAutenticacao(), navegar=useNavigate(), [params]=useSearchParams()
  const [form,setForm]=useState({nome:'',email:'',senha:''}),[erro,setErro]=useState(''),[erros,setErros]=useState({}),[ocupado,setOcupado]=useState(false)
  async function salvar(e){e.preventDefault();setOcupado(true);setErro('');setErros({})
    try{await cadastrar(form);const destino=params.get('voltarPara')||'/';navegar(destino.startsWith('/')&&!destino.startsWith('//')&&!destino.includes('\\')?destino:'/',{replace:true})}
    catch(e){setErro(obterMensagemErro(e));setErros(obterErrosDosCampos(e))}finally{setOcupado(false)}}
  return <section className="card p-4 mx-auto" style={{maxWidth:520}}><h1>Criar conta</h1><p>Cadastre-se para comprar seus jogos favoritos.</p>{erro&&<div role="alert" className="alert alert-danger">{erro}</div>}
  <form onSubmit={salvar} noValidate>{[['nome','Nome','text','name'],['email','E-mail','email','email'],['senha','Senha','password','new-password']].map(([campo,rotulo,tipo,auto])=><div className="mb-3" key={campo}><label className="form-label" htmlFor={campo}>{rotulo}</label><input id={campo} type={tipo} autoComplete={auto} className={'form-control'+(erros[campo]?' is-invalid':'')} value={form[campo]} onChange={e=>setForm(f=>({...f,[campo]:e.target.value}))}/><div className="invalid-feedback">{erros[campo]}</div>{campo==='senha'&&<div className="form-text">De 8 a 128 caracteres. Use uma senha exclusiva.</div>}</div>)}<button className="btn btn-primary" disabled={ocupado}>{ocupado?'Criando conta...':'Criar conta'}</button></form><p className="mt-3 mb-0"><Link to={'/login?'+params.toString()}>Já tenho conta</Link></p></section>
}
