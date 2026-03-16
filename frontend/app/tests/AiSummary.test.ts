import { describe, it, expect } from 'vitest'
import { mount } from '@vue/test-utils'
import { createVuetify } from 'vuetify'
import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'
import AiSummary from '../components/AiSummary.vue'

const vuetify = createVuetify({ components, directives })

function mountAiSummary(props: { summary: string; whyItMatters?: string | null }) {
  return mount(AiSummary, {
    global: { plugins: [vuetify] },
    props
  })
}

describe('AiSummary', () => {
  it('renders the summary text', () => {
    const wrapper = mountAiSummary({ summary: 'This is an AI-generated summary.' })
    expect(wrapper.text()).toContain('This is an AI-generated summary.')
  })

  it('renders the AI Summary heading', () => {
    const wrapper = mountAiSummary({ summary: 'Test summary' })
    expect(wrapper.text()).toContain('AI Summary')
  })

  it('renders whyItMatters when provided', () => {
    const wrapper = mountAiSummary({
      summary: 'Summary text',
      whyItMatters: 'This matters for young voters.'
    })
    expect(wrapper.text()).toContain('Why It Matters')
    expect(wrapper.text()).toContain('This matters for young voters.')
  })

  it('hides whyItMatters section when null', () => {
    const wrapper = mountAiSummary({
      summary: 'Summary text',
      whyItMatters: null
    })
    expect(wrapper.text()).not.toContain('Why It Matters')
  })

  it('displays robot icon', () => {
    const wrapper = mountAiSummary({ summary: 'Test' })
    const icon = wrapper.find('.mdi-robot')
    expect(icon.exists()).toBe(true)
  })
})
