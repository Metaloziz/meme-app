import { useState, type DragEvent, type FormEvent } from 'react'
import { createMeme } from '../api/client'
import { Modal } from './ui/Modal'

type AddMemeFormProps = {
  onClose: () => void
  onCreated: () => void
}

export function AddMemeForm({ onClose, onCreated }: AddMemeFormProps) {
  const [title, setTitle] = useState('')
  const [description, setDescription] = useState('')
  const [year, setYear] = useState(String(new Date().getFullYear()))
  const [image, setImage] = useState<File | null>(null)
  const [previewUrl, setPreviewUrl] = useState<string | null>(null)
  const [error, setError] = useState<string | null>(null)
  const [loading, setLoading] = useState(false)

  function handleFile(file: File | null) {
    if (previewUrl) {
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

    if (!image) {
      setError('Выберите изображение')
      return
    }

    setLoading(true)
    setError(null)

    const formData = new FormData()
    formData.append('title', title.trim())
    formData.append('description', description.trim())
    formData.append('year', year)
    formData.append('image', image)

    try {
      await createMeme(formData)
      onCreated()
      onClose()
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Не удалось добавить мем')
    } finally {
      setLoading(false)
    }
  }

  return (
    <Modal title="Добавить мем" onClose={onClose} wide>
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
          {previewUrl ? (
            <img src={previewUrl} alt="Превью" className="dropzone__preview" />
          ) : (
            <p>Перетащите изображение сюда или выберите файл</p>
          )}
          <input
            type="file"
            accept="image/jpeg,image/png,image/webp"
            onChange={(e) => handleFile(e.target.files?.[0] ?? null)}
            required={!image}
          />
        </div>
        {error && <p className="form-error">{error}</p>}
        <div className="modal-actions">
          <button type="button" className="btn-secondary" onClick={onClose}>
            Отмена
          </button>
          <button type="submit" className="btn-primary" disabled={loading}>
            {loading ? 'Сохранение...' : 'Добавить'}
          </button>
        </div>
      </form>
    </Modal>
  )
}
