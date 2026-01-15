import { createRouter, createWebHistory } from "vue-router";
import { useOidcStore } from "./store/oidc";
import { useUserStore } from "./store/user";
import { useFormStore } from "./store/form";
import { useMessageStore } from "./store/message";

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
      path: "/new-user",
      component: () => import("./components/pages/NewUser.vue"),
      meta: { requiresAuth: true },
      name: "new-user",
    },
    {
      path: "/profile/edit",
      component: () => import("./components/pages/EditProfile.vue"),
      name: "edit-profile",
      meta: { requiresAuth: true },
    },
    {
      path: "/messages",
      name: "messages",
      component: () => import("./components/communication/Messages.vue"),
      meta: { requiresAuth: true },
    },
    {
      path: "/messages/:messageId/reply",
      name: "replyToMessage",
      component: () => import("./components/communication/ReplyToMessage.vue"),
      meta: { requiresAuth: true },
    },
    {
      path: "/messages/new",
      name: "newMessage",
      component: () => import("./components/communication/NewMessage.vue"),
      meta: { requiresAuth: true },
    },
    {
      path: "/manage-users/:educationInstitutionName",
      component: () => import("./components/pages/ManageUsers.vue"),
      name: "manage-users",
      meta: { requiresAuth: true },
      props: true,
    },
    {
      path: "/manage-users/:educationInstitutionName/add-user",
      component: () => import("./components/pages/AddUser.vue"),
      name: "add-user",
      meta: { requiresAuth: true },
      props: true,
    },
    {
      path: "/education-institution/edit",
      component: () => import("./components/pages/EditInstitution.vue"),
      name: "edit-education-institution",
      meta: { requiresAuth: true },
    },
    {
      path: "/program/:programId",
      component: () => import("./components/pages/Program.vue"),
      name: "programDetail",
      meta: { requiresAuth: true, requiresVerification: true },
      props: true,
    },
    {
      path: "/program-profiles",
      component: () => import("./components/pages/ProgramProfiles.vue"),
      name: "program-profiles",
      meta: { requiresAuth: true },
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
      meta: { requiresAuth: true },
      name: "required-invitation",
    },
    {
      path: "/generic-registration-error",
      component: () => import("./components/pages/GenericRegistrationError.vue"),
      meta: { requiresAuth: true },
      name: "generic-registration-error",
    },
    {
      path: "/access-denied",
      component: () => import("./components/pages/AccessDenied.vue"),
      meta: { requiresAuth: true },
      name: "access-denied",
    },
        {
      path: "/access-denied-mismatch",
      component: () => import("./components/pages/AccessDeniedMismatch.vue"),
      meta: { requiresAuth: true },
      name: "access-denied-mismatch",
    },
    {
      path: "/terms-of-use",
      component: () => import("./components/pages/PageNotFound.vue"),
      meta: { requiresAuth: false },
      name: "terms-of-use",
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
router.beforeEach(async (to, _, next) => {
  const oidcStore = useOidcStore();
  const userStore = useUserStore();

  const user = await oidcStore.getUser();

  if (!user) {
    const formStore = useFormStore();
    const messageStore = useMessageStore();
    // Reset user store to clear any stale data
    userStore.setPspUserProfile(null);
    userStore.setEducationInstitution(null);
    formStore.$reset();
    messageStore.$reset();
  }

  // instead of having to check every route record with
  // to.matched.some(record => record.meta.requiresAuth)
  if (to.meta.requiresAuth && !user) {
    // this route requires auth, check if logged in
    // if not, redirect to login page.
    return next({
      path: "/login",
      query: { redirect_to: to.fullPath },
    });
  }

  // Always call next() to allow navigation to continue
  next();
});

// Gaurd for new user page (redirect to dashboard if they have accepted Terms of Use)
router.beforeEach(async (to, _, next) => {
  const oidcStore = useOidcStore();
  const userStore = useUserStore();

  // Only check terms acceptance for authenticated users
  const user = await oidcStore.getUser();
  if (!user) {
    return next(); // Let auth guard handle unauthenticated users
  }

  if (to.path === "/new-user" && userStore.hasUserProfile && userStore.hasAcceptedTermsOfUse) {
    return next({ path: "/" });
  }
  next();
});

// Gaurd for rest of the routes (redirect to new user page if they have not accepted Terms of Use)
router.beforeEach(async (to, _, next) => {
  const oidcStore = useOidcStore();
  const userStore = useUserStore();

  // Only check terms acceptance for authenticated users
  const user = await oidcStore.getUser();
  if (!user) {
    return next(); // Let auth guard handle unauthenticated users
  }

  // Don't redirect if already going to login, new-user, or other public routes
  const publicPaths = ["/login", "/new-user", "/silent-callback", "/logout-callback", "/verify"];
  if (publicPaths.some((path) => to.path.startsWith(path))) {
    return next();
  }

  if (userStore.hasUserProfile && !userStore.hasAcceptedTermsOfUse) {
    return next({ path: "/new-user" });
  }
  next();
});

// Guard for login page (redirect to dashboard if already authenticated)
router.beforeEach(async (to, _, next) => {
  const oidcStore = useOidcStore();
  const user = await oidcStore.getUser();

  if (to.path === "/login" && user) {
    return next({ path: "/" });
  }
  next();
});

export default router;
