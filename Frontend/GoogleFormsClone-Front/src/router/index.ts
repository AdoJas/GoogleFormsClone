import { createRouter, createWebHistory } from 'vue-router'
import HomePage from '../pages/HomePage.vue'
import LoginPage from '../pages/LoginPage.vue'
import RegisterPage from '../pages/RegisterPage.vue'
import DashboardPage from '../pages/DashboardPage.vue'
import FormBuilderPage from '../pages/FormBuilderPage.vue'
import FormRendererPage from '../pages/FormRendererPage.vue' 
import { useAuth } from '../composables/useAuth'

const routes = [
    { path: '/', name: 'Home', component: HomePage },
    { path: '/login', name: 'Login', component: LoginPage , meta : { hideHeader: true } },
    { path: '/register', name: 'Register', component: RegisterPage , meta : { hideHeader: true } },
    { path: '/dashboard', name: 'Dashboard', component: DashboardPage },
    { path: '/form-builder/:formId?', name: 'FormBuilder', component: FormBuilderPage },
    { path: '/forms/:formId', name: 'FormRenderer', component: FormRendererPage }, 
]

const router = createRouter({
    history: createWebHistory(),
    routes,
})

router.beforeEach((to, from, next) => {
    const { user } = useAuth()
    const token = localStorage.getItem('accessToken')

    if ((to.path === '/login' || to.path === '/register') && (user.value || token)) {
        return next('/dashboard')
    }

    if ((to.path === '/dashboard' || to.path.startsWith('/form-builder')) && !(user.value || token)) {
        return next('/login')
    }

    next()
})

export default router
