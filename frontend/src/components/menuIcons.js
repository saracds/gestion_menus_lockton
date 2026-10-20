import { BarChart3, FileText, LayoutGrid, ShieldAlert, Users } from 'lucide-react'

// Ícono para cada menú según su ruta (identificador funcional)
const icons = {
  polizas: FileText,
  clientes: Users,
  siniestros: ShieldAlert,
  reportes: BarChart3,
}

export const getMenuIcon = (route) => icons[route] ?? LayoutGrid
