<template>
  <v-row>
    <p v-if="educationList.length == 0" class="small">No Education has been added yet. You may add multiple education entries.</p>
    <EducationCard
      v-for="(education, id) in educations"
      :key="id"
      :education="education"
      class="mb-4"
      @edit="handleEdit(education, id)"
      @delete="handleDelete(id)"
    />
  </v-row>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import EducationCard from "@/components/EducationCard.vue";
import type { Components } from "@/types/openapi";

export interface EducationData {
  education: Components.Schemas.Transcript;
  educationId: string | number;
}

export default defineComponent({
  name: "EducationList",
  components: { EducationCard },
  props: {
    educations: {
      type: Object as () => { [id: string]: Components.Schemas.Transcript },
      required: true,
    },
  },
  emits: {
    edit: (_educationData: EducationData) => true,
    delete: (_educationId: string | number) => true,
  },
  computed: {
    educationList() {
      return Object.values(this.educations);
    },
  },
  methods: {
    handleEdit(education: Components.Schemas.Transcript, educationId: string | number) {
      // Re-emit the event to the parent component
      this.$emit("edit", { education, educationId });
    },
    handleDelete(educationId: string | number) {
      // Re-emit the event to the parent component
      this.$emit("delete", educationId);
    },
  },
});
</script>
