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
    isLoading.value = false

    collections.value.forEach((collection, index) => {
        const style = document.createElement('style');
        style.textContent = `
            .background-${index} {
                background-image: url(${collection.coverImage?.uri});
            }
            .background-${index}::before {
                background-image: url(${collection.coverImage?.uri});
            }
        `;
        document.head.append(style);
    });
});
async function getCollections() {
    collections.value = ((await api.get("imageCollections")).data as ImageCollection[]).filter(collection => collection.shouldBeDisplayed === true);
}
</script>

<template>
    <div v-for="(collection, index) in collections"
        :key="index"
        class="collection-wrapper">
        <div class="background-image"
            :style="collection.coverImage?.uri && { backgroundImage: `url(${collection.coverImage.uri})` }" />
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
    margin: 1rem 0 1rem 0;
    background-size: cover;
    background-position: center;
    position: relative;
    overflow: hidden;
}

.background-image {
    position: absolute;
    top: 0;
    right: 0;
    bottom: 0;
    left: 0;
    background-size: cover;
    background-position: center;
    z-index: 1;
}

.collection-link {
    height: 100%;
    width: 100%;
    display: flex;
    justify-content: center;
    align-items: center;
    font-size: 3rem;
    text-transform: uppercase;
    color: var(--lightest-color);
    opacity: 0.7;
    text-decoration: none;
    backdrop-filter: blur(5px);
}

a {
    z-index: 2;
    letter-spacing: 2px;
}

.collection-link,
.background-image,
.collection-wrapper {
    transition: all 0.1s ease-in;
}

.collection-wrapper:hover .background-image {
    transform: scale(1.1);
    filter: brightness(0.7) blur(8px);
}

.collection-link:hover {
    color: var(--lightest-color);
    text-shadow: var(--text-shadow);
    opacity: 1;
}

@media (max-width: 768px) {
    .collection-wrapper {
        margin: 1rem 0 1rem 0;
        height: 10rem;
    }
}
</style>