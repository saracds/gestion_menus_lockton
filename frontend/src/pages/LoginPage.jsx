import { useState } from 'react'
import { Navigate, useNavigate } from 'react-router-dom'
import { ShieldCheck } from 'lucide-react'
import { useAuth } from '@/context/AuthContext'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Alert } from '@/components/ui/alert'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import FieldError from '@/components/FieldError'
import { validateLogin } from '@/lib/validations'

export default function LoginPage() {
  const { user, login } = useAuth()
  const navigate = useNavigate()
  const [form, setForm] = useState({ nombreUsuario: '', contrasena: '' })
  const [error, setError] = useState('')
  const [fieldErrors, setFieldErrors] = useState({})
  const [loading, setLoading] = useState(false)

  if (user) return <Navigate to="/" replace />

  const handleSubmit = async (e) => {
    e.preventDefault()
    setError('')
    const errors = validateLogin(form)
    setFieldErrors(errors)
    if (Object.keys(errors).length > 0) return

    setLoading(true)
    try {
      await login(form.nombreUsuario, form.contrasena) // las credenciales se validan en la API
      navigate('/')
    } catch (err) {
      setError(err.message)
    } finally {
      setLoading(false)
    }
  }

  return (
    <div className="flex min-h-screen items-center justify-center p-4">
      <Card className="w-full max-w-sm">
        <CardHeader className="items-center text-center">
          <ShieldCheck className="mb-2 h-10 w-10 text-primary" />
          <CardTitle className="text-2xl">Iniciar sesión</CardTitle>
          <CardDescription>Ingrese su usuario y contraseña</CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} noValidate className="space-y-4">
            <div className="space-y-2">
              <Label htmlFor="username">Nombre de usuario</Label>
              <Input
                id="username"
                autoComplete="username"
                value={form.nombreUsuario}
                onChange={(e) => setForm({ ...form, nombreUsuario: e.target.value })}
              />
              <FieldError message={fieldErrors.nombreUsuario} />
            </div>
            <div className="space-y-2">
              <Label htmlFor="password">Contraseña</Label>
              <Input
                id="password"
                type="password"
                autoComplete="current-password"
                value={form.contrasena}
                onChange={(e) => setForm({ ...form, contrasena: e.target.value })}
              />
              <FieldError message={fieldErrors.contrasena} />
            </div>
            {error && <Alert variant="destructive">{error}</Alert>}
            <Button type="submit" className="w-full" disabled={loading}>
              {loading ? 'Validando...' : 'Ingresar'}
            </Button>
          </form>
        </CardContent>
      </Card>
    </div>
  )
}
