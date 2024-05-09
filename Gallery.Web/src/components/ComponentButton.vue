<script setup lang="ts">
import type { ButtonType } from '@/assets/types';
import { defineProps, toRefs } from 'vue';

const props = defineProps({
    buttonType: {
        type: String as () => ButtonType,
        required: true
    },
    buttonText: {
        type: String,
    },
    onClick: {
        type: Function,
        default: null,
    },
    submit: {
        type: Boolean,
        default: false,
    }
})
const { buttonText, onClick, submit } = toRefs(props);
const handleClick = () => {
    if (!submit.value && onClick.value) {
        onClick.value();
    }
};
</script>

<template>
    <button @click="handleClick" :type="submit ? 'submit' : 'button'" :class="buttonType">
        <slot class="button-icon"/>
        {{ buttonText }}
    </button>
</template>
<style scoped>
button {
    display:flex;
    align-items: center;
    justify-content: center;
    text-transform: uppercase;
    padding: 1rem;
    font-size: 1.2rem;
    cursor: pointer;
    border: none;
    text-decoration: none;
    font-family: inherit;
}
button:hover {
    box-shadow: 0 0 10px 0 rgba(0, 0, 0, 0.2);
}
.primary {
    background-color: var(--primary-color);
    color: var(--lightest-color);
}
.primary:hover {
    background-color: var(--primary-color-hover);
}
.secondary {
    border: 2px solid;
    border-color: var(--secondary-color);
    color: var(--secondary-color);
}
.secondary:hover {
    border-color: var(--secondary-color-hover);
    color: var(--secondary-color-hover);
}
.warning {
    background-color: var(--warning-color);
    color: var(--lightest-color);
}
.warning:hover {
    background-color: var(--warning-color-hover);
}
@media (max-width: 768px) {
    button {
        font-size: 1rem;
        padding:0.7rem;
    }
}
</style>