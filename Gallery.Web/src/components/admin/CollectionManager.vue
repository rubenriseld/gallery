<script setup lang="ts">
import Modal from '@/components/Modal.vue';
import ComponentButton from '@/components/ComponentButton.vue';
import FormButtons from '@/components/form/FormButtons.vue';
import FormInput from '@/components/form/FormInput.vue';
import ManagerWrapper from '@/components/admin/ManagerWrapper.vue';
import IconMore from '@/components/icons/IconMore.vue';
import IconLess from '@/components/icons/IconLess.vue';

import api from '@/api';
import { ref, computed, watch, onMounted, onUnmounted } from 'vue';
import { capitalize, shouldRenderInput } from '@/assets/functions/managerHelperFunctions';

import { Operation } from '@/assets/enums/operation';
import type { Image, ImageCollection, CreateImageCollection, UpdateImageCollection, Tag, ImagePreview } from '@/assets/types';

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
});


const collections = ref<ImageCollection[]>(props.collections);
watch([() => props.collections], () => {
    collections.value = props.collections;
});
const selectedCollection = ref<ImageCollection | CreateImageCollection | UpdateImageCollection | null>(null);
let initialFormData: any = null;

const selectedOperation = ref<Operation>(Operation.None);
let previousOperation: Operation = Operation.None;
const isDeleteOperation = computed(() => selectedOperation.value === Operation.Delete);
const isCancelOperation = computed(() => selectedOperation.value === Operation.Cancel);

const canSelectCoverImage = ref<boolean>(false);
const coverImageUri = computed(() => {
    return ((selectedCollection.value as ImageCollection)?.images?.find(image => image.imageId === formData.value.coverImageId)?.uri || null);
});

const canDragAndDrop = ref<boolean>(false);
const draggedIndex = ref<number | null>(null);
let selectedCollectionImagePreviews: Image[] | null = null;
const reorderImages = computed(() => {
    return ((selectedCollection.value as ImageCollection)?.images || []).map((image, index) => ({
        imageId: image.imageId,
        orderInImageCollection: index
    }));
});

const formData = ref();

const isMobilePreviewOpen = ref(false);
const windowWidth = ref(window.innerWidth);
const isMobile = computed(() => windowWidth.value <= 768);

onMounted(() => {
    window.addEventListener('resize', handleResize);
});

onUnmounted(() => {
    window.removeEventListener('resize', handleResize);
});

function handleResize() {
    windowWidth.value = window.innerWidth;
};

function onDragStart(index: number) {
    selectedCollectionImagePreviews = [...(selectedCollection.value as ImageCollection)?.images];
    draggedIndex.value = index;
}

function onDragEnter(index: number) {
    if (draggedIndex.value !== null) {
        const images = [...(selectedCollection.value as ImageCollection)?.images];

        const draggedImage = images[draggedIndex.value as number];
        images.splice(draggedIndex.value as number, 1);
        images.splice(index, 0, draggedImage);

        images.forEach((image, i) => {
            image.orderInImageCollection = i;
        });

        (selectedCollection.value as ImageCollection).images = images;

        draggedIndex.value = index;
        formData.value = { ...formData.value, reorderImages: reorderImages.value };
    }
}

function onDrop() {
    draggedIndex.value = null;
}

async function openCreateForm() {
    selectedCollection.value = {
        name: '',
        description: '',
        shouldBeDisplayed: false,
    };
    formData.value = { ...selectedCollection.value };
    initialFormData = formData.value;
    selectedOperation.value = Operation.Create;
}

function openUpdateForm(collection: ImageCollection) {
    selectedCollection.value = collection;
    formData.value = {
        name: collection.name,
        description: collection.description,
        coverImageId: collection.coverImage?.imageId || null,
        reorderImages: reorderImages.value,
        shouldBeDisplayed: collection.shouldBeDisplayed
    };
    initialFormData = formData.value;
    selectedOperation.value = Operation.Update;
}

function updateFormDataWithEmittedValue(formDataIndex: string, event: any) {
    formData.value = { ...formData.value, [formDataIndex]: event };
}

function openDeletePromptModal(collection: ImageCollection) {
    selectedCollection.value = collection;
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
    selectedCollection.value = null;
    canSelectCoverImage.value = false;
    isMobilePreviewOpen.value = false;
    canDragAndDrop.value = false;
    initialFormData = null;
}
function cancelOperation() {
    selectedOperation.value = previousOperation;
}

function toggleCanSelectCoverImage() {
    canSelectCoverImage.value = !canSelectCoverImage.value;
}
function selectCoverImage(imageId: string) {
    if (!canSelectCoverImage.value) return;
    const previousExistingCoverImageId = formData.value.coverImageId;
    formData.value = { ...formData.value, coverImageId: (imageId === previousExistingCoverImageId ? null : imageId) };
}
function cancelSelectCoverImage() {
    formData.value = { ...formData.value, coverImageId: (selectedCollection.value as ImageCollection).coverImage?.imageId || '' };
    canSelectCoverImage.value = false;
}
function isSelectedCoverImage(imageId: string): boolean {
    return (imageId === (formData.value.coverImageId));
}
function toggleDragAndDrop() {
    canDragAndDrop.value = !canDragAndDrop.value;
}
function cancelDragAndDrop() {
    if (selectedCollectionImagePreviews !== null) {
        (selectedCollection.value as ImageCollection).images = selectedCollectionImagePreviews;
    }
    canDragAndDrop.value = false;
}

async function updateCollection() {
    await api.put('imageCollections/' + (selectedCollection.value as ImageCollection).imageCollectionId, JSON.stringify(formData.value as UpdateImageCollection));
    clearSelections();
    await props.refresh();
}
async function createCollection() {
    await api.post('imageCollections/', JSON.stringify(formData.value));
    clearSelections();
    await props.refresh();
}
async function deleteCollection() {
    await api.delete('imageCollections/' + (selectedCollection.value as ImageCollection).imageCollectionId);
    clearSelections();
    await props.refresh();
}
</script>

<template>
    <Modal v-model:isVisible='isDeleteOperation'
        modalType="warning"
        :confirm="deleteCollection"
        :modalText="`Are you sure you want to delete ${selectedCollection?.name || 'this item'}?`"
        :confirmText='`Delete`'
        @close-modal='cancelOperation' />

    <Modal v-model:isVisible='isCancelOperation'
        modalType="confirm"
        :confirm="clearSelections"
        :modalText="`You have unsaved changes. Are you sure you want to cancel?`"
        :confirmText='`Ok`'
        @close-modal='cancelOperation' />

    <ManagerWrapper :objectIsSelected="selectedCollection !== null"
        :openCreateForm="openCreateForm"
        :openCreateFormText="'Add collection'"
        :clearSelections="clearSelections">
        <template #objectDisplay>
            <div v-for='(collection, index) in props.collections'
                class="collection-object"
                :key='index'
                @click='openUpdateForm(collection)'>
                <p>{{ collection.name }}</p>
            </div>
        </template>
        <template #additionalMenuItems>
            <div v-if="(selectedOperation === Operation.Update || previousOperation === Operation.Update)"
                class="select-cover-image-button-wrapper">
                <ComponentButton v-if="!canSelectCoverImage && !canDragAndDrop"
                    buttonType="secondary"
                    :onClick="toggleCanSelectCoverImage"
                    buttonText="Select cover image" />
                <template v-else-if="canSelectCoverImage">
                    <ComponentButton buttonType="secondary"
                        :onClick="cancelSelectCoverImage"
                        buttonText="Cancel" />
                    <ComponentButton buttonType="primary"
                        :onClick="toggleCanSelectCoverImage"
                        buttonText="Select" />
                </template>
                <ComponentButton v-if="!canSelectCoverImage && !canDragAndDrop"
                    buttonType="secondary"
                    :onClick="toggleDragAndDrop"
                    buttonText="Reorder" />
                <template v-else-if="canDragAndDrop">
                    <ComponentButton buttonType="secondary"
                        :onClick="cancelDragAndDrop"
                        buttonText="Cancel" />
                    <ComponentButton buttonType="primary"
                        :onClick="toggleDragAndDrop"
                        buttonText="Done" />
                </template>
            </div>

        </template>
        <template #formContent>
            <button v-if="isMobile"
                @click="isMobilePreviewOpen = !isMobilePreviewOpen"
                class="collection-preview-button">
                <p>Collection preview</p>
                <IconMore v-if="!isMobilePreviewOpen" />
                <IconLess v-else />
            </button>
            <div :class="{
                'collection-image-preview-hidden': isMobile && !isMobilePreviewOpen,
                'collection-image-preview-wrapper': true
            }">
                <div @dragover.prevent
                    @drop="onDrop"
                    class="collection-image-preview-container">
                    <img v-for="(image, index) in (selectedCollection as ImageCollection).images"
                        @click="selectCoverImage(image.imageId)"
                        :src="(image.uri)"
                        :alt="image.title"
                        :key="index"
                        :draggable="canDragAndDrop"
                        @dragstart="onDragStart(index)"
                        @dragenter="onDragEnter(index)"
                        :class="{
                            'collection-image-preview-object': true,
                            'selected-cover-image': isSelectedCoverImage(image.imageId),
                            'selectable-image-preview-object': canSelectCoverImage,
                            'dragable-image-preview-object': canDragAndDrop,
                        }">
                </div>
            </div>
            <form
                v-if='selectedCollection && (selectedOperation === Operation.Update || previousOperation === Operation.Update)'
                @submit.prevent="updateCollection">
                <img v-if="!isMobilePreviewOpen && coverImageUri !== null"
                    :src="coverImageUri"
                    class="cover-image">
                <h3 :class="`object-title ${!formData?.name ? 'untitled-object' : ''}`">
                    {{ formData?.name || "Untitled collection" }}</h3>
                <div>
                    <template v-for="(property, index) in selectedCollection">
                        <FormInput v-if="shouldRenderInput(index)"
                            :key="index"
                            :label="capitalize(index)"
                            :placeholder="capitalize(index)"
                            :modelValue="formData[index]"
                            :propertyType="property"
                            @update:modelValue="updateFormDataWithEmittedValue(index, $event)" />
                    </template>
                    <div class="checkbox-wrapper">
                        <label>Display on the site</label>
                        <input type="checkbox"
                            v-model="formData.shouldBeDisplayed">
                    </div>
                    <FormButtons :cancelAction="cancel"
                        submitText="Update" />
                </div>
                <ComponentButton buttonType="warning"
                    :onClick='() => openDeletePromptModal(selectedCollection as ImageCollection)'
                    buttonText="Delete permanently" />
            </form>
            <form v-else-if='(selectedOperation === Operation.Create || previousOperation === Operation.Create)'
                @submit.prevent="createCollection">
                <template v-for="(property, index) in selectedCollection">
                    <FormInput v-if="shouldRenderInput(index)"
                        :key="index"
                        :label="capitalize(index)"
                        :placeholder="capitalize(index)"
                        :modelValue="formData[index]"
                        @update:modelValue="updateFormDataWithEmittedValue(index, $event)" />
                </template>
                <div class="checkbox-wrapper">
                    <label>Display on the site</label>
                    <input type="checkbox"
                        v-model="formData.shouldBeDisplayed">
                </div>
                <FormButtons :cancelAction="cancel"
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

.collection-image-preview-wrapper {
    width: 50%;
}

.collection-image-preview-hidden {
    display: none;
}

.collection-preview-button {
    display: flex;
    align-items: center;
    justify-content: space-between;
    padding: 0.5rem;
    color: var(--dark-color);
}

.collection-preview-button svg {
    width: 2rem;
    height: 2rem;
}

.collection-preview-button p {
    font-size: 1.1rem;
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
}

.cover-image {
    max-height: 20rem;
    margin-bottom: 1rem;
}

.collection-image-preview-object {
    opacity: 0.5;
    filter: grayscale(30%);
}

.selectable-image-preview-object,
.dragable-image-preview-object {
    cursor: pointer;
    transition: all 0.08s ease-in-out;
    filter: none;
}

.collection-image-preview-object:hover,
.selectable-image-preview-object.selected-cover-image,
.dragable-image-preview-object {
    filter: none;
    opacity: 1;
}

.selectable-image-preview-object.selected-cover-image {
    filter: none;
    outline: 4px solid var(--primary-color);
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


}
</style>