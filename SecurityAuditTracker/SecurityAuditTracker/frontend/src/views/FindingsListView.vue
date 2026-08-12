<script setup>
import { ref, onMounted, watch } from 'vue'
import api from '../services/api'

const findings = ref([])
const statusFilter = ref('')
const severityFilter = ref('')
const error = ref('')

async function loadFindings() {
  error.value = ''
  try {
    const params = {}
    if (statusFilter.value) params.status = statusFilter.value
    if (severityFilter.value) params.severity = severityFilter.value
    const { data } = await api.get('/findings', { params })
    findings.value = data
  } catch (err) {
    error.value = 'Could not load findings.'
  }
}

onMounted(loadFindings)
watch([statusFilter, severityFilter], loadFindings)

function severityClass(sev) {
  return `badge sev-${sev.toLowerCase()}`
}
</script>

<template>
  <div>
    <div class="header-row">
      <h1>Findings</h1>
    </div>

    <div class="filters">
      <select v-model="statusFilter">
        <option value="">All Statuses</option>
        <option value="Open">Open</option>
        <option value="InProgress">In Progress</option>
        <option value="Resolved">Resolved</option>
        <option value="AcceptedRisk">Accepted Risk</option>
      </select>
      <select v-model="severityFilter">
        <option value="">All Severities</option>
        <option value="Critical">Critical</option>
        <option value="High">High</option>
        <option value="Medium">Medium</option>
        <option value="Low">Low</option>
      </select>
    </div>

    <p v-if="error" class="error">{{ error }}</p>

    <table v-if="findings.length" class="findings-table">
      <thead>
        <tr>
          <th>Title</th><th>Severity</th><th>Status</th><th>Owner</th><th>Due</th><th></th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="f in findings" :key="f.id" :class="{ overdue: f.isOverdue }">
          <td>{{ f.title }}</td>
          <td><span :class="severityClass(f.severity)">{{ f.severity }}</span></td>
          <td>{{ f.status }}</td>
          <td>{{ f.ownerName || 'Unassigned' }}</td>
          <td>{{ f.dueDate ? new Date(f.dueDate).toLocaleDateString() : '—' }}</td>
          <td><router-link :to="`/findings/${f.id}`">View</router-link></td>
        </tr>
      </tbody>
    </table>
    <p v-else-if="!error">No findings match these filters.</p>
  </div>
</template>

<style scoped>
.header-row { display: flex; justify-content: space-between; align-items: center; }
.filters { display: flex; gap: 0.75rem; margin: 1rem 0; }
.filters select { padding: 0.4rem; border-radius: 4px; border: 1px solid #cbd5e1; }
.findings-table { width: 100%; border-collapse: collapse; }
.findings-table th, .findings-table td { text-align: left; padding: 0.6rem; border-bottom: 1px solid #e2e8f0; }
.findings-table tr.overdue { background: #fef2f2; }
.badge { padding: 0.15rem 0.5rem; border-radius: 12px; font-size: 0.75rem; font-weight: 600; }
.sev-critical { background: #fee2e2; color: #b91c1c; }
.sev-high { background: #ffedd5; color: #c2410c; }
.sev-medium { background: #fef9c3; color: #a16207; }
.sev-low { background: #dcfce7; color: #166534; }
.error { color: #dc2626; }
</style>
