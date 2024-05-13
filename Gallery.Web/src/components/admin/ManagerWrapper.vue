<script setup lang="ts">
import type { ImageCollection, Tag, Image } from '@/assets/types';
import ComponentButton from '@/components/ComponentButton.vue'
import IconBack from '@/components/icons/IconBack.vue'
import { ref, watch } from 'vue';

const props = defineProps({
    openCreateForm: {
        type: Function,
        required: true
    },
    openCreateFormText: {
        type: String,
        required: true
    },
    objectIsSelected: {
        type: Boolean,
        required: true,
        default: false
    },
    clearSelections: {
        type: Function,
        required: true
    }
})
const objectIsSelected = ref<boolean>(props.objectIsSelected)

watch([() => props.objectIsSelected], () => {
    objectIsSelected.value = props.objectIsSelected
})

</script>

<template>
    <div v-if="!objectIsSelected"
        class="manager-wrapper">
        <div class="manager-menu">
            <ComponentButton buttonType="secondary"
                buttonText="Select multiple" />
            <ComponentButton buttonType="primary"
                :onClick='openCreateForm'
                :buttonText="openCreateFormText" />
        </div>
        <div class="object-wrapper">
            <slot name="objectDisplay" />
        </div>
    </div>
    <div v-else
        class="manager-wrapper">
        <div class="object-menu">
            <IconBack class="go-back-button"
                @click="props.clearSelections"></IconBack>
            <slot name="additionalMenuItems" />
        </div>
        <div class="form-content">
            <slot name="formContent" />
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

.manager-menu,
.object-menu {
    display: flex;
    align-items: center;
    background-color: var(--lightest-color);
}

.manager-menu {
    justify-content: space-between;
}

.object-menu {
    justify-content: flex-start;
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

.form-content {
    display: flex;
    flex-direction: row;
    justify-content: space-between;
    margin-top: 2rem;
}

.form-content:deep(.object-title){
    margin: 0.5rem 0 1rem 0;
    font-size: 1.8rem;
    color: var(--darker-color);
}
.form-content:deep(.untitled-object) {
   
    color: var(--dark-color);
    font-style: italic;
}

.form-content:deep(form) {
    display: flex;
    width: 40%;
    flex-direction: column;
    justify-content: space-between;
    max-height: 40rem;
}
.form-content:deep(form > button.warning){
    margin-top:4rem;
}

@media (max-width: 768px) {
    .manager-wrapper {
        padding: 1rem;
    }

    .object-menu {
        justify-content: space-between;
    }

    .form-content {
        flex-direction: column;
        margin-top:1rem;
    }

    .form-content:deep(form) {
        margin-top: 1rem;
        width: 100%;
    }
}
</style>