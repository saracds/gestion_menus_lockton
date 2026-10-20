// Cliente HTTP único para toda la app.
// - Agrega el token JWT en cada petición.
// - Convierte los errores de la API en mensajes legibles.
// - Si la API responde 401 (token vencido), cierra la sesión.

const TOKEN_KEY = 'lockton_token'

export const tokenStorage = {
  get: () => sessionStorage.getItem(TOKEN_KEY),
  set: (token) => sessionStorage.setItem(TOKEN_KEY, token),
  clear: () => sessionStorage.removeItem(TOKEN_KEY),
}

export class ApiError extends Error {
  constructor(status, message, data) {
    super(message)
    this.status = status
    this.data = data
  }
}

async function request(method, path, body) {
  const headers = { 'Content-Type': 'application/json' }
  const token = tokenStorage.get()
  if (token) headers.Authorization = `Bearer ${token}`

  const response = await fetch(`/api${path}`, {
    method,
    headers,
    body: body ? JSON.stringify(body) : undefined,
  })

  const data = response.status === 204 ? null : await response.json().catch(() => null)

  if (!response.ok) {
    // Sesión expirada o inválida (excepto en el propio login)
    if (response.status === 401 && path !== '/auth/login') {
      tokenStorage.clear()
      sessionStorage.removeItem('lockton_user')
      window.location.href = '/login'
    }
    throw new ApiError(response.status, extractMessage(data, response.status), data)
  }
  return data
}

// Formatos posibles: ProblemDetails { detail } o errores de validación { errors: {campo: [..]} }
function extractMessage(data, status) {
  if (data?.errors) return Object.values(data.errors).flat().join(' ')
  if (data?.detail) return data.detail
  if (data?.title) return data.title
  return `Error ${status}`
}

// Endpoints de la API, agrupados para que las páginas no conozcan URLs
export const api = {
  login: (nombreUsuario, contrasena) => request('POST', '/auth/login', { nombreUsuario, contrasena }),

  getUsers: () => request('GET', '/users'),
  createUser: (usuario) => request('POST', '/users', usuario),
  getUserMenus: (userId) => request('GET', `/users/${userId}/menus`),
  assignUserMenus: (userId, idsMenus) => request('PUT', `/users/${userId}/menus`, { idsMenus }),

  getMenus: () => request('GET', '/menus'),
  getModule: (route) => request('GET', `/modules/${encodeURIComponent(route)}`),
}
