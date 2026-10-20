import { Route, Routes } from 'react-router-dom'
import Layout from '@/components/Layout'
import ProtectedRoute from '@/components/ProtectedRoute'
import LoginPage from '@/pages/LoginPage'
import HomePage from '@/pages/HomePage'
import UsersPage from '@/pages/UsersPage'
import AssignMenusPage from '@/pages/AssignMenusPage'
import ModulePage from '@/pages/ModulePage'
import NotFoundPage from '@/pages/NotFoundPage'

export default function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />

      {/* Todo lo de adentro exige sesión iniciada */}
      <Route
        element={
          <ProtectedRoute>
            <Layout />
          </ProtectedRoute>
        }
      >
        <Route index element={<HomePage />} />
        <Route path="modulo/:route" element={<ModulePage />} />
        <Route path="usuarios" element={<ProtectedRoute adminOnly><UsersPage /></ProtectedRoute>} />
        <Route path="asignacion-menus" element={<ProtectedRoute adminOnly><AssignMenusPage /></ProtectedRoute>} />
        <Route path="*" element={<NotFoundPage />} />
      </Route>
    </Routes>
  )
}
