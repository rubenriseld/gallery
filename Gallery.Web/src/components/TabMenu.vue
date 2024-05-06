<script setup lang="ts">
import { ref, onMounted} from 'vue'
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

// Expose methods and variables
const expose = { setActiveTab, tabs, currentTab };
</script>
<template>
    <div>
        <div class="tabs">
            <button v-for="(tab, index) in tabs" :key="index" @click="setActiveTab(index)"
                :class="{ active: activeTab === index }">{{ tab }}</button>
        </div>
        <div class="tab-content">
            <slot :name="currentTab"></slot>
        </div>
    </div>
</template>
<style scoped>
.tabs {
    display: flex;
}

.tabs button {
    cursor: pointer;
    padding: 10px 15px;
    border: none;
    background-color: #f0f0f0;
    margin-right: 5px;
}

.tabs button.active {
    background-color: #ccc;
}

.tab-content {
    margin-top: 10px;
}
</style>