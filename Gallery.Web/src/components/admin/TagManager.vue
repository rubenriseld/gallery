<script setup lang="ts">
import Modal from '@/components/Modal.vue';
import IconBack from '@/components/icons/IconBack.vue';
import ComponentButton from '@/components/ComponentButton.vue';
import FormButtons from '@/components/form/FormButtons.vue';
import FormInput from '@/components/form/FormInput.vue';

import api from '@/api';
import { ref, computed, watch } from 'vue'
import { shouldRenderInput, capitalize } from '@/assets/functions/managerHelperFunctions';

import { Operation } from '@/assets/enums/operation';
import type { Tag, TagFormFields } from '@/assets/types'

const props = defineProps({
    tags: {
        type: Array as () => (Tag[]),
        default: []
    },
    refresh: {
        type: Function,
        required: true
    }
})

const selectedTag = ref<TagFormFields | null>(null)
const tags = ref<Tag[]>(props.tags)
const selectedOperation = ref<Operation>(Operation.None)
const isDeleteOperation = computed(() => selectedOperation.value === Operation.Delete)
const formData = ref()

watch([() => props.tags], () => {
    tags.value = props.tags
})

async function openCreateForm() {

    selectedTag.value = {
        tagId: '',
        name: '',
    }
    formData.value = { ...selectedTag.value }
    selectedOperation.value = Operation.Create
}

function openUpdateForm(object: Tag) {
    selectedTag.value = object
    formData.value = { ...selectedTag.value }
    selectedOperation.value = Operation.Update;
}

function openDeletePromptModal(object: Tag) {
    selectedTag.value = object
    selectedOperation.value = Operation.Delete
}
function clearSelections() {
    selectedOperation.value = Operation.None
    selectedTag.value = null
}

function updateFormDataWithEmittedValue(formDataIndex: string, event: any) {
    formData.value = { ...formData.value, [formDataIndex]: event }
}
async function updateTag() {
    await api.put('tags/' + formData.value.tagId, JSON.stringify(formData.value))
    clearSelections()
    await props.refresh()
}
async function createTag() {
    await api.post('tags/', JSON.stringify(formData.value))
    clearSelections()
    await props.refresh()
}
async function deleteTag() {
    await api.delete('tags/' + formData.value.tagId)
    clearSelections()
    await props.refresh()
}
</script>

<template>
    <Modal v-model:isVisible='isDeleteOperation'
        :confirm="deleteTag"
        :modalText="`Are you sure you want to delete ${selectedTag?.name || 'this item'}?`"
        :confirmText='`Delete`'
        @close-modal='clearSelections' />

    <div v-if="!selectedTag"
        class="manager-wrapper">
        <div class="manager-menu">
            <ComponentButton buttonType="secondary"
                buttonText="Select multiple" />
            <ComponentButton buttonType="primary"
                :onClick='openCreateForm'
                buttonText="Add tags" />
        </div>
        <div class="object-wrapper">
            <div v-for='(tag, index) in tags'
                class="tag-object"
                :key='index'
                @click='openUpdateForm(tag)'>
                <p class="object-name">{{ tag.name }}</p>

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
            <form v-if='selectedTag && selectedOperation !== Operation.Create'
                @submit.prevent="updateTag">
                <div>
                    <template v-for="(property, index) in selectedTag">
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
                    :onClick='() => openDeletePromptModal(selectedTag as Tag)'
                    buttonText="Delete permanently" />
            </form>
            <form v-else-if='selectedOperation === Operation.Create'
                @submit.prevent="createTag">
                <template v-for="(property, index) in selectedTag">
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

.tag-object:hover {
    background-color: var(--mid-color);
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

.form-content {
    display: flex;
    flex-direction: row;
    justify-content: center;
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

    .form-wrapper {
        margin: 0 1rem 1rem 1rem;
    }

    .form-content {
        margin-top: 0;
        flex-direction: column;
    }

    form {
        width: 100%;
    }
}
</style>