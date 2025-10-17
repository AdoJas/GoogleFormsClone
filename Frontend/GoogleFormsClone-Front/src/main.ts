import { createApp } from 'vue'
import App from './App.vue'
import router from './router'
import 'vuetify/styles'
import './style.css'

import '@mdi/font/css/materialdesignicons.css'

import { createVuetify } from 'vuetify'
import { aliases, mdi } from 'vuetify/iconsets/mdi'

import * as components from 'vuetify/components'
import * as directives from 'vuetify/directives'

const vuetify = createVuetify({
    components,
    directives,
    icons: {
        defaultSet: 'mdi',
        aliases,
        sets: { mdi },
    },
    theme: {
        defaultTheme: 'light',
        themes: {
            light: {
                dark: false,
                colors: {
                    primary: '#1976D2',
                    secondary: '#424242',
                    accent: '#82B1FF',
                    error: '#FF5252',
                    info: '#2196F3',
                    success: '#4CAF50',
                    warning: '#FFC107',
                },
            },
            dark: {
                dark: true,
                colors: {
                    primary: '#2196F3',
                    secondary: '#FFCDD2',
                    accent: '#FF4081',
                    error: '#f44336',
                    info: '#03A9F4',
                    success: '#4CAF50',
                    warning: '#FB8C00',
                },
            },
        },
    },
})

createApp(App)
    .use(router)
    .use(vuetify)
    .mount('#app')

export { vuetify }
