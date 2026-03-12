import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'
import SearchBar from '../components/SearchBar.vue'

const vuetify = createVuetify({ components, directives })

function mountSearchBar(props = {}) {
  return mount(SearchBar, {
    global: { plugins: [vuetify] },
    props
  })
}

describe('SearchBar', () => {
  it('renders with search input', () => {
    const wrapper = mountSearchBar()
    const input = wrapper.find('input[type="text"]')
    expect(input.exists()).toBe(true)
  })

  it('emits search event on Enter key', async () => {
    const wrapper = mountSearchBar({ modelValue: 'housing' })
    const input = wrapper.find('input[type="text"]')
    await input.trigger('keyup.enter')
    expect(wrapper.emitted('search')).toBeTruthy()
  })

  it('has a hidden file input for image upload', () => {
    const wrapper = mountSearchBar()
    const fileInput = wrapper.find('input[type="file"]')
    expect(fileInput.exists()).toBe(true)
    expect(fileInput.attributes('accept')).toContain('image/')
  })

  it('shows image chip when imageFile prop is set', async () => {
    const file = new File([''], 'poster.jpg', { type: 'image/jpeg' })
    const wrapper = mountSearchBar({ imageFile: file })
    await wrapper.vm.$nextTick()
    expect(wrapper.text()).toContain('poster.jpg')
  })
})
