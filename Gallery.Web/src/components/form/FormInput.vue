<script setup lang="ts">

const props = defineProps({
    label: String,
    modelValue: null,
    placeholder: String
})

const emit = defineEmits(['update:modelValue'])
function emitData(event: Event) {
    const value = (event.target as HTMLSelectElement)?.value;
    console.log(value)
    emit('update:modelValue', value)
}
</script>

<template>
    <div class="form-input-wrapper">
        <label :for="label">{{ label }}</label>
        <input :placeholder="placeholder"
            :value="modelValue"
            @input="emitData($event)">
    </div>
</template>

<style scoped>
.form-input-wrapper {
    width: 100%;
    margin-bottom: 1rem;
    display: flex;
    flex-direction: column;
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