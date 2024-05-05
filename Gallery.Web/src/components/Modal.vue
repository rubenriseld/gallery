<script setup lang='ts'>
import IconClose from './icons/IconClose.vue';

const props = defineProps({
    modalText: String,
    confirmText: String,
    confirm: Function,
    isVisible: Boolean
})
const emits = defineEmits(['closeModal'])

const closeModal = () => {
    emits('closeModal')
}
const confirmAction = () => {
    if (props.confirm) props.confirm()
    closeModal()
}

</script>
<template>
    <div v-if='$props.isVisible'>
        <p>{{props.modalText}}</p>
        <button @click='closeModal'><IconClose/></button>
        <button @click='confirmAction'>{{ $props.confirmText }}</button>
    </div>
</template>