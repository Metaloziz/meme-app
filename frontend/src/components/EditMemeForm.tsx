import { useState, type DragEvent, type FormEvent } from 'react'
import { updateMeme, resolveImageUrl } from '../api/client'
import { Modal } from './ui/Modal'
import type { Meme } from '../types/meme'

type EditMemeFormProps = {
  meme: Meme
  imageCacheKey: number
  onClose: () => void
  onUpdated: () => void
}

export function EditMemeForm({ meme, imageCacheKey, onClose, onUpdated }: EditMemeFormProps) {
  const [title, setTitle] = useState(meme.title)
  const [description, setDescription] = useState(meme.description)
  const [year, setYear] = useState(String(meme.year))
  const [image, setImage] = useState<File | null>(null)
  const [previewUrl, setPreviewUrl] = useState<string | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [loading, setLoading] = useState(false)

  const currentImageUrl = `${resolveImageUrl(meme.id)}?v=${imageCacheKey}`

  function handleFile(file: File | null) {
    if (previewUrl?.startsWith('blob:')) {
      URL.revokeObjectURL(previewUrl)
    }

    setImage(file)
    setPreviewUrl(file ? URL.createObjectURL(file) : null)
  }

  function handleDrop(event: DragEvent) {
    event.preventDefault()
    const file = event.dataTransfer.files[0]
    if (file) {
      handleFile(file)
    }
  }

  async function handleSubmit(event: FormEvent) {
    event.preventDefault()
    setLoading(true)
    setError(null)

    const formData = new FormData()
    formData.append('title', title.trim())
    formData.append('description', description.trim())
    formData.append('year', year)
    if (image) {
      formData.append('image', image)
    }

    try {
      await updateMeme(meme.id, formData)
      onUpdated()
      onClose()
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Не удалось сохранить изменения')
    } finally {
      setLoading(false)
    }
  }

  return (
    <Modal title="Редактировать мем" onClose={onClose} wide>
      <form className="form" onSubmit={handleSubmit}>
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
        <div
          className="dropzone"
          onDragOver={(e) => e.preventDefault()}
          onDrop={handleDrop}
        >
          <img
            src={previewUrl ?? currentImageUrl}
            alt="Превью"
            className="dropzone__preview"
          />
          <p className="dropzone__hint">Новое изображение необязательно — оставьте текущее или выберите файл</p>
          <input
            type="file"
            accept="image/jpeg,image/png,image/webp"
            onChange={(e) => handleFile(e.target.files?.[0] ?? null)}
          />
        </div>
        {error && <p className="form-error">{error}</p>}
        <div className="modal-actions">
          <button type="button" className="btn-secondary" onClick={onClose}>
            Отмена
          </button>
          <button type="submit" className="btn-primary" disabled={loading}>
            {loading ? 'Сохранение...' : 'Сохранить'}
          </button>
        </div>
      </form>
    </Modal>
  )
}
