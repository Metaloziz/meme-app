import { SearchBar } from './SearchBar'
import { useAuth } from '../context/AuthContext'

type HeaderProps = {
  memeCount: number
  query: string
  onQueryChange: (value: string) => void
  onLoginClick: () => void
  onAddClick: () => void
}

export function Header({
  memeCount,
  query,
  onQueryChange,
  onLoginClick,
  onAddClick,
}: HeaderProps) {
  const { isAuthenticated, logout } = useAuth()

  return (
    <header className="hero">
      <div className="hero-top">
        <div>
          <p className="eyebrow">Top Memes Archive</p>
          <h1>Самые популярные мемы интернета</h1>
          <p className="meme-count">{memeCount} мемов в каталоге</p>
        </div>
        <div className="auth-actions">
          {isAuthenticated ? (
            <>
              <button type="button" className="btn-primary" onClick={onAddClick}>
                + Добавить мем
              </button>
              <button type="button" className="btn-secondary" onClick={logout}>
                Выйти
              </button>
            </>
          ) : (
            <button type="button" className="btn-primary" onClick={onLoginClick}>
              Войти
            </button>
          )}
        </div>
      </div>
      <p className="subtitle">
        Каталог легендарных мемов с рейтингом популярности. Ищите, смотрите и добавляйте новые.
      </p>
      <SearchBar value={query} onChange={onQueryChange} />
    </header>
  )
}
