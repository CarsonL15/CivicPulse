export function useApi() {
  const config = useRuntimeConfig()
  const baseUrl = config.public.apiBaseUrl

  function getToken(): string | null {
    if (import.meta.client) {
      return localStorage.getItem('token')
    }
    return null
  }

  function headers(): HeadersInit {
    const h: HeadersInit = { 'Content-Type': 'application/json' }
    const token = getToken()
    if (token) h['Authorization'] = `Bearer ${token}`
    return h
  }

  async function request<T>(path: string, options: RequestInit = {}): Promise<T> {
    const response = await fetch(`${baseUrl}${path}`, {
      ...options,
      headers: { ...headers(), ...options.headers }
    })

    if (!response.ok) {
      const error = await response.text()
      throw new Error(error || `Request failed: ${response.status}`)
    }

    if (response.status === 204) return undefined as T
    return response.json()
  }

  return {
    get: <T>(path: string) => request<T>(path),
    post: <T>(path: string, body?: unknown) =>
      request<T>(path, { method: 'POST', body: body ? JSON.stringify(body) : undefined }),
    put: <T>(path: string, body?: unknown) =>
      request<T>(path, { method: 'PUT', body: body ? JSON.stringify(body) : undefined }),
    del: (path: string) => request<void>(path, { method: 'DELETE' })
  }
}
