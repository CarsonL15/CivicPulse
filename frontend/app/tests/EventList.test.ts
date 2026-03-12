import { describe, it, expect, vi, beforeAll } from 'vitest'
import { mount } from '@vue/test-utils'
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'
import EventList from '../components/EventList.vue'

const vuetify = createVuetify({ components, directives })

// Mock ResizeObserver for Vuetify components that need it
beforeAll(() => {
  vi.stubGlobal('ResizeObserver', class {
    observe() {}
    unobserve() {}
    disconnect() {}
  })
})

// Mock useAuth since EventCard child uses it
vi.stubGlobal('useAuth', () => ({
  isAuthenticated: ref(false),
  isOrganizer: ref(false),
  isAdmin: ref(false),
  user: ref(null),
  login: vi.fn(),
  logout: vi.fn(),
  register: vi.fn(),
  initAuth: vi.fn()
}))

vi.stubGlobal('navigateTo', vi.fn())

const mockEvents = [
  {
    id: 1, title: 'Event One', description: 'Desc 1', eventDate: '2026-04-10T00:00:00',
    time: '6 PM', location: 'City Hall', category: 'Housing', aiSummary: 'Summary 1', isBookmarked: false
  },
  {
    id: 2, title: 'Event Two', description: 'Desc 2', eventDate: '2026-04-11T00:00:00',
    time: '7 PM', location: 'Library', category: 'Education', aiSummary: 'Summary 2', isBookmarked: false
  }
]

function mountEventList(props = {}) {
  return mount(EventList, {
    global: {
      plugins: [vuetify],
      stubs: {
        EventCard: {
          props: ['event'],
          template: '<div class="event-card-stub">{{ event.title }}</div>'
        }
      }
    },
    props: { events: [], ...props }
  })
}

describe('EventList', () => {
  it('shows loading spinner when loading', () => {
    const wrapper = mountEventList({ loading: true })
    expect(wrapper.find('.v-progress-circular').exists()).toBe(true)
  })

  it('shows empty message when no events', () => {
    const wrapper = mountEventList({ events: [] })
    expect(wrapper.text()).toContain('No events found')
  })

  it('shows custom empty message', () => {
    const wrapper = mountEventList({ events: [], emptyMessage: 'Nothing here' })
    expect(wrapper.text()).toContain('Nothing here')
  })

  it('renders event cards when events provided', () => {
    const wrapper = mountEventList({ events: mockEvents })
    expect(wrapper.text()).toContain('Event One')
    expect(wrapper.text()).toContain('Event Two')
  })

  it('renders correct number of columns', () => {
    const wrapper = mountEventList({ events: mockEvents })
    const cols = wrapper.findAll('.v-col')
    expect(cols.length).toBe(2)
  })

  it('does not show loading spinner when not loading', () => {
    const wrapper = mountEventList({ events: mockEvents })
    expect(wrapper.find('.v-progress-circular').exists()).toBe(false)
  })
})
