<template>
  <v-breadcrumbs class="pl-0" :items="breadcrumbItems" color="primary">
    <template #divider>/</template>
  </v-breadcrumbs>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useRoute } from "vue-router";
import { useApplicationStore } from "@/store/application";
import { useUserStore } from "@/store/user";

export interface ItemsType {
  title: string;
  disabled: boolean;
  href: string;
}

export interface BreadcrumbItem extends ItemsType {
  routeName?: string; // Add routeName to track which route this represents
}

export default defineComponent({
  name: "Breadcrumb",
  components: {},
  setup() {
    const route = useRoute();
    const applicationStore = useApplicationStore();
    const userStore = useUserStore();

    return { route, applicationStore, userStore };
  },
  computed: {
    isUnder19InApplication(): boolean {
      return (
        this.userStore.isUnder19 &&
        (this.applicationStore.hasDraftApplication ||
          this.route.name === "application-certification" ||
          this.route.name === "application-requirements" ||
          this.route.name === "declaration" ||
          this.route.name === "consent-required")
      );
    },
    baseItems(): ItemsType[] {
      return [{ title: "Home", disabled: false, href: "/" }];
    },
    addConsentStep() {
      return (items: ItemsType[]): ItemsType[] => {
        if (this.isUnder19InApplication) {
          // Find the index of the "Requirements" item
          const requirementsIndex = items.findIndex((item) => item.title === "Requirements");
          if (requirementsIndex !== -1) {
            // Insert "Consent required" immediately before "Requirements"
            const beforeRequirements = items.slice(0, requirementsIndex);
            const afterRequirements = items.slice(requirementsIndex);
            return [...beforeRequirements, { title: "Consent required", disabled: false, href: "/application/consent-required" }, ...afterRequirements];
          }
        }
        return items;
      };
    },

    // Define the ranking for application pages
    applicationPageRankings(): Record<string, number> {
      return {
        "application-certification": 1,
        "application-transfer": 1, // Same rank as certification since they're alternatives
        "consent-required": 2,
        "application-requirements": 3,
        declaration: 4,
      };
    },
    // Helper to insert items at correct position based on ranking
    insertAtCorrectRank() {
      return (items: BreadcrumbItem[], newItem: BreadcrumbItem): BreadcrumbItem[] => {
        const rank = this.applicationPageRankings[newItem.routeName || ""];
        if (rank === undefined) {
          // If no rank, append to end
          return [...items, newItem];
        }

        // Find the correct position based on rank
        let insertIndex = items.length;
        for (let i = 0; i < items.length; i++) {
          const itemRank = this.applicationPageRankings[items[i].routeName || ""];
          if (itemRank !== undefined && itemRank > rank) {
            insertIndex = i;
            break;
          }
        }

        // Insert at the correct position
        const before = items.slice(0, insertIndex);
        const after = items.slice(insertIndex);
        return [...before, newItem, ...after];
      };
    },
    // Build application breadcrumbs using ranking
    buildApplicationBreadcrumbs(): ItemsType[] {
      let items: BreadcrumbItem[] = [...this.baseItems];

      // Add pages based on current route and application state
      const pagesToAdd: BreadcrumbItem[] = [];
      const currentRouteName = this.route.name as string;

      // Add certification type selection (for new applications)
      if (!this.applicationStore.isDraftApplicationRenewal && !this.applicationStore.isDraftApplicationLaborMobility) {
        const step = {
          title: "Apply for new certification",
          disabled: currentRouteName === "application-certification",
          href: "/application/certification",
          routeName: "application-certification",
        };
        pagesToAdd.push(step);
        if (currentRouteName === "application-certification") {
          // We're on this step, so stop adding future steps
          pagesToAdd.forEach((page) => {
            items = this.insertAtCorrectRank(items, page);
          });
          return items;
        }
      }

      // Add transfer eligibility (for labor mobility)
      if (this.applicationStore.isDraftApplicationLaborMobility) {
        const step = {
          title: "Check your transfer eligibility",
          disabled: currentRouteName === "application-transfer",
          href: "/application/transfer",
          routeName: "application-transfer",
        };
        pagesToAdd.push(step);
        if (currentRouteName === "application-transfer") {
          pagesToAdd.forEach((page) => {
            items = this.insertAtCorrectRank(items, page);
          });
          return items;
        }
      }

      // Add consent required (for under 19)
      if (this.isUnder19InApplication) {
        const step = {
          title: "Consent required",
          disabled: currentRouteName === "consent-required",
          href: "/application/consent-required",
          routeName: "consent-required",
        };
        pagesToAdd.push(step);
        if (currentRouteName === "consent-required") {
          pagesToAdd.forEach((page) => {
            items = this.insertAtCorrectRank(items, page);
          });
          return items;
        }
      }

      // Add requirements
      const requirementsStep = {
        title: "Requirements",
        disabled: currentRouteName === "application-requirements",
        href: "/application/certification/requirements",
        routeName: "application-requirements",
      };
      pagesToAdd.push(requirementsStep);
      if (currentRouteName === "application-requirements") {
        pagesToAdd.forEach((page) => {
          items = this.insertAtCorrectRank(items, page);
        });
        return items;
      }

      // Add declaration
      const declarationStep = {
        title: "Declaration",
        disabled: currentRouteName === "declaration",
        href: "/application/declaration",
        routeName: "declaration",
      };
      pagesToAdd.push(declarationStep);
      if (currentRouteName === "declaration") {
        pagesToAdd.forEach((page) => {
          items = this.insertAtCorrectRank(items, page);
        });
        return items;
      }

      // If we get here, we're on a route that's not in our defined flow
      // Just add all the steps we've collected
      pagesToAdd.forEach((page) => {
        items = this.insertAtCorrectRank(items, page);
      });

      return items;
    },

    breadcrumbItems(): ItemsType[] {
      // For application-related routes, use the ranking system
      if (this.isApplicationRoute(this.route.name as string)) {
        return this.buildApplicationBreadcrumbs;
      }

      // For other routes, use the existing switch statement
      return this.generateBreadcrumbs(this.route.name as string, this.route.params as Record<string, string>);
    },
  },
  methods: {
    generateBreadcrumbs(routeName: string, params: Record<string, string | string[]>): ItemsType[] {
      switch (routeName) {
        case "certification-requirements":
          return [...this.baseItems, { title: "Certification requirements", disabled: true, href: "/certification-requirements" }];

        case "viewComprehensiveReport":
          return [
            ...this.baseItems,
            { title: "Application", disabled: false, href: `/manage-application/${params.applicationId}` },
            {
              title: "Comprehensive Report",
              disabled: true,
              href: `/manage-application/${params.applicationId}/transcript/${params.transcriptId}/comprehensive-evaluation`,
            },
          ];

        case "viewCourseOutline":
          return [
            ...this.baseItems,
            { title: "Application", disabled: false, href: `/manage-application/${params.applicationId}` },
            {
              title: "Course outlines or syllabi",
              disabled: true,
              href: `/manage-application/${params.applicationId}/transcript/${params.transcriptId}/course-outline`,
            },
          ];

        case "viewProgramConfirmation":
          return [
            ...this.baseItems,
            { title: "Application", disabled: false, href: `/manage-application/${params.applicationId}` },
            {
              title: "Program confirmation",
              disabled: true,
              href: `/manage-application/${params.applicationId}/transcript/${params.transcriptId}/program-confirmation`,
            },
          ];

        case "viewTranscriptDetails":
          return [
            ...this.baseItems,
            { title: "Application", disabled: false, href: `/manage-application/${params.applicationId}` },
            { title: "Transcript", disabled: true, href: `/manage-application/${params.applicationId}/transcript/${params.transcriptId}` },
          ];

        case "view-character-reference":
          return [
            ...this.baseItems,
            { title: "Application", disabled: false, href: `/manage-application/${params.applicationId}` },
            { title: "Character reference", disabled: true, href: `/manage-application/${params.applicationId}/character-reference/${params.referenceId}` },
          ];

        // Manage work experience references

        case "addCharacterReference":
          return [
            ...this.baseItems,
            { title: "Application", disabled: false, href: `/manage-application/${params.applicationId}` },
            { title: "Add", disabled: true, href: `/manage-application/${params.applicationId}/character-reference/add` },
          ];
        case "updateCharacterReference":
          return [
            ...this.baseItems,
            { title: "Application", disabled: false, href: `/manage-application/${params.applicationId}` },
            { title: "Character reference", disabled: false, href: `/manage-application/${params.applicationId}/character-reference/${params.referenceId}` },
            { title: "Add", disabled: true, href: `/manage-application/${params.applicationId}/character-reference/${params.referenceId}/edit` },
          ];

        // Manage work experience references
        case "viewWorkExperienceReference":
          return [
            ...this.baseItems,
            { title: "Application", disabled: false, href: `/manage-application/${params.applicationId}` },
            {
              title: "Work experience reference",
              disabled: true,
              href: `/manage-application/${params.applicationId}/work-experience-reference/${params.referenceId}`,
            },
          ];
        case "manageWorkExperienceReferences":
          return [
            ...this.baseItems,
            { title: "Application", disabled: false, href: `/manage-application/${params.applicationId}` },
            { title: "Work experience references", disabled: true, href: `/manage-application/${params.applicationId}/work-experience-references` },
          ];
        case "addWorkExperienceReference":
          return [
            ...this.baseItems,
            { title: "Application", disabled: false, href: `/manage-application/${params.applicationId}` },
            {
              title: "Work experience reference",
              disabled: false,
              href: `/manage-application/${params.applicationId}/work-experience-references`,
            },
            { title: "Add", disabled: true, href: `/manage-application/${params.applicationId}/work-experience-reference/add` },
          ];
        case "updateWorkExperienceReference":
          return [
            ...this.baseItems,
            { title: "Application", disabled: false, href: `/manage-application/${params.applicationId}` },
            {
              title: "Work experience reference",
              disabled: false,
              href: `/manage-application/${params.applicationId}/work-experience-reference/${params.referenceId}`,
            },
            { title: "Add", disabled: true, href: `/manage-application/${params.applicationId}/work-experience-reference/${params.referenceId}/edit` },
          ];
        case "manageProfessionalDevelopment":
          return [
            ...this.baseItems,
            { title: "Application", disabled: false, href: `/manage-application/${params.applicationId}` },
            { title: "Professional development", disabled: true, href: `/manage-application/${params.applicationId}/professional-development` },
          ];
        case "addProfessionalDevelopment":
          return [
            ...this.baseItems,
            { title: "Application", disabled: false, href: `/manage-application/${params.applicationId}` },
            { title: "Professional development", disabled: false, href: `/manage-application/${params.applicationId}/professional-development` },
            { title: "Add", disabled: true, href: `/manage-application/${params.applicationId}/professional-development/add` },
          ];
        case "add-previous-name":
          return [
            ...this.baseItems,
            { title: "Profile", disabled: false, href: "/profile" },
            { title: "Add previous name", disabled: true, href: "/profile/add-previous-name" },
          ];

        case "verify-previous-name":
          return [
            ...this.baseItems,
            { title: "Profile", disabled: false, href: "/profile" },
            { title: "Verify previous name", disabled: true, href: `/profile/verify-previous-name/${params.previousNameId}` },
          ];

        case "edit-profile":
          return [...this.baseItems, { title: "Profile", disabled: false, href: "/profile" }, { title: "Edit profile", disabled: true, href: "/profile/edit" }];

        case "verifyIdentification":
          return [...this.baseItems, { title: "Verify identification", disabled: true, href: "/verify-identification" }];

        case "createAccount":
          return [...this.baseItems, { title: "Create account", disabled: true, href: "/create-account" }];

        case "certificate-terms-and-conditions":
          return [...this.baseItems, { title: "Terms and conditions", disabled: true, href: `/certificate-terms-and-conditions/${params.certificationId}` }];

        case "my-other-certifications":
          return [...this.baseItems, { title: "My other certifications", disabled: true, href: "/my-other-certifications" }];

        case "profile":
          return [...this.baseItems, { title: "Profile", disabled: true, href: "/profile" }];

        case "messages":
          return [...this.baseItems, { title: "Messages", disabled: true, href: "/messages" }];

        case "replyToMessage":
          return [
            ...this.baseItems,
            { title: "Messages", disabled: false, href: "/messages" },
            { title: "Reply to message", disabled: true, href: `/messages/${params.messageId}/reply` },
          ];

        case "manageApplication":
          return [...this.baseItems, { title: "Application", disabled: true, href: `/manage-application/${params.applicationId}` }];

        case "icra-eligibility-requirements":
          return [
            ...this.baseItems,
            { title: "Apply with international certificate", disabled: false, href: "/icra/eligibility" },
            { title: "Requirements", disabled: true, href: "/icra/eligibility/requirements" },
          ];


        case "icra-eligibility":
          return [...this.baseItems, { title: "Apply with international certificate", disabled: true, href: "/icra/eligibility" }];

        default:
          return this.baseItems;
      }
    },

    // Helper to check if current route is application-related
    isApplicationRoute(routeName: string): boolean {
      return ["application-certification", "application-transfer", "application-requirements", "consent-required", "declaration"].includes(routeName);
    },
  },
});
</script>

<style>
.v-breadcrumbs__item:not(.v-breadcrumbs__item--disabled),
.v-breadcrumbs-item:not(.v-breadcrumbs-item--disabled) {
  text-decoration: underline !important;
}
</style>
