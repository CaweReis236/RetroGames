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
export default function App() {
  return <Routes><Route element={<Layout />}><Route index element={<Catalogo />} /><Route path="jogos/:id" element={<DetalheJogo />} /><Route path="login" element={<Login />} /><Route element={<RotaProtegida somenteAdministrador/>}><Route path="admin" element={<RotaProtegida somenteAdministrador/>}><Route index element={<h1>Administração</h1>}/><Route path="jogos" element={<Jogos/>}/><Route path="jogos/novo" element={<FormularioJogo key="novo"/>}/><Route path="jogos/:id/editar" element={<FormularioJogo key="editar"/>}/><Route path="plataformas" element={<Plataforma/>}/></Route></Route><Route path="*" element={<NaoEncontrada />} /></Route></Routes>
}
