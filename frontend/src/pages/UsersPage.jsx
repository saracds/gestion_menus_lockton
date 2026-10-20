import { useEffect, useState } from 'react'
import { UserPlus } from 'lucide-react'
import { api } from '@/services/api'
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'
import { Label } from '@/components/ui/label'
import { Alert } from '@/components/ui/alert'
import { Badge } from '@/components/ui/badge'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from '@/components/ui/table'
import FieldError from '@/components/FieldError'
import { ROLES, validateNewUser } from '@/lib/validations'

// Mismos nombres que CrearUsuarioDto en el backend
const emptyForm = { nombreCompleto: '', nombreUsuario: '', contrasena: '', rol: ROLES.USER }

export default function UsersPage() {
  const [users, setUsers] = useState([])
  const [form, setForm] = useState(emptyForm)
  const [message, setMessage] = useState(null) // { type: 'success' | 'destructive', text }
  const [saving, setSaving] = useState(false)
  const [fieldErrors, setFieldErrors] = useState({})

  const loadUsers = () =>
    api.getUsers().then(setUsers).catch((err) => setMessage({ type: 'destructive', text: err.message }))

  useEffect(() => {
    loadUsers()
  }, [])

  const handleChange = (e) => setForm({ ...form, [e.target.name]: e.target.value })

  const handleSubmit = async (e) => {
    e.preventDefault()
    setMessage(null)
    const errors = validateNewUser(form) // validación en el cliente (mensajes en español)
    setFieldErrors(errors)
    if (Object.keys(errors).length > 0) return

    setSaving(true)
    try {
      const created = await api.createUser(form)
      setMessage({ type: 'success', text: `Usuario "${created.nombreUsuario}" creado correctamente.` })
      setForm(emptyForm)
      loadUsers()
    } catch (err) {
      setMessage({ type: 'destructive', text: err.message }) // validaciones del backend
    } finally {
      setSaving(false)
    }
  }

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold">Administración de usuarios</h1>

      <Card>
        <CardHeader>
          <CardTitle>Crear usuario</CardTitle>
          <CardDescription>La contraseña debe tener mínimo 8 caracteres, con letras y números.</CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={handleSubmit} noValidate className="grid gap-4 md:grid-cols-5 md:items-start">
            <div className="space-y-2">
              <Label htmlFor="nombreCompleto">Nombre completo</Label>
              <Input id="nombreCompleto" name="nombreCompleto" value={form.nombreCompleto} onChange={handleChange} />
              <FieldError message={fieldErrors.nombreCompleto} />
            </div>
            <div className="space-y-2">
              <Label htmlFor="nombreUsuario">Nombre de usuario</Label>
              <Input id="nombreUsuario" name="nombreUsuario" value={form.nombreUsuario} onChange={handleChange} />
              <FieldError message={fieldErrors.nombreUsuario} />
            </div>
            <div className="space-y-2">
              <Label htmlFor="contrasena">Contraseña</Label>
              <Input id="contrasena" name="contrasena" type="password" value={form.contrasena} onChange={handleChange} />
              <FieldError message={fieldErrors.contrasena} />
            </div>
            <div className="space-y-2">
              <Label htmlFor="rol">Rol</Label>
              <select
                id="rol"
                name="rol"
                value={form.rol}
                onChange={handleChange}
                className="flex h-9 w-full rounded-md border border-input bg-transparent px-3 text-sm shadow-sm"
              >
                <option value={ROLES.USER}>{ROLES.USER}</option>
                <option value={ROLES.ADMIN}>{ROLES.ADMIN}</option>
              </select>
              <FieldError message={fieldErrors.rol} />
            </div>
            <Button type="submit" disabled={saving} className="md:mt-6">
              <UserPlus /> {saving ? 'Guardando...' : 'Crear'}
            </Button>
          </form>
          {message && <Alert variant={message.type} className="mt-4">{message.text}</Alert>}
        </CardContent>
      </Card>

      <Card>
        <CardHeader>
          <CardTitle>Usuarios registrados</CardTitle>
        </CardHeader>
        <CardContent>
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Id</TableHead>
                <TableHead>Nombre</TableHead>
                <TableHead>Usuario</TableHead>
                <TableHead>Rol</TableHead>
                <TableHead>Estado</TableHead>
                <TableHead>Creado</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {users.map((u) => (
                <TableRow key={u.id}>
                  <TableCell>{u.id}</TableCell>
                  <TableCell>{u.nombreCompleto}</TableCell>
                  <TableCell>{u.nombreUsuario}</TableCell>
                  <TableCell>
                    <Badge variant={u.rol === ROLES.ADMIN ? 'default' : 'secondary'}>{u.rol}</Badge>
                  </TableCell>
                  <TableCell>
                    <Badge variant={u.activo ? 'outline' : 'destructive'}>{u.activo ? 'Activo' : 'Inactivo'}</Badge>
                  </TableCell>
                  <TableCell>{new Date(u.fechaCreacion).toLocaleDateString('es-CO')}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </CardContent>
      </Card>
    </div>
  )
}
