import { createRouter, createWebHistory } from "vue-router";

import { useUserStore } from "./store/user";

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: "/",
      component: () => import("./components/pages/Home.vue"),
      meta: { requiresAuth: true },
    },

    {
      path: "/login",
      component: () => import("./components/pages/Login.vue"),
      meta: { requiresAuth: false },
    },
    {
      path: "/signin-callback",
      component: () => import("./components/pages/SigninCallback.vue"),
      meta: { requiresAuth: false },
    },
    {
      path: "/silent-callback",
      component: () => import("./components/pages/SilentCallback.vue"),
      meta: { requiresAuth: false },
    },
    {
      path: "/logout-callback",
      component: () => import("./components/pages/LogoutCallback.vue"),
      meta: { requiresAuth: false },
    },
    {
      path: "/terms-of-use",
      component: () => import("./components/pages/TermsOfUse.vue"),
      meta: { requiresAuth: false },
    },
    {
      path: "/accessibility",
      component: () => import("./components/pages/Accessibility.vue"),
      meta: { requiresAuth: false },
    },
    {
      path: "/privacy",
      component: () => import("./components/pages/Privacy.vue"),
      meta: { requiresAuth: false },
    },
    {
      path: "/contact-us",
      component: () => import("./components/pages/ContactUs.vue"),
      meta: { requiresAuth: false },
    },
    {
      path: "/disclaimer",
      component: () => import("./components/pages/Disclaimer.vue"),
      meta: { requiresAuth: false },
    },
  ],
});

// Gaurd for authentication protected routes
router.beforeEach((to, _) => {
  const userStore = useUserStore();

  // instead of having to check every route record with
  // to.matched.some(record => record.meta.requiresAuth)
  if (to.meta.requiresAuth && !userStore.isAuthenticated) {
    // this route requires auth, check if logged in
    // if not, redirect to login page.
    return {
      path: "/login",
      // save the location we were at to come back later
      query: { redirect: to.fullPath },
    };
  }
});

// Guard for login page (redirect to home if already logged in)
router.beforeEach((to, _, next) => {
  const userStore = useUserStore();

  if (to.path === "/login" && userStore.isAuthenticated) next({ path: "/" });
  else next();
});

export default router;
