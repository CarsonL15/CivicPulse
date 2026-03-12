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
              primary: '#0f2b3e',
              secondary: '#1a4a6e',
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
  css: [
    '~/assets/css/animations.css',
    '~/assets/css/typography.css',
  ],
  app: {
    pageTransition: { name: 'page', mode: 'out-in' },
    head: {
      title: 'CivicPulse',
      link: [
        {
          rel: 'stylesheet',
          href: 'https://fonts.googleapis.com/css2?family=Inter:wght@400;500;600;700;800&display=swap'
        }
      ],
      meta: [
        { name: 'description', content: 'Discover civic events that matter to you' }
      ]
    }
  }
})
