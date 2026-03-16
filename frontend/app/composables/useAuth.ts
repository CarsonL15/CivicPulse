interface UserDto {
  id: string
  email: string
  displayName: string
  role: string
}

interface AuthResponse {
  token: string
  user: UserDto
}

export function useAuth() {
  const user = useState<UserDto | null>('auth-user', () => null)
  const token = useState<string | null>('auth-token', () => null)
  const api = useApi()

  const isAuthenticated = computed(() => !!token.value)
  const isOrganizer = computed(() => user.value?.role === 'Organizer' || user.value?.role === 'Admin')
  const isAdmin = computed(() => user.value?.role === 'Admin')

  function init() {
    if (import.meta.client) {
      const savedToken = localStorage.getItem('token')
      const savedUser = localStorage.getItem('user')
      if (savedToken && savedUser) {
        token.value = savedToken
        user.value = JSON.parse(savedUser)
      }
    }
  }

  async function login(email: string, password: string) {
    const response = await api.post<AuthResponse>('/api/auth/login', { email, password })
    token.value = response.token
    user.value = response.user
    if (import.meta.client) {
      localStorage.setItem('token', response.token)
      localStorage.setItem('user', JSON.stringify(response.user))
    }
  }

  async function register(email: string, password: string, displayName: string, role: string) {
    const response = await api.post<AuthResponse>('/api/auth/register', {
      email, password, displayName, role
    })
    token.value = response.token
    user.value = response.user
    if (import.meta.client) {
      localStorage.setItem('token', response.token)
      localStorage.setItem('user', JSON.stringify(response.user))
    }
  }

  function logout() {
    token.value = null
    user.value = null
    if (import.meta.client) {
      localStorage.removeItem('token')
      localStorage.removeItem('user')
    }
    navigateTo('/login')
  }

  return { user, token, isAuthenticated, isOrganizer, isAdmin, init, login, register, logout }
}
