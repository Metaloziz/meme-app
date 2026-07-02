import { useState } from 'react'
import { deleteMeme } from './api/client'
import { AddMemeForm } from './components/AddMemeForm'
import { Header } from './components/Header'
import { LoginForm } from './components/LoginForm'
import { MemeGrid } from './components/MemeGrid'
import { ConfirmDialog } from './components/ui/ConfirmDialog'
import { Spinner } from './components/ui/Spinner'
import { Toast } from './components/ui/Toast'
import { useAuth } from './context/AuthContext'
import { useMemes } from './hooks/useMemes'
import type { Meme } from './types/meme'
import './App.css'

function App() {
  const { isAuthenticated } = useAuth()
  const [query, setQuery] = useState('')
  const [refreshKey, setRefreshKey] = useState(0)
  const [showLogin, setShowLogin] = useState(false)
  const [showAdd, setShowAdd] = useState(false)
  const [memeToDelete, setMemeToDelete] = useState<Meme | null>(null)
  const [deleting, setDeleting] = useState(false)
  const [toast, setToast] = useState<string | null>(null)

  const { memes, loading, error } = useMemes(query, refreshKey)

  function refresh() {
    setRefreshKey((value) => value + 1)
  }

  function showToast(message: string) {
    setToast(message)
    setTimeout(() => setToast(null), 3000)
  }

  async function confirmDelete() {
    if (!memeToDelete) {
      return
    }

    setDeleting(true)
    try {
      await deleteMeme(memeToDelete.id)
      setMemeToDelete(null)
      refresh()
      showToast('Мем удалён')
    } catch (err) {
      showToast(err instanceof Error ? err.message : 'Ошибка удаления')
    } finally {
      setDeleting(false)
    }
  }

  return (
    <div className="app">
      <Header
        memeCount={memes.length}
        query={query}
        onQueryChange={setQuery}
        onLoginClick={() => setShowLogin(true)}
        onAddClick={() => setShowAdd(true)}
      />

      <main>
        {loading && (
          <div className="loading-state">
            <Spinner />
            <p>Загрузка мемов...</p>
          </div>
        )}
        {error && <p className="status error">{error}</p>}
        {!loading && !error && (
          <MemeGrid
            memes={memes}
            canDelete={isAuthenticated}
            onDelete={setMemeToDelete}
            isSearch={Boolean(query.trim())}
          />
        )}
      </main>

      {showLogin && <LoginForm onClose={() => setShowLogin(false)} />}
      {showAdd && (
        <AddMemeForm
          onClose={() => setShowAdd(false)}
          onCreated={() => {
            refresh()
            showToast('Мем добавлен')
          }}
        />
      )}
      {memeToDelete && (
        <ConfirmDialog
          title="Удалить мем?"
          message={`«${memeToDelete.title}» будет удалён без возможности восстановления.`}
          onConfirm={confirmDelete}
          onCancel={() => setMemeToDelete(null)}
          loading={deleting}
        />
      )}
      {toast && <Toast message={toast} />}
    </div>
  )
}

export default App
