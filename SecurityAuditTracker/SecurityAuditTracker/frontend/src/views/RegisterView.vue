<script setup>
import { ref } from 'vue'
import { useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'

const name = ref('')
const email = ref('')
const password = ref('')
const role = ref('Auditor')
const error = ref('')
const loading = ref(false)

const auth = useAuthStore()
const router = useRouter()

async function handleSubmit() {
  error.value = ''
  loading.value = true
  try {
    await auth.register(name.value, email.value, password.value, role.value)
    router.push('/dashboard')
  } catch (err) {
    error.value = err.response?.data || 'Registration failed.'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="auth-card">
    <h2>Create Account</h2>
    <form @submit.prevent="handleSubmit">
      <label>Name</label>
      <input v-model="name" required />

      <label>Email</label>
      <input v-model="email" type="email" required />

      <label>Password (min 8 chars)</label>
      <input v-model="password" type="password" minlength="8" required />

      <label>Role</label>
      <select v-model="role">
        <option value="Auditor">Auditor</option>
        <option value="Owner">Owner</option>
        <option value="Manager">Manager</option>
      </select>

      <p v-if="error" class="error">{{ error }}</p>

      <button type="submit" :disabled="loading">{{ loading ? 'Creating...' : 'Register' }}</button>
    </form>
    <p>Already have an account? <router-link to="/login">Sign in</router-link></p>
  </div>
</template>

<style scoped>
.auth-card { max-width: 360px; margin: 4rem auto; padding: 2rem; border: 1px solid #e2e8f0; border-radius: 8px; }
label { display: block; margin-top: 0.75rem; font-size: 0.85rem; color: #475569; }
input, select { width: 100%; padding: 0.5rem; margin-top: 0.25rem; border: 1px solid #cbd5e1; border-radius: 4px; box-sizing: border-box; }
button { width: 100%; margin-top: 1.25rem; padding: 0.6rem; background: #1e293b; color: white; border: none; border-radius: 4px; cursor: pointer; }
.error { color: #dc2626; font-size: 0.85rem; margin-top: 0.75rem; }
</style>
