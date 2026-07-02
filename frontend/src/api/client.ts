import type { Meme } from '../types/meme'

const API_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5202'

export async function fetchMemes(query?: string): Promise<Meme[]> {
  const params = new URLSearchParams()
  if (query?.trim()) {
    params.set('q', query.trim())
  }

  const suffix = params.toString() ? `?${params.toString()}` : ''
  const response = await fetch(`${API_URL}/api/memes${suffix}`)

  if (!response.ok) {
    throw new Error('Не удалось загрузить мемы')
  }

  const data = await response.json()
  return data.map((item: Record<string, unknown>) => ({
    id: item.id as number,
    title: item.title as string,
    description: item.description as string,
    imageUrl: item.imageUrl as string,
    year: item.year as number,
    popularityScore: item.popularityScore as number,
  }))
}

export function resolveImageUrl(imageUrl: string): string {
  const base = import.meta.env.BASE_URL
  return `${base}${imageUrl.replace(/^\//, '')}`
}
