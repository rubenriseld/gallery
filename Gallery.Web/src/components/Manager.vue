<script setup lang="ts">
import Modal from './Modal.vue';
import IconBack from './icons/IconBack.vue';
import ComponentButton from './ComponentButton.vue';
import PillCheckbox from './PillCheckbox.vue';
import { ref, onMounted, computed, watch } from 'vue'
import { Operation } from '@/assets/enums/operation';
import type { Image, ImageFormFields, ImageCollection, ImageCollectionFormFields, Tag, TagFormFields, ManagerObjectType, ImagePreview } from '@/assets/types'

const selectedObject = ref<ImageFormFields | ImageCollectionFormFields | TagFormFields | null>(null)
const objectsToManage = ref<Image[] | ImageCollection[] | Tag[]>([])
const imagePreviews = ref<ImagePreview[]>([])
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

// Function to handle the file select event
const handleFileSelect = (event: Event) => {
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
        formData.value = { ...selectedObject.value }
    }
    else if (props.objectType === 'tags') {
        selectedObject.value = object as Tag
        formData.value = { ...selectedObject.value }
    }
    selectedOperation.value = Operation.Update;
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
function shouldRenderInput(index: string): boolean {
    const excludedFormFields = ['tagId', 'imageId', 'imageCollectionId', 'tagIds', 'uri', 'imageCollectionName', 'images']
    return !excludedFormFields.includes(index)
}
function capitalize(string: string): string {
    return (string).charAt(0).toUpperCase() + (string).slice(1)
}
function toggleDeleteButton(index: number, show: boolean) {
    showDeleteButton.value[index] = show
}
</script>

<template>

    <Modal v-model:isVisible='isDeleteOperation' :confirm="() => performAction('delete')"
        :modalText="objectType === 'images' ?
            `Are you sure you want to delete ${(selectedObject as ImageFormFields)?.title || 'this image'}?` :
            `Are you sure you want to delete ${(selectedObject as ImageCollectionFormFields | TagFormFields)?.name || 'this item'}?`" :confirmText='`Delete`' @close-modal='clearSelections' />

    <div v-if="!selectedObject" class="manager-wrapper">
        <div class="manager-menu">
            <ComponentButton buttonType="secondary" buttonText="Select multiple" />
            <ComponentButton buttonType="primary" :onClick='openCreateForm'
                :buttonText="objectType !== 'images' ? `Add ${capitalize(objectType.slice(0, -1))}` : `Add ${capitalize(objectType)}`" />
        </div>
        <div class="object-wrapper">
            <div v-for='(object, index) in objectsToManage'
                :class="{ 'image-object': objectType === 'images', 'collection-object': objectType === 'collections', 'tag-object': objectType === 'tags' }"
                :key='index' @click='openUpdateForm(object)' @mouseenter='toggleDeleteButton(index, true)'
                @mouseleave='toggleDeleteButton(index, false)'>
                <p class="object-name" v-if="objectType !== 'images'">{{ (object as ImageCollection | Tag).name }}</p>
                <img v-else :src='(object as Image).uri' :alt='(object as Image).title'>
            </div>
        </div>
    </div>


    <div v-else class="form-wrapper">
        <IconBack class="go-back-button" @click='clearSelections'></IconBack>
        <div class="form-content">
            <h4 class="object-name" v-if="objectType !== 'images'">{{ (selectedObject as ImageCollection | Tag).name
                }}</h4>
            <img class="form-image-preview" v-else-if="selectedOperation !== Operation.Create" :src='(selectedObject as unknown as Image).uri'
                :alt='(selectedObject as unknown as Image).title'>
            <div v-else class="upload-image-preview-wrapper">
                <img class="upload-image-preview-object" v-for='(image, index) in imagePreviews' :key='index' :src='(image.imageSrc)' :alt='image.imageSrc'>
            </div>
            <form v-if='selectedObject && selectedOperation !== Operation.Create'
                @submit.prevent="performAction('update')">
                <div>
                    <template v-for="(property, index) in selectedObject">
                        <div v-if="shouldRenderInput(index)" :key="index" class="form-input-wrapper">
                            <label :for="index">{{ capitalize(index) }}</label>
                            <input v-model="formData[index]">
                        </div>
                    </template>
                    <template v-if="objectType === 'images'">
                        <div class="form-input-wrapper">
                            <label for="imageCollectionId">Collection</label>
                            <select class="dropdown-select" id='imageCollectionId' v-model='formData.imageCollectionId'>
                                <option class="dropdown-option" v-for='(imageCollection, index) in collections'
                                    :key='index' :value='imageCollection.imageCollectionId'>{{ imageCollection.name }}
                                </option>
                            </select>
                        </div>
                        <div class="form-input-wrapper">

                            <label for="tagIds">Tags</label>
                            <div class="tag-wrapper">
                                <PillCheckbox v-for="(tag, tagIndex) in tags" :key="tagIndex" :tag="tag"
                                    :tagIndex="tagIndex" :formData="formData" />
                            </div>
                        </div>
                    </template>
                    <div class="update-buttons-wrapper">
                        <ComponentButton buttonType="secondary" :onClick='clearSelections' buttonText="Cancel" />
                        <ComponentButton class="submit-button" buttonType="primary" :submit="true" buttonText="Update" />
                    </div>
                </div>

                <ComponentButton buttonType="warning"
                    :onClick='() => openDeletePromptModal(selectedObject as Image | ImageCollection | Tag)'
                    buttonText="Delete permanently" />
            </form>

            <form v-else-if='selectedOperation === Operation.Create' @submit.prevent="performAction('create')">
                <template v-if="objectType !== 'images'" v-for="(property, index) in selectedObject">
                    <div v-if="shouldRenderInput(index)" :key="index" class="form-input-wrapper">
                        <label :for="index">{{ capitalize(index) }}</label>
                        <input v-model="formData[index]" :placeholder="capitalize(index)">
                    </div>
                </template>
                <div v-else class="image-upload-wrapper">
                    <input class="file-input" type='file' multiple @change='handleFileSelect'>
                </div>
                <div class="submit-button-wrapper">
                    <ComponentButton buttonType="secondary" :onClick='clearSelections' buttonText="Cancel" />
                    <ComponentButton class="submit-button" buttonType="primary" :submit="true"
                        :buttonText="objectType !== 'images' ? 'Add' : 'Upload'" />
                </div>
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
    padding: 1rem;
    background-color: var(--lightest-color);
}

.manager-menu {
    display: flex;
    justify-content: space-between;
    margin: 1rem;
    background-color: var(--lightest-color);
}

.go-back-button {
    width:2rem;
    height: 2rem;
    font-weight: 900;
    cursor: pointer;
    margin-bottom: 1rem;
    color: var(--primary-color);
    transition: all 0.1s ease-in-out;
}
.go-back-button:hover{
    color: var(--primary-color-hover);
    transform: scale(1.2);
}

.object-wrapper {
    display: flex;
    flex-wrap: wrap;
    padding: 0 0.5rem 0 0.5rem;
    margin-bottom: 1rem;
}

.image-object{
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
    font-weight: 300;
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
    width: 20%;
    height: 10rem;
    padding: 0.5rem;
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

.form-input-wrapper, .image-upload-wrapper {
    width: 100%;
    margin-bottom: 1rem;
    display: flex;
    flex-direction: column;
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

.dropdown-select {
    width: 100%;
    padding: 0.5rem;
    border-radius: none;
    border: 1px solid var(--mid-color);
    background-color: #fff;
    color: #333;
    font-size: 16px;
}

.dropdown-option {
    padding: 0.5rem;
    cursor: pointer;
    background-color: #fff;
    color: #333;
    font-size: 16px;

}

.tag-wrapper {
    display: flex;
    flex-wrap: wrap;
    margin: 0 -0.5rem 0 -0.5rem;
}

.update-buttons-wrapper,
.submit-button-wrapper {
    margin-bottom: 1rem;
    display: flex;
    justify-content: flex-end;
}

.submit-button {
    margin-left: 1rem;
}

@media (max-width: 768px) {
    .manager-wrapper{
        padding: 0;
    }
    .image-object {
        width: 33.3333%;
        height: 12rem;
    }
    .form-wrapper {
        margin: 0 1rem 1rem 1rem;
    }
    .form-content{
        margin-top:0;
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
    .update-buttons-wrapper button,
    .submit-button-wrapper button {
       width: 50%;
    }

    .upload-image-preview-wrapper {
        order: 1;
    }
}
</style>