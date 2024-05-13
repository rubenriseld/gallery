<script setup lang="ts">
import Modal from '@/components/Modal.vue';
import ComponentButton from '@/components/ComponentButton.vue';
import FormButtons from '@/components/form/FormButtons.vue';
import FormInput from '@/components/form/FormInput.vue';
import ManagerWrapper from '@/components/admin/ManagerWrapper.vue';

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
    <ManagerWrapper :objectIsSelected="selectedTag !== null"
        :openCreateForm="openCreateForm"
        :openCreateFormText="'Add tag'"
        :clearSelections="clearSelections">
        <template #objectDisplay>
            <div v-for='(tag, index) in tags'
                class="tag-object"
                :key='index'
                @click='openUpdateForm(tag)'>
                <p class="object-name">{{ tag.name }}</p>

            </div>
        </template>
        <template #formContent>
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
        </template>
    </ManagerWrapper>
</template>

<style scoped>
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

@media (max-width: 768px) {
.tag-object{
    width: 100%;
}
}
</style>