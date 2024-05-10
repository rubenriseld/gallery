<script setup lang="ts">
import PillCheckbox from './PillCheckbox.vue'

const props = defineProps({
    tags: Array,
    formData: Object
})

</script>

<template>
    <div class="form-input-wrapper">
        <label for="tagIds">Tags</label>
        <div class="tag-wrapper">
            <PillCheckbox v-for="(tag, tagIndex) in tags" :key="tagIndex" :tag="tag" :tagIndex="tagIndex"
                :formData="formData" />
        </div>
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

.tag-wrapper {
    display: flex;
    flex-wrap: wrap;
    margin: 0 -0.5rem 0 -0.5rem;
}
</style>