<script setup lang="ts">
import Modal from '@/components/Modal.vue';
import ComponentButton from '@/components/ComponentButton.vue';
import FormButtons from '@/components/form/FormButtons.vue';
import CollectionDropdownSelect from '@/components/form/CollectionDropdownSelect.vue';
import TagWrapper from '@/components/form/TagWrapper.vue';
import FormInput from '@/components/form/FormInput.vue';
import ManagerWrapper from '@/components/admin/ManagerWrapper.vue';

import api from '@/api';
import { ref, computed, watch } from 'vue';
import { shouldRenderInput, capitalize } from '@/assets/functions/managerHelperFunctions';

import { Operation } from '@/assets/enums/operation';
import type { Image, UpdateImage, CreateImage, ImageCollection, Tag, ImagePreview } from '@/assets/types';

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
});

const selectedImage = ref<Image | CreateImage | UpdateImage | null>(null);
const images = ref<Image[]>(props.images);
const collections = ref<ImageCollection[]>(props.collections);
const tags = ref<Tag[]>(props.tags);
const imagePreviews = ref<ImagePreview[]>([]);

let initialFormData: any = null;

const selectedOperation = ref<Operation>(Operation.None);
let previousOperation: Operation = Operation.None;
const isDeleteOperation = computed(() => selectedOperation.value === Operation.Delete);
const isCancelOperation = computed(() => selectedOperation.value === Operation.Cancel);

const formData = ref()
const fileInput = ref<HTMLInputElement>()

watch([() => props.tags, () => props.collections, () => props.images], () => {
    tags.value = props.tags
    collections.value = props.collections
    images.value = props.images
});

function openFileInput() {
    fileInput.value?.click();
}
// Function to handle the file select event
function handleFileSelect(event: Event) {
    const fileList = (event.target as HTMLInputElement).files;
    formData.value = [];
    imagePreviews.value = [];

    if (fileList) {
        for (let i = 0; i < fileList.length; i++) {
            // Add the file to the formData array
            (formData.value as File[]).push(fileList[i]);

            // Create a preview of the image
            const reader = new FileReader();
            reader.onload = (x) => {
                imagePreviews.value.push({
                    imageFile: fileList[i] as File,
                    imageSrc: x.target?.result as string
                });
            }
            reader.readAsDataURL(fileList[i]);
        }
    }
}

async function openCreateForm() {
    selectedImage.value = {} as CreateImage;
    formData.value = { ...selectedImage.value };
    initialFormData = formData.value;
    selectedOperation.value = Operation.Create;
}

function openUpdateForm(image: Image) {
    selectedImage.value = image;
    const tagIds = image.tags.map(tag => tag.tagId);
    formData.value = {
        title: image.title,
        description: image.description,
        imageCollectionId: image.imageCollectionId,
        sold: image.sold,
        tagIds: tagIds
    };
    initialFormData = formData.value;
    selectedOperation.value = Operation.Update;
}

function updateFormDataWithEmittedValue(formDataIndex: string, event: any) {
    formData.value = { ...formData.value, [formDataIndex]: event }
}

function openDeletePromptModal(image: Image) {
    selectedImage.value = image;
    previousOperation = selectedOperation.value;
    selectedOperation.value = Operation.Delete;
}
function cancel() {
    if (initialFormData !== null && initialFormData !== formData.value) {
        openCancelPromptModal();
    } else {
        clearSelections();
    }
}
function openCancelPromptModal() {
    previousOperation = selectedOperation.value;
    selectedOperation.value = Operation.Cancel;
}
function clearSelections() {
    selectedOperation.value = Operation.None;
    previousOperation = Operation.None;
    selectedImage.value = null;
    imagePreviews.value = [];
    initialFormData = null;
}
function cancelOperation() {
    selectedOperation.value = previousOperation;
}

async function updateImage() {
    await api.put('images/' + (selectedImage.value as Image).imageId, JSON.stringify(formData.value as UpdateImage));
    clearSelections();
    await props.refresh();
}
async function createImage() {
    await api.post('images/', formData.value, {
        headers: {
            'Content-Type': 'multipart/form-data'
        }
    });
    clearSelections();
    await props.refresh();
}
async function deleteImage() {
    await api.delete('images/' + (selectedImage.value as Image).imageId);
    clearSelections();
    await props.refresh();
}
</script>

<template>
    <Modal v-model:isVisible='isDeleteOperation'
        :confirm="deleteImage"
        :modalText="`Are you sure you want to delete ${(selectedImage as Image)?.title || 'this image'}?`"
        :confirmText='`Delete`'
        @close-modal='clearSelections' />

    <Modal v-model:isVisible='isCancelOperation'
        modalType="confirm"
        :confirm="clearSelections"
        :modalText="`You have unsaved changes. Are you sure you want to cancel?`"
        :confirmText='`Ok`'
        @close-modal='cancelOperation' />

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
            <img v-if="(selectedOperation === Operation.Update || previousOperation === Operation.Update)"
                :src='(selectedImage as unknown as Image).uri'
                :alt='(selectedImage as unknown as Image).title'
                class="form-image-preview">
            <div v-else
                class="image-upload-preview-wrapper">
                <img v-for='(image, index) in imagePreviews'
                    :key='index'
                    :src='(image.imageSrc)'
                    :alt='image.imageSrc'
                    class="image-upload-preview-object">
            </div>
            <form
                v-if='selectedImage && (selectedOperation === Operation.Update || previousOperation === Operation.Update)'
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
                    <TagWrapper v-if="tags.length"
                        :tags="tags"
                        :formData="formData" />
                    <div class="checkbox-wrapper">
                        <label>Sold</label>
                        <input type="checkbox"
                            v-model="formData.sold">
                    </div>
                    <FormButtons :cancelAction="cancel"
                        submitText="Update" />
                </div>
                <ComponentButton buttonType="warning"
                    :onClick='() => openDeletePromptModal(selectedImage as Image)'
                    buttonText="Delete permanently" />
            </form>

            <form v-else-if='(selectedOperation === Operation.Create || previousOperation === Operation.Create)'
                @submit.prevent="createImage">
                <div class="file-select-wrapper">
                    <ComponentButton buttonType="secondary"
                        :onClick='openFileInput'
                        buttonText="Select images"
                        class="file-select-button" />
                    <p class="file-select-text">{{ (formData.length) ?
                        `${formData.length} images selected` : 'Select images to upload' }}</p>
                    <input class="file-input"
                        type='file'
                        ref="fileInput"
                        multiple
                        @change='handleFileSelect'
                        style="display:none;">
                </div>
                <FormButtons :cancelAction="cancel"
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

.image-upload-preview-wrapper {
    width: 50%;
    display: flex;
    flex-direction: row;
    flex-wrap: wrap;
}

.image-upload-preview-object {
    width: calc(20% - 1rem);
    height: 10rem;
    margin: 0.5rem;
    box-sizing: border-box;
    display: flex;
    object-fit: cover;
}

.file-select-wrapper {
    width: 100%;
    display: flex;
    flex-direction: column;
    justify-content: flex-end;
    margin-bottom: 2rem;
}

.file-select-button {
    width: 100%;
}

.file-select-text {
    margin-top: 1rem;
    font-size: 1.3rem;
    color: var(--dark-color);
    align-self: flex-end;
}

.checkbox-wrapper {
    display: flex;
    flex-direction: column;
    align-items: flex-start;
    margin: 1rem 0 1rem 0;
}

.checkbox-wrapper input {
    margin-top: 0.5rem;
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

    .file-select-wrapper {
        margin-bottom: 1rem;
    }

    .image-upload-wrapper input {
        font-size: 1.3rem;
    }

    .image-upload-preview-wrapper {
        order: 1;
        width: 100%;
        margin: 1rem -0.5rem 1rem -0.5rem;
    }

    .image-upload-preview-object {
        width: calc(33.3333% - 1rem);
        height: 10rem;
        box-sizing: border-box;
        display: flex;
        object-fit: cover;
    }
}
</style>