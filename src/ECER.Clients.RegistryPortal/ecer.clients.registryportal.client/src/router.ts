import { createRouter, createWebHistory } from "vue-router";

import { useUserStore } from "./store/user";

const router = createRouter({
  history: createWebHistory(),
  routes: [
    {
      path: "/",
      redirect: "/my-certifications",
      component: () => import("./components/pages/Dashboard.vue"),
      meta: { requiresAuth: true },
      children: [
        {
          path: "profile",
          component: () => import("./components/Profile.vue"),
        },
        {
          path: "my-certifications",
          redirect: "/my-certifications/in-progress",
          component: () => import("./components/CertificationTabs.vue"),
          children: [
            {
              path: "in-progress",
              component: () => import("./components/Certifications.vue"),
            },
            {
              path: "completed",
              component: () => import("./components/Certifications.vue"),
            },
          ],
        },
        {
          path: "messages",
          component: () => import("./components/Messages.vue"),
        },
        {
          path: "settings",
          component: () => import("./components/Settings.vue"),
        },
      ],
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
      path: "/application",
      component: () => import("./components/Wizard.vue"),
      meta: { requiresAuth: true },
    },
    {
      path: "/new-user",
      component: () => import("./components/pages/NewUser.vue"),
      meta: { requiresAuth: true },
    },
    {
      path: "/terms-of-use",
      component: () => import("./components/pages/TermsOfUse.vue"),
      meta: { requiresAuth: false },
    },
    {
      path: "/terms-of-use/from-new-user",
      component: () => import("./components/pages/TermsOfUse.vue"),
      meta: { requiresAuth: true },
      props: { hasBackButton: true },
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
    };
  }
});

// Guard for login page (redirect to dashboard if already logged in)
router.beforeEach((to, _, next) => {
  const userStore = useUserStore();

  if (to.path === "/login" && userStore.isAuthenticated) next({ path: "/" });
  else next();
});

export default router;
