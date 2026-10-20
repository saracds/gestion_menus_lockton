import { ShieldX } from 'lucide-react'

export default function ForbiddenPage({ message = 'No tiene permisos para acceder a este recurso.', children }) {
  return (
    <div className="mx-auto mt-16 max-w-lg text-center">
      <ShieldX className="mx-auto mb-4 h-14 w-14 text-destructive" />
      <h1 className="text-2xl font-bold">403 · Acceso denegado</h1>
      <p className="mt-2 text-muted-foreground">{message}</p>
      {children}
    </div>
  )
}
