import type { Meme } from '../types/meme'
import { MemeCard } from './MemeCard'

type MemeGridProps = {
  memes: Meme[]
  imageVersions?: Record<number, number>
  canManage?: boolean
  onEdit?: (meme: Meme) => void
  onDelete?: (meme: Meme) => void
  isSearch?: boolean
}

export function MemeGrid({ memes, imageVersions, canManage, onEdit, onDelete, isSearch }: MemeGridProps) {
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
          imageVersion={imageVersions?.[meme.id]}
          canManage={canManage}
          onEdit={onEdit}
          onDelete={onDelete}
        />
      ))}
    </section>
  )
}
