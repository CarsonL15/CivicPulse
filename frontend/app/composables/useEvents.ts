interface EventDto {
  id: number
  title: string
  description: string
  eventDate: string
  time: string
  location: string
  category: string
  aiSummary: string | null
  whyItMatters: string | null
  organizerName: string
  organizerId: string
  isBookmarked: boolean
  isApproved: boolean
}

interface EventCreateDto {
  title: string
  description: string
  eventDate: string
  time: string
  location: string
  category: number
}

export function useEvents() {
  const api = useApi()

  async function getEvents(category?: string, page = 1, pageSize = 20): Promise<EventDto[]> {
    const params = new URLSearchParams()
    if (category) params.append('category', category)
    params.append('page', page.toString())
    params.append('pageSize', pageSize.toString())
    return api.get<EventDto[]>(`/api/events?${params}`)
  }

  async function getEvent(id: number): Promise<EventDto> {
    return api.get<EventDto>(`/api/events/${id}`)
  }

  async function createEvent(dto: EventCreateDto): Promise<EventDto> {
    return api.post<EventDto>('/api/events', dto)
  }

  async function updateEvent(id: number, dto: Partial<EventCreateDto>): Promise<EventDto> {
    return api.put<EventDto>(`/api/events/${id}`, dto)
  }

  async function deleteEvent(id: number): Promise<void> {
    return api.del(`/api/events/${id}`)
  }

  async function getMyEvents(): Promise<EventDto[]> {
    return api.get<EventDto[]>('/api/events/my-events')
  }

  return { getEvents, getEvent, createEvent, updateEvent, deleteEvent, getMyEvents }
}
