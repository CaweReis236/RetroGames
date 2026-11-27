import { Navigate, Outlet, useLocation } from 'react-router-dom'
import useAutenticacao from '../hooks/useAutenticacao.js'
export default function RotaProtegida({somenteAdministrador=false}) {
  const {carregando, estaAutenticado, ehAdministrador} = useAutenticacao()
  const local = useLocation()
  if(carregando) return <p role="status">Verificando sessão...</p>
  if(!estaAutenticado) return <Navigate replace to={'/login?voltarPara='+encodeURIComponent(local.pathname+local.search)}/>
  if(somenteAdministrador && !ehAdministrador) return <Navigate replace to="/"/>
  return <Outlet/>
}
