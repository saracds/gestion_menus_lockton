import { clsx } from 'clsx'
import { twMerge } from 'tailwind-merge'

// Utilidad estándar de shadcn: combina clases de Tailwind sin conflictos
export function cn(...inputs) {
  return twMerge(clsx(inputs))
}
