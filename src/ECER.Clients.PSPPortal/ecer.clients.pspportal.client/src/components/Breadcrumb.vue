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
