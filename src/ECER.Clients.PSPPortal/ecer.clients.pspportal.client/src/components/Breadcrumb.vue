<template>
  <v-breadcrumbs class="pl-0" :items="breadcrumbItems">
    <template #divider>/</template>
    <template #item="{ item }">
      <v-breadcrumbs-item
        :class="{
          'text-decoration-underline text-primary': !item.disabled,
          'text-grey-very-dark': item.disabled,
        }"
        :disabled="false"
        :href="item.disabled ? undefined : item.href"
      >
        {{ item.title }}
      </v-breadcrumbs-item>
    </template>
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
            title: "Edit institution contact info",
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
            title: "Institution information",
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
            title: "Institution information",
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
              title: "All applications",
              disabled: false,
              href: "/program-applications",
            },
            {
              title: "Basic or post-basic program application",
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
              title: "All applications",
              disabled: false,
              href: "/program-applications",
            },
            {
              title: "Application for new campus",
              disabled: true,
              href: "/program-application-info",
            },
          ];
        } else if (applicationType === "AddOnlineorHybridDeliveryMethod") {
          return [
            home,
            {
              title: "All applications",
              disabled: false,
              href: "/program-applications",
            },
            {
              title:
                "Application for adding an online or hybrid delivery method",
              disabled: true,
              href: "/program-application-info",
            },
          ];
        } else if (applicationType === "SatelliteProgram") {
          return [
            home,
            {
              title: "Satellite application",
              disabled: true,
              href: "/program-application-info",
            },
          ];
        }
      }
      if (routeName === "program-application-begin") {
        const applicationType = this.route.params.applicationType as string;
        const campusId = this.route.params.campusId as string;
        if (applicationType === "NewBasicECEPostBasicProgram") {
          return [
            home,
            {
              title: "Basic or post-basic program application",
              disabled: false,
              href: `/program-application-info/${applicationType}`,
            },
            {
              title: "Begin an application",
              disabled: true,
              href: "/program-application-begin",
            },
          ];
        } else if (
          applicationType === "NewCampusatRecognizedPrivateInstitution"
        ) {
          return [
            home,
            {
              title: "New campus application",
              disabled: false,
              href: `/program-application-info/${applicationType}/${campusId}`,
            },
            {
              title: "Begin an application",
              disabled: true,
              href: "/program-application-begin",
            },
          ];
        } else if (applicationType === "AddOnlineorHybridDeliveryMethod") {
          return [
            home,
            {
              title: "All applications",
              disabled: false,
              href: "/program-applications",
            },
            {
              title:
                "Application for adding an online or hybrid delivery method",
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
