<script setup lang="ts">
import Modal from './Modal.vue';
import IconDelete from './icons/IconDelete.vue';
import IconAdd from './icons/IconAdd.vue';
import { ref, onMounted, computed, watch } from 'vue'
import { Operation } from '@/assets/enums/operation';
import type { Image, ImageFormFields, ImageCollection, ImageCollectionFormFields, Tag, TagFormFields, ManagerObjectType } from '@/assets/types'

const selectedObject = ref<ImageFormFields | ImageCollectionFormFields | TagFormFields | null>(null)
const objectsToManage = ref<Image[] | ImageCollection[] | Tag[]>([])
const selectedOperation = ref<Operation>(Operation.None)
const formData = ref()
const isDeleteOperation = computed(() => selectedOperation.value === Operation.Delete)
const showDeleteButton = ref<boolean[]>([])

const props = defineProps({
    objectType: {
        type: String as () => ManagerObjectType,
        required: true
    },
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
    refresh: {
        type: Function,
        required: true
    },
    create: {
        type: Function,
        required: true
    },
    update: {
        type: Function,
        required: true
    },
    delete: {
        type: Function,
        required: true
    },
})

onMounted(async () => {
    showDeleteButton.value = Array(props[props.objectType].length).fill(false)
    updateObjectsToManage()
})
watch([() => props.images, () => props.collections, () => props.tags], () => {
    updateObjectsToManage();
})

function updateObjectsToManage() {
    if (props.objectType === 'images') {
        objectsToManage.value = props.images
    } else if (props.objectType === 'collections') {
        objectsToManage.value = props.collections
    } else if (props.objectType === 'tags') {
        objectsToManage.value = props.tags
    }
}

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
    if (props.objectType === 'images') {
        selectedObject.value = {
            imageId: '',
            title: '',
            description: '',
            imageCollectionId: '',
            tagIds: []
        }
    }
    else if (props.objectType === 'collections') {
        selectedObject.value = {
            imageCollectionId: '',
            name: '',
            description: '',
        }
    }
    else if (props.objectType === 'tags') {
        selectedObject.value = {
            tagId: '',
            name: '',
        }
    }
    formData.value = { ...selectedObject.value }
    selectedOperation.value = Operation.Create
}

function openUpdateForm(object: Image | ImageCollection | Tag) {
    if (props.objectType === 'images') {
        selectedObject.value = convertImageToImageFormFields(object as Image)
        const tagIds = (object as Image).tags.map(tag => tag.tagId)
        formData.value = { ...selectedObject.value, tagIds }
    }
    else if (props.objectType === 'collections') {
        selectedObject.value = object as ImageCollection
        formData.value = { ...selectedObject.value }
    }
    else if (props.objectType === 'tags') {
        selectedObject.value = object as Tag
        formData.value = { ...selectedObject.value }
    }
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

function openDeletePromptModal(object: Image | ImageCollection | Tag) {
    if (props.objectType === 'images') {
        selectedObject.value = convertImageToImageFormFields(object as Image)
    }
    else if (props.objectType === 'collections') {
        selectedObject.value = object as ImageCollection
    }
    else if (props.objectType === 'tags') {
        selectedObject.value = object as Tag
    }
    selectedOperation.value = Operation.Delete
}
function clearSelections() {
    selectedOperation.value = Operation.None
    selectedObject.value = null
}
async function performAction(action: ('update' | 'create' | 'delete')) {
    if (selectedObject.value) {
        await props[action](formData.value as ImageFormFields | ImageCollectionFormFields | TagFormFields)
        await props.refresh()
        clearSelections()
    }
}
function shouldRenderInput(index: string): boolean {
  return !index.includes('Id')
}
function toggleDeleteButton(index: number, show: boolean) {
    showDeleteButton.value[index] = show
}
</script>

<template>
    <div>
        <div class='object' v-for='(object, index) in (objectsToManage)' :key='index' @click='openUpdateForm(object)'
            @mouseenter='toggleDeleteButton(index, true)' @mouseleave='toggleDeleteButton(index, false)'>
            <h4>{{ objectType === 'images' ? (object as Image).title : (object as ImageCollection | Tag).name }}</h4>
            <template v-if="objectType === 'images'">
                <img :src='(object as Image).uri' height="100px" :alt='(object as Image).title'>
                <p v-for='(tag, index) in (object as Image).tags' :key='index'>{{ tag.name }}</p>
            </template>
            <button v-if='showDeleteButton[index]' class='delete-button' @click.stop='openDeletePromptModal(object)'>
                <IconDelete />
            </button>
        </div>
        <Modal v-model:isVisible='isDeleteOperation' :confirm="() => performAction('delete')" :modalText="objectType === 'images' ?
            `Are you sure you want to delete ${(selectedObject as ImageFormFields)?.title}?` :
            `Are you sure you want to delete ${(selectedObject as ImageCollectionFormFields | TagFormFields)?.name}?`"
            :confirmText='`Delete`' @close-modal='clearSelections' />

        <button class='add-button' @click='openCreateForm'>
            <IconAdd />
        </button>

        <form v-if='selectedOperation === Operation.Update' @submit.prevent="performAction('update')">
            <template v-for="(property, index) in selectedObject">
                <input v-if="shouldRenderInput(index)" :key="index" v-model="formData[index]">
            </template>
            <template v-if="objectType === 'images'">
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
            </template>
            <button class='cancel-button' @click='clearSelections'>Cancel</button>
            <button class='submit-button' type='submit'>Update</button>
        </form>
        <form v-else-if='selectedOperation === Operation.Create' @submit.prevent="performAction('create')">
            <input type='file' multiple @change='handleFileSelect'>
            <button class='submit-button' type='submit'>Upload</button>
        </form>
    </div>
</template>

<style scoped>
.object {
    border: 1px black solid;
    position: relative;
}

.delete-button {
    position: absolute;
    top: 5px;
    right: 5px;
    display: none;
}

.object:hover .delete-button {
    display: block;
}

.multiselect {
    width: 50px !important;
    position: relative;
}
</style>