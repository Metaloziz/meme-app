import { useEffect, useState } from 'react'
import { fetchMemes } from './api/client'
import { MemeGrid } from './components/MemeGrid'
import { SearchBar } from './components/SearchBar'
import type { Meme } from './types/meme'
import './App.css'

function App() {
  const [memes, setMemes] = useState<Meme[]>([])
  const [query, setQuery] = useState('')
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const controller = new AbortController()
    const timeout = setTimeout(async () => {
      try {
        setLoading(true)
        setError(null)
        const data = await fetchMemes(query)
        if (!controller.signal.aborted) {
          setMemes(data)
        }
      } catch {
        if (!controller.signal.aborted) {
          setError('API недоступен. Проверьте, что бэкенд запущен.')
        }
      } finally {
        if (!controller.signal.aborted) {
          setLoading(false)
        }
      }
    }, 300)

    return () => {
      controller.abort()
      clearTimeout(timeout)
    }
  }, [query])

  return (
    <div className="app">
      <header className="hero">
        <p className="eyebrow">Top Memes Archive</p>
        <h1>Самые популярные мемы интернета</h1>
        <p className="subtitle">
          Каталог легендарных мемов с рейтингом популярности. Данные приходят из C# API.
        </p>
        <SearchBar value={query} onChange={setQuery} />
      </header>

      <main>
        {loading && <p className="status">Загрузка...</p>}
        {error && <p className="status error">{error}</p>}
        {!loading && !error && <MemeGrid memes={memes} />}
      </main>
    </div>
  )
}

export default App
