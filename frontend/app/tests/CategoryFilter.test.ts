import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'
import CategoryFilter from '../components/CategoryFilter.vue'

const vuetify = createVuetify({ components, directives })

function mountCategoryFilter(props = {}) {
  return mount(CategoryFilter, {
    global: { plugins: [vuetify] },
    props
  })
}

describe('CategoryFilter', () => {
  it('renders all four category chips', () => {
    const wrapper = mountCategoryFilter()
    const chips = wrapper.findAll('.v-chip')
    expect(chips.length).toBe(4)
  })

  it('displays correct category labels', () => {
    const wrapper = mountCategoryFilter()
    const text = wrapper.text()
    expect(text).toContain('Education')
    expect(text).toContain('Housing')
    expect(text).toContain('Public Safety')
    expect(text).toContain('Environment')
  })
})
