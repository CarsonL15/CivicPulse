// https://nuxt.com/docs/api/configuration/nuxt-config
export default defineNuxtConfig({
  compatibilityDate: '2024-11-01',
  devtools: { enabled: true },
  future: {
    compatibilityVersion: 4
  },
  ssr: false,
  modules: ['vuetify-nuxt-module', '@pinia/nuxt'],
  vuetify: {
    vuetifyOptions: {
      theme: {
        defaultTheme: 'civicPulseTheme',
        themes: {
          civicPulseTheme: {
            dark: false,
            colors: {
              primary: '#1565C0',
              secondary: '#26A69A',
              accent: '#FF7043',
              background: '#FAFAFA',
              surface: '#FFFFFF',
              error: '#D32F2F',
              info: '#1976D2',
              success: '#388E3C',
              warning: '#F57C00',
            }
          }
        }
      }
    }
  },
  runtimeConfig: {
    public: {
      apiBaseUrl: process.env.NUXT_PUBLIC_API_BASE_URL || 'http://localhost:5000'
    }
  },
  app: {
    head: {
      title: 'CivicPulse',
      meta: [
        { name: 'description', content: 'Discover civic events that matter to you' }
      ]
    }
  }
})
