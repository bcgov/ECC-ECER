import { createRouter, createWebHistory } from "vue-router";
import { useOidcStore } from "./store/oidc";
import { useUserStore } from "./store/user";

const router = createRouter({
  history: createWebHistory(),
  scrollBehavior(_to, _from, _savedPosition) {
    return { top: 0 };
  },
  routes: [
    {
      path: "/",
      component: () => import("./components/pages/Dashboard.vue"),
      meta: { requiresAuth: false }, // This is handled in dashboard component itself
      name: "dashboard",
    },
    {
      path: "/login",
      component: () => import("./components/pages/Login.vue"),
      meta: { requiresAuth: false },
      name: "login",
    },
    {
      path: "/silent-callback",
      component: () => import("./components/pages/SilentCallback.vue"),
      meta: { requiresAuth: false },
    },
    {
      path: "/verify/:token",
      component: () => import("./components/pages/Verify.vue"),
      meta: { requiresAuth: false },
    },
    {
      path: "/invalid-invitation",
      component: () => import("./components/pages/InvalidInvitation.vue"),
      meta: { requiresAuth: false },
      name: "invalid-invitation",
    },
    {
      path: "/required-invitation",
      component: () => import("./components/pages/RequiredInvitation.vue"),
      meta: { requiresAuth: false },
      name: "required-invitation",
    },
    {
      path: "/generic-registration-error",
      component: () => import("./components/pages/GenericRegistrationError.vue"),
      meta: { requiresAuth: false },
      name: "generic-registration-error",
    },
    {
      path: "/access-denied",
      component: () => import("./components/pages/AccessDenied.vue"),
      meta: { requiresAuth: false },
      name: "access-denied",
    },
    {
      path: "/logout-callback",
      component: () => import("./components/pages/LogoutCallback.vue"),
      meta: { requiresAuth: false },
    },
    { path: "/:pathMatch(.*)*", name: "not-found", component: () => import("./components/pages/PageNotFound.vue") },
  ],
});

// Gaurd for authenticated routes
router.beforeEach(async (to, _) => {
  const oidcStore = useOidcStore();
  const userStore = useUserStore();

  const user = await oidcStore.getUser();

  if (!user) {
    // Reset user store to clear any stale data
    userStore.setPspUserProfile(null);
  }

  // instead of having to check every route record with
  // to.matched.some(record => record.meta.requiresAuth)
  if (to.meta.requiresAuth && !user) {
    // this route requires auth, check if logged in
    // if not, redirect to login page.
    return {
      path: "/login",
      query: { redirect_to: to.fullPath },
    };
  }
});

// Guard for login page (redirect to dashboard if already authenticated)
router.beforeEach(async (to, _, next) => {
  const oidcStore = useOidcStore();
  const user = await oidcStore.getUser();

  if (to.path === "/login" && user) next({ path: "/" });
  else next();
});

export default router;
