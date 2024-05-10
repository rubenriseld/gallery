<script setup lang="ts">
import api from '@/api';
import { ref, onMounted } from 'vue';
import { useRoute } from 'vue-router';

import type { ImageCollection } from '@/assets/types';

const isLoading = ref(true);
const collectionId = ref<string>('');
const collections = ref<ImageCollection[]>([]);
const route = useRoute()

onMounted(async () => {
    const id = route.params.collectionId as string
    collectionId.value = id
    await getCollections()
    console.log(collections.value)
    isLoading.value = false
})

async function getCollections() {
    collections.value = (await api.get("imageCollections")).data as ImageCollection[]
}

</script>

<template>
    <div v-for="(collection, index) in collections"
        :key="index"
        class="collection-wrapper">
        <RouterLink class="collection-link"
            :to="`collection/${collection.imageCollectionId}`">{{ collection.name }}
        </RouterLink>
    </div>
</template>

<style scoped>
.collection-wrapper {
    height: 15rem;
    display: flex;
    justify-content: center;
    align-items: center;
    background-color: var(--mid-color);
    margin: 1rem 2rem 1rem 2rem;
}

.collection-link {
    height: 100%;
    display: flex;
    margin: 0 1rem 0 1rem;
    justify-content: center;
    align-items: center;
    font-size: 3rem;
    text-transform: uppercase;
    color: var(--darker-color);
    text-decoration: none;
    transition: all 0.1s ease-in;
}

.collection-link:hover {
    transform: scale(1.1);
}

@media (max-width: 768px) {
    .collection-wrapper {
        margin: 1rem 1rem 1rem 1rem;
    }
}
</style>