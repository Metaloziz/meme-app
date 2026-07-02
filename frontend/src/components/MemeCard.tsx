import type { Meme } from '../types/meme'
import { resolveImageUrl } from '../api/client'

type MemeCardProps = {
  meme: Meme
}

export function MemeCard({ meme }: MemeCardProps) {
  return (
    <article className="meme-card">
      <div className="meme-card__image-wrap">
        <img src={resolveImageUrl(meme.id)} alt={meme.title} loading="lazy" />
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
