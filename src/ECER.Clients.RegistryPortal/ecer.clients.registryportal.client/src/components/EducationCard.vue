<template>
  <v-card min-width="100%" variant="elevated" rounded="lg" class="custom-card-styling" elevation="4">
    <template #title>
      <v-card-title>
        <p v-if="education.postSecondaryInstitution == null" class="font-weight-bold">{{ education.educationalInstitutionName }}</p>
        <p v-else class="font-weight-bold">{{ education.postSecondaryInstitution.name }}</p>
      </v-card-title>
    </template>
    <template #append>
      <v-tooltip text="Edit Education" location="top">
        <template #activator="{ props }">
          <v-btn v-bind="props" icon="mdi-pencil" variant="plain" @click="editEducation" :loading="loadingStore.isLoading('draftapplication_put')" />
        </template>
      </v-tooltip>
      <v-tooltip text="Delete Education" location="top">
        <template #activator="{ props }">
          <v-btn
            v-bind="props"
            icon="mdi-trash-can-outline"
            variant="plain"
            @click="deleteEducation"
            :loading="loadingStore.isLoading('draftapplication_put')"
          />
        </template>
      </v-tooltip>
    </template>
  </v-card>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useLoadingStore } from "@/store/loading";

import type { Components } from "@/types/openapi";

export default defineComponent({
  name: "EducationCard",
  setup() {
    const loadingStore = useLoadingStore();

    return { loadingStore };
  },
  props: {
    education: {
      type: Object as () => Components.Schemas.Transcript,
      required: true,
    },
  },
  emits: {
    edit: (_education: Components.Schemas.Transcript) => true,
    delete: (_education: Components.Schemas.Transcript) => true,
  },
  methods: {
    editEducation() {
      // Emit an event or implement edit logic here
      this.$emit("edit", this.education);
    },
    deleteEducation() {
      // Emit an event or implement delete logic here
      this.$emit("delete", this.education);
    },
  },
});
</script>

<style scoped lang="scss">
.custom-card-styling {
  border: 1px solid #adb5bd;
  padding: 0.5rem;
}
</style>
