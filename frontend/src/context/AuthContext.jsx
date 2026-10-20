import { createContext, useCallback, useContext, useEffect, useState } from 'react'
import { api, tokenStorage } from '@/services/api'
import { ROLES } from '@/lib/validations'

// Estado global de sesión: usuario autenticado + sus menús asignados.
const AuthContext = createContext(null)
const USER_KEY = 'lockton_user'

export function AuthProvider({ children }) {
  const [user, setUser] = useState(() => {
    const saved = sessionStorage.getItem(USER_KEY)
    return saved && tokenStorage.get() ? JSON.parse(saved) : null
  })
  const [menus, setMenus] = useState([])
  const [loadingMenus, setLoadingMenus] = useState(true)

  // Los menús SIEMPRE se piden a la API (fuente de verdad), nunca se inventan en el cliente.
  const loadMenus = useCallback(async (currentUser) => {
    if (!currentUser) {
      setMenus([])
      setLoadingMenus(false)
      return
    }
    setLoadingMenus(true)
    try {
      setMenus(await api.getUserMenus(currentUser.id))
    } finally {
      setLoadingMenus(false)
    }
  }, [])

  useEffect(() => {
    loadMenus(user)
  }, [user, loadMenus])

  const login = async (nombreUsuario, contrasena) => {
    const result = await api.login(nombreUsuario, contrasena) // { token, expiraEn, usuario }
    tokenStorage.set(result.token)
    sessionStorage.setItem(USER_KEY, JSON.stringify(result.usuario))
    setUser(result.usuario)
  }

  const logout = () => {
    tokenStorage.clear()
    sessionStorage.removeItem(USER_KEY)
    setUser(null)
  }

  const value = {
    user,
    menus,
    loadingMenus,
    isAdmin: user?.rol === ROLES.ADMIN,
    login,
    logout,
    refreshMenus: () => loadMenus(user),
  }

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export const useAuth = () => useContext(AuthContext)
