<script setup lang="ts">
import type { Image, ImageFormFields } from '@/assets/types/image'
import Modal from './Modal.vue';
import api from '@/api'
import { ref, onMounted, computed } from 'vue'
import { Operation } from '@/assets/enums/operation';
import IconDelete from './icons/IconDelete.vue';
import IconAdd from './icons/IconAdd.vue';
import type { ImageCollection } from '@/assets/types/imageCollection';
import type { Tag } from '@/assets/types/tag';
const selectedImage = ref<ImageFormFields | null>(null)
const selectedOperation = ref<Operation>(Operation.None)
const formData = ref()
const isDeleteOperation = computed(() => selectedOperation.value === Operation.Delete)
const showDeleteButton = ref<boolean[]>([])

const props = defineProps({
    images: {
        type: Object as () => (Image[]),
        default: []
    },
    collections: {
        type: Object as () => (ImageCollection[]),
        default: []
    },
    tags: {
        type: Object as () => (Tag[]),
        default: []
    },
    refresh: Function
})
onMounted(async () => {
    showDeleteButton.value = Array(props.images.length).fill(false)
})

const handleFileSelect = (event: Event) => {
    const fileList = (event.target as HTMLInputElement).files
    formData.value = []

    if (fileList) {
        for (let i = 0; i < fileList.length; i++) {
            (formData.value as File[]).push(fileList[i])
        }
    }
}

async function openCreateForm() {
    selectedImage.value = {
        imageId: '',
        title: '',
        description: '',
        imageCollectionId: '',
        tagIds: []
    }
    formData.value = { ...selectedImage.value }
    selectedOperation.value = Operation.Create
}

function openUpdateForm(image: Image) {
    selectedImage.value = convertImageToImageFormFields(image)
    const tagIds = image.tags.map(tag => tag.tagId)
    formData.value = { ...selectedImage.value, tagIds }
    selectedOperation.value = Operation.Update;
}

function convertImageToImageFormFields(image: Image) {
    const { imageId, title, description, imageCollectionId, tags } = image

    const tagIds = tags.map(tag => tag.tagId)

    const imageFormFields: ImageFormFields = {
        imageId,
        title,
        description,
        imageCollectionId,
        tagIds
    }
    return imageFormFields
}

function openDeletePromptModal(image: Image) {
    selectedOperation.value = Operation.Delete
    selectedImage.value = convertImageToImageFormFields(image)
}
function clearSelections() {
    selectedOperation.value = Operation.None
    selectedImage.value = null
}
async function updateImage() {
    if (selectedImage.value && formData.value) {
        await api.put('images/' + selectedImage.value.imageId, JSON.stringify(formData.value))
    }
    props.refresh && props.refresh()

    clearSelections()
}

async function createImage() {
    if (selectedImage.value) {
        await api.post('images/', formData.value, {
            headers: {
                'Content-Type': 'multipart/form-data'
            }
        })
    }
    props.refresh && props.refresh()

    clearSelections()
}
async function deleteImage() {
    if (selectedImage.value) {
        await api.delete('images/' + selectedImage.value.imageId)
    }
    props.refresh && props.refresh()

    clearSelections()
}

function toggleDeleteButton(index: number, show: boolean) {
    showDeleteButton.value[index] = show
}
</script>

<template>
    <div>
        <div class='image' v-for='(image, index) in images' :key='index' @click='openUpdateForm(image)'
            @mouseenter='toggleDeleteButton(index, true)' @mouseleave='toggleDeleteButton(index, false)'>
            <h4>{{ image.title }}</h4>
            <p>{{ image.imageCollectionName }}</p>
            <img :src='image.uri' :alt='image.title'>
            <p v-for='(tag, index) in image.tags' :key='index'>{{ tag.name }}</p>
            <button v-if='showDeleteButton[index]' class='delete-button' @click.stop='openDeletePromptModal(image)'>
                <IconDelete />
            </button>
        </div>
        <Modal v-model:isVisible='isDeleteOperation' :confirm="deleteImage"
            :modalText='`Are you sure you want to delete ${selectedImage?.title}?`' :confirmText='`Delete`'
            @close-modal='clearSelections' />

        <button class='add-button' @click='openCreateForm'>
            <IconAdd />
        </button>

        <form v-if='selectedOperation === Operation.Update' @submit.prevent='updateImage'>
            <input id='title' v-model='formData.title' required>
            <input id='description' v-model='formData.description'>
            <select id='imageCollectionId' v-model='formData.imageCollectionId'>
                <option v-for='(imageCollection, index) in collections' :key='index'
                    :value='imageCollection.imageCollectionId'>{{ imageCollection.name }}</option>
            </select>
            <div>
                <label v-for="(tag, tagIndex) in tags" :key="tagIndex">
                    <input type="checkbox" :value="tag.tagId" v-model="formData.tagIds">
                    {{ tag.name }}
                </label>
            </div>
            <button class='cancel-button' @click='clearSelections'>Cancel</button>
            <button class='submit-button' type='submit'>Update</button>
        </form>
        <form v-else-if='selectedOperation === Operation.Create' @submit.prevent='createImage'>
            <input type='file' multiple @change='handleFileSelect'>
            <button class='submit-button' type='submit'>Upload</button>
        </form>
    </div>
</template>

<style scoped>
.image {
    border: 1px black solid;
    position: relative;
}

.delete-button {
    position: absolute;
    top: 5px;
    right: 5px;
    display: none;
}

.image:hover .delete-button {
    display: block;
}

.multiselect {
    width: 50px !important;
    position: relative;
}
</style>
