import './assets/main.css'

import { createApp } from 'vue'
import { createWebHistory, createRouter } from 'vue-router'
import App from './App.vue'
import IndexView from './views/IndexView.vue'
import AdminView from './views/AdminView.vue'
import CollectionView from './views/CollectionView.vue'
import LoginView from './views/LoginView.vue'
import { createStore } from 'vuex';

const store = createStore({
    state: {
        isAuthenticated: false,
    },
    mutations: {
        SET_AUTH(state: any, isAuthenticated: boolean) {
            state.isAuthenticated = isAuthenticated;
        },
    },
});
const routes = [
    { path: '/', component: IndexView },
    { path: '/admin', component: AdminView },
    { path: '/collection/:collectionId', name: 'collection', component: CollectionView, props: true },
    { path: '/login', component: LoginView }
];

const router = createRouter({
    history: createWebHistory(),
    routes,
});

router.beforeEach(async (to, from, next) => {
    if (to.path === '/admin' && store.state.isAuthenticated === false) {
        next({ path: '/login', query: { redirect: '/admin' } });
    }
    else next();
})

createApp(App)
    .use(router)
    .use(store)
    .mount('#app');