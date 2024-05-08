<script setup lang='ts'>
import api from '@/api'
import { onMounted, onUnmounted, ref, watch } from 'vue'
import { useRouter } from "vue-router"
import IconMenu from './icons/IconMenu.vue'
import IconClose from './icons/IconClose.vue'

const props = defineProps({
    isAuthenticated: Boolean
})

const isAuthenticated = ref<boolean>(props.isAuthenticated)
const router = useRouter()

const menuItems = [
    { name: 'Home', path: '/' },
    { name: 'Collection', path: '/collection' },
    { name: 'Admin', path: '/admin' },
]

const isMenuOpen = ref(false)

onMounted(async () => {
    isAuthenticated.value = await authenticate()
})
watch([isAuthenticated], async () => {
    console.log("auth menu")
    isAuthenticated.value = await authenticate()
    console.log(isAuthenticated.value)
})

async function logout() {
    await api.post('auth/logout')
    isAuthenticated.value = await authenticate()
    isMenuOpen.value = false
    router.push('/')
}
async function authenticate(): Promise<boolean> {
    try {
        await api.get('auth/check')
        return true
    }
    catch (error) {
        return false
    }
}
</script>

<template>
    <div class="menu-item-wrapper">
        <RouterLink to="/" class="logo">Ruben Riseld</RouterLink>
    </div>
    <div class="menu-container">
        <div class="menu-icon" @click="isMenuOpen = !isMenuOpen">
            <IconMenu v-if="!isMenuOpen" />
            <IconClose v-else="isMenuOpen" />
        </div>
        <nav :class="{ 'show-menu': isMenuOpen }">
            <div class="menu-items">
                <template v-for='(item, index) in menuItems' :key='index'>
                    <div v-if="item.path !== '/admin' || isAuthenticated" class="menu-item-wrapper">
                        <RouterLink :to="item.path" class="menu-item" @click="isMenuOpen = false">{{
                            item.name }}</RouterLink>
                    </div>
                </template>
            </div>
            <div v-if="isAuthenticated" class="menu-item-wrapper logout-wrapper">
                <button @click="logout" class="logout-button">Log out</button>
            </div>
        </nav>
    </div>
</template>

<style scoped>
.menu-container {
    position: flex;
}

.menu-icon {
    color: #121212;
    margin: 2rem;
    cursor: pointer;
    display: flex;
    align-items: center;
    z-index: 2;
    position: relative;
}

nav {
    position: fixed;
    top: 0;
    right: 0;
    width: 100%;
    height: 100%;
    padding-top: 5rem;
    box-sizing: border-box;
    background-color: #ffffff;
    z-index: 1;
    display: none;
}

.menu-items {
    display: flex;
    flex-direction: column;
    justify-content: flex-end;
}

.menu-item-wrapper {
    padding: 2rem;
    height: 5rem;
    box-sizing: border-box;
    display: flex;
    justify-content: flex-end;
}

.menu-item,
.logo,
.logout-button {
    color: #121212;
    text-decoration: none;
    font-weight: bold;
    text-transform: uppercase;
    font-size: 1.2rem;
    transition: background-color 0.3s;
    font-family: inherit;
}

.menu-item-wrapper:hover {
    background-color: #f0f0f0;
    text-decoration: underline;
}

.show-menu {
    display: flex;
    flex-direction: column;
    justify-content: space-between;
    width: 20rem;
    box-shadow: -2px 0 5px rgba(0, 0, 0, 0.1);
}

@media (max-width: 768px) {
    .show-menu {
        width: 100%;
        padding: 0;
    }

    .show-menu>.menu-item-wrapper {
        display: flex;
        justify-content: center;
    }
}
</style>
