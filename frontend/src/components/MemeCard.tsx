import { useState } from 'react'
import type { Meme } from '../types/meme'
import { resolveImageUrl } from '../api/client'

type MemeCardProps = {
  meme: Meme
  imageCacheKey?: number
  canManage?: boolean
  onEdit?: (meme: Meme) => void
  onDelete?: (meme: Meme) => void
}

export function MemeCard({ meme, imageCacheKey, canManage, onEdit, onDelete }: MemeCardProps) {
  const [loaded, setLoaded] = useState(false)

  return (
    <article className="meme-card">
      <div className="meme-card__image-wrap">
        {!loaded && <div className="meme-card__skeleton" />}
        <img
          src={resolveImageUrl(meme.id, imageCacheKey)}
          alt={meme.title}
          loading="lazy"
          onLoad={() => setLoaded(true)}
          className={loaded ? 'is-loaded' : ''}
        />
        {canManage && (
          <div className="meme-card__actions">
            {onEdit && (
              <button
                type="button"
                className="meme-card__action"
                onClick={() => onEdit(meme)}
                aria-label={`Редактировать ${meme.title}`}
                title="Редактировать"
              >
                ✎
              </button>
            )}
            {onDelete && (
              <button
                type="button"
                className="meme-card__action meme-card__action--danger"
                onClick={() => onDelete(meme)}
                aria-label={`Удалить ${meme.title}`}
                title="Удалить"
              >
                🗑
              </button>
            )}
          </div>
        )}
      </div>
      <div className="meme-card__body">
        <div className="meme-card__meta">
          <span>{meme.year}</span>
          <span className="meme-card__score">★ {meme.popularityScore}</span>
        </div>
        <h2>{meme.title}</h2>
        <p>{meme.description}</p>
      </div>
    </article>
  )
}
