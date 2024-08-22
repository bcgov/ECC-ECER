<template>
  <v-row>
    <p v-if="referenceList.length == 0" class="small">No work experience reference added yet.</p>
    <WorkExperienceReferenceCard
      v-for="(reference, id) in references"
      :key="id"
      :reference="reference"
      class="my-4"
      @edit="handleEdit(reference, id)"
      @delete="handleDelete(id)"
    />
  </v-row>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import type { Components } from "@/types/openapi";

import WorkExperienceReferenceCard from "./WorkExperienceReferenceCard.vue";

export interface WorkExperienceReferenceData {
  reference: Components.Schemas.WorkExperienceReference;
  referenceId: string | number;
}

export default defineComponent({
  name: "WorkExperienceReferenceList",
  components: { WorkExperienceReferenceCard },
  props: {
    references: {
      type: Object as () => { [id: string]: Components.Schemas.WorkExperienceReference },
      required: true,
    },
  },
  emits: {
    edit: (_referenceData: WorkExperienceReferenceData) => true,
    delete: (_referenceId: string | number) => true,
  },
  computed: {
    referenceList() {
      return Object.values(this.references);
    },
  },
  methods: {
    handleEdit(reference: Components.Schemas.WorkExperienceReference, referenceId: string | number) {
      // Re-emit the event to the parent component
      this.$emit("edit", { reference, referenceId });
    },
    handleDelete(referenceId: string | number) {
      // Re-emit the event to the parent component
      this.$emit("delete", referenceId);
    },
  },
});
</script>
