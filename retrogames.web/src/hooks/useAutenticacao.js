import { useContext } from 'react'
import { autenticacaoContext } from '../contexts/autenticacaoContext.js'
export default function useAutenticacao() { return useContext(autenticacaoContext) }
