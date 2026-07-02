import { createContext, useContext, useMemo, useState, type ReactNode } from 'react'
import { getToken, login as apiLogin, logout as apiLogout } from '../api/client'

type AuthContextValue = {
  isAuthenticated: boolean
  login: (email: string, password: string) => Promise<void>
  logout: () => void
}

const AuthContext = createContext<AuthContextValue | null>(null)

export function AuthProvider({ children }: { children: ReactNode }) {
  const [token, setTokenState] = useState<string | null>(() => getToken())

  const value = useMemo<AuthContextValue>(
    () => ({
      isAuthenticated: Boolean(token),
      login: async (email, password) => {
        await apiLogin(email, password)
        setTokenState(getToken())
      },
      logout: () => {
        apiLogout()
        setTokenState(null)
      },
    }),
    [token],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth() {
  const context = useContext(AuthContext)
  if (!context) {
    throw new Error('useAuth must be used within AuthProvider')
  }
  return context
}
