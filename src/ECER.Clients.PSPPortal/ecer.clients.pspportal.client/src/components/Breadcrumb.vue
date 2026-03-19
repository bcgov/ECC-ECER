<template>
  <v-breadcrumbs class="pl-0" :items="breadcrumbItems" color="primary">
    <template #divider>/</template>
  </v-breadcrumbs>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useRoute } from "vue-router";

export interface ItemsType {
  title: string;
  disabled: boolean;
  href: string;
}

export default defineComponent({
  name: "Breadcrumb",
  components: {},
  setup() {
    const route = useRoute();
    return { route };
  },
  computed: {
    breadcrumbItems(): ItemsType[] {
      const home: ItemsType = { title: "Home", disabled: false, href: "/" };
      const staticRoutes: Record<string, ItemsType[]> = {
        "edit-profile": [
          home,
          {
            title: "My contact details",
            disabled: true,
            href: "/profile/edit",
          },
        ],
        "manage-users": [
          home,
          { title: "Manage users", disabled: true, href: "/manage-users" },
        ],
        "education-institution": [
          home,
          {
            title: "Institution information",
            disabled: true,
            href: this.route.path,
          },
        ],
        "edit-education-institution": [
          home,
          {
            title: "Edit Institution",
            disabled: true,
            href: "/education-institution/edit",
          },
        ],
        "program-profiles": [
          home,
          {
            title: "Program profiles",
            disabled: true,
            href: "/program-profiles",
          },
        ],
        "all-program-profiles": [
          home,
          {
            title: "Program profiles",
            disabled: false,
            href: "/program-profiles",
          },
          {
            title: "All program profiles",
            disabled: true,
            href: "/all-program-profiles",
          },
        ],
        "initiate-program-update": [
          home,
          {
            title: "Update a program profile",
            disabled: true,
            href: "/program/:programId/initiate-update",
          },
        ],
        "program-applications": [
          home,
          {
            title: "All applications",
            disabled: true,
            href: "/program-applications",
          },
        ],
        "program-application-begin": [
          home,
          {
            title: "Begin an application",
            disabled: true,
            href: "/program-application-begin",
          },
        ],
        messages: [
          home,
          {
            title: "Messages",
            disabled: true,
            href: "/communication/new-message",
          },
        ],
      };

      const routeName = this.route.name as string;
      if (staticRoutes[routeName]) return staticRoutes[routeName];

      if (routeName === "add-user") {
        const educationInstitutionName = this.route.params
          .educationInstitutionName as string;
        return [
          home,
          {
            title: "Manage users",
            disabled: false,
            href: `/manage-users/${educationInstitutionName}`,
          },
          { title: "Invite user", disabled: true, href: this.route.path },
        ];
      }
      if (routeName === "campus") {
        const institutionId = this.route.params.institutionId as string;
        return [
          home,
          {
            title: "Institution information",
            disabled: false,
            href: `/education-institution/${institutionId}`,
          },
          {
            title: "Campus information",
            disabled: true,
            href: this.route.path,
          },
        ];
      }
      if (routeName === "add-campus") {
        const institutionId = this.route.params.institutionId as string;
        return [
          home,
          {
            title: "Institution info",
            disabled: false,
            href: `/education-institution/${institutionId}`,
          },
          { title: "Add campus", disabled: true, href: this.route.path },
        ];
      }
      if (routeName === "add-satellite-location") {
        const institutionId = this.route.params.institutionId as string;
        return [
          home,
          {
            title: "Institution info",
            disabled: false,
            href: `/education-institution/${institutionId}`,
          },
          {
            title: "Add satellite location",
            disabled: true,
            href: this.route.path,
          },
        ];
      }
      if (routeName === "edit-campus") {
        const institutionId = this.route.params.institutionId as string;
        const campusId = this.route.params.campusId as string;
        return [
          home,
          {
            title: "Institution info",
            disabled: false,
            href: `/education-institution/${institutionId}`,
          },
          {
            title: "Campus information",
            disabled: false,
            href: `/education-institution/${institutionId}/campus/${campusId}`,
          },
          { title: "Edit location", disabled: true, href: this.route.path },
        ];
      }
      if (routeName === "programApplicationInfo") {
        const applicationType = this.route.params.applicationType as string;
        if (applicationType === "NewBasicECEPostBasicProgram") {
          return [
            home,
            {
              title: "Program application information",
              disabled: true,
              href: "/program-application-info",
            },
          ];
        } else if (
          applicationType === "NewCampusatRecognizedPrivateInstitution"
        ) {
          return [
            home,
            {
              title: "Application for new campus",
              disabled: true,
              href: "/program-application-info",
            },
          ];
        }
      }
      return [];
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
