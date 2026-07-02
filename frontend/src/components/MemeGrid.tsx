import type { Meme } from '../types/meme'
import { MemeCard } from './MemeCard'

type MemeGridProps = {
  memes: Meme[]
  imageCacheKey?: number
  canManage?: boolean
  onEdit?: (meme: Meme) => void
  onDelete?: (meme: Meme) => void
  isSearch?: boolean
}

export function MemeGrid({ memes, imageCacheKey, canManage, onEdit, onDelete, isSearch }: MemeGridProps) {
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
          imageCacheKey={imageCacheKey}
          canManage={canManage}
          onEdit={onEdit}
          onDelete={onDelete}
        />
      ))}
    </section>
  )
}
