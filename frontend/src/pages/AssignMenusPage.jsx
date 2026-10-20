import { useEffect, useState } from 'react'
import { Save } from 'lucide-react'
import { api } from '@/services/api'
import { useAuth } from '@/context/AuthContext'
import { Button } from '@/components/ui/button'
import { Label } from '@/components/ui/label'
import { Alert } from '@/components/ui/alert'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'

export default function AssignMenusPage() {
  const { user: currentUser, refreshMenus } = useAuth()
  const [users, setUsers] = useState([])
  const [menus, setMenus] = useState([])
  const [selectedUserId, setSelectedUserId] = useState('')
  const [selectedMenuIds, setSelectedMenuIds] = useState([])
  const [message, setMessage] = useState(null)
  const [saving, setSaving] = useState(false)

  // Carga inicial: usuarios + catálogo de menús
  useEffect(() => {
    Promise.all([api.getUsers(), api.getMenus()])
      .then(([u, m]) => {
        setUsers(u)
        setMenus(m)
      })
      .catch((err) => setMessage({ type: 'destructive', text: err.message }))
  }, [])

  // Al elegir un usuario, se marcan los menús que ya tiene
  useEffect(() => {
    setMessage(null)
    if (!selectedUserId) return setSelectedMenuIds([])
    api
      .getUserMenus(selectedUserId)
      .then((assigned) => setSelectedMenuIds(assigned.map((m) => m.id)))
      .catch((err) => setMessage({ type: 'destructive', text: err.message }))
  }, [selectedUserId])

  const toggleMenu = (menuId) =>
    setSelectedMenuIds((prev) => (prev.includes(menuId) ? prev.filter((id) => id !== menuId) : [...prev, menuId]))

  const handleSave = async () => {
    setSaving(true)
    setMessage(null)
    try {
      await api.assignUserMenus(selectedUserId, selectedMenuIds)
      setMessage({ type: 'success', text: 'Menús actualizados correctamente.' })
      // Si el admin se modificó a sí mismo, se refresca su menú lateral
      if (Number(selectedUserId) === currentUser.id) refreshMenus()
    } catch (err) {
      setMessage({ type: 'destructive', text: err.message })
    } finally {
      setSaving(false)
    }
  }

  return (
    <div className="space-y-6">
      <h1 className="text-2xl font-bold">Asignación de menús</h1>

      <Card className="max-w-2xl">
        <CardHeader>
          <CardTitle>Menús por usuario</CardTitle>
          <CardDescription>Seleccione un usuario y marque o desmarque los menús que puede usar.</CardDescription>
        </CardHeader>
        <CardContent className="space-y-6">
          <div className="space-y-2">
            <Label htmlFor="user">Usuario</Label>
            <select
              id="user"
              value={selectedUserId}
              onChange={(e) => setSelectedUserId(e.target.value)}
              className="flex h-9 w-full rounded-md border border-input bg-transparent px-3 text-sm shadow-sm"
            >
              <option value="">-- Seleccione --</option>
              {users.map((u) => (
                <option key={u.id} value={u.id}>
                  {u.nombreCompleto} (@{u.nombreUsuario})
                </option>
              ))}
            </select>
          </div>

          {selectedUserId && (
            <div className="space-y-3">
              <Label>Menús disponibles</Label>
              <div className="grid gap-2 sm:grid-cols-2">
                {menus.map((menu) => (
                  <label
                    key={menu.id}
                    className="flex cursor-pointer items-start gap-3 rounded-md border p-3 hover:bg-muted/50"
                  >
                    <input
                      type="checkbox"
                      className="mt-1 h-4 w-4 accent-slate-900"
                      checked={selectedMenuIds.includes(menu.id)}
                      onChange={() => toggleMenu(menu.id)}
                    />
                    <span>
                      <span className="block text-sm font-medium">{menu.nombre}</span>
                      <span className="block text-xs text-muted-foreground">{menu.descripcion}</span>
                    </span>
                  </label>
                ))}
              </div>
              <Button onClick={handleSave} disabled={saving}>
                <Save /> {saving ? 'Guardando...' : 'Guardar asignación'}
              </Button>
            </div>
          )}

          {message && <Alert variant={message.type}>{message.text}</Alert>}
        </CardContent>
      </Card>
    </div>
  )
}
