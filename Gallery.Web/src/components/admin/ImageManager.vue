<script setup lang="ts">
import Modal from '@/components/Modal.vue';
import IconBack from '@/components/icons/IconBack.vue';
import ComponentButton from '@/components/ComponentButton.vue';
import FormButtons from '@/components/form/FormButtons.vue';
import CollectionDropdownSelect from '@/components/form/CollectionDropdownSelect.vue';
import TagWrapper from '@/components/form/TagWrapper.vue';
import FormInput from '@/components/form/FormInput.vue';
import ManagerWrapper from '@/components/admin/ManagerWrapper.vue'

import api from '@/api';
import { ref, computed, watch } from 'vue'
import { shouldRenderInput, capitalize } from '@/assets/functions/managerHelperFunctions';

import { Operation } from '@/assets/enums/operation';
import type { Image, UpdateImage, CreateImage, ImageCollection, Tag, ImagePreview } from '@/assets/types'

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

const selectedImage = ref<Image | CreateImage | UpdateImage | null>(null)
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
    selectedImage.value = {} as CreateImage
    formData.value = { ...selectedImage.value }
    selectedOperation.value = Operation.Create
}

function openUpdateForm(image: Image) {
    selectedImage.value = image
    const tagIds = image.tags.map(tag => tag.tagId)
    formData.value = { 
        title : image.title,
        description : image.description,
        imageCollectionId : image.imageCollectionId,
        tagIds : tagIds
     }
    selectedOperation.value = Operation.Update;
}

function updateFormDataWithEmittedValue(formDataIndex: string, event: any) {
    formData.value = { ...formData.value, [formDataIndex]: event }
}

function openDeletePromptModal(image: Image) {
    selectedImage.value = image
    selectedOperation.value = Operation.Delete
}
function clearSelections() {
    selectedOperation.value = Operation.None
    selectedImage.value = null
    imagePreviews.value = []
}

async function updateImage() {
    await api.put('images/' + (selectedImage.value as Image).imageId, JSON.stringify(formData.value as UpdateImage))
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
    await api.delete('images/' + (selectedImage.value as Image).imageId)
    clearSelections()
    await props.refresh()
}
</script>

<template>
    <Modal v-model:isVisible='isDeleteOperation'
        :confirm="deleteImage"
        :modalText="`Are you sure you want to delete ${(selectedImage as Image)?.title || 'this image'}?`"
        :confirmText='`Delete`'
        @close-modal='clearSelections' />

    <ManagerWrapper :objectIsSelected="selectedImage !== null"
        :openCreateForm="openCreateForm"
        :openCreateFormText="'Add images'"
        :clearSelections="clearSelections">
        <template #objectDisplay>
            <div v-for='(image, index) in props.images'
                class="image-object"
                :key='index'
                @click='openUpdateForm(image)'>

                <img :src='(image as Image).uri'
                    :alt='(image as Image).title'>
            </div>
        </template>
        <template #formContent>
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
                <h3 class="object-title">{{ formData?.title || 'Untitled' }}</h3>
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
        </template>
    </ManagerWrapper>
</template>

<style scoped>
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
    object-fit: cover;
}

@media (max-width: 768px) {
    .image-object {
        width: 33.3333%;
        height: 12rem;
    }

    .form-image-preview {
        width: 100%;
        height: 20rem;
        margin-bottom: 2rem;
    }

    .upload-image-preview-wrapper {
        order: 1;
    }
}
</style>