<script setup>
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import api from '../services/api'

const route = useRoute()
const router = useRouter()
const isEdit = !!route.params.id

const form = ref({
  title: '',
  description: '',
  severity: 'Medium',
  frameworkReference: 'ISO27001',
  controlReference: '',
  ownerId: null,
  dueDate: ''
})
const error = ref('')
const loading = ref(false)

onMounted(async () => {
  if (isEdit) {
    const { data } = await api.get(`/findings/${route.params.id}`)
    form.value = {
      title: data.title,
      description: data.description,
      severity: data.severity,
      frameworkReference: data.frameworkReference,
      controlReference: data.controlReference || '',
      ownerId: null, // owner reassignment by ID requires a user picker; left as future enhancement
      dueDate: data.dueDate ? data.dueDate.substring(0, 10) : ''
    }
  }
})

async function handleSubmit() {
  error.value = ''
  loading.value = true
  try {
    const payload = { ...form.value, dueDate: form.value.dueDate || null }
    if (isEdit) {
      await api.put(`/findings/${route.params.id}`, payload)
      router.push(`/findings/${route.params.id}`)
    } else {
      const { data } = await api.post('/findings', payload)
      router.push(`/findings/${data.id}`)
    }
  } catch (err) {
    error.value = 'Could not save finding. Check required fields.'
  } finally {
    loading.value = false
  }
}
</script>

<template>
  <div class="form-card">
    <h1>{{ isEdit ? 'Edit Finding' : 'New Finding' }}</h1>
    <form @submit.prevent="handleSubmit">
      <label>Title</label>
      <input v-model="form.title" required />

      <label>Description</label>
      <textarea v-model="form.description" required></textarea>

      <label>Severity</label>
      <select v-model="form.severity">
        <option>Critical</option><option>High</option><option>Medium</option><option>Low</option>
      </select>

      <label>Framework Reference</label>
      <select v-model="form.frameworkReference">
        <option value="ISO27001">ISO 27001</option>
        <option value="PCIDSSv4">PCI DSS v4.0</option>
        <option value="NISTSP80053">NIST SP 800-53</option>
        <option value="BoGGuidelines">Bank of Ghana Guidelines</option>
        <option value="Other">Other</option>
      </select>

      <label>Control Reference (optional)</label>
      <input v-model="form.controlReference" placeholder="e.g. A.9.2.3" />

      <label>Due Date</label>
      <input v-model="form.dueDate" type="date" />

      <p v-if="error" class="error">{{ error }}</p>
      <button type="submit" :disabled="loading">{{ loading ? 'Saving...' : 'Save Finding' }}</button>
    </form>
  </div>
</template>

<style scoped>
.form-card { max-width: 520px; }
label { display: block; margin-top: 1rem; font-size: 0.85rem; color: #475569; }
input, select, textarea { width: 100%; padding: 0.5rem; margin-top: 0.25rem; border: 1px solid #cbd5e1; border-radius: 4px; box-sizing: border-box; font-family: inherit; }
textarea { min-height: 90px; }
button { margin-top: 1.5rem; padding: 0.6rem 1.2rem; background: #1e293b; color: white; border: none; border-radius: 4px; cursor: pointer; }
.error { color: #dc2626; margin-top: 0.75rem; }
</style>
