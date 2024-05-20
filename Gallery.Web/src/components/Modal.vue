<script setup lang='ts'>
import type { ModalType } from '@/assets/types';
import ComponentButton from '@/components/ComponentButton.vue';
import { ref } from 'vue';

const isLoading = ref<boolean>(false);

const props = defineProps({
    modalType: {
        type: String as () => ModalType,
        default: 'confirm'
    },
    modalText: String,
    confirmText: String,
    confirm: Function,
    isVisible: Boolean
})
const emits = defineEmits(['closeModal']);

function closeModal() {
    emits('closeModal');
}
async function confirmAction() {
    isLoading.value = true;
    props.confirm && await props.confirm();
    isLoading.value = false;
    closeModal();
}

</script>

<template>
    <div v-if='$props.isVisible'
        class="modal">
        <p v-if="isLoading">Loading...</p>
        <template v-else>
            <p>{{ props.modalText }}</p>
            <div class="modal-button-wrapper">
                <ComponentButton buttonType="secondary"
                    :onClick='closeModal'
                    buttonText="Cancel" />
                <ComponentButton :buttonType="`${modalType === 'warning' ? 'warning' : 'primary'}`"
                    :onClick='confirmAction'
                    :buttonText="$props.confirmText" />
            </div>
        </template>
    </div>
</template>

<style scoped>
.modal {
    position: fixed;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    z-index: 10;
    background-color: var(--lightest-color);
    padding: 2rem;
    display: flex;
    flex-direction: column;
    justify-items: center;
    box-shadow: 0 0 0 1000rem rgba(0, 0, 0, 0.4);
}

p {
    margin-bottom: 2rem;
}

.modal-button-wrapper {
    display: flex;
    flex-direction: row;
    justify-content: space-between;
    align-items: center;
}

@media (max-width: 768px) {
    .modal {
        width: 70%;
    }
}
</style>