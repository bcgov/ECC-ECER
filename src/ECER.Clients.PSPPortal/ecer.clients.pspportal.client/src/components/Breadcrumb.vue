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
      if (this.route.name === "edit-profile") {
        return [
          { title: "Home", disabled: false, href: "/" },
          { title: "My contact details", disabled: true, href: "/profile/edit" },
        ];
      }
      if (this.route.name === "manage-users") {
        return [
          { title: "Home", disabled: false, href: "/" },
          { title: "Manage users", disabled: true, href: "/manage-users" },
        ];
      }
      if (this.route.name === "add-user") {
        const educationInstitutionName = this.route.params.educationInstitutionName as string;
        return [
          { title: "Home", disabled: false, href: "/" },
          { title: "Manage users", disabled: false, href: `/manage-users/${educationInstitutionName}` },
          { title: "Invite user", disabled: true, href: this.route.path },
        ];
      }
      if (this.route.name === "edit-education-institution") {
        return [
          { title: "Home", disabled: false, href: "/" },
          { title: "Edit Institution", disabled: true, href: "/education-institution/edit" },
        ];
      }
      if (this.route.name === "program-profiles") {
        return [
          { title: "Home", disabled: false, href: "/" },
          { title: "Program profiles", disabled: true, href: "/program-profiles" },
        ];
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
