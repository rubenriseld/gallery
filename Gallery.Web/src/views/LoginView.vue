<script setup lang="ts">
import { ref, defineProps } from "vue"
import api from "@/api"
import { useRouter } from "vue-router"

const props = defineProps({
    queryParam: { type: String, default: null },
})

const formData = ref({
    email: '',
    password: ''
})

const router = useRouter()

async function login() {
    try {
        const response = await api.post('auth/login',
            JSON.stringify(formData.value))

        if (response.status === 200) {
            router.push(props.queryParam ? props.queryParam : "/")
        }
    } catch (error) {
        console.error("Error logging in:", error)
    }
}
</script>
<template>
    <h2>Login</h2>
    <form @submit.prevent="login">
        <div>
            <label for="email">Email:</label>
            <input type="email" id="email" v-model="formData.email" required>
        </div>
        <div>
            <label for="password">Password:</label>
            <input type="password" id="password" v-model="formData.password" required>
        </div>
        <button type="submit">Login</button>
    </form>
</template>