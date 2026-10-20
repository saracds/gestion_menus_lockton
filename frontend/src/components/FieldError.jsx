// Mensaje de error debajo de un campo del formulario
export default function FieldError({ message }) {
  if (!message) return null
  return <p className="text-xs text-destructive">{message}</p>
}
