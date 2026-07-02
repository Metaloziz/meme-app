type SearchBarProps = {
  value: string
  onChange: (value: string) => void
}

export function SearchBar({ value, onChange }: SearchBarProps) {
  return (
    <label className="search-bar">
      <span>Поиск мемов</span>
      <input
        type="search"
        placeholder="Doge, Drake, Stonks..."
        value={value}
        onChange={(event) => onChange(event.target.value)}
      />
    </label>
  )
}
