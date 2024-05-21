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
function clearSelectedImage(event: MouseEvent) {
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
        class="selected-image-wrapper">
        <div class="image-and-controls-wrapper">
            <button :class="{ 'opacity-0': !hasPrevImage, 'disabled': !hasPrevImage }"
                class="nav-button prev-button"
                @click.stop="prevImage">
                <IconChevronLeft class="nav-icon" />
            </button>
            <div class="selected-image-info">
                <div class="collection-details">
                    <p class="collection-name">{{ collection?.name }}</p>
                    <p class="image-counter">{{ imageNumber }} / {{ collection?.images.length || 0 }}</p>
                </div>
                <img :src="selectedImage.uri"
                    :alt="selectedImage.title ? `'Artwork titled ${selectedImage.title}` : 'Untitled artwork'"
                    class="selected-image" />
                <div class="image-details">
                    <p class="image-title">{{ selectedImage.title || 'Untitled' }}</p>
                    <p class="image-description">{{ selectedImage.description || 'No description.' }}
                        {{ selectedImage.sold ? 'SOLD' : '' }}</p>
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
            <div class="selected-image-background"
                @click.stop="clearSelectedImage"></div>
        </div>
    </div>
    <div v-else-if="!isLoading && collection"
        class="collection-wrapper">
        <div class="image-wrapper">
            <template v-for="image in collection?.images"
                :key="image.imageId">
                <div class="image-container"
                    @click="selectImage(image)"
                    @mouseover="hoveredImage = image.imageId"
                    @mouseleave="hoveredImage = ''">
                    <img :src="image.uri"
                        :alt="image.title ? `'Artwork titled ${image.title}` : 'Untitled artwork'" />
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

.collection-wrapper h2,
.collection-wrapper p {
    margin-left: 1rem;
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

span {
    text-transform: uppercase;
}

button {
    z-index: 10;
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



.selected-image-wrapper {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    display: flex;
    justify-content: center;
    backdrop-filter: brightness(0.2) blur(80px);
    z-index: 3;
}

.selected-image-background {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
}

.collection-details {
    position: fixed;
    height: 3rem;
    font-size: 2rem;
    top: 1rem;
    left: 50%;
    transform: translateX(-50%);
    display: flex;
    flex-direction: column;
    align-items: center;
}

.image-counter {
    color: var(--lightest-color);
}

.image-details {
    position: fixed;
    bottom: 1rem;
    height: 3rem;
    left: 50%;
    transform: translateX(-50%);
    display: flex;
    flex-direction: column;
    align-items: center;
}



.image-counter,
.image-title {
    font-weight: 500;
    color: var(--lightest-color);
}

.collection-name,
.image-description {
    color: var(--light-color);
    opacity: 0.8;
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
}

.close-button .nav-icon {
    height: 2.5rem;
    width: 2.5rem;
}

.selected-image {
    max-height: 80vh;
    max-width: 80vw;
    position: fixed;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    object-fit: contain;
    cursor: pointer;
}

.opacity-0 {
    opacity: 0;
}

.disabled {
    pointer-events: none;
}

@media (max-width:1920px) {
    .close-button {
        right: 1rem;
    }
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

    .collection-wrapper {
        margin: 0;
    }

    .selected-image-info {
        height: 80%;
        width: 100%;
        display: flex;
        align-self: center;
    }

    .selected-image {
        width: 100%;
        max-height: none;
        max-width: none;
        position: relative;
    }

    .image-and-controls-wrapper {
        background-color: var(--lightest-color);
    }

    .collection-details {
        display: none;
    }

    .image-details {
        bottom: 1.5rem;
    }

    .image-title,
    .image-description {
        color: var(--darker-color);
    }

    .nav-button {
        position: fixed;
        bottom: 1rem;
        padding: 0;
    }

    .close-button {
        height: 3rem;
    }

    .prev-button {
        left: 1rem;
    }

    .next-button {
        right: 1rem;
    }

    .nav-icon {
        color: var(--dark-color);
    }

    .nav-button:hover .nav-icon {
        color: var(--darker-color);
    }
}
</style>