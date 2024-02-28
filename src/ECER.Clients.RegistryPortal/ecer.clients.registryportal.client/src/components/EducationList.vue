<template>
  <v-row>
    <p v-if="educations.length == 0" class="small">No Education has been added yet. You may add multiple education entries.</p>
    <EducationCard v-for="education in educations" :key="education.id ?? ' '" :education="education" class="mb-4" @edit="handleEdit" @delete="handleDelete" />
  </v-row>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import EducationCard from "@/components/EducationCard.vue";
import type { Components } from "@/types/openapi";

export default defineComponent({
  name: "EducationList",
  components: { EducationCard },
  props: {
    educations: {
      type: Array as () => Components.Schemas.Transcript[],
      required: true,
    },
  },
  emits: {
    edit: (_education: Components.Schemas.Transcript) => true,
    delete: (_education: Components.Schemas.Transcript) => true,
  },
  methods: {
    handleEdit(education: Components.Schemas.Transcript) {
      // Re-emit the event to the parent component
      this.$emit("edit", education);
    },
    handleDelete(education: Components.Schemas.Transcript) {
      // Re-emit the event to the parent component
      this.$emit("delete", education);
    },
  },
});
</script>
