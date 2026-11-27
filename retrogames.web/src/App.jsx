import { Routes, Route } from 'react-router-dom'
import Layout from './components/Layout.jsx'
import Catalogo from './pages/Catalogo/Catalogo.jsx'
import DetalheJogo from './pages/Catalogo/DetalheJogo.jsx'
import NaoEncontrada from './pages/NaoEncontrada.jsx'
import Login from './pages/Login/Login.jsx'
export default function App() {
  return <Routes><Route element={<Layout />}><Route index element={<Catalogo />} /><Route path="jogos/:id" element={<DetalheJogo />} /><Route path="login" element={<Login />} /><Route path="*" element={<NaoEncontrada />} /></Route></Routes>
}
