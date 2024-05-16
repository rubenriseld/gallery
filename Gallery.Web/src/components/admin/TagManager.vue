<script setup lang="ts">
import Modal from '@/components/Modal.vue';
import ComponentButton from '@/components/ComponentButton.vue';
import FormButtons from '@/components/form/FormButtons.vue';
import FormInput from '@/components/form/FormInput.vue';
import ManagerWrapper from '@/components/admin/ManagerWrapper.vue';

import api from '@/api';
import { ref, computed, watch } from 'vue';
import { shouldRenderInput, capitalize } from '@/assets/functions/managerHelperFunctions';

import { Operation } from '@/assets/enums/operation';
import type { Tag, UpdateTag, CreateTag } from '@/assets/types';

const props = defineProps({
    tags: {
        type: Array as () => (Tag[]),
        default: []
    },
    refresh: {
        type: Function,
        required: true
    }
});

const selectedTag = ref<Tag | UpdateTag | CreateTag | null>(null);
const tags = ref<Tag[]>(props.tags);

let initialFormData: any = null;

const selectedOperation = ref<Operation>(Operation.None);
let previousOperation: Operation = Operation.None;
const isDeleteOperation = computed(() => selectedOperation.value === Operation.Delete);
const isCancelOperation = computed(() => selectedOperation.value === Operation.Cancel);

const formData = ref();

watch([() => props.tags], () => {
    tags.value = props.tags;
})

async function openCreateForm() {
    selectedTag.value = { name: '' };
    formData.value = { ...selectedTag.value };
    initialFormData = formData.value;
    selectedOperation.value = Operation.Create;
}

function openUpdateForm(tag: Tag) {
    selectedTag.value = tag;
    formData.value = { ...selectedTag.value };
    initialFormData = formData.value;
    selectedOperation.value = Operation.Update;
}

function openDeletePromptModal(tag: Tag) {
    selectedTag.value = tag;
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
    selectedTag.value = null;
    initialFormData = null;
}
function cancelOperation() {
    selectedOperation.value = previousOperation;
}
function updateFormDataWithEmittedValue(formDataIndex: string, event: any) {
    formData.value = { ...formData.value, [formDataIndex]: event };
}
async function updateTag() {
    await api.put('tags/' + (selectedTag.value as Tag).tagId, JSON.stringify(formData.value as UpdateTag));
    clearSelections();
    await props.refresh();
}
async function createTag() {
    await api.post('tags/', JSON.stringify(formData.value as CreateTag));
    clearSelections();
    await props.refresh();
}
async function deleteTag() {
    await api.delete('tags/' + (selectedTag.value as Tag).tagId);
    clearSelections();
    await props.refresh();
}
</script>

<template>
    <Modal v-model:isVisible='isDeleteOperation'
        :confirm="deleteTag"
        :modalText="`Are you sure you want to delete ${selectedTag?.name || 'this item'}?`"
        :confirmText='`Delete`'
        @close-modal='clearSelections' />

    <Modal v-model:isVisible='isCancelOperation'
        modalType="confirm"
        :confirm="clearSelections"
        :modalText="`You have unsaved changes. Are you sure you want to cancel?`"
        :confirmText='`Ok`'
        @close-modal='cancelOperation' />

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
            <form
                v-if='selectedTag && (selectedOperation === Operation.Update || previousOperation === Operation.Update)'
                @submit.prevent="updateTag">
                <h3 class="object-title">{{ formData?.name || 'Untitled' }}</h3>
                <div>
                    <template v-for="(property, index) in selectedTag">
                        <FormInput v-if="shouldRenderInput(index)"
                            :key="index"
                            :label="capitalize(index)"
                            :placeholder="capitalize(index)"
                            :modelValue="formData[index]"
                            @update:modelValue="updateFormDataWithEmittedValue(index, $event)" />
                    </template>
                    <FormButtons :cancelAction="cancel"
                        submitText="Update" />
                </div>
                <ComponentButton buttonType="warning"
                    :onClick='() => openDeletePromptModal(selectedTag as Tag)'
                    buttonText="Delete permanently" />
            </form>
            <form v-else-if='(selectedOperation === Operation.Create || previousOperation === Operation.Create)'
                @submit.prevent="createTag">
                <template v-for="(property, index) in selectedTag">
                    <FormInput v-if="shouldRenderInput(index)"
                        :key="index"
                        :label="capitalize(index)"
                        :placeholder="capitalize(index)"
                        :modelValue="formData[index]"
                        @update:modelValue="updateFormDataWithEmittedValue(index, $event)" />
                </template>
                <FormButtons :cancelAction="cancel"
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

form {
    margin: auto;
}

@media (max-width: 768px) {
    .tag-object {
        width: 100%;
    }
}
</style>