import {useEffect,useState} from 'react'
import {carrinhoContext} from './carrinhoContext.js'
const chave='retrogames.carrinho.v1'
function ler(){try{const dados=JSON.parse(localStorage.getItem(chave)||'[]');return Array.isArray(dados)?dados.filter(i=>Number.isInteger(i.idJogo)&&i.idJogo>0&&Number.isInteger(i.quantidade)&&i.quantidade>=1&&i.quantidade<=10&&typeof i.nome==='string'&&Number.isFinite(i.preco)&&i.preco>=0):[]}catch{return []}}
export default function CarrinhoProvider({children}){
  const [itens,setItens]=useState(ler)
  useEffect(()=>{try{localStorage.setItem(chave,JSON.stringify(itens))}catch{/* armazenamento indisponível */}},[itens])
  function adicionar(jogo){if(!jogo.ativo||jogo.estoque<1)return;setItens(anteriores=>{const atual=anteriores.find(i=>i.idJogo===jogo.id);const item={idJogo:jogo.id,nome:jogo.nome,preco:jogo.preco,quantidade:Math.min(10,jogo.estoque,(atual?.quantidade||0)+1)};return atual?anteriores.map(i=>i.idJogo===jogo.id?item:i):[...anteriores,item]})}
  function alterar(id,quantidade){if(!Number.isInteger(quantidade)||quantidade<1||quantidade>10)return;setItens(anteriores=>anteriores.map(i=>i.idJogo===id?{...i,quantidade}:i))}
  function remover(id){setItens(anteriores=>anteriores.filter(i=>i.idJogo!==id))}
  function limpar(){setItens([])}
  const total=itens.reduce((s,i)=>s+i.preco*i.quantidade,0), quantidade=itens.reduce((s,i)=>s+i.quantidade,0)
  return <carrinhoContext.Provider value={{itens,adicionar,alterar,remover,limpar,total,quantidade}}>{children}</carrinhoContext.Provider>
}
