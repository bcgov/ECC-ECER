import { createRouter, createWebHistory } from "vue-router";

const router = createRouter({
  history: createWebHistory(),
  scrollBehavior(_to, _from, _savedPosition) {
    return { top: 0 };
  },
  routes: [
    {
      path: "/",
      component: () => import("./components/pages/Dashboard.vue"),
      name: "dashboard",
    },
  ],
});

export default router;
