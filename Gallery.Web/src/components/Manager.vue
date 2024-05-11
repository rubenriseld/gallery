<script setup lang="ts">
import Modal from './Modal.vue';
import IconBack from './icons/IconBack.vue';
import ComponentButton from './ComponentButton.vue';
import FormButtons from './FormButtons.vue';
import CollectionDropdownSelect from './CollectionDropdownSelect.vue';
import TagWrapper from './TagWrapper.vue';
import FormInput from './FormInput.vue';

import { ref, onMounted, computed, watch } from 'vue'

import { Operation } from '@/assets/enums/operation';
import type { Image, ImageFormFields, ImageCollection, ImageCollectionFormFields, Tag, TagFormFields, ManagerObjectType, ImagePreview } from '@/assets/types'

const selectedObject = ref<ImageFormFields | ImageCollectionFormFields | TagFormFields | null>(null)
const objectsToManage = ref<Image[] | ImageCollection[] | Tag[]>([])
const imagePreviews = ref<ImagePreview[]>([])
const selectedOperation = ref<Operation>(Operation.None)
const canSelectCoverImage = ref<boolean>(false)
const isDeleteOperation = computed(() => selectedOperation.value === Operation.Delete)
const formData = ref()

const props = defineProps({
    objectType: {
        type: String as () => ManagerObjectType,
        required: true
    },
    images: {
        type: Array as () => (Image[]),
        default: []
    },
    collections: {
        type: Array as () => (ImageCollection[]),
        default: []
    },
    tags: {
        type: Array as () => (Tag[]),
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

// Function to handle the file select event
function handleFileSelect(event: Event) {
    const fileList = (event.target as HTMLInputElement).files
    formData.value = []

    if (fileList) {
        for (let i = 0; i < fileList.length; i++) {
            // Add the file to the formData array
            (formData.value as File[]).push(fileList[i])

            // Create a preview of the image
            const reader = new FileReader();
            reader.onload = (x) => {
                imagePreviews.value.push({
                    imageFile: fileList[i] as File,
                    imageSrc: x.target?.result as string
                })
            }
            reader.readAsDataURL(fileList[i]);
        }
    }
}

async function openCreateForm() {
    if (props.objectType === 'images') {
        selectedObject.value = {
            uri: '',
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
        formData.value = { ...selectedObject.value, coverImageId: (object as ImageCollection).coverImage?.imageId || '' }
        console.log("update form opened", formData.value)
    }
    else if (props.objectType === 'tags') {
        selectedObject.value = object as Tag
        formData.value = { ...selectedObject.value }
    }
    selectedOperation.value = Operation.Update;
}

function updateFormDataWithEmittedValue(formDataIndex: string, event: any) {
    formData.value = { ...formData.value, [formDataIndex]: event }
}

function convertImageToImageFormFields(image: Image) {
    const { uri, imageId, title, description, imageCollectionId, tags } = image

    const tagIds = tags.map(tag => tag.tagId)

    const imageFormFields: ImageFormFields = {
        uri,
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
        selectedObject.value = object as Image
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
    imagePreviews.value = []
}
async function performAction(action: ('update' | 'create' | 'delete')) {
    if (selectedObject.value) {
        await props[action](formData.value as ImageFormFields | ImageCollectionFormFields | TagFormFields)
        await props.refresh()
        clearSelections()
    }
}
function toggleCanSelectCoverImage() {
    canSelectCoverImage.value = !canSelectCoverImage.value
}
function selectCoverImage(imageId: string) {
    if (!canSelectCoverImage.value) return
    const previousExistingCoverImageId = formData.value.coverImageId
    formData.value = { ...formData.value, coverImageId: (imageId === previousExistingCoverImageId ? null : imageId) }
}
function cancelSelectCoverImage() {
    formData.value = { ...formData.value, coverImageId: (selectedObject.value as ImageCollection).coverImage?.imageId || '' }
    canSelectCoverImage.value = false
}
function isSelectedCoverImage(imageId: string): boolean {
    return (imageId === (formData.value.coverImageId))
}
function shouldRenderInput(formDataIndex: string): boolean {
    const excludedFormFields = ['tagId', 'imageId', 'imageCollectionId', 'tagIds', 'coverImageId', 'uri', 'imageCollectionName', 'coverImage', 'images']
    return !excludedFormFields.includes(formDataIndex)
}
function capitalize(string: string): string {
    return (string).charAt(0).toUpperCase() + (string).slice(1)
}
</script>

<template>
    <Modal v-model:isVisible='isDeleteOperation'
        :confirm="() => performAction('delete')"
        :modalText="objectType === 'images' ?
            `Are you sure you want to delete ${(selectedObject as ImageFormFields)?.title || 'this image'}?` :
            `Are you sure you want to delete ${(selectedObject as ImageCollectionFormFields | TagFormFields)?.name || 'this item'}?`"
        :confirmText='`Delete`'
        @close-modal='clearSelections' />

    <div v-if="!selectedObject"
        class="manager-wrapper">
        <div class="manager-menu">
            <ComponentButton buttonType="secondary"
                buttonText="Select multiple" />
            <ComponentButton buttonType="primary"
                :onClick='openCreateForm'
                :buttonText="objectType !== 'images' ? `Add ${objectType.slice(0, -1)}` : `Add ${objectType}`" />
        </div>
        <div class="object-wrapper">
            <div v-for='(object, index) in objectsToManage'
                :class="{ 'image-object': objectType === 'images', 'collection-object': objectType === 'collections', 'tag-object': objectType === 'tags' }"
                :key='index'
                @click='openUpdateForm(object)'>
                <p v-if="objectType !== 'images'"
                    class="object-name">{{ (object as ImageCollection | Tag).name }}</p>
                <img v-else
                    :src='(object as Image).uri'
                    :alt='(object as Image).title'>
            </div>
        </div>
    </div>
    <div v-else
        class="manager-wrapper">
        <div class="manager-menu collection-menu">
            <IconBack class="go-back-button"
                @click='clearSelections'></IconBack>
            <div v-if="objectType === 'collections' && selectedOperation !== Operation.Create"
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
        </div>
        <div class="form-content">
            <div v-if="objectType !== 'images'"
                class="collection-image-preview-wrapper">
                <!-- <h4 class="object-name">{{ (selectedObject as ImageCollection | Tag).name }}</h4> -->

                <div v-if="objectType === 'collections'"
                    class="collection-image-preview-container">
                    <img v-for='(image, index) in (selectedObject as ImageCollection).images'
                        @click="selectCoverImage(image.imageId)"
                        :key='index'
                        :src='(image.uri)'
                        :alt='image.title'
                        :class="`collection-image-preview-object 
                        ${isSelectedCoverImage(image.imageId) && 'selected-cover-image'}
                        ${canSelectCoverImage && 'selectable-image-preview-object'}`">
                </div>
            </div>
            <img v-else-if="selectedOperation !== Operation.Create"
                :src='(selectedObject as unknown as Image).uri'
                :alt='(selectedObject as unknown as Image).title'
                class="form-image-preview">
            <div v-else
                class="upload-image-preview-wrapper">
                <img v-for='(image, index) in imagePreviews'
                    :key='index'
                    :src='(image.imageSrc)'
                    :alt='image.imageSrc'
                    class="upload-image-preview-object">
            </div>
            <form v-if='selectedObject && selectedOperation !== Operation.Create'
                @submit.prevent="performAction('update')">
                <div>
                    <template v-for="(property, index) in selectedObject">
                        <FormInput v-if="shouldRenderInput(index)"
                            :key="index"
                            :label="capitalize(index)"
                            :placeholder="capitalize(index)"
                            :modelValue="formData[index]"
                            @update:modelValue="updateFormDataWithEmittedValue(index, $event)" />
                    </template>
                    <template v-if="objectType === 'images'">
                        <CollectionDropdownSelect :imageCollections="collections"
                            :modelValue="formData.imageCollectionId"
                            @update:modelValue="formData.imageCollectionId = $event" />
                        <TagWrapper :tags="tags"
                            :formData="formData" />
                    </template>
                    <FormButtons :cancelAction="clearSelections"
                        submitText="Update" />
                </div>

                <ComponentButton buttonType="warning"
                    :onClick='() => openDeletePromptModal(selectedObject as Image | ImageCollection | Tag)'
                    buttonText="Delete permanently" />
            </form>

            <form v-else-if='selectedOperation === Operation.Create'
                @submit.prevent="performAction('create')">
                <template v-if="objectType !== 'images'"
                    v-for="(property, index) in selectedObject">
                    <FormInput v-if="shouldRenderInput(index)"
                        :key="index"
                        :label="capitalize(index)"
                        :placeholder="capitalize(index)"
                        :modelValue="formData[index]"
                        @update:modelValue="updateFormDataWithEmittedValue(index, $event)" />
                </template>
                <div v-else
                    class="image-upload-wrapper">
                    <input class="file-input"
                        type='file'
                        multiple
                        @change='handleFileSelect'>
                </div>
                <FormButtons :cancelAction="clearSelections"
                    :submitText="objectType !== 'images' ? 'Add' : 'Upload'" />
            </form>
        </div>
    </div>
</template>

<style scoped>
.manager-wrapper {
    display: flex;
    flex-direction: column;
    align-items: space-between;
    width: 100%;
    padding: 2rem;
    background-color: var(--lightest-color);
}

.manager-menu {
    display: flex;
    justify-content: space-between;
    background-color: var(--lightest-color);
}

.collection-menu {
    justify-content: flex-start;
    align-items: center;
}

.collection-menu button {
    margin-left: 1rem;
}

.select-cover-image-button-wrapper {
    display: flex;
}

.go-back-button {
    width: 2rem;
    height: 2rem;
    font-weight: 900;
    cursor: pointer;
    color: var(--primary-color);
    transition: all 0.1s ease-in-out;
}

.go-back-button:hover {
    color: var(--primary-color-hover);
    transform: scale(1.2);
}

.object-wrapper {
    display: flex;
    flex-wrap: wrap;
    margin: 1rem -0.5rem 1rem -0.5rem;
}

.image-object {
    width: 20%;
    cursor: pointer;
    padding: 0.5rem;
    box-sizing: border-box;
    height: 20rem;
    display: flex;
    overflow: hidden;
}

.image-object:hover {
    filter: brightness(0.5);

}

.collection-object:hover,
.tag-object:hover {
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

.tag-object {
    background-color: var(--light-color);
    font-size: 1.2rem;
    padding: 1rem;
    margin: 0.5rem;
    cursor: pointer;
}

.object-name {
    padding-left: 0.5rem;
}

img {
    width: 100%;
    height: 100%;
    object-fit: cover;
}

.form-image-preview {
    width: 50%;
    max-height: 40rem;
    object-fit: cover;
}

.upload-image-preview-wrapper,
.collection-image-preview-wrapper {
    width: 50%;
    display: flex;
    flex-direction: row;
    flex-wrap: wrap;
}

.collection-image-preview-container {
    width: 100%;
    display: flex;
    flex-direction: row;
    flex-wrap: wrap;
}

.upload-image-preview-object,
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

.form-wrapper {
    margin: 2rem;
    width: 100%;
    display: flex;
    flex-direction: column;
}

.form-content {
    display: flex;
    flex-direction: row;
    justify-content: space-between;
    margin-top: 2rem;
}

form {
    display: flex;
    width: 40%;
    flex-direction: column;
    justify-content: space-between;
    max-height: 40rem;
}

label {
    margin-bottom: 0.5rem;
}

input {
    height: 2rem;
    padding: 0.25rem;
    font-size: 1rem;
    font-family: inherit;
}

@media (max-width: 768px) {
    .manager-wrapper {
        padding: 0;
    }

    .image-object {
        width: 33.3333%;
        height: 12rem;
    }

    .form-wrapper {
        margin: 0 1rem 1rem 1rem;
    }

    .form-content {
        margin-top: 0;
        flex-direction: column;
    }

    .form-image-preview {
        width: 100%;
        height: 20rem;
        margin-bottom: 2rem;
    }

    form {
        width: 100%;
    }

    .upload-image-preview-wrapper {
        order: 1;
    }
}
</style>