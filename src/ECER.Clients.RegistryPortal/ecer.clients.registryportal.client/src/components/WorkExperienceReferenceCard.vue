<template>
  <v-card min-width="100%" variant="elevated" rounded="lg" class="custom-card-styling" elevation="4">
    <template #title>
      <v-card-title>
        <v-row>
          <v-col>
            <p class="font-weight-bold">{{ referenceFullName }}</p>
          </v-col>
          <v-col>
            <p>{{ referenceHours }}</p>
          </v-col>

          <div></div>
        </v-row>
      </v-card-title>
    </template>
    <template #append>
      <v-tooltip text="Edit reference" location="top">
        <template #activator="{ props }">
          <v-btn v-bind="props" icon="mdi-pencil" variant="plain" @click="editReference" :loading="loadingStore.isLoading('draftapplication_put')" />
        </template>
      </v-tooltip>
      <v-tooltip text="Delete reference" location="top">
        <template #activator="{ props }">
          <v-btn
            v-bind="props"
            icon="mdi-trash-can-outline"
            variant="plain"
            @click="deleteReference"
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
  name: "WorkExperienceReferenceCard",
  setup() {
    const loadingStore = useLoadingStore();

    return { loadingStore };
  },
  props: {
    reference: {
      type: Object as () => Components.Schemas.WorkExperienceReference,
      required: true,
    },
  },

  emits: {
    edit: (_reference: Components.Schemas.WorkExperienceReference) => true,
    delete: (_reference: Components.Schemas.WorkExperienceReference) => true,
  },
  computed: {
    referenceHours() {
      return `${this.reference.hours} hours`;
    },
    referenceFullName() {
      return `${this.reference.firstName} ${this.reference.lastName}`;
    },
  },
  methods: {
    editReference() {
      // Emit an event or implement edit logic here
      this.$emit("edit", this.reference);
    },
    deleteReference() {
      // Emit an event or implement delete logic here
      this.$emit("delete", this.reference);
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
