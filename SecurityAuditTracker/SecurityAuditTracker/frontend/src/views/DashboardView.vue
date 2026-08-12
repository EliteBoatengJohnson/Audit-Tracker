<script setup>
import { ref, onMounted } from 'vue'
import api from '../services/api'

const summary = ref(null)
const error = ref('')

onMounted(async () => {
  try {
    const { data } = await api.get('/dashboard/summary')
    summary.value = data
  } catch (err) {
    error.value = 'Could not load dashboard summary.'
  }
})
</script>

<template>
  <div>
    <h1>Dashboard</h1>
    <p v-if="error" class="error">{{ error }}</p>

    <div v-if="summary" class="cards">
      <div class="card"><span class="num">{{ summary.totalFindings }}</span><span>Total Findings</span></div>
      <div class="card open"><span class="num">{{ summary.openCount }}</span><span>Open</span></div>
      <div class="card"><span class="num">{{ summary.inProgressCount }}</span><span>In Progress</span></div>
      <div class="card resolved"><span class="num">{{ summary.resolvedCount }}</span><span>Resolved</span></div>
      <div class="card"><span class="num">{{ summary.acceptedRiskCount }}</span><span>Accepted Risk</span></div>
      <div class="card overdue"><span class="num">{{ summary.overdueCount }}</span><span>Overdue</span></div>
    </div>

    <div v-if="summary" class="severity-breakdown">
      <h3>By Severity</h3>
      <ul>
        <li v-for="(count, sev) in summary.bySeverity" :key="sev">{{ sev }}: {{ count }}</li>
      </ul>
    </div>
  </div>
</template>

<style scoped>
.cards { display: grid; grid-template-columns: repeat(auto-fit, minmax(140px, 1fr)); gap: 1rem; margin-top: 1.5rem; }
.card { background: #f8fafc; border: 1px solid #e2e8f0; border-radius: 8px; padding: 1rem; display: flex; flex-direction: column; align-items: center; }
.card .num { font-size: 1.8rem; font-weight: 700; }
.card.open .num { color: #dc2626; }
.card.resolved .num { color: #16a34a; }
.card.overdue .num { color: #ea580c; }
.severity-breakdown { margin-top: 2rem; }
.error { color: #dc2626; }
</style>
