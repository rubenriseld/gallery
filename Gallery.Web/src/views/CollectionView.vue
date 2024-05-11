<script setup lang="ts">
import IconChevronLeft from '@/components/icons/IconChevronLeft.vue';
import IconChevronRight from '@/components/icons/IconChevronRight.vue';
import IconClose from '@/components/icons/IconClose.vue';

import api from '@/api';
import { ref, onMounted, watch, computed } from 'vue';
import { useRoute } from 'vue-router';

import type { Image, ImageCollection } from '@/assets/types';

const isLoading = ref(true);
const collectionId = ref<string>('');
const collection = ref<ImageCollection>();
const hoveredImage = ref<string>('');
const route = useRoute()
const selectedImage = ref<Image | null>(null);



onMounted(async () => {
    const id = route.params.collectionId as string
    collectionId.value = id
    await getCollection()
    console.log(collection.value)
    isLoading.value = false
})

watch(route, async (to, from) => {
    isLoading.value = true
    collectionId.value = to.params.collectionId as string;
    await getCollection()
    isLoading.value = false
}, { immediate: true })

function selectImage(image: Image) {
    selectedImage.value = image
}
function nextImage() {
    const currentIndex = collection.value?.images.findIndex(image => image.imageId === selectedImage.value?.imageId);
    if (currentIndex !== undefined && currentIndex !== -1 && currentIndex < (collection.value?.images.length ?? 0) - 1) {
        selectedImage.value = collection.value?.images[currentIndex + 1] || null
    }
}

function prevImage() {
    const currentIndex = collection.value?.images.findIndex(image => image.imageId === selectedImage.value?.imageId);
    if (currentIndex !== undefined && currentIndex !== -1 && currentIndex > 0) {
        selectedImage.value = currentIndex > 0 ? collection.value?.images[currentIndex - 1] as Image | null : null
    }
}
function clearSelectedImage() {
    selectedImage.value = null;
}
const imageNumber = computed(() => {
    if (!selectedImage.value || !collection.value) return 0;
    return selectedImage.value ? collection.value.images.findIndex(image => image.imageId === selectedImage.value?.imageId) + 1 : 0
})

const hasNextImage = computed(() => {
    if (!selectedImage.value || !collection.value) return false;
    const currentIndex = collection.value.images.findIndex(image => image.imageId === (selectedImage.value?.imageId ?? ''))
    return currentIndex < collection.value.images.length - 1
})

const hasPrevImage = computed(() => {
    if (!selectedImage.value || !collection.value) return false;
    const currentIndex = collection.value.images.findIndex(image => image.imageId === (selectedImage.value?.imageId ?? ''))
    return currentIndex > 0;
})

async function getCollection() {
    collection.value = (await api.get(`imageCollections/${collectionId.value}`)).data as ImageCollection
}

</script>

<template>
    <div v-if="selectedImage"
        class="selected-image-wrapper"
        @click="clearSelectedImage">
        <div class="image-and-controls-wrapper">
            <button :class="{ 'opacity-0': !hasPrevImage, 'disabled': !hasPrevImage }"
                class="nav-button prev-button"
                @click.stop="prevImage">
                <IconChevronLeft class="nav-icon" />
            </button>
            <div class="selected-image-info">
                <img :src="selectedImage.uri"
                    :alt="selectedImage.description"
                    class="selected-image" />
                <div class="image-details">
                    <p>{{ imageNumber }}/{{ collection?.images.length || 0 }}</p>
                    <h3>{{ selectedImage.title || 'Untitled' }}</h3>
                    <p>{{ selectedImage.description || 'No description.' }}</p>
                </div>
            </div>
            <button :class="{ 'opacity-0': !hasNextImage, 'disabled': !hasNextImage }"
                class="nav-button next-button"
                @click.stop="nextImage">
                <IconChevronRight class="nav-icon" />
            </button>
            <button class="nav-button close-button"
                @click.stop="clearSelectedImage">
                <IconClose class="nav-icon" />
            </button>
        </div>
    </div>
    <div v-else-if="!isLoading && collection"
        class="collection-wrapper">
        <h2>{{ collection?.name }}</h2>
        <p>{{ collection?.description }}</p>
        <div class="image-wrapper">
            <template v-for="image in collection?.images"
                :key="image.imageId">
                <div class="image-container"
                    @click="selectImage(image)"
                    @mouseover="hoveredImage = image.imageId"
                    @mouseleave="hoveredImage = ''">
                    <img :src="image.uri"
                        :alt="image.description" />
                    <div v-if="hoveredImage === image.imageId"
                        class="image-overlay">
                        <span class="image-title">{{ image.title || 'Untitled' }}</span>
                    </div>
                </div>
            </template>
        </div>
    </div>
</template>

<style scoped>
.collection-wrapper {
    display: flex;
    flex-direction: column;
    margin-left: 1rem;
    margin-right: 1rem;
}

h2 {
    font-size: 2rem;
    letter-spacing: 2px;
}

h2,
p {
    margin: 0 1rem 1rem 1rem;
}

.image-wrapper {
    display: flex;
    flex-direction: row;
    flex-wrap: wrap;
}

.image-and-controls-wrapper {
    display: flex;
    width: inherit;
    justify-content: space-between;
    align-items: center;
    max-width: var(--max-width);
}

.image-container {
    position: relative;
    height: 20rem;
    margin: 1rem;
    flex-basis: calc(20% - 2rem);
    overflow: hidden;
    cursor: pointer;
    transition: transform 0.1s ease-in-out;
}

img {
    width: 100%;
    height: inherit;
    object-fit: cover;
    transition: transform 0.1s ease-in-out;
}

.image-container:hover img {
    transform: scale(1.05);
    filter: brightness(0.3) blur(8px);
}

.image-overlay {
    position: absolute;
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
}

.image-title {
    font-size: 2rem;
    text-transform: uppercase;
    font-weight: 500;
    color: var(--lightest-color);
    text-align: center;
    text-shadow: var(--text-shadow);
}

.selected-image-wrapper {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    display: flex;
    justify-content: center;
    backdrop-filter: brightness(0.1) blur(4px);
    z-index: 3;
}



.image-details {
    position: fixed;
    bottom: 1rem;
    left: 50%;
    transform: translateX(-50%);
    color: var(--lightest-color);
    display: flex;
    flex-direction: column;
    align-items: center;
}

.nav-button {
    font-size: 2rem;
    padding: 2rem;
    background: none;
    border: none;
    cursor: pointer;

}

.nav-icon {
    height: 3.5rem;
    width: 3.5rem;
    color: var(--light-color);
}

.nav-button:hover .nav-icon {
    transform: scale(1.2);
    color: var(--lightest-color);

}

.close-button {
    position: absolute;
    top: 1rem;
    right: calc((1rem + 100vw - var(--max-width)) / 2);
    ;
}

.close-button .nav-icon {
    color: var(--light-color);
    height: 2.5rem;
    width: 2.5rem;
}

.selected-image {
    max-height: 80vh;
    max-width: 80vw;
    object-fit: contain;
    cursor: pointer;
}

.opacity-0 {
    opacity: 0;
}

.disabled {
    pointer-events: none;
}

@media (max-width: 768px) {
    .image-wrapper {
        margin: 0 0.5rem 0 0.5rem;
    }

    .image-container {
        margin: 0.5rem;
        height: 10rem;
        flex-basis: calc(33.333333% - 1rem);
    }

    .image-container:hover {
        transform: none;
    }

    .image-container:hover img {
        transform: none;
        filter: none;
    }

    .image-title {
        display: none;
    }

    .collection-wrapper {
        margin: 0;
    }
}
</style>