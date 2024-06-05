import { createRouter, createWebHistory } from "vue-router";

import { useApplicationStore } from "./store/application";
import { useOidcStore } from "./store/oidc";
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
      component: () => import("./components/pages/Dashboard.vue"),
      meta: { requiresAuth: true },
    },
    {
      path: "/profile",
      component: () => import("./components/Profile.vue"),
      meta: { requiresAuth: true },
    },
    {
      path: "/messages",
      component: () => import("./components/Messages.vue"),
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
      path: "/manage-application/:applicationId",
      name: "manageApplication",
      component: () => import("./components/ApplicationSummary.vue"),
      meta: { requiresAuth: true },
    },
    {
      path: "/manage-application/:applicationId/work-experience-reference/:referenceId",
      name: "viewWorkExperienceReference",
      component: () => import("./components/ViewWorkExperienceReference.vue"),
      meta: { requiresAuth: true },
      props: true,
    },
    {
      path: "/manage-application/:applicationId/character-reference/:referenceId",
      name: "viewCharacterReference",
      component: () => import("./components/ViewCharacterReference.vue"),
      meta: { requiresAuth: true },
      props: true,
    },
    {
      path: "/manage-application/:applicationId/work-experience-reference/:referenceId/edit",
      name: "upsertWorkExperienceReference",
      component: () => import("./components/UpsertWorkExperienceReference.vue"),
      meta: { requiresAuth: true },
      props: true,
    },
    {
      path: "/manage-application/:applicationId/character-reference/:referenceId/edit",
      name: "upsertCharacterReference",
      component: () => import("./components/UpsertCharacterReference.vue"),
      meta: { requiresAuth: true },
      props: true,
    },
    {
      path: "/manage-application/:applicationId/work-experience-reference/add",
      name: "addWorkExperienceReference",
      component: () => import("./components/UpsertWorkExperienceReference.vue"),
      meta: { requiresAuth: true },
      props: true,
    },
    {
      path: "/manage-application/:applicationId/character-reference/add",
      name: "addCharacterReference",
      component: () => import("./components/UpsertCharacterReference.vue"),
      meta: { requiresAuth: true },
      props: true,
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
      path: "/submitted/:applicationId",
      name: "submitted",
      component: () => import("./components/pages/Submitted.vue"),
      props: true,
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
    {
      path: "/verify/:token",
      component: () => import("./components/reference/Reference.vue"),
      meta: { requiresAuth: false },
    },
    {
      path: "/invalid-reference",
      component: () => import("./components/reference/Invalid.vue"),
      meta: { requiresAuth: false },
    },
    {
      path: "/reference-submitted",
      component: () => import("./components/pages/ReferenceSubmitted.vue"),
      meta: { requiresAuth: false },
    },
    { path: "/:pathMatch(.*)*", name: "not-found", component: () => import("./components/pages/PageNotFound.vue") },
  ],
});

// Gaurd for authenticated routes
router.beforeEach(async (to, _) => {
  const oidcStore = useOidcStore();
  const user = await oidcStore.getUser();

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

// Guard for authenticated routes that require user info
router.beforeEach(async (to, _) => {
  const userStore = useUserStore();
  const oidcStore = useOidcStore();
  const user = await oidcStore.getUser();

  // instead of having to check every route record with
  // to.matched.some(record => record.meta.requiresAuth)
  if (!to.path.startsWith("/new-user") && to.meta.requiresAuth && user && !userStore.hasUserInfo) {
    return {
      path: "/new-user",
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

// Guard for /new-user page(s) (redirect to dashboard if already authenticated and has user info)
router.beforeEach(async (to, _, next) => {
  const userStore = useUserStore();
  const oidcStore = useOidcStore();
  const user = await oidcStore.getUser();

  if (to.path.startsWith("/new-user") && user && userStore.hasUserInfo) next({ path: "/" });
  else next();
});

// Guard to save draft application before navigating away from /application
router.beforeEach((to, from, next) => {
  const applicationStore = useApplicationStore();

  if (from.path === "/application" && to.name !== "submitted" && applicationStore.hasDraftApplication) {
    applicationStore.saveDraft();
    next();
  } else next();
});

export default router;
