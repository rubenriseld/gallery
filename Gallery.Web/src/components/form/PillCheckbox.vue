<script setup lang="ts">

const props = defineProps(['tag', 'tagIndex', 'formData']);

</script>
<template>
    <label :class="`pill ${formData.tagIds.includes(tag.tagId) && 'active'}`">
        {{ tag.name }}
        <input type="checkbox"
            :value="tag.tagId"
            v-model="formData.tagIds"
            :id="'tagCheckbox_' + tagIndex"
            class="checkbox-hidden" />
    </label>
</template>

<style scoped>
.checkbox-hidden {
    display: none;
}

.pill {
    padding: 0.5rem;
    margin: 0.5rem;
    background-color: var(--light-color);
    cursor: pointer;
    user-select: none;
}

.pill.active {
    background-color: var(--primary-color);
    color: var(--lightest-color);
}

.pill:hover {
    background-color: var(--secondary-color-hover);
}

.pill.active:hover {
    background-color: var(--primary-color-hover);
}
</style>
