export function useBookmarks() {
  const api = useApi()

  async function getBookmarks() {
    return api.get<any[]>('/api/bookmarks')
  }

  async function addBookmark(eventId: number) {
    return api.post(`/api/bookmarks/${eventId}`)
  }

  async function removeBookmark(eventId: number) {
    return api.del(`/api/bookmarks/${eventId}`)
  }

  return { getBookmarks, addBookmark, removeBookmark }
}
