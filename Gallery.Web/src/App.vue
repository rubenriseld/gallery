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
    <footer>
        <div class="contact-info">
            <a href="mailto:rubenriseld@outlook.com">rubenriseld@outlook.com</a>
        </div>
        <div>
            <p>&copy; Ruben Riseld 2024</p>
        </div>
    </footer>
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

footer {
    padding: 2rem;
    display: flex;
    justify-content: space-between;
}

a {
    color: var(--darker-color);
    text-decoration: none;
    font: inherit;
    padding-bottom: 0.2rem;
}

a:hover {
    background: linear-gradient(var(--view-primary-color), var(--view-primary-color)) no-repeat;
    background-size: 100% 4px;
    background-position: 0 100%;
}

a,
p {
    font-size: 1.3rem;
    letter-spacing: 0.4px;
}

@media screen and (max-width: 768px) {
    footer {
        padding: 1rem;
    }
}
</style>