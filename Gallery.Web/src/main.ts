import './assets/main.css'

import { createApp } from 'vue'
import { createWebHistory, createRouter } from 'vue-router'
import api from './api'
import App from './App.vue'
import IndexView from './views/IndexView.vue'
import AdminView from './views/AdminView.vue'
import CollectionView from './views/CollectionView.vue'
import LoginView from './views/LoginView.vue'


const routes = [
    { path: '/', component: IndexView },
    { path: '/admin', component: AdminView },
    { path: '/collection', component: CollectionView },
    { path: '/login', component: LoginView }
]

const router = createRouter({
    history: createWebHistory(),
    routes,
})

router.beforeEach(async (to, from, next) => {

    if (to.path === '/admin') {
        const isAuthenticated = await authenticate()
        !isAuthenticated ? next({ path: '/login', query: { redirect: '/admin' } })
            : next()
    }
    else next()
})

async function authenticate(): Promise<boolean> {
    try {
        await api.get('auth/check')
        return true
    }
    catch (error) {
        return false
    }
}

createApp(App)
    .use(router)
    .mount('#app')