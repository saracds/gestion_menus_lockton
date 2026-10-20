import { NavLink, Outlet } from 'react-router-dom'
import { Home, ListChecks, LogOut, ShieldCheck, UserCog } from 'lucide-react'
import { useAuth } from '@/context/AuthContext'
import { Button } from '@/components/ui/button'
import { Badge } from '@/components/ui/badge'
import { getMenuIcon } from '@/components/menuIcons'
import { cn } from '@/lib/utils'

function SideLink({ to, icon: Icon, children }) {
  return (
    <NavLink
      to={to}
      end
      className={({ isActive }) =>
        cn(
          'flex items-center gap-3 rounded-md px-3 py-2 text-sm transition-colors',
          isActive ? 'bg-primary text-primary-foreground' : 'text-slate-600 hover:bg-slate-100'
        )
      }
    >
      <Icon className="h-4 w-4" />
      {children}
    </NavLink>
  )
}

export default function Layout() {
  const { user, menus, loadingMenus, isAdmin, logout } = useAuth()

  return (
    <div className="flex min-h-screen">
      <aside className="flex w-64 flex-col border-r bg-white">
        <div className="flex items-center gap-2 border-b px-5 py-4">
          <ShieldCheck className="h-6 w-6 text-primary" />
          <span className="font-semibold">Lockton App</span>
        </div>

        <nav className="flex-1 space-y-6 p-4">
          <SideLink to="/" icon={Home}>Inicio</SideLink>

          {/* MENÚ DINÁMICO: solo lo que la API devuelve para este usuario */}
          <div className="space-y-1">
            <p className="px-3 text-xs font-semibold uppercase text-muted-foreground">Mis módulos</p>
            {loadingMenus && <p className="px-3 text-sm text-muted-foreground">Cargando...</p>}
            {!loadingMenus && menus.length === 0 && (
              <p className="px-3 text-sm text-muted-foreground">Sin menús asignados</p>
            )}
            {menus.map((menu) => (
              <SideLink key={menu.id} to={`/modulo/${menu.ruta}`} icon={getMenuIcon(menu.ruta)}>
                {menu.nombre}
              </SideLink>
            ))}
          </div>

          {isAdmin && (
            <div className="space-y-1">
              <p className="px-3 text-xs font-semibold uppercase text-muted-foreground">Administración</p>
              <SideLink to="/usuarios" icon={UserCog}>Usuarios</SideLink>
              <SideLink to="/asignacion-menus" icon={ListChecks}>Asignación de menús</SideLink>
            </div>
          )}
        </nav>

        <div className="border-t p-4">
          <p className="text-sm font-medium">{user.nombreCompleto}</p>
          <div className="mb-3 flex items-center gap-2 text-xs text-muted-foreground">
            @{user.nombreUsuario} <Badge variant={isAdmin ? 'default' : 'secondary'}>{user.rol}</Badge>
          </div>
          <Button variant="outline" size="sm" className="w-full" onClick={logout}>
            <LogOut /> Cerrar sesión
          </Button>
        </div>
      </aside>

      <main className="flex-1 p-8">
        <Outlet />
      </main>
    </div>
  )
}
