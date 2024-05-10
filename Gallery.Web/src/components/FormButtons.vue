<script setup lang="ts">
import ComponentButton from './ComponentButton.vue';

const props = defineProps({
    cancelAction: Function,
    submitText: String
})
</script>

<template>
    <div class="submit-buttons-wrapper">
        <ComponentButton buttonType="secondary"
            :onClick="cancelAction"
            buttonText="Cancel" />
        <ComponentButton class="submit-button"
            buttonType="primary"
            :submit="true"
            :buttonText="submitText" />
    </div>
</template>

<style scoped>
.submit-buttons-wrapper {
    margin-bottom: 1rem;
    display: flex;
    justify-content: flex-end;
}

.submit-button {
    margin-left: 1rem;
}

@media (max-width: 768px) {
    .submit-buttons-wrapper button {
        width: 50%;
    }
}
</style>