import { Navigate } from 'react-router-dom'
import { useAuth } from '@/context/AuthContext'
import ForbiddenPage from '@/pages/ForbiddenPage'

// Protege rutas del frontend. Es solo una capa de experiencia de usuario:
// la seguridad real está en la API (que valida JWT, rol y menús en cada petición).
export default function ProtectedRoute({ children, adminOnly = false }) {
  const { user, isAdmin } = useAuth()

  if (!user) return <Navigate to="/login" replace />
  if (adminOnly && !isAdmin) return <ForbiddenPage message="Esta sección es solo para administradores." />

  return children
}
