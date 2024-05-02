<script setup lang='ts'>
const menuItems = [
    { name: 'Home', path: '/' },
    { name: 'Collection', path: '/collection' }
]
</script>
<template>
    <nav>
        <ul>
            <li v-for='(item, index) in menuItems' :key='index'>
                <RouterLink :to=item.path>{{ item.name }}</RouterLink>>
            </li>
        </ul>
    </nav>
</template>