export default function Paginacao({ pagina, totalPaginas, aoMudar }) {
  if (totalPaginas <= 1) return null
  const numeros = [...new Set([1, ...Array.from({ length: 5 }, (_, i) => pagina + i - 2), totalPaginas])].filter(n => n >= 1 && n <= totalPaginas).sort((a,b)=>a-b)
  return <nav aria-label="Páginas do catálogo" className="mt-4"><ul className="pagination flex-wrap">
    <li className="page-item"><button className="page-link" disabled={pagina <= 1} onClick={() => aoMudar(pagina - 1)}>Anterior</button></li>
    {numeros.map(n => <li key={n} className={'page-item' + (n === pagina ? ' active' : '')}><button className="page-link" aria-current={n === pagina ? 'page' : undefined} onClick={() => aoMudar(n)}>{n}</button></li>)}
    <li className="page-item"><button className="page-link" disabled={pagina >= totalPaginas} onClick={() => aoMudar(pagina + 1)}>Próxima</button></li>
  </ul></nav>
}
