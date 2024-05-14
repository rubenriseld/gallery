<script setup lang="ts">
import Modal from '@/components/Modal.vue';
import ComponentButton from '@/components/ComponentButton.vue';
import FormButtons from '@/components/form/FormButtons.vue';
import FormInput from '@/components/form/FormInput.vue';
import ManagerWrapper from '@/components/admin/ManagerWrapper.vue';
import IconMore from '@/components/icons/IconMore.vue';
import IconLess from '@/components/icons/IconLess.vue';

import api from '@/api';
import { ref, computed, watch } from 'vue';
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
const selectedCollection = ref<ImageCollection | CreateImageCollection | UpdateImageCollection | null>(null);

const selectedOperation = ref<Operation>(Operation.None);
const isDeleteOperation = computed(() => selectedOperation.value === Operation.Delete);

const canSelectCoverImage = ref<boolean>(false);
const coverImageUri = computed(() => {
    return ((selectedCollection.value as ImageCollection)?.images.find(image => image.imageId === formData.value.coverImageId)?.uri || '');
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


const isMobilePreviewOpen = ref(false);

const formData = ref();

watch([() => props.collections], () => {
    collections.value = props.collections;
});


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
    };

    formData.value = { ...selectedCollection.value };
    selectedOperation.value = Operation.Create;
}

function openUpdateForm(collection: ImageCollection) {
    selectedCollection.value = collection;
    formData.value = {
        name: collection.name,
        description: collection.description,
        coverImageId: collection.coverImage?.imageId || null,
        reorderImages: reorderImages.value
    };
    selectedOperation.value = Operation.Update;
}

function updateFormDataWithEmittedValue(formDataIndex: string, event: any) {
    formData.value = { ...formData.value, [formDataIndex]: event };
}

function openDeletePromptModal(collection: ImageCollection) {
    selectedCollection.value = collection;
    selectedOperation.value = Operation.Delete;
}
function clearSelections() {
    selectedOperation.value = Operation.None;
    selectedCollection.value = null;
    canSelectCoverImage.value = false;
    isMobilePreviewOpen.value = false;
    canDragAndDrop.value = false;
}
function cancelDeleteOperation() {
    selectedOperation.value = Operation.None;
}

function toggleCanSelectCoverImage() {
    canSelectCoverImage.value = !canSelectCoverImage.value;
    isMobilePreviewOpen.value = canSelectCoverImage.value;
}
function selectCoverImage(imageId: string) {
    if (!canSelectCoverImage.value) return;
    const previousExistingCoverImageId = formData.value.coverImageId;
    formData.value = { ...formData.value, coverImageId: (imageId === previousExistingCoverImageId ? null : imageId) };
}
function cancelSelectCoverImage() {
    formData.value = { ...formData.value, coverImageId: (selectedCollection.value as ImageCollection).coverImage?.imageId || '' };
    canSelectCoverImage.value = false;
    isMobilePreviewOpen.value = false;
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
        :confirm="deleteCollection"
        :modalText="`Are you sure you want to delete ${selectedCollection?.name || 'this item'}?`"
        :confirmText='`Delete`'
        @close-modal='cancelDeleteOperation' />

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
        <template #additionalMenuItems
            :class="'collection-menu'">
            <div v-if="selectedOperation !== Operation.Create"
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
                    buttonText="Toggle Drag and Drop" />
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
            <button @click="isMobilePreviewOpen = !isMobilePreviewOpen"
                class="collection-preview-button">
                <IconMore v-if="!isMobilePreviewOpen" />
                <IconLess v-else />
                <p>Collection preview</p>
            </button>
            <div
                :class="`${isMobilePreviewOpen ? 'collection-image-preview-wrapper' : 'collection-image-preview-hidden'}`">

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
            <form v-if='selectedCollection && selectedOperation !== Operation.Create'
                @submit.prevent="updateCollection">
                <img v-if="!isMobilePreviewOpen"
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

.collection-image-preview-hidden,
.collection-image-preview-wrapper {
    width: 50%;

}

.collection-image-preview-wrapper {
    display: flex;
    flex-direction: row;
    flex-wrap: wrap;
}

.collection-preview-button {
    /* border-bottom: 2px solid var(--mid-color); */
    background-color: var(--light-color);
    display: none;
    padding: 0.5rem;
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
}

.cover-image {
    max-height: 20rem;
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