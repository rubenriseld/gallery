<script setup lang="ts">
import Menu from './components/Menu.vue'

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