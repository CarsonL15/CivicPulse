interface EventSearchResult {
  id: number
  title: string
  aiSummary: string | null
  category: string
  eventDate: string
  location: string
  score: number
}

export function useSearch() {
  const query = ref('')
  const category = ref<string | null>(null)
  const results = ref<EventSearchResult[]>([])
  const loading = ref(false)
  const imageFile = ref<File | null>(null)
  const api = useApi()

  async function search() {
    if (!query.value.trim()) {
      results.value = []
      return
    }

    loading.value = true
    try {
      const params = new URLSearchParams()
      params.append('q', query.value)
      if (category.value) params.append('category', category.value)
      results.value = await api.get<EventSearchResult[]>(`/api/search?${params}`)
    } catch (e) {
      console.error('Search failed:', e)
      results.value = []
    } finally {
      loading.value = false
    }
  }

  async function searchByImage(file: File) {
    loading.value = true
    imageFile.value = file
    try {
      const formData = new FormData()
      formData.append('image', file)
      const params = new URLSearchParams()
      if (category.value) params.append('category', category.value)

      const config = useRuntimeConfig()
      const baseUrl = config.public.apiBaseUrl
      const token = import.meta.client ? localStorage.getItem('token') : null
      const headers: Record<string, string> = {}
      if (token) headers['Authorization'] = `Bearer ${token}`

      const response = await fetch(`${baseUrl}/api/search/image?${params}`, {
        method: 'POST',
        headers,
        body: formData,
      })
      if (!response.ok) throw new Error(`Search failed: ${response.status}`)
      results.value = await response.json()
    } catch (e) {
      console.error('Image search failed:', e)
      results.value = []
    } finally {
      loading.value = false
    }
  }

  return { query, category, results, loading, imageFile, search, searchByImage }
}
