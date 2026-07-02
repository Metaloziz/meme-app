import type { Meme } from '../types/meme'
import { MemeCard } from './MemeCard'

type MemeGridProps = {
  memes: Meme[]
  canDelete?: boolean
  onDelete?: (meme: Meme) => void
  isSearch?: boolean
}

export function MemeGrid({ memes, canDelete, onDelete, isSearch }: MemeGridProps) {
  if (memes.length === 0) {
    return (
      <div className="empty-state">
        <h3>{isSearch ? 'Ничего не найдено' : 'Каталог пуст'}</h3>
        <p>
          {isSearch
            ? 'Попробуйте другой запрос — например, Doge или Drake.'
            : 'Пока нет мемов. Войдите как админ и добавьте первый.'}
        </p>
      </div>
    )
  }

  return (
    <section className="meme-grid">
      {memes.map((meme) => (
        <MemeCard
          key={meme.id}
          meme={meme}
          canDelete={canDelete}
          onDelete={onDelete}
        />
      ))}
    </section>
  )
}
