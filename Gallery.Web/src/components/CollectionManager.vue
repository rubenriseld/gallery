<script setup lang="ts">
import type { ImageCollection, ImageCollectionFormFields } from '@/types/imageCollection'
import Modal from './Modal.vue';
import api from '@/api'
import { ref, onMounted, computed } from 'vue'
import { Operation } from '@/assets/enums/operation';
import IconDelete from './icons/IconDelete.vue';
import IconAdd from './icons/IconAdd.vue';

const collections = ref<ImageCollection[]>([])
const selectedCollection = ref<ImageCollectionFormFields | null>(null)
const selectedOperation = ref<Operation>(Operation.None)
const formData = ref()
const isDeleteOperation = computed(() => selectedOperation.value === Operation.Delete)

const showDeleteButton = ref<boolean[]>([]);

onMounted(async () => {
    await getCollections()
    showDeleteButton.value = Array(collections.value.length).fill(false);
})

async function openCreateForm() {
    selectedCollection.value = {
        imageCollectionId: '',
        name: '',
        description: '',
    }
    formData.value = { ...selectedCollection.value }
    selectedOperation.value = Operation.Create
}
function openUpdateForm(collection: ImageCollection) {
    selectedCollection.value = collection
    formData.value = { ...selectedCollection.value }
    selectedOperation.value = Operation.Update
}

async function getCollections() {
    collections.value = (await api.get('imageCollections')).data as ImageCollection[]
}
function openDeletePromptModal(collection: ImageCollection) {
    selectedOperation.value = Operation.Delete
    selectedCollection.value = collection
}
function clearSelections() {
    selectedOperation.value = Operation.None
    selectedCollection.value = null
}
async function updateCollection() {
    if (selectedCollection.value) {
        console.log(formData.value)
        await api.put('imageCollections/' + selectedCollection.value.imageCollectionId, JSON.stringify(formData.value))
        await getCollections()
    }
    clearSelections()
}
async function createCollection() {
    if (selectedCollection.value) {
        await api.post('imageCollections/', JSON.stringify(formData.value))
        await getCollections()
    }
    clearSelections()
}
async function deleteCollection() {
    if (selectedCollection.value) {
        await api.delete('imageCollections/' + selectedCollection.value.imageCollectionId)
        await getCollections()
    }
    clearSelections()
}

function toggleDeleteButton(index: number, show: boolean) {
    showDeleteButton.value[index] = show;
}
</script>

<template>
    <div>
        <div class='collection' v-for='(collection, index) in collections' :key='index'
            @click='openUpdateForm(collection)' @mouseenter='toggleDeleteButton(index, true)'
            @mouseleave='toggleDeleteButton(index, false)'>
            <h4>{{ collection.name }}</h4>
            <p>{{ collection.description }}</p>
            <button v-if='showDeleteButton[index]' class='delete-button'
                @click.stop='openDeletePromptModal(collection)'>
                <IconDelete />
            </button>
        </div>
        <Modal v-model:isVisible='isDeleteOperation' :confirm="deleteCollection"
            :modalText='`Are you sure you want to delete ${selectedCollection?.name}?`' :confirmText='`Delete`'
            @close-modal='clearSelections' />
        <button class='add-button' @click='openCreateForm'>
            <IconAdd />
        </button>

        <form v-if='selectedOperation === Operation.Update' @submit.prevent='updateCollection'>
            <input id='name' v-model='formData.name' required>
            <input id='description' v-model='formData.description'>
            <!-- <template v-for="(property, index) in selectedCollection">
                <input v-if="!isPrimaryKey(property)" :key="index" v-model="formData[index]">
            </template> -->
            <button class='cancel-button' @click='clearSelections'>Cancel</button>
            <button class='submit-button' type='submit'>Update</button>
        </form>
        <form v-else-if='selectedOperation === Operation.Create' @submit.prevent='createCollection'>
            <input id='name' v-model='formData.name' required>
            <input id='description' v-model='formData.description'>
            <!-- <template v-for="(property, index) in selectedCollection">
                <input v-if="!isPrimaryKey(property)" :key="index" v-model="formData[index]">
            </template> -->
            <button class='submit-button' type='submit'>Create</button>
        </form>
    </div>
</template>

<style scoped>
.collection {
    border: 1px black solid;
    position: relative;
}

.delete-button {
    position: absolute;
    top: 5px;
    right: 5px;
    display: none;
}

.collection:hover .delete-button {
    display: block;
}
</style>
