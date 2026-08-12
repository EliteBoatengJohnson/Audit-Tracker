<script setup>
import { ref, onMounted } from 'vue'
import { useRoute, useRouter } from 'vue-router'
import { useAuthStore } from '../stores/auth'
import api from '../services/api'

const route = useRoute()
const router = useRouter()
const auth = useAuthStore()

const finding = ref(null)
const logs = ref([])
const comment = ref('')
const newStatus = ref('')
const error = ref('')

async function load() {
  const { data: f } = await api.get(`/findings/${route.params.id}`)
  finding.value = f
  const { data: l } = await api.get(`/findings/${route.params.id}/remediation`)
  logs.value = l
}

onMounted(load)

async function addLog() {
  error.value = ''
  try {
    await api.post(`/findings/${route.params.id}/remediation`, {
      comment: comment.value,
      statusChangedTo: newStatus.value || null
    })
    comment.value = ''
    newStatus.value = ''
    await load()
  } catch (err) {
    error.value = 'Could not add remediation update.'
  }
}

async function deleteFinding() {
  if (!confirm('Delete this finding permanently?')) return
  await api.delete(`/findings/${route.params.id}`)
  router.push('/findings')
}
</script>

<template>
  <div v-if="finding">
    <div class="header-row">
      <h1>{{ finding.title }}</h1>
      <div class="actions" v-if="['Auditor','Manager'].includes(auth.role)">
        <router-link :to="`/findings/${finding.id}/edit`">Edit</router-link>
        <button v-if="auth.role === 'Manager'" @click="deleteFinding" class="danger">Delete</button>
      </div>
    </div>

    <div class="meta">
      <span class="badge">{{ finding.severity }}</span>
      <span class="badge">{{ finding.status }}</span>
      <span class="badge">{{ finding.frameworkReference }}<span v-if="finding.controlReference"> — {{ finding.controlReference }}</span></span>
      <span v-if="finding.isOverdue" class="badge overdue-badge">OVERDUE</span>
    </div>

    <p class="description">{{ finding.description }}</p>

    <dl class="details">
      <dt>Owner</dt><dd>{{ finding.ownerName || 'Unassigned' }}</dd>
      <dt>Created by</dt><dd>{{ finding.createdByName }}</dd>
      <dt>Due date</dt><dd>{{ finding.dueDate ? new Date(finding.dueDate).toLocaleDateString() : '—' }}</dd>
      <dt>Created</dt><dd>{{ new Date(finding.createdAt).toLocaleString() }}</dd>
    </dl>

    <h3>Remediation / Audit Trail</h3>
    <ul class="log-list">
      <li v-for="log in logs" :key="log.id">
        <div class="log-meta">{{ log.changedByName }} · {{ new Date(log.timestamp).toLocaleString() }}
          <span v-if="log.statusChangedTo"> · status → {{ log.statusChangedTo }}</span>
        </div>
        <div>{{ log.comment }}</div>
      </li>
      <li v-if="!logs.length">No remediation updates yet.</li>
    </ul>

    <form @submit.prevent="addLog" class="log-form">
      <textarea v-model="comment" placeholder="Add a remediation update..." required></textarea>
      <select v-model="newStatus">
        <option value="">Keep current status</option>
        <option value="Open">Open</option>
        <option value="InProgress">In Progress</option>
        <option value="Resolved">Resolved</option>
        <option value="AcceptedRisk">Accepted Risk</option>
      </select>
      <p v-if="error" class="error">{{ error }}</p>
      <button type="submit">Add Update</button>
    </form>
  </div>
</template>

<style scoped>
.header-row { display: flex; justify-content: space-between; align-items: center; }
.actions { display: flex; gap: 0.75rem; align-items: center; }
.actions button.danger { background: #dc2626; color: white; border: none; padding: 0.4rem 0.8rem; border-radius: 4px; cursor: pointer; }
.meta { display: flex; gap: 0.5rem; margin: 0.75rem 0; }
.badge { background: #f1f5f9; padding: 0.2rem 0.6rem; border-radius: 12px; font-size: 0.8rem; }
.overdue-badge { background: #fee2e2; color: #b91c1c; }
.description { margin: 1rem 0; line-height: 1.5; }
.details { display: grid; grid-template-columns: 120px 1fr; gap: 0.4rem; margin: 1rem 0 2rem; }
.details dt { color: #64748b; }
.log-list { list-style: none; padding: 0; margin: 1rem 0; }
.log-list li { border-bottom: 1px solid #e2e8f0; padding: 0.6rem 0; }
.log-meta { font-size: 0.8rem; color: #64748b; }
.log-form { display: flex; flex-direction: column; gap: 0.5rem; max-width: 480px; margin-top: 1rem; }
.log-form textarea { padding: 0.5rem; border: 1px solid #cbd5e1; border-radius: 4px; min-height: 70px; }
.log-form select { padding: 0.4rem; border-radius: 4px; border: 1px solid #cbd5e1; }
.log-form button { padding: 0.5rem; background: #1e293b; color: white; border: none; border-radius: 4px; cursor: pointer; }
.error { color: #dc2626; }
</style>
