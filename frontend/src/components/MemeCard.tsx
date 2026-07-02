import { useState } from 'react'
import type { Meme } from '../types/meme'
import { resolveImageUrl } from '../api/client'

type MemeCardProps = {
  meme: Meme
  canDelete?: boolean
  onDelete?: (meme: Meme) => void
}

export function MemeCard({ meme, canDelete, onDelete }: MemeCardProps) {
  const [loaded, setLoaded] = useState(false)

  return (
    <article className="meme-card">
      <div className="meme-card__image-wrap">
        {!loaded && <div className="meme-card__skeleton" />}
        <img
          src={resolveImageUrl(meme.id)}
          alt={meme.title}
          loading="lazy"
          onLoad={() => setLoaded(true)}
          className={loaded ? 'is-loaded' : ''}
        />
        {canDelete && onDelete && (
          <button
            type="button"
            className="meme-card__delete"
            onClick={() => onDelete(meme)}
            aria-label={`Удалить ${meme.title}`}
            title="Удалить мем"
          >
            🗑
          </button>
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
