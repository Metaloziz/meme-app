import type { Meme } from '../types/meme'
import { MemeCard } from './MemeCard'

type MemeGridProps = {
  memes: Meme[]
}

export function MemeGrid({ memes }: MemeGridProps) {
  if (memes.length === 0) {
    return <p className="empty-state">Ничего не найдено. Попробуйте другой запрос.</p>
  }

  return (
    <section className="meme-grid">
      {memes.map((meme) => (
        <MemeCard key={meme.id} meme={meme} />
      ))}
    </section>
  )
}
