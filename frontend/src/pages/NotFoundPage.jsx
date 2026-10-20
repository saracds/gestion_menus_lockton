import { Link } from 'react-router-dom'

export default function NotFoundPage() {
  return (
    <div className="mx-auto mt-16 max-w-lg text-center">
      <h1 className="text-2xl font-bold">404 · Página no encontrada</h1>
      <Link to="/" className="mt-4 inline-block text-sm underline">Volver al inicio</Link>
    </div>
  )
}
