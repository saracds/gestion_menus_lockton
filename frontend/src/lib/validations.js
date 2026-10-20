// Validaciones del lado del cliente, con mensajes en español.
// Son las MISMAS reglas del backend (CrearUsuarioDto / LoginDto),
// pero aquí solo mejoran la experiencia: la API vuelve a validar siempre.

export const ROLES = { ADMIN: 'Administrador', USER: 'Usuario' }

export function validateLogin({ nombreUsuario, contrasena }) {
  const errors = {}
  if (!nombreUsuario.trim()) errors.nombreUsuario = 'El nombre de usuario es obligatorio.'
  if (!contrasena) errors.contrasena = 'La contraseña es obligatoria.'
  return errors
}

export function validateNewUser({ nombreCompleto, nombreUsuario, contrasena, rol }) {
  const errors = {}
  const name = nombreCompleto.trim()
  const user = nombreUsuario.trim()

  if (!name) errors.nombreCompleto = 'El nombre completo es obligatorio.'
  else if (name.length < 3 || name.length > 100)
    errors.nombreCompleto = 'El nombre completo debe tener entre 3 y 100 caracteres.'

  if (!user) errors.nombreUsuario = 'El nombre de usuario es obligatorio.'
  else if (user.length < 3 || user.length > 50)
    errors.nombreUsuario = 'El nombre de usuario debe tener entre 3 y 50 caracteres.'
  else if (!/^[a-zA-Z0-9._-]+$/.test(user))
    errors.nombreUsuario = 'Solo se admiten letras sin tilde, números, punto, guion y guion bajo (sin espacios).'

  if (!contrasena) errors.contrasena = 'La contraseña es obligatoria.'
  else if (contrasena.length < 8 || contrasena.length > 100)
    errors.contrasena = 'La contraseña debe tener entre 8 y 100 caracteres.'
  else if (!/[A-Za-z]/.test(contrasena) || !/\d/.test(contrasena))
    errors.contrasena = 'La contraseña debe contener al menos una letra y un número.'

  if (![ROLES.ADMIN, ROLES.USER].includes(rol)) errors.rol = "El rol debe ser 'Administrador' o 'Usuario'."

  return errors
}
