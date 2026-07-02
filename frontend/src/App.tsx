import { useCallback, useEffect, useState } from 'react'
import { fetchMemes } from './api/client'
import { AddMemeForm } from './components/AddMemeForm'
import { LoginForm } from './components/LoginForm'
import { MemeGrid } from './components/MemeGrid'
import { SearchBar } from './components/SearchBar'
import { useAuth } from './context/AuthContext'
import type { Meme } from './types/meme'
import './App.css'

function App() {
  const { isAuthenticated, logout } = useAuth()
  const [memes, setMemes] = useState<Meme[]>([])
  const [query, setQuery] = useState('')
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [showLogin, setShowLogin] = useState(false)
  const [refreshKey, setRefreshKey] = useState(0)

  const loadMemes = useCallback(async (searchQuery: string, signal: AbortSignal) => {
    try {
      setLoading(true)
      setError(null)
      const data = await fetchMemes(searchQuery)
      if (!signal.aborted) {
        setMemes(data)
      }
    } catch {
      if (!signal.aborted) {
        setError('API недоступен. Проверьте, что бэкенд запущен.')
      }
    } finally {
      if (!signal.aborted) {
        setLoading(false)
      }
    }
  }, [])

  useEffect(() => {
    const controller = new AbortController()
    const timeout = setTimeout(() => {
      void loadMemes(query, controller.signal)
    }, 300)

    return () => {
      controller.abort()
      clearTimeout(timeout)
    }
  }, [query, refreshKey, loadMemes])

  return (
    <div className="app">
      <header className="hero">
        <div className="hero-top">
          <div>
            <p className="eyebrow">Top Memes Archive</p>
            <h1>Самые популярные мемы интернета</h1>
          </div>
          <div className="auth-actions">
            {isAuthenticated ? (
              <button type="button" className="btn-secondary" onClick={logout}>
                Выйти
              </button>
            ) : (
              <button type="button" className="btn-primary" onClick={() => setShowLogin(true)}>
                Войти
              </button>
            )}
          </div>
        </div>
        <p className="subtitle">
          Каталог легендарных мемов с рейтингом популярности. Данные приходят из C# API.
        </p>
        <SearchBar value={query} onChange={setQuery} />
      </header>

      {isAuthenticated && (
        <AddMemeForm onCreated={() => setRefreshKey((value) => value + 1)} />
      )}

      <main>
        {loading && <p className="status">Загрузка...</p>}
        {error && <p className="status error">{error}</p>}
        {!loading && !error && <MemeGrid memes={memes} />}
      </main>

      {showLogin && <LoginForm onClose={() => setShowLogin(false)} />}
    </div>
  )
}

export default App
