import { Routes, Route } from 'react-router-dom'
import Layout from './components/Layout.jsx'
import Catalogo from './pages/Catalogo/Catalogo.jsx'
import DetalheJogo from './pages/Catalogo/DetalheJogo.jsx'
import NaoEncontrada from './pages/NaoEncontrada.jsx'
import Login from './pages/Login/Login.jsx'
import RotaProtegida from './components/RotaProtegida.jsx'
import Plataforma from './pages/Auxiliares/Plataforma.jsx'
import Jogos from './pages/Administracao/Jogos.jsx'
import FormularioJogo from './pages/Administracao/FormularioJogo.jsx'
import Cadastro from './pages/Cadastro/Cadastro.jsx'
import Carrinho from './pages/Carrinho/Carrinho.jsx'
import MeusPedidos from './pages/Pedidos/MeusPedidos.jsx'
export default function App() {
  return <Routes><Route element={<Layout />}><Route index element={<Catalogo />} /><Route path="jogos/:id" element={<DetalheJogo />} /><Route path="cadastro" element={<Cadastro/>}/><Route path="login" element={<Login />} /><Route element={<RotaProtegida/>}><Route path="carrinho" element={<Carrinho/>}/><Route path="meus-pedidos" element={<MeusPedidos/>}/></Route><Route element={<RotaProtegida somenteAdministrador/>}><Route path="admin" element={<RotaProtegida somenteAdministrador/>}><Route index element={<h1>Administração</h1>}/><Route path="jogos" element={<Jogos/>}/><Route path="jogos/novo" element={<FormularioJogo key="novo"/>}/><Route path="jogos/:id/editar" element={<FormularioJogo key="editar"/>}/><Route path="plataformas" element={<Plataforma/>}/></Route></Route><Route path="*" element={<NaoEncontrada />} /></Route></Routes>
}
