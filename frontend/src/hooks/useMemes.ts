import { useCallback, useEffect, useState } from 'react'
import { fetchMemes } from '../api/client'
import type { Meme } from '../types/meme'

export function useMemes(query: string) {
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
  }, [query])

  const removeMeme = useCallback((id: number) => {
    setMemes((current) => current.filter((meme) => meme.id !== id))
  }, [])

  const upsertMeme = useCallback((meme: Meme) => {
    setMemes((current) => {
      const index = current.findIndex((item) => item.id === meme.id)
      if (index === -1) {
        return [meme, ...current]
      }

      const next = [...current]
      next[index] = meme
      return next
    })
  }, [])

  const reload = useCallback(async () => {
    try {
      setError(null)
      const data = await fetchMemes(query)
      setMemes(data)
    } catch {
      setError('API недоступен. Проверьте, что бэкенд запущен.')
    }
  }, [query])

  return { memes, loading, error, removeMeme, upsertMeme, reload }
}
