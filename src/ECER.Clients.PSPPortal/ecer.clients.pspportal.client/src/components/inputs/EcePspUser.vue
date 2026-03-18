<template>
  <v-row>
    <v-col cols="12">
      <p>
        Select the individual contact for this campus or location. This user
        must have access to the PSP portal.
      </p>
    </v-col>
    <v-col cols="12">
      <p>
        Note: If the correct user is not listed here, please invite them to this
        portal under
        <router-link
          :to="{
            name: 'manage-users',
            params: { educationInstitutionName: institutionName },
          }"
        >
          "Manage Users"
        </router-link>
        .
      </p>
    </v-col>
    <v-col cols="12" sm="6">
      <p>Select a user from {{ institutionName }}</p>
      <v-select
        :model-value="modelValue"
        variant="outlined"
        class="mt-2"
        :items="users"
        item-title="name"
        item-value="id"
        hide-details="auto"
        v-bind="$attrs"
        @update:model-value="
          (value: string) => $emit('update:model-value', value)
        "
      />
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";

export interface PspUserItem {
  id: string;
  name: string;
}

export default defineComponent({
  name: "EcePspUser",
  inheritAttrs: false,
  props: {
    modelValue: {
      type: String,
      default: null,
    },
    users: {
      type: Array as PropType<PspUserItem[]>,
      default: () => [],
    },
    institutionName: {
      type: String,
      default: "",
    },
  },
  emits: {
    "update:model-value": (_value: string | null) => true,
  },
});
</script>
