import type { Meme } from '../types/meme'

const API_URL = import.meta.env.VITE_API_URL ?? 'http://localhost:5202'
const TOKEN_KEY = 'meme_app_token'

export function getApiUrl(): string {
  return API_URL
}

export function getToken(): string | null {
  return localStorage.getItem(TOKEN_KEY)
}

export function setToken(token: string | null): void {
  if (token) {
    localStorage.setItem(TOKEN_KEY, token)
  } else {
    localStorage.removeItem(TOKEN_KEY)
  }
}

export async function login(email: string, password: string): Promise<void> {
  const response = await fetch(`${API_URL}/api/auth/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, password }),
  })

  if (!response.ok) {
    const body = await response.json().catch(() => ({}))
    throw new Error((body as { message?: string }).message ?? 'Ошибка входа')
  }

  const data = await response.json()
  setToken(data.token)
}

export function logout(): void {
  setToken(null)
}

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

  return response.json()
}

export async function createMeme(formData: FormData): Promise<Meme> {
  const token = getToken()
  if (!token) {
    throw new Error('Требуется авторизация')
  }

  const response = await fetch(`${API_URL}/api/memes`, {
    method: 'POST',
    headers: { Authorization: `Bearer ${token}` },
    body: formData,
  })

  if (!response.ok) {
    const body = await response.json().catch(() => ({}))
    throw new Error((body as { message?: string }).message ?? 'Не удалось добавить мем')
  }

  return response.json()
}

export async function deleteMeme(id: number): Promise<void> {
  const token = getToken()
  if (!token) {
    throw new Error('Требуется авторизация')
  }

  const response = await fetch(`${API_URL}/api/memes/${id}`, {
    method: 'DELETE',
    headers: { Authorization: `Bearer ${token}` },
  })

  if (!response.ok && response.status !== 204) {
    const body = await response.json().catch(() => ({}))
    throw new Error((body as { message?: string }).message ?? 'Не удалось удалить мем')
  }
}

export async function updateMeme(id: number, formData: FormData): Promise<Meme> {
  const token = getToken()
  if (!token) {
    throw new Error('Требуется авторизация')
  }

  const response = await fetch(`${API_URL}/api/memes/${id}`, {
    method: 'PUT',
    headers: { Authorization: `Bearer ${token}` },
    body: formData,
  })

  if (!response.ok) {
    const body = await response.json().catch(() => ({}))
    throw new Error((body as { message?: string }).message ?? 'Не удалось обновить мем')
  }

  return response.json()
}

export function resolveImageUrl(memeId: number, cacheKey?: number): string {
  const base = `${API_URL}/api/memes/${memeId}/image`
  return cacheKey ? `${base}?v=${cacheKey}` : base
}
