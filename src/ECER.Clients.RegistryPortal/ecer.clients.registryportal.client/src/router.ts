import { createRouter, createWebHistory } from "vue-router";

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: "/",
      component: () => import("./components/pages/Home.vue"),
    },
    {
      path: "/login",
      component: () => import("./components/pages/Login.vue"),
    },
  ],
});

export default router;
