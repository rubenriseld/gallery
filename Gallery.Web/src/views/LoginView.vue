<script setup lang="ts">
import ComponentButton from "@/components/ComponentButton.vue";

import api from "@/api";
import { ref } from "vue";
import { useRouter, useRoute } from "vue-router";
import { useStore } from "vuex";

const formData = ref({
    email: '',
    password: ''
});

const store = useStore();
const router = useRouter();
const route = useRoute();

async function login() {
    try {
        const response = await api.post('auth/login', JSON.stringify(formData.value));
        if (response.status === 200) {
            store.commit('SET_AUTH', true);
            const queryParam = route.query.redirect as string;
            router.push(queryParam || "/");
        }
    } catch (error) {
        console.error("Error logging in:", error);
    }
}
</script>

<template>
    <div class="login-wrapper">
        <h2>Login</h2>
        <form @submit.prevent="login">
            <div class="form-input-wrapper">
                <label for="email">Email:</label>
                <input type="email"
                    id="email"
                    v-model="formData.email"
                    required
                    placeholder="Email">
            </div>
            <div class="form-input-wrapper">
                <label for="password">Password:</label>
                <input type="password"
                    id="password"
                    v-model="formData.password"
                    required
                    placeholder="Password">
            </div>
            <ComponentButton class="login-button"
                buttonType="secondary"
                :submit="true"
                buttonText="Log in" />
        </form>
    </div>
</template>

<style scoped>
.login-wrapper {
    display: flex;
    flex-direction: column;
    align-items: center;
    margin: 1rem;
}

h2 {
    font-size: 2rem;
}

form {
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

.login-button {
    margin-top: 1rem;
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