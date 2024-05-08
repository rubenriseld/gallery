<script setup lang="ts">
import { ref } from "vue"
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
    <div class="login-wrapper">
        <h2>Login</h2>
        <form @submit.prevent="login">
            <div class="form-input-wrapper">
                <label for="email">Email:</label>
                <input type="email" id="email" v-model="formData.email" required placeholder="Email">
            </div>
            <div class="form-input-wrapper">
                <label for="password">Password:</label>
                <input type="password" id="password" v-model="formData.password" required placeholder="Password">
            </div>
                <button type="submit" class="login-button">Login</button>
        </form>
    </div>
</template>
<style scoped>
.login-wrapper {
    flex-direction: column;
    align-items: center;
}
h2 {
    font-size: 2rem;
}
form{
    width: 50%;
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
.form-input-wrapper {
    width: 100%;
    margin: 1rem 0 1rem 0;
    display: flex;
    flex-direction: column;
}
label,
.login-button {
    color: #121212;
    font-family: inherit;
    font-size: 1.2rem;
}
.login-button {
    padding: 2rem;
    background-color: #fff;
}
.login-button:hover {
    background-color: #f0f0f0;
    cursor: pointer;
}
@media (max-width: 768px) {
    form {
        width: 100%;
    }

    .show-menu>.menu-item-wrapper {
        display: flex;
        justify-content: center;
    }
}
</style>