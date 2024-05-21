<script setup lang='ts'>
import IconMenu from '@/components/icons/IconMenu.vue';
import IconClose from '@/components/icons/IconClose.vue';

import api from '@/api';
import { onMounted, ref, computed } from 'vue';
import { useRouter } from "vue-router";

import type { ImageCollection } from '@/assets/types';
import { useStore } from 'vuex';

const store = useStore();

const collections = ref<ImageCollection[]>([]);
const isAuthenticated = computed(() => store.state.isAuthenticated);
const isMenuOpen = ref(false)
const isLoading = ref(true)
const router = useRouter()

onMounted(async () => {
    isLoading.value = true;
    await getCollections();
    updateMenuItems();
    isLoading.value = false;
});
const menuItems = ref<Array<{ name: string; path: string }>>([
    { name: 'Home', path: '/' },
    { name: 'Admin', path: '/admin' }]
);

function updateMenuItems() {
    collections.value.map(collection =>
        menuItems.value.push({
            name: collection.name,
            path: `/collection/${collection.imageCollectionId}`
        }));
}

async function getCollections() {
    try {
        const response = await api.get('imageCollections')
        if (response.status === 200) {
            collections.value = (response.data as ImageCollection[]).filter(collection => collection.shouldBeDisplayed === true)
        }
    }
    catch (error) {
        console.error('Error fetching collections:', error)
    }
}
async function logout() {
    try {
        const response = await api.post('auth/logout');
        if (response.status === 204) {
            store.commit('SET_AUTH', false);
        }
    }
    catch (error) {
        console.error('Error logging out:', error);
    }
    isMenuOpen.value = false;
    router.push('/');
}
</script>

<template v-if="!isLoading">
    <div class="menu-item-wrapper">
        <RouterLink to="/"
            class="logo">Ruben Riseld</RouterLink>
    </div>
    <div class="menu-container">
        <div @click="isMenuOpen = !isMenuOpen">
            <IconMenu class="menu-icon"
                v-if="!isMenuOpen" />
            <IconClose class="menu-icon"
                v-else="isMenuOpen" />
        </div>
        <nav :class="{ 'show-menu': isMenuOpen }">
            <div class="menu-items">
                <template v-for='(item, index) in menuItems'
                    :key='index'>
                    <div v-if="item.path !== '/admin' || isAuthenticated"
                        class="menu-item-wrapper">
                        <RouterLink :to="item.path"
                            class="menu-item"
                            @click="isMenuOpen = false">{{
                                item.name }}</RouterLink>
                    </div>
                </template>
            </div>
            <div v-if="isAuthenticated"
                class="menu-item-wrapper logout-wrapper">
                <RouterLink class="menu-item logout-button"
                    to="/"
                    @click="logout"
                    buttonText="">Log out</RouterLink>
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
    padding: 2rem;
    cursor: pointer;
    display: flex;
    align-items: center;
    position: relative;
    z-index: 5;
}

nav {
    position: fixed;
    top: 0;
    right: calc((100vw - var(--max-width)) / 2);
    width: 100%;
    height: 100%;
    padding-top: 5rem;
    box-sizing: border-box;
    background-color: var(--lightest-color);
    display: none;
}

.menu-items {
    display: flex;
    flex-direction: column;
    justify-content: flex-end;
}

.menu-item-wrapper {
    padding: 2rem 2rem 1.5rem 2rem;
    box-sizing: border-box;
    display: flex;
    justify-content: flex-end;
}

.menu-item,
.logo {
    text-decoration: none;
    text-transform: uppercase;
    padding-bottom: 0.5rem;
    font-size: 1.5rem;
    letter-spacing: 2px;
    transition: background-color 0.3s;
    color: var(--darker-color);
    font-family: inherit;
}

.menu-item:hover,
.logo:hover {
    background: linear-gradient(var(--view-primary-color), var(--view-primary-color)) no-repeat;
    background-size: 100% 4px;
    background-position: 0 95%;
}

.show-menu {
    display: flex;
    flex-direction: column;
    justify-content: space-between;
    width: 20rem;
    box-shadow: -5px 0 5px rgba(0, 0, 0, 0.1);

    padding-right: 1rem;
    overflow: visible;
    background-color: var(--lightest-color);
}

@media (max-width: 768px) {
    .show-menu {
        width: 100%;
        padding: 0;
        background: none;
        background-color: var(--lightest-color);
    }

    .menu-items {
        padding-top: 5rem;
    }

    .menu-icon {
        padding: 1rem;
        width: 2rem;
        height: 2rem;
    }

    .menu-item-wrapper {
        padding: 1rem;
        display: flex;
    }
}

@media (max-width: 1920px) {
    nav {
        right: 0;
    }
}

@media (min-width: 1920px) {
    .show-menu {
        background: linear-gradient(var(--view-primary-color), var(--view-primary-color)) no-repeat;
        background-size: 4px 100cap;
        background-position: 100% 50%;
        background-color: var(--lightest-color);
    }
}
</style>
