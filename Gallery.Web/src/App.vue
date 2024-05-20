<script setup lang="ts">
import { onMounted } from 'vue';
import Menu from './components/Menu.vue'
import api from './api';
import { useStore } from 'vuex';

const store = useStore();
onMounted(async () => {
    try {
        const response = await api.get("auth/check");
        if (response.status === 200) {
            store.commit('SET_AUTH', true);
        }
    }
    catch (error) {
        console.error('Error checking authentication:', error);
    }
})
</script>

<template>
    <header>
        <Menu />
    </header>
    <main>
        <RouterView></RouterView>
    </main>
</template>

<style scoped>
header {
    position: fixed;
    top: 0;
    width: 100%;
    max-width: var(--max-width);
    height: 5rem;
    z-index: 3;
    display: flex;
    justify-content: space-between;
    background-color: var(--lightest-color);
    align-items: center;
    overflow: hidden;
}

main {
    margin-top: 5rem;
    font-family: inherit;
    background-color: inherit;
    overflow: hidden;
}
</style>