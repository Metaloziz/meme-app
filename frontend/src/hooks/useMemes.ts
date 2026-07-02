import { useEffect, useState } from 'react'
import { fetchMemes } from '../api/client'
import type { Meme } from '../types/meme'

export function useMemes(query: string, refreshKey = 0) {
  const [memes, setMemes] = useState<Meme[]>([])
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    const controller = new AbortController()
    const timeout = setTimeout(async () => {
      try {
        setLoading(true)
        setError(null)
        const data = await fetchMemes(query)
        if (!controller.signal.aborted) {
          setMemes(data)
        }
      } catch {
        if (!controller.signal.aborted) {
          setError('API недоступен. Проверьте, что бэкенд запущен.')
        }
      } finally {
        if (!controller.signal.aborted) {
          setLoading(false)
        }
      }
    }, 300)

    return () => {
      controller.abort()
      clearTimeout(timeout)
    }
  }, [query, refreshKey])

  return { memes, loading, error }
}
