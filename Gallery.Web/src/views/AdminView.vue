<script setup lang='ts'>
import CollectionManager from '@/components/CollectionManager.vue'
import TagManager from '@/components/TagManager.vue'
import ImageManager from '@/components/ImageManager.vue'
import TabMenu from '@/components/TabMenu.vue';
import api from '@/api';
import { onMounted, ref } from 'vue';
import type { ImageCollection } from '@/assets/types/imageCollection';
import type { Image } from '@/assets/types/image';
import type { Tag } from '@/assets/types/tag';

const collections = ref<ImageCollection[]>([])
const images = ref<Image[]>([])
const tags = ref<Tag[]>([])

onMounted(async () => {
    await refreshData()
   
})
async function refreshData() {
    await getTags()
    await getImages()
    await getCollections()
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
        <template #Collections>
          <CollectionManager :collections="collections" :refresh="refreshData" />
        </template>
        <template #Tags>
          <TagManager :tags="tags" :refresh="refreshData" />
        </template>
        <template #Images>
          <ImageManager :collections="collections" :tags="tags" :images="images" :refresh="refreshData" />
        </template>
      </TabMenu>
    </div>
  </template>