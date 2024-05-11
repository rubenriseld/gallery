<script setup lang="ts">
import Modal from '@/components/Modal.vue';
import IconBack from '@/components/icons/IconBack.vue';
import ComponentButton from '@/components/ComponentButton.vue';
import FormButtons from '@/components/form/FormButtons.vue';
import CollectionDropdownSelect from '@/components/form/CollectionDropdownSelect.vue';
import TagWrapper from '@/components/form/TagWrapper.vue';
import FormInput from '@/components/form/FormInput.vue';

import api from '@/api';
import { ref, computed, watch } from 'vue'
import { shouldRenderInput, capitalize } from '@/assets/functions/managerHelperFunctions';

import { Operation } from '@/assets/enums/operation';
import type { Image, ImageFormFields, ImageCollection, Tag, ImagePreview } from '@/assets/types'

const props = defineProps({
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
    }
})

const selectedImage = ref<ImageFormFields | null>(null)
const images = ref<Image[]>(props.images)
const collections = ref<ImageCollection[]>(props.collections)
const tags = ref<Tag[]>(props.tags)
const imagePreviews = ref<ImagePreview[]>([])
const selectedOperation = ref<Operation>(Operation.None)
const isDeleteOperation = computed(() => selectedOperation.value === Operation.Delete)
const formData = ref()

watch([() => props.tags, () => props.collections, () => props.images], () => {
    tags.value = props.tags
    collections.value = props.collections
    images.value = props.images
})

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
    selectedImage.value = {
        uri: '',
        imageId: '',
        title: '',
        description: '',
        imageCollectionId: '',
        tagIds: []
    }
    formData.value = { ...selectedImage.value }
    selectedOperation.value = Operation.Create
}

function openUpdateForm(object: Image | ImageCollection | Tag) {
    selectedImage.value = convertImageToImageFormFields(object as Image)
    const tagIds = (object as Image).tags.map(tag => tag.tagId)
    formData.value = { ...selectedImage.value, tagIds }

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
    selectedImage.value = object as Image
    selectedOperation.value = Operation.Delete
}
function clearSelections() {
    selectedOperation.value = Operation.None
    selectedImage.value = null
    imagePreviews.value = []
}

async function updateImage() {
    await api.put('images/' + formData.value.imageId, JSON.stringify(formData.value))
    clearSelections()
    await props.refresh()
}
async function createImage() {
    await api.post('images/', formData.value, {
        headers: {
            'Content-Type': 'multipart/form-data'
        }
    })
    clearSelections()
    await props.refresh()
}
async function deleteImage() {
    await api.delete('images/' + formData.value.imageId)
    clearSelections()
    await props.refresh()
}
</script>

<template>
    <Modal v-model:isVisible='isDeleteOperation'
        :confirm="deleteImage"
        :modalText="`Are you sure you want to delete ${(selectedImage as ImageFormFields)?.title || 'this image'}?`"
        :confirmText='`Delete`'
        @close-modal='clearSelections' />

    <div v-if="!selectedImage"
        class="manager-wrapper">
        <div class="manager-menu">
            <ComponentButton buttonType="secondary"
                buttonText="Select multiple" />
            <ComponentButton buttonType="primary"
                :onClick='openCreateForm'
                buttonText="Add images" />
        </div>
        <div class="object-wrapper">
            <div v-for='(object, index) in props.images'
                class="image-object"
                :key='index'
                @click='openUpdateForm(object)'>

                <img :src='(object as Image).uri'
                    :alt='(object as Image).title'>
            </div>
        </div>
    </div>
    <div v-else
        class="manager-wrapper">
        <div class="manager-menu collection-menu">
            <IconBack class="go-back-button"
                @click='clearSelections'></IconBack>
        </div>
        <div class="form-content">
            <img v-if="selectedOperation !== Operation.Create"
                :src='(selectedImage as unknown as Image).uri'
                :alt='(selectedImage as unknown as Image).title'
                class="form-image-preview">
            <div v-else
                class="upload-image-preview-wrapper">
                <img v-for='(image, index) in imagePreviews'
                    :key='index'
                    :src='(image.imageSrc)'
                    :alt='image.imageSrc'
                    class="upload-image-preview-object">
            </div>
            <form v-if='selectedImage && selectedOperation !== Operation.Create'
                @submit.prevent="updateImage">
                <div>
                    <template v-for="(property, index) in selectedImage">
                        <FormInput v-if="shouldRenderInput(index)"
                            :key="index"
                            :label="capitalize(index)"
                            :placeholder="capitalize(index)"
                            :modelValue="formData[index]"
                            @update:modelValue="updateFormDataWithEmittedValue(index, $event)" />
                    </template>
                    <CollectionDropdownSelect :imageCollections="collections"
                        :modelValue="formData.imageCollectionId"
                        @update:modelValue="formData.imageCollectionId = $event" />
                    <TagWrapper :tags="tags"
                        :formData="formData" />
                    <FormButtons :cancelAction="clearSelections"
                        submitText="Update" />
                </div>

                <ComponentButton buttonType="warning"
                    :onClick='() => openDeletePromptModal(selectedImage as Image)'
                    buttonText="Delete permanently" />
            </form>

            <form v-else-if='selectedOperation === Operation.Create'
                @submit.prevent="createImage">
                <div class="image-upload-wrapper">
                    <input class="file-input"
                        type='file'
                        multiple
                        @change='handleFileSelect'>
                </div>
                <FormButtons :cancelAction="clearSelections"
                    submitText="Upload" />
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

.upload-image-preview-wrapper {
    width: 50%;
    display: flex;
    flex-direction: row;
    flex-wrap: wrap;
}

.upload-image-preview-object {
    width: calc(20% - 1rem);
    height: 10rem;
    margin: 0.5rem;
    box-sizing: border-box;
    display: flex;
    overflow: hidden;
    object-fit: cover;
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