import { useEffect, useState } from 'react'
import { useParams } from 'react-router-dom'
import { CheckCircle2 } from 'lucide-react'
import { useAuth } from '@/context/AuthContext'
import { api } from '@/services/api'
import { Button } from '@/components/ui/button'
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import ForbiddenPage from '@/pages/ForbiddenPage'

// Página genérica para cualquier módulo (/modulo/polizas, /modulo/clientes, ...).
// Tiene DOS controles de acceso:
//  1. Frontend: si el menú no está asignado, ni siquiera se muestra el contenido.
//  2. Backend: el contenido viene de GET /api/modules/{route}, que responde 403 si no hay permiso.
export default function ModulePage() {
  const { route } = useParams()
  const { menus, loadingMenus } = useAuth()
  const [content, setContent] = useState(null)
  const [error, setError] = useState(null)
  const [apiProof, setApiProof] = useState(null)

  const isAssigned = menus.some((m) => m.ruta === route)

  useEffect(() => {
    setContent(null)
    setError(null)
    setApiProof(null)
    if (!loadingMenus && isAssigned) {
      api.getModule(route).then(setContent).catch(setError)
    }
  }, [route, isAssigned, loadingMenus])

  // Botón de demostración: llama a la API aunque el menú no esté asignado,
  // para evidenciar que el backend también bloquea el acceso.
  const tryApiDirectly = async () => {
    try {
      setApiProof({ status: 200, body: await api.getModule(route) })
    } catch (err) {
      setApiProof({ status: err.status, body: err.data })
    }
  }

  if (loadingMenus) return <p className="text-muted-foreground">Cargando...</p>

  if (!isAssigned || error?.status === 403) {
    return (
      <ForbiddenPage message={`No tiene asignado el módulo "${route}".`}>
        <div className="mt-6 space-y-3">
          <Button variant="outline" onClick={tryApiDirectly}>Intentar acceso directo a la API</Button>
          {apiProof && (
            <pre className="rounded-md bg-slate-900 p-4 text-left text-xs text-slate-100">
              {`GET /api/modules/${route}  →  HTTP ${apiProof.status}\n\n${JSON.stringify(apiProof.body, null, 2)}`}
            </pre>
          )}
        </div>
      </ForbiddenPage>
    )
  }

  if (error) return <p className="text-destructive">{error.message}</p>
  if (!content) return <p className="text-muted-foreground">Cargando módulo...</p>

  return (
    <Card className="max-w-2xl">
      <CardHeader>
        <CardTitle className="text-2xl">{content.nombre}</CardTitle>
        <CardDescription>{content.descripcion}</CardDescription>
      </CardHeader>
      <CardContent className="flex items-center gap-2 text-emerald-700">
        <CheckCircle2 className="h-5 w-5" /> {content.mensaje}
      </CardContent>
    </Card>
  )
}
