import { describe, it, expect, vi, beforeAll } from 'vitest'
import { mount } from '@vue/test-utils'
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'
import EventForm from '../components/EventForm.vue'

const vuetify = createVuetify({ components, directives })

// Mock ResizeObserver for Vuetify components
beforeAll(() => {
  vi.stubGlobal('ResizeObserver', class {
    observe() {}
    unobserve() {}
    disconnect() {}
  })
})

function mountEventForm(props = {}) {
  return mount(EventForm, {
    global: { plugins: [vuetify] },
    props
  })
}

describe('EventForm', () => {
  it('renders all form fields', () => {
    const wrapper = mountEventForm()
    expect(wrapper.text()).toContain('Event Title')
    expect(wrapper.text()).toContain('Description')
    expect(wrapper.text()).toContain('Date')
    expect(wrapper.text()).toContain('Time')
    expect(wrapper.text()).toContain('Location')
    expect(wrapper.text()).toContain('Category')
  })

  it('renders submit button with default label', () => {
    const wrapper = mountEventForm()
    expect(wrapper.text()).toContain('Create Event')
  })

  it('renders submit button with custom label', () => {
    const wrapper = mountEventForm({ submitLabel: 'Update Event' })
    expect(wrapper.text()).toContain('Update Event')
  })

  it('renders category select', () => {
    const wrapper = mountEventForm()
    const select = wrapper.find('.v-select')
    expect(select.exists()).toBe(true)
  })

  it('emits submit event on form submission', async () => {
    const wrapper = mountEventForm()
    await wrapper.find('form').trigger('submit.prevent')
    expect(wrapper.emitted('submit')).toBeTruthy()
  })

  it('has date input field', () => {
    const wrapper = mountEventForm()
    const dateInput = wrapper.find('input[type="date"]')
    expect(dateInput.exists()).toBe(true)
  })
})
