<template>
  <v-row>
    <p v-if="educations.length == 0" class="small">No Education has been added yet. You may add multiple education entries.</p>
    <EducationCard v-for="education in educations" :key="education.id" :education="education" class="mb-4" @edit="handleEdit" @delete="handleDelete" />
  </v-row>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import EducationCard from "@/components/EducationCard.vue";

export default defineComponent({
  name: "EducationList",
  components: { EducationCard },
  props: {
    educations: {
      type: Array as () => Education[],
      required: true,
    },
  },
  emits: {
    edit: (_education: Education) => true,
    delete: (_education: Education) => true,
  },
  methods: {
    handleEdit(education: Education) {
      // Re-emit the event to the parent component
      this.$emit("edit", education);
    },
    handleDelete(education: Education) {
      // Re-emit the event to the parent component
      this.$emit("delete", education);
    },
  },
});
</script>
