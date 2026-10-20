import { Link } from 'react-router-dom'
import { useAuth } from '@/context/AuthContext'
import { Card, CardDescription, CardHeader, CardTitle } from '@/components/ui/card'
import { getMenuIcon } from '@/components/menuIcons'

export default function HomePage() {
  const { user, menus } = useAuth()

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-2xl font-bold">Bienvenido(a), {user.nombreCompleto}</h1>
        <p className="text-muted-foreground">Estos son los módulos que tiene asignados.</p>
      </div>

      <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-4">
        {menus.map((menu) => {
          const Icon = getMenuIcon(menu.ruta)
          return (
            <Link key={menu.id} to={`/modulo/${menu.ruta}`}>
              <Card className="h-full transition-shadow hover:shadow-md">
                <CardHeader>
                  <Icon className="mb-2 h-6 w-6 text-primary" />
                  <CardTitle>{menu.nombre}</CardTitle>
                  <CardDescription>{menu.descripcion}</CardDescription>
                </CardHeader>
              </Card>
            </Link>
          )
        })}
      </div>
    </div>
  )
}
