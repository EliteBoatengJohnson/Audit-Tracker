<script setup>
import { useAuthStore } from './stores/auth'
import { useRouter } from 'vue-router'

const auth = useAuthStore()
const router = useRouter()

function handleLogout() {
  auth.logout()
  router.push('/login')
}
</script>

<template>
  <div class="app-shell">
    <nav v-if="auth.isAuthenticated" class="navbar">
      <div class="nav-brand">Security Audit Tracker</div>
      <div class="nav-links">
        <router-link to="/dashboard">Dashboard</router-link>
        <router-link to="/findings">Findings</router-link>
        <router-link v-if="['Auditor','Manager'].includes(auth.role)" to="/findings/new">New Finding</router-link>
      </div>
      <div class="nav-user">
        <span>{{ auth.user?.name }} ({{ auth.role }})</span>
        <button @click="handleLogout">Logout</button>
      </div>
    </nav>
    <main class="content">
      <router-view />
    </main>
  </div>
</template>

<style scoped>
.app-shell { min-height: 100vh; display: flex; flex-direction: column; }
.navbar {
  display: flex; align-items: center; justify-content: space-between;
  padding: 0.75rem 1.5rem; background: #1e293b; color: white;
}
.nav-brand { font-weight: 700; }
.nav-links a { color: #cbd5e1; margin-right: 1.25rem; text-decoration: none; }
.nav-links a.router-link-active { color: white; font-weight: 600; }
.nav-user { display: flex; align-items: center; gap: 0.75rem; font-size: 0.9rem; }
.nav-user button {
  background: #334155; color: white; border: none; padding: 0.4rem 0.8rem;
  border-radius: 4px; cursor: pointer;
}
.content { flex: 1; padding: 1.5rem; max-width: 1100px; margin: 0 auto; width: 100%; }
</style>
