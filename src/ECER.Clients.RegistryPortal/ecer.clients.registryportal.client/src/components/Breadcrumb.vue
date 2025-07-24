<template>
  <v-breadcrumbs class="pl-0" :items="items" color="primary">
    <template #divider>/</template>
  </v-breadcrumbs>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";

export interface ItemsType {
  title: string;
  disabled: boolean;
  href: string;
}

export default defineComponent({
  name: "Breadcrumb",
  components: {},
  props: {
    items: {
      type: Array as PropType<ItemsType[]>,
      required: true,
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
