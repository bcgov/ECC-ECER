import { createRouter, createWebHistory } from "vue-router";

import { useApplicationStore } from "./store/application";
import { useOidcStore } from "./store/oidc";
import { useUserStore } from "./store/user";
import { useCertificationStore } from "./store/certification";
import { useFormStore } from "./store/form";
import { useMessageStore } from "./store/message";
import { useWizardStore } from "./store/wizard";

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
      meta: { requiresAuth: false }, // This is handled in dashboard component itself
      name: "dashboard",
    },
    {
      path: "/profile",
      component: () => import("./components/Profile.vue"),
      name: "profile",
      meta: { requiresAuth: true, requiresVerification: true },
    },
    {
      path: "/messages",
      name: "messages",
      component: () => import("./components/Messages.vue"),
      meta: { requiresAuth: true },
    },
    {
      path: "/messages/:messageId/reply",
      name: "replyToMessage",
      component: () => import("./components/ReplyToMessage.vue"),
      meta: { requiresAuth: true },
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
      path: "/logout-callback",
      component: () => import("./components/pages/LogoutCallback.vue"),
      meta: { requiresAuth: false },
    },
    {
      path: "/manage-application/:applicationId",
      name: "manageApplication",
      component: () => import("./components/ApplicationSummary.vue"),
      meta: { requiresAuth: true, requiresVerification: true },
    },
    {
      path: "/manage-application/:applicationId/work-experience-reference/:referenceId",
      name: "viewWorkExperienceReference",
      component: () => import("./components/ViewWorkExperienceReference.vue"),
      meta: { requiresAuth: true, requiresVerification: true },
      props: true,
    },
    {
      path: "/manage-application/:applicationId/character-reference/:referenceId",
      name: "viewCharacterReference",
      component: () => import("./components/ViewCharacterReference.vue"),
      meta: { requiresAuth: true, requiresVerification: true },
      props: true,
    },
    {
      path: "/manage-application/:applicationId/work-experience-reference/:referenceId/edit",
      name: "updateWorkExperienceReference",
      component: () => import("./components/UpsertWorkExperienceReference.vue"),
      meta: { requiresAuth: true, requiresVerification: true },
      props: true,
    },
    {
      path: "/manage-application/:applicationId/character-reference/:referenceId/edit",
      name: "updateCharacterReference",
      component: () => import("./components/UpsertCharacterReference.vue"),
      meta: { requiresAuth: true, requiresVerification: true },
      props: true,
    },
    {
      path: "/manage-application/:applicationId/character-reference/add",
      name: "addCharacterReference",
      component: () => import("./components/UpsertCharacterReference.vue"),
      meta: { requiresAuth: true, requiresVerification: true },
      props: true,
    },
    {
      path: "/manage-application/:applicationId/work-experience-reference/add",
      name: "addWorkExperienceReference",
      component: () => import("./components/UpsertWorkExperienceReference.vue"),
      meta: { requiresAuth: true, requiresVerification: true },
      props: true,
    },
    {
      path: "/manage-application/:applicationId/professional-development/add",
      name: "addProfessionalDevelopment",
      component: () => import("./components/AddProfessionalDevelopment.vue"),
      meta: { requiresAuth: true, requiresVerification: true },
      props: true,
    },
    {
      path: "/manage-application/:applicationId/work-experience-references",
      name: "manageWorkExperienceReferences",
      component: () => import("./components/ManageWorkExperienceReferenceList.vue"),
      meta: { requiresAuth: true, requiresVerification: true },
      props: true,
    },
    {
      path: "/manage-application/:applicationId/professional-development",
      name: "manageProfessionalDevelopment",
      component: () => import("./components/ManageProfessionalDevelopmentList.vue"),
      meta: { requiresAuth: true, requiresVerification: true },
      props: true,
    },
    {
      path: "/application/certification",
      component: () => import("./components/CertificationType.vue"),
      meta: { requiresAuth: true, requiresVerification: true },
      name: "application-certification",
    },
    {
      path: "/application/certification/requirements",
      component: () => import("./components/ApplicationRequirements.vue"),
      meta: { requiresAuth: true },
      name: "application-requirements",
    },
    {
      path: "/certification-requirements",
      component: () => import("./components/CertificationRequirements.vue"),
      meta: { requiresAuth: true },
      name: "certification-requirements",
      props: (route) => {
        const { query } = route;
        let certificationTypes = query.certificationTypes;
        if (certificationTypes && !Array.isArray(certificationTypes)) {
          certificationTypes = [certificationTypes];
        }
        return { certificationTypes: certificationTypes || [], isRenewal: query.isRenewal === "true" };
      },
    },
    {
      path: "/application/declaration",
      component: () => import("./components/Declaration.vue"),
      meta: { requiresAuth: true },
      name: "declaration",
    },
    {
      path: "/application",
      component: () => import("./components/pages/Application.vue"),
      meta: { requiresAuth: true, requiresVerification: true },
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
      path: "/profile/edit",
      component: () => import("./components/pages/EditProfile.vue"),
      name: "edit-profile",
      meta: { requiresAuth: true, requiresVerification: true },
    },
    {
      path: "/profile/add-previous-name",
      component: () => import("./components/pages/AddPreviousName.vue"),
      name: "add-previous-name",
      meta: { requiresAuth: true, requiresVerification: true },
    },
    {
      path: "/profile/verify-previous-name/:previousNameId",
      component: () => import("./components/pages/VerifyPreviousName.vue"),
      name: "verify-previous-name",
      props: true,
      meta: { requiresAuth: true, requiresVerification: true },
    },
    {
      path: "/submitted/:applicationId",
      name: "submitted",
      component: () => import("./components/pages/Submitted.vue"),
      props: true,
      meta: { requiresAuth: true, requiresVerification: true },
    },
    {
      path: "/verify/:token",
      component: () => import("./components/reference/Reference.vue"),
      meta: { requiresAuth: false },
      name: "verify",
    },
    {
      path: "/invalid-reference",
      component: () => import("./components/reference/Invalid.vue"),
      meta: { requiresAuth: false },
      name: "invalid-reference",
    },
    {
      path: "/reference-submitted",
      component: () => import("./components/pages/ReferenceSubmitted.vue"),
      meta: { requiresAuth: false },
      name: "reference-submitted",
    },
    {
      path: "/lookup/certification",
      component: () => import("./components/LookupCertification.vue"),
      meta: { requiresAuth: false },
      name: "lookup-certification",
    },
    {
      path: "/lookup/certification/record",
      component: () => import("./components/LookupCertificationRecord.vue"),
      meta: { requiresAuth: false },
      name: "lookup-certification-record",
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
    const applicationStore = useApplicationStore();
    const certificationStore = useCertificationStore();
    const formStore = useFormStore();
    const messageStore = useMessageStore();
    const wizardStore = useWizardStore();
    userStore.$reset();
    applicationStore.$reset();
    certificationStore.$reset();
    formStore.$reset();
    messageStore.$reset();
    wizardStore.$reset();
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

  if (to.meta.requiresVerification && !userStore.isVerified) {
    return {
      path: "/",
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

// Guard to prevent portal user from modifying Manual application that is in-progress(draft)
router.beforeEach((to, _, next) => {
  const applicationStore = useApplicationStore();

  if (to.path === "/application" && applicationStore.applicationStatus === "Draft" && applicationStore.applicationOrigin === "Manual") {
    next({ path: "/" });
  } else next();
});

export default router;
