<script setup lang="ts">
import type { ImageCollection } from '@/assets/types';

const props = defineProps({
    imageCollections: { type: Array as () => ImageCollection[], default: () => [] },
    modelValue: null,
    options: Array
})

const emit = defineEmits(['update:modelValue'])

</script>

<template>
    <div class="form-input-wrapper">
        <label for="imageCollectionId">Collection</label>
        <select class="dropdown-select"
            id='imageCollectionId'
            :value="modelValue"
            @change="emit('update:modelValue', ($event.target as HTMLSelectElement)?.value)">
            <option class="dropdown-option"
                v-for="(imageCollection, index) in imageCollections"
                :key="index"
                :value="imageCollection.imageCollectionId">{{ imageCollection.name }}</option>
        </select>
    </div>
</template>

<style scoped>
.form-input-wrapper {
    width: 100%;
    margin-bottom: 1rem;
    display: flex;
    flex-direction: column;
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

label {
    margin-bottom: 0.5rem;
}

input {
    height: 2rem;
    padding: 0.25rem;
    font-size: 1rem;
    font-family: inherit;
}
</style>