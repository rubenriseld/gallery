<script setup lang="ts">
import type { Tag } from '@/types/tag'
import Modal from './Modal.vue';
import api from '@/api'
import { ref, onMounted, computed } from 'vue'
import { Operation } from '@/assets/enums/operation';
import IconDelete from './icons/IconDelete.vue';
import IconAdd from './icons/IconAdd.vue';

const tags = ref<Tag[]>([])
const selectedTag = ref<Tag | null>(null)
const selectedOperation = ref<Operation>(Operation.None)
const formData = ref()
const isDeleteOperation = computed(() => selectedOperation.value === Operation.Delete)

const showDeleteButton = ref<boolean[]>([]);

onMounted(async () => {
    await getTags()
    showDeleteButton.value = Array(tags.value.length).fill(false);
})

async function openCreateForm() {
    selectedTag.value = {
        tagId: '',
        name: '',
    }
    formData.value = { ...selectedTag.value }
    selectedOperation.value = Operation.Create
}
function openUpdateForm(tag: Tag) {
    selectedTag.value = tag
    formData.value = { ...selectedTag.value }
    selectedOperation.value = Operation.Update
}

async function getTags() {
    tags.value = (await api.get('tags')).data as Tag[]
}
function openDeletePromptModal(tag: Tag) {
    selectedOperation.value = Operation.Delete
    selectedTag.value = tag
}
function clearSelections() {
    selectedOperation.value = Operation.None
    selectedTag.value = null
}
async function updateTag() {
    if (selectedTag.value) {
        await api.put('tags/' + selectedTag.value.tagId, JSON.stringify(formData.value))
        await getTags()
    }
    clearSelections()
}
async function createTag() {
    if (selectedTag.value) {
        await api.post('tags/', JSON.stringify(formData.value))
        await getTags()
    }
    clearSelections()
}
async function deleteTag() {
    if (selectedTag.value) {
        await api.delete('tags/' + selectedTag.value.tagId)
        await getTags()
    }
    clearSelections()
}

function toggleDeleteButton(index: number, show: boolean) {
    showDeleteButton.value[index] = show;
}
</script>

<template>
    <div>
        <div class='tag' v-for='(tag, index) in tags' :key='index' @click='openUpdateForm(tag)'
            @mouseenter='toggleDeleteButton(index, true)' @mouseleave='toggleDeleteButton(index, false)'>
            <h4>{{ tag.name }}</h4>
            <button v-if='showDeleteButton[index]' class='delete-button' @click.stop='openDeletePromptModal(tag)'>
                <IconDelete />
            </button>
        </div>
        <Modal v-model:isVisible='isDeleteOperation' :confirm="deleteTag"
            :modalText='`Are you sure you want to delete ${selectedTag?.name}?`' :confirmText='`Delete`'
            @close-modal='clearSelections' />
        <button class='add-button' @click='openCreateForm'>
            <IconAdd />
        </button>

        <form v-if='selectedOperation === Operation.Update' @submit.prevent='updateTag'>
            <input id='name' v-model='formData.name' required>
            <button class='cancel-button' @click='clearSelections'>Cancel</button>
            <button class='submit-button' type='submit'>Update</button>
        </form>
        <form v-else-if='selectedOperation === Operation.Create' @submit.prevent='createTag'>
            <input id='name' v-model='formData.name' required>
            <button class='submit-button' type='submit'>Create</button>
        </form>
    </div>
</template>

<style scoped>
.tag {
    border: 1px black solid;
    position: relative;
}

.delete-button {
    position: absolute;
    top: 5px;
    right: 5px;
    display: none;
}

.tag:hover .delete-button {
    display: block;
}
</style>