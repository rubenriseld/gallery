<script setup lang="ts">
import Menu from './components/Menu.vue'

</script>

<template>
    <header>
        <Menu/>
    </header>
    <main>

        <body>
            <RouterView class="view"></RouterView>
        </body>
    </main>
</template>
<style scoped>
* {
    margin: 0;
    padding: 0;
    font-family: 'Courier New', Courier, monospace;
}

#app {
    max-width: 1280px;
    margin: auto;
}

body {
    margin-top: 5rem;
    background-color: rgb(219, 219, 219);
    font-family: 'Courier New', Courier, monospace;
}

button {
    background-color: transparent;
    border: none;
    font-family: inherit !important;
}

.view {
    padding: 2rem 0 2rem 0;
    display: flex;
}

@media (hover: hover) {
    a:hover {
        /* background-color: hsla(160, 100%, 37%, 0.2); */
    }
}

@media (min-width: 1024px) {
    #app {}
}

header {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 5rem;
    background-color: #ffffff;
    z-index: 1000;
    display: flex;
    justify-content: space-between;
    /* Add this line */
    align-items: center
}
</style>