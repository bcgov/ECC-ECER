<template>
  <v-row>
    <!-- Five year variations Ite + Sne -->
    <v-col v-if="applicationStore.draftApplicationFlow === 'FiveYearWithIteAndSne'">
      <!--prettier-ignore-->
      <p class="mb-3">
        You must have completed:
      </p>
      <ul class="ml-10">
        <li>Basic early childhood education training program</li>
        <li>Infant and toddler educator training program</li>
        <li>Special needs early childhood educator training program</li>
      </ul>
    </v-col>
    <v-col v-if="applicationStore.draftApplicationFlow === 'FiveYearWithIte'">
      <!--prettier-ignore-->
      <p class="mb-3">
        You must have completed:
      </p>
      <ul class="ml-10">
        <li>Basic early childhood education training program</li>
        <li>Infant and toddler educator training program</li>
      </ul>
    </v-col>
    <v-col v-if="applicationStore.draftApplicationFlow === 'FiveYearWithSne'">
      <!--prettier-ignore-->
      <p class="mb-3">
        You must have completed:
      </p>
      <ul class="ml-10">
        <li>Basic early childhood education training program</li>
        <li>Special needs early childhood educator training program</li>
      </ul>
    </v-col>

    <!-- Ite + Sne Only -->
    <v-col v-if="applicationStore.draftApplicationFlow === 'IteAndSneRegistrant'">
      <!--prettier-ignore-->
      <p class="mb-3">
        You must have completed:
      </p>
      <ul class="ml-10 mb-3">
        <li>Infant and toddler educator training program</li>
        <li>Special needs early childhood educator training program</li>
      </ul>
    </v-col>
    <v-col v-if="applicationStore.draftApplicationFlow === 'IteRegistrant'">
      <p class="mb-3">You must have completed an infant and toddler educator training program.</p>
    </v-col>
    <v-col v-if="applicationStore.draftApplicationFlow === 'SneRegistrant'">
      <p class="mb-3">You must have completed a special needs early childhood educator training program.</p>
    </v-col>

    <!-- Five year or One year variation -->
    <v-col v-if="applicationStore.draftApplicationFlow === 'FiveYear' || applicationStore.draftApplicationFlow === 'OneYear'">
      <p>You must have completed a basic early childhood education program.</p>
    </v-col>

    <!-- EceAssistant -->
    <v-col v-if="applicationStore.draftApplicationFlow === 'Assistant'">
      <p class="mb-3">You must have completed an early childhood education course in at least one of the following:</p>
      <ul class="ml-10 mb-3">
        <li>Child guidance</li>
        <li>Child health, safety, and nutrition</li>
        <li>Child development</li>
      </ul>
      <p>It must have been completed in the last 5 years</p>
    </v-col>

    <!-- renewal flows -->
    <v-col v-if="applicationStore.draftApplicationFlow === 'AssistantRenewal'">
      <p class="mb-3">You must have completed a new course in an early childhood education training program.</p>
      <p class="mb-4">The course must:</p>
      <ul class="ml-10">
        <li>Have been completed within the last 5 years</li>
        <li>
          Be part of a
          <a
            href="https://www2.gov.bc.ca/gov/content/education-training/early-learning/teach/training-and-professional-development/become-an-early-childhood-educator/recognized-ece-institutions"
          >
            program recognized by the ECE Registry
          </a>
        </li>
        <li>Be a new course - it cannot be one you previously used in an application for an ECE Assistant certification</li>
      </ul>
    </v-col>
  </v-row>
  <v-row>
    <v-col sm="12" md="10" lg="8" xl="6">
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
