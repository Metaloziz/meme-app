import { useState, type FormEvent } from 'react'
import { createMeme } from '../api/client'

type AddMemeFormProps = {
  onCreated: () => void
}

export function AddMemeForm({ onCreated }: AddMemeFormProps) {
  const [title, setTitle] = useState('')
  const [description, setDescription] = useState('')
  const [year, setYear] = useState(String(new Date().getFullYear()))
  const [image, setImage] = useState<File | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [loading, setLoading] = useState(false)
  const [success, setSuccess] = useState(false)

  async function handleSubmit(event: FormEvent) {
    event.preventDefault()

    if (!image) {
      setError('Выберите изображение')
      return
    }

    setLoading(true)
    setError(null)
    setSuccess(false)

    const formData = new FormData()
    formData.append('title', title.trim())
    formData.append('description', description.trim())
    formData.append('year', year)
    formData.append('image', image)

    try {
      await createMeme(formData)
      setTitle('')
      setDescription('')
      setYear(String(new Date().getFullYear()))
      setImage(null)
      setSuccess(true)
      onCreated()
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Не удалось добавить мем')
    } finally {
      setLoading(false)
    }
  }

  return (
    <section className="add-meme-form">
      <h2>Добавить мем</h2>
      <form onSubmit={handleSubmit}>
        <label>
          Название
          <input value={title} onChange={(e) => setTitle(e.target.value)} required maxLength={120} />
        </label>
        <label>
          Описание
          <textarea
            value={description}
            onChange={(e) => setDescription(e.target.value)}
            required
            rows={3}
            maxLength={500}
          />
        </label>
        <label>
          Год
          <input
            type="number"
            value={year}
            onChange={(e) => setYear(e.target.value)}
            min={1990}
            max={2100}
          />
        </label>
        <label>
          Изображение (JPEG, PNG, WebP, до 2 MB)
          <input
            type="file"
            accept="image/jpeg,image/png,image/webp"
            onChange={(e) => setImage(e.target.files?.[0] ?? null)}
            required
          />
        </label>
        {error && <p className="form-error">{error}</p>}
        {success && <p className="form-success">Мем успешно добавлен!</p>}
        <button type="submit" className="btn-primary" disabled={loading}>
          {loading ? 'Сохранение...' : 'Добавить мем'}
        </button>
      </form>
    </section>
  )
}
