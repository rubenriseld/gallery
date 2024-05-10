<script setup lang="ts">
import { ref, onMounted } from 'vue'
import { useRouter } from 'vue-router'

const tabs = ref<string[]>(['Collections', 'Tags', 'Images'])
const activeTab = ref(0)
const currentTab = ref<string>('Collections')
const router = useRouter()

const setActiveTab = (index: number) => {
    activeTab.value = index;
    currentTab.value = tabs.value[index];
    router.push({ query: { tab: index.toString() } })
}

// Restore active tab from URL query parameter on component mount
onMounted(() => {
    const tabParam = router.currentRoute.value.query.tab
    if (tabParam && !Array.isArray(tabParam) && !isNaN(Number(tabParam))) {
        const index = parseInt(tabParam as string)
        if (index >= 0 && index < tabs.value.length) {
            setActiveTab(index)
        }
    }
})

</script>

<template>
    <div class="tab-wrapper">
        <div class="tab-menu">
            <button v-for="(tab, index) in tabs"
                :key="index"
                @click="setActiveTab(index)"
                :class="{ active: activeTab === index }"
                class="tab-button">{{ tab }}</button>
        </div>
        <div class="tab-content">
            <slot :name="currentTab"></slot>
        </div>
    </div>
</template>

<style scoped>
.tab-wrapper {
    width: 100%;
    display: flex;
    flex-direction: column
}

.tab-menu {
    display: flex;
    width: 100%;
    margin: 2rem 2rem 1.5rem 2rem;
    justify-items: center;
}

.tab-content {
    display: flex;
}

.tab-button {
    text-transform: uppercase;
    margin-right: 1.5rem;
    font-size: 1.2rem;
    cursor: pointer;
    border: none;
    font-family: inherit;
    padding-bottom: 0.5rem;
}

.tab-button.active,
.tab-button.active:hover,
.tab-button:hover {
    background: linear-gradient(var(--primary-color), var(--primary-color)) no-repeat;
    background-size: 100% 4px;
    background-position: 0 100%;
}

.tab-button:hover {
    background: linear-gradient(var(--secondary-color), var(--secondary-color)) no-repeat;
    background-size: 100% 2px;
    background-position: 0 95%;
}

@media (max-width: 768px) {
    .tab-menu {
        margin: 1rem 1rem 0.5rem 1rem;
    }

    .tab-button {
        font-size: 1rem;
        margin-right: 1rem;
    }
}
</style>