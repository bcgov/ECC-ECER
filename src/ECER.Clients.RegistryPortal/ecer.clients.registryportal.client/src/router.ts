import { createRouter, createWebHistory } from "vue-router";

import { useApplicationStore } from "./store/application";
import { useUserStore } from "./store/user";

const router = createRouter({
  history: createWebHistory(),
  scrollBehavior(_to, _from, _savedPosition) {
    // always scroll to top
    return { top: 0 };
  },
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
              name: "in-progress",
              component: () => import("./components/Certifications.vue"),
            },
            {
              path: "completed",
              name: "completed",
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
      component: () => import("./components/pages/Application.vue"),
      meta: { requiresAuth: true },
    },
    {
      path: "/new-user",
      component: () => import("./components/pages/NewUser.vue"),
      meta: { requiresAuth: true },
    },
    {
      path: "/new-user/terms-of-use",
      component: () => import("./components/pages/TermsOfUse.vue"),
      meta: { requiresAuth: true },
      props: { hasBackButton: true },
    },
    {
      path: "/terms-of-use",
      component: () => import("./components/pages/TermsOfUse.vue"),
      meta: { requiresAuth: false },
    },
    {
      path: "/submitted",
      component: () => import("./components/pages/Submitted.vue"),
      meta: { requiresAuth: true },
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

// Gaurd for authenticated routes
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

// Guard for authenticated routes that require user info
router.beforeEach((to, _) => {
  const userStore = useUserStore();

  // instead of having to check every route record with
  // to.matched.some(record => record.meta.requiresAuth)
  if (!to.path.startsWith("/new-user") && to.meta.requiresAuth && userStore.isAuthenticated && !userStore.hasUserInfo) {
    return {
      path: "/new-user",
    };
  }
});

// Guard for login page (redirect to dashboard if already authenticated)
router.beforeEach((to, _, next) => {
  const userStore = useUserStore();

  if (to.path === "/login" && userStore.isAuthenticated) next({ path: "/" });
  else next();
});

// Guard for /new-user page(s) (redirect to dashboard if already authenticated and has user info)
router.beforeEach((to, _, next) => {
  const userStore = useUserStore();

  if (to.path.startsWith("/new-user") && userStore.isAuthenticated && userStore.hasUserInfo) next({ path: "/" });
  else next();
});

// Guard to save draft application before navigating away from /application
router.beforeEach((_, from, next) => {
  const applicationStore = useApplicationStore();

  if (from.path === "/application" && applicationStore.hasDraftApplication) {
    applicationStore.saveDraft();
    next();
  } else next();
});

export default router;
