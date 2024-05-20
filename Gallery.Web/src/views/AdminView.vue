<script setup lang='ts'>
import TabMenu from '@/components/admin/TabMenu.vue'
import CollectionManager from '@/components/admin/CollectionManager.vue'
import TagManager from '@/components/admin/TagManager.vue'
import ImageManager from '@/components/admin/ImageManager.vue'

import api from '@/api'
import { onMounted, ref } from 'vue'

import type { ImageCollection, Image, Tag } from '@/assets/types'

const collections = ref<ImageCollection[]>([])
const images = ref<Image[]>([])
const tags = ref<Tag[]>([])
const isLoading = ref(true)

onMounted(async () => {
    isLoading.value = true
    await fetchData()
    isLoading.value = false
})
async function fetchData() {
    await getTags()
    await getImages()
    await getCollections()
}
async function getTags() {
    try {
        const response = await api.get('tags')
        if (response.status === 200) {
            tags.value = response.data as Tag[];
        }
    }
    catch (error) {
        console.error('Error fetching tags:', error)
    }
}
async function getImages() {
    try {
        const response = await api.get('images')
        if (response.status === 200) {
            images.value = response.data as Image[];
        }
    }
    catch (error) {
        console.error('Error fetching images:', error)
    }
}
async function getCollections() {
    try {
        const response = await api.get('imageCollections')
        if (response.status === 200) {
            collections.value = (response.data as ImageCollection[]).filter(collection => collection.shouldBeDisplayed === true)
        }
    }
    catch (error) {
        console.error('Error fetching collections:', error)
    }
}
</script>

<template>
    <div>
        <TabMenu>
            <template #Collections
                v-if="!isLoading">
                <CollectionManager :key="'collections'"
                    :collections="collections"
                    :refresh="getCollections" />
            </template>
            <template #Tags
                v-if="!isLoading">
                <TagManager :key="'tags'"
                    :tags="tags"
                    :refresh="getTags" />
            </template>
            <template #Images
                v-if="!isLoading">
                <ImageManager :key="'images'"
                    :images="images"
                    :tags="tags"
                    :collections="collections"
                    :refresh="fetchData" />
            </template>
        </TabMenu>
    </div>
</template>