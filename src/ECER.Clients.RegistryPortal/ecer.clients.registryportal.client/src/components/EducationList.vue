<template>
  <v-row>
    <v-col v-if="applicationStore.isDraftCertificateTypeFiveYears">
      <p class="mb-3">
        You must have completed
        <a
          href="https://www2.gov.bc.ca/gov/content/education-training/early-learning/teach/training-and-professional-development/become-an-early-childhood-educator/recognized-ece-institutions"
        >
          program(s) recognized by the ECE Registry
        </a>
        in:
      </p>
      <ul class="ml-10">
        <li>Basic early childhood education</li>
        <li>Infant and toddler educator training</li>
        <li>Special needs early childhood educator training</li>
      </ul>
    </v-col>
    <v-col v-else-if="applicationStore.isDraftCertificateTypeOneYear">
      <p>
        You must have completed a basic early childhood education program. It must be a
        <a
          href="https://www2.gov.bc.ca/gov/content/education-training/early-learning/teach/training-and-professional-development/become-an-early-childhood-educator/recognized-ece-institutions"
        >
          program recognized by the ECE Registry
        </a>
      </p>
    </v-col>
    <v-col v-else-if="applicationStore.isDraftCertificateTypeEceAssistant">
      <p class="mb-3">You must have completed an early childhood education course in at least one of the following:</p>
      <ul class="ml-10 mb-3">
        <li>Child guidance</li>
        <li>Child health, safety, and nutrition</li>
        <li>Child development</li>
      </ul>
      <p class="mb-3">The course must:</p>
      <ul class="ml-10">
        <li>Have been completed within the last 5 years</li>
        <li>
          <a
            href="https://www2.gov.bc.ca/gov/content/education-training/early-learning/teach/training-and-professional-development/become-an-early-childhood-educator/recognized-ece-institutions"
          >
            Be a program recognized by the ECE Registry
          </a>
        </li>
      </ul>
    </v-col>
  </v-row>
  <v-row>
    <v-col>
      <EducationCard
        v-for="(education, id) in educations"
        :key="id"
        :education="education"
        class="my-4"
        @edit="handleEdit(education, id)"
        @delete="handleDelete(id)"
      />
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import EducationCard from "@/components/EducationCard.vue";
import { useApplicationStore } from "@/store/application";
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
  setup() {
    const applicationStore = useApplicationStore();

    return { applicationStore };
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
