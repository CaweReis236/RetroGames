import {useContext} from 'react'
import {carrinhoContext} from '../contexts/carrinhoContext.js'
export default function useCarrinho(){return useContext(carrinhoContext)}
