import { describe, it, expect, vi } from 'vitest'
import { mount } from '@vue/test-utils'
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'
import EventCard from '../components/EventCard.vue'

const vuetify = createVuetify({ components, directives })

// Mock useAuth as a global since Nuxt auto-imports it
vi.stubGlobal('useAuth', () => ({
  isAuthenticated: ref(true),
  isOrganizer: ref(false),
  isAdmin: ref(false),
  user: ref(null),
  login: vi.fn(),
  logout: vi.fn(),
  register: vi.fn(),
  initAuth: vi.fn()
}))

// Mock navigateTo since it's a Nuxt global
vi.stubGlobal('navigateTo', vi.fn())

const mockEvent = {
  id: 1,
  title: 'City Council Meeting',
  description: 'Discussion about new zoning laws and budget allocations for the upcoming fiscal year in the city.',
  eventDate: '2026-04-10T00:00:00',
  time: '6:00 PM - 8:00 PM',
  location: 'Cheney City Hall',
  category: 'Housing',
  aiSummary: 'The city council will discuss zoning changes.',
  isBookmarked: false
}

function mountEventCard(event = mockEvent) {
  return mount(EventCard, {
    global: { plugins: [vuetify] },
    props: { event }
  })
}

describe('EventCard', () => {
  it('renders event title', () => {
    const wrapper = mountEventCard()
    expect(wrapper.text()).toContain('City Council Meeting')
  })

  it('renders event location', () => {
    const wrapper = mountEventCard()
    expect(wrapper.text()).toContain('Cheney City Hall')
  })

  it('renders category chip', () => {
    const wrapper = mountEventCard()
    expect(wrapper.text()).toContain('Housing')
  })

  it('renders formatted date', () => {
    const wrapper = mountEventCard()
    expect(wrapper.text()).toContain('Apr')
    expect(wrapper.text()).toContain('2026')
  })

  it('displays AI summary when available', () => {
    const wrapper = mountEventCard()
    expect(wrapper.text()).toContain('The city council will discuss zoning changes.')
  })

  it('displays truncated description when no AI summary', () => {
    const longDesc = 'A'.repeat(200)
    const wrapper = mountEventCard({
      ...mockEvent,
      aiSummary: null,
      description: longDesc
    })
    expect(wrapper.text()).toContain('...')
  })

  it('shows bookmark button when authenticated', () => {
    const wrapper = mountEventCard()
    const btns = wrapper.findAll('.v-btn')
    expect(btns.length).toBeGreaterThan(0)
  })

  it('emits toggleBookmark when bookmark button clicked', async () => {
    const wrapper = mountEventCard()
    const btns = wrapper.findAll('.v-btn')
    const bookmarkBtn = btns[btns.length - 1]
    await bookmarkBtn.trigger('click')
    expect(wrapper.emitted('toggleBookmark')).toBeTruthy()
  })
})
