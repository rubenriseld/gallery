<script setup lang="ts">
import Menu from './components/Menu.vue'
</script>

<template>
  <header>
    <Menu></Menu>
  </header>
  <main>
    <RouterView></RouterView>
  </main>
</template>