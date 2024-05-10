<script setup lang='ts'>
import TabMenu from '@/components/TabMenu.vue'
import Manager from '@/components/Manager.vue'

import api from '@/api'
import * as requests from '@/requests'
import { onMounted, ref } from 'vue'

import type { ImageCollection, Image, Tag } from '@/assets/types'

const collections = ref<ImageCollection[]>([])
const images = ref<Image[]>([])
const tags = ref<Tag[]>([])
const isLoading = ref(true)

onMounted(async () => {
    await refreshData()
})
async function refreshData() {
    isLoading.value = true
    await getTags()
    await getImages()
    await getCollections()
    isLoading.value = false
}
async function getTags() {
    tags.value = (await api.get('tags')).data as Tag[]
}
async function getImages() {
    images.value = (await api.get('images')).data as Image[]
}
async function getCollections() {
    collections.value = (await api.get('imageCollections')).data as ImageCollection[]
}

</script>

<template>
    <div>
        <TabMenu>
            <template #Collections
                v-if="!isLoading">
                <Manager :key="'collections'"
                    :collections="collections"
                    :objectType="'collections'"
                    :refresh="refreshData"
                    :create="requests.createCollection"
                    :update="requests.updateCollection"
                    :delete="requests.deleteCollection" />
            </template>
            <template #Tags
                v-if="!isLoading">
                <Manager :key="'tags'"
                    :tags="tags"
                    :objectType="'tags'"
                    :refresh="refreshData"
                    :create="requests.createTag"
                    :update="requests.updateTag"
                    :delete="requests.deleteTag" />
            </template>
            <template #Images
                v-if="!isLoading">
                <Manager :key="'images'"
                    :images="images"
                    :tags="tags"
                    :collections="collections"
                    :objectType="'images'"
                    :refresh="refreshData"
                    :create="requests.createImage"
                    :update="requests.updateImage"
                    :delete="requests.deleteImage" />
            </template>
        </TabMenu>
    </div>
</template>