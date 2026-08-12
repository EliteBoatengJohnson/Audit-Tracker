import { createRouter, createWebHistory } from 'vue-router'
import { useAuthStore } from '../stores/auth'

import LoginView from '../views/LoginView.vue'
import RegisterView from '../views/RegisterView.vue'
import DashboardView from '../views/DashboardView.vue'
import FindingsListView from '../views/FindingsListView.vue'
import FindingDetailView from '../views/FindingDetailView.vue'
import FindingFormView from '../views/FindingFormView.vue'

const routes = [
  { path: '/', redirect: '/dashboard' },
  { path: '/login', name: 'login', component: LoginView, meta: { public: true } },
  { path: '/register', name: 'register', component: RegisterView, meta: { public: true } },
  { path: '/dashboard', name: 'dashboard', component: DashboardView },
  { path: '/findings', name: 'findings-list', component: FindingsListView },
  { path: '/findings/new', name: 'findings-new', component: FindingFormView, meta: { roles: ['Auditor', 'Manager'] } },
  { path: '/findings/:id', name: 'findings-detail', component: FindingDetailView, props: true },
  { path: '/findings/:id/edit', name: 'findings-edit', component: FindingFormView, props: true, meta: { roles: ['Auditor', 'Manager'] } }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

router.beforeEach((to) => {
  const auth = useAuthStore()

  if (!to.meta.public && !auth.isAuthenticated) {
    return { name: 'login' }
  }

  if (to.meta.roles && !to.meta.roles.includes(auth.role)) {
    return { name: 'dashboard' }
  }

  return true
})

export default router
