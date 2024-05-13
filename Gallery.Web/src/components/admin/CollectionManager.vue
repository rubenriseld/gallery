<script setup lang="ts">
import Modal from '@/components/Modal.vue';
import IconBack from '@/components/icons/IconBack.vue';
import ComponentButton from '@/components/ComponentButton.vue';
import FormButtons from '@/components/form/FormButtons.vue';
import FormInput from '@/components/form/FormInput.vue';
import ManagerWrapper from '@/components/admin/ManagerWrapper.vue';
import IconMore from '@/components/icons/IconMore.vue';
import IconLess from '@/components/icons/IconLess.vue';

import api from '@/api';
import { ref, computed, watch } from 'vue'
import { capitalize, shouldRenderInput } from '@/assets/functions/managerHelperFunctions';

import { Operation } from '@/assets/enums/operation';
import type { Image, ImageCollection, ImageCollectionFormFields, Tag, ImagePreview } from '@/assets/types'

const props = defineProps({

    images: {
        type: Array as () => (Image[]),
        default: []
    },
    collections: {
        type: Array as () => (ImageCollection[]),
        default: []
    },
    refresh: {
        type: Function,
        required: true
    }
})

const selectedCollection = ref<ImageCollectionFormFields | null>(null)
const collections = ref<ImageCollection[]>(props.collections)
const imagePreviews = ref<ImagePreview[]>([])
const selectedOperation = ref<Operation>(Operation.None)
const canSelectCoverImage = ref<boolean>(false)
const isDeleteOperation = computed(() => selectedOperation.value === Operation.Delete)
const formData = ref()
const isMobilePreviewOpen = ref(false)

watch([() => props.collections], () => {
    collections.value = props.collections
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

function openUpdateForm(object: Image | ImageCollection | Tag) {
    selectedCollection.value = object as ImageCollection
    formData.value = { ...selectedCollection.value, coverImageId: (object as ImageCollection).coverImage?.imageId || '' }
    selectedOperation.value = Operation.Update;
}

function updateFormDataWithEmittedValue(formDataIndex: string, event: any) {
    formData.value = { ...formData.value, [formDataIndex]: event }
}

function openDeletePromptModal(object: Image | ImageCollection | Tag) {
    selectedCollection.value = object as ImageCollection
    selectedOperation.value = Operation.Delete
}
function clearSelections() {
    selectedOperation.value = Operation.None
    selectedCollection.value = null
    imagePreviews.value = []
}

function toggleCanSelectCoverImage() {
    canSelectCoverImage.value = !canSelectCoverImage.value
    isMobilePreviewOpen.value = canSelectCoverImage.value
}
function selectCoverImage(imageId: string) {
    if (!canSelectCoverImage.value) return
    const previousExistingCoverImageId = formData.value.coverImageId
    formData.value = { ...formData.value, coverImageId: (imageId === previousExistingCoverImageId ? null : imageId) }
}
function cancelSelectCoverImage() {
    formData.value = { ...formData.value, coverImageId: (selectedCollection.value as ImageCollection).coverImage?.imageId || '' }
    canSelectCoverImage.value = false
    isMobilePreviewOpen.value = false
}
function isSelectedCoverImage(imageId: string): boolean {
    return (imageId === (formData.value.coverImageId))
}

async function updateCollection() {
    await api.put('imageCollections/' + formData.value.imageCollectionId, JSON.stringify(formData.value))
    clearSelections()
    await props.refresh()
}
async function createCollection() {
    await api.post('imageCollections/', JSON.stringify(formData.value))
    clearSelections()
    await props.refresh()
}
async function deleteCollection() {
    await api.delete('imageCollections/' + formData.value.imageCollectionId)
    clearSelections()
    await props.refresh()
}
</script>

<template>
    <Modal v-model:isVisible='isDeleteOperation'
        :confirm="() => deleteCollection"
        :modalText="`Are you sure you want to delete ${selectedCollection?.name || 'this item'}?`"
        :confirmText='`Delete`'
        @close-modal='clearSelections' />

    <ManagerWrapper :objectIsSelected="selectedCollection !== null"
        :openCreateForm="openCreateForm"
        :openCreateFormText="'Add images'"
        :clearSelections="clearSelections">
        <template #objectDisplay>
            <div v-for='(object, index) in props.collections'
                class="collection-object"
                :key='index'
                @click='openUpdateForm(object)'>
                <p>{{ object.name }}</p>
            </div>
        </template>
        <template #additionalMenuItems
            :class="'collection-menu'">
            <div v-if="selectedOperation !== Operation.Create"
                class="select-cover-image-button-wrapper">
                <template v-if="!canSelectCoverImage">
                    <ComponentButton buttonType="secondary"
                        :onClick="toggleCanSelectCoverImage"
                        buttonText="Select cover image" />
                </template>
                <template v-else>
                    <ComponentButton buttonType="secondary"
                        :onClick="cancelSelectCoverImage"
                        buttonText="Cancel" />
                    <ComponentButton buttonType="primary"
                        :onClick="toggleCanSelectCoverImage"
                        buttonText="Select" />
                </template>
            </div>
        </template>
        <template #formContent>
            <button @click="isMobilePreviewOpen = !isMobilePreviewOpen"
                class="collection-preview-button">
                <IconMore v-if="!isMobilePreviewOpen" />
                <IconLess v-else />
                <p>Collection preview</p>
            </button>
            <div
                :class="`${isMobilePreviewOpen ? 'collection-image-preview-wrapper' : 'collection-image-preview-hidden'}`">

                <div class="collection-image-preview-container">
                    <img v-for='(image, index) in (selectedCollection as ImageCollection).images'
                        @click="selectCoverImage(image.imageId)"
                        :key='index'
                        :src='(image.uri)'
                        :alt='image.title'
                        :class="`collection-image-preview-object 
                        ${isSelectedCoverImage(image.imageId) ? 'selected-cover-image' : ''}
                        ${canSelectCoverImage ? 'selectable-image-preview-object' : ''}`">
                </div>
            </div>
            <form v-if='selectedCollection && selectedOperation !== Operation.Create'
                @submit.prevent="updateCollection">
                <h3 :class="`object-title ${!formData?.name ? 'untitled-object' : ''}`">
                    {{ formData?.name || "Untitled collection" }}</h3>
                <div>
                    <template v-for="(property, index) in selectedCollection">
                        <FormInput v-if="shouldRenderInput(index)"
                            :key="index"
                            :label="capitalize(index)"
                            :placeholder="capitalize(index)"
                            :modelValue="formData[index]"
                            @update:modelValue="updateFormDataWithEmittedValue(index, $event)" />
                    </template>
                    <FormButtons :cancelAction="clearSelections"
                        submitText="Update" />
                </div>
                <ComponentButton buttonType="warning"
                    :onClick='() => openDeletePromptModal(selectedCollection as ImageCollection)'
                    buttonText="Delete permanently" />
            </form>
            <form v-else-if='selectedOperation === Operation.Create'
                @submit.prevent="createCollection">
                <template v-for="(property, index) in selectedCollection">
                    <FormInput v-if="shouldRenderInput(index)"
                        :key="index"
                        :label="capitalize(index)"
                        :placeholder="capitalize(index)"
                        :modelValue="formData[index]"
                        @update:modelValue="updateFormDataWithEmittedValue(index, $event)" />
                </template>

                <FormButtons :cancelAction="clearSelections"
                    submitText="Add" />
            </form>
        </template>
    </ManagerWrapper>
</template>

<style scoped>
.select-cover-image-button-wrapper {
    margin-left: 1rem;
    display: flex;
}

.select-cover-image-button-wrapper>button:nth-child(1) {
    margin-right: 1rem;
}

.collection-object:hover {
    background-color: var(--mid-color);
}

.collection-object {
    background-color: var(--light-color);
    width: 100%;
    font-size: 1.2rem;
    margin: 0.5rem;
    padding: 1rem;
    cursor: pointer;
}

img {
    width: 100%;
    height: 100%;
    object-fit: cover;
}

.collection-image-preview-wrapper,
.collection-image-preview-hidden {
    width: 50%;
    display: flex;
    flex-direction: row;
    flex-wrap: wrap;
}

.collection-preview-button {
    border-bottom: 2px solid var(--slightly-dark-color);
    display: none;
}

.collection-preview-button>svg {
    width: 2rem;
    height: 2rem;
    color: var(--slightly-dark-color);
}

.collection-preview-button>p {
    font-size: 1.1rem;
    color: var(--dark-color);
}

.collection-image-preview-container {
    width: 100%;
    display: flex;
    flex-direction: row;
    flex-wrap: wrap;
}

.collection-image-preview-object {
    width: calc(20% - 1rem);
    height: 10rem;
    margin: 0.5rem;
    box-sizing: border-box;
    display: flex;
    overflow: hidden;
    object-fit: cover;
}

.collection-image-preview-object {
    opacity: 0.8;
}

.selectable-image-preview-object {
    cursor: pointer;
    transition: all 0.1s ease-in-out;
}

.selectable-image-preview-object:hover {
    transform: scale(1.05);
    opacity: 1;
}

.selected-cover-image {
    border: 4px solid var(--secondary-color);
}

@media (max-width: 768px) {
    .collection-image-preview-wrapper {
        width: calc(100% + 1rem);
        margin: 0 -0.5rem 0 -0.5rem;
    }

    .collection-image-preview-object {
        width: calc(33.3333% - 1rem);
        margin: 0.5rem;
        box-sizing: border-box;
        display: flex;
        overflow: hidden;
        object-fit: cover;
    }

    .collection-image-preview-hidden {
        display: none;
    }

    .collection-preview-button {
        cursor: pointer;
        display: flex;
        align-items: center;
        width: 100%;
        text-align: left;
        color: var(--darker-color);
    }
}
</style>