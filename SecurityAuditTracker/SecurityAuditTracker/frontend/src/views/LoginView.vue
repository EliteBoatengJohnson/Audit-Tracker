<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const email = ref('')
const password = ref('')
const error = ref('')
const loading = ref(false)

const auth = useAuthStore()
const router = useRouter()

async function handleSubmit() {
  error.value = ''
  loading.value = true
  try {
    await auth.login(email.value, password.value)
    router.push('/dashboard')
  } catch (err) {
    error.value = err.response?.data || 'Login failed. Check your credentials.'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="auth-card">
    <h2>Sign In</h2>
    <form @submit.prevent="handleSubmit">
      <label>Email</label>
      <input v-model="email" type="email" required />

      <label>Password</label>
      <input v-model="password" type="password" required />

      <p v-if="error" class="error">{{ error }}</p>

      <button type="submit" :disabled="loading">{{ loading ? 'Signing in...' : 'Sign In' }}</button>
    </form>
    <p>No account? <router-link to="/register">Register</router-link></p>
  </div>
</template>

<style scoped>
.auth-card { max-width: 360px; margin: 4rem auto; padding: 2rem; border: 1px solid #e2e8f0; border-radius: 8px; }
label { display: block; margin-top: 0.75rem; font-size: 0.85rem; color: #475569; }
input { width: 100%; padding: 0.5rem; margin-top: 0.25rem; border: 1px solid #cbd5e1; border-radius: 4px; box-sizing: border-box; }
button { width: 100%; margin-top: 1.25rem; padding: 0.6rem; background: #1e293b; color: white; border: none; border-radius: 4px; cursor: pointer; }
.error { color: #dc2626; font-size: 0.85rem; margin-top: 0.75rem; }
</style>
