<template>
  <v-card :rounded="true" :border="true" flat class="pa-6 border-primary border-opacity-100 w-100">
    <div class="d-flex justify-end">
      <v-chip :color="chipColour" variant="outlined" size="small">{{ statusText }}</v-chip>
    </div>

    <div class="mb-4">
      <p>PROGRAM PROFILE:</p>
      <p class="font-weight-bold">{{ name }}</p>
      <p class="mt-2">PROGRAM TYPE:</p>
      <p class="font-weight-bold">{{ programType }}</p>
      <v-row>
        <v-col>
          <p class="mt-2">START DATE:</p>
          <p class="font-weight-bold">{{ formatDate(program?.startDate || "", "LLLL d, yyyy") }}</p>
        </v-col>
        <v-col>
          <p class="mt-2">END DATE:</p>
          <p class="font-weight-bold">{{ formatDate(program?.endDate || "", "LLLL d, yyyy") }}</p>
        </v-col>
      </v-row>
    </div>

    <v-card-actions class="d-flex flex-row justify-start ga-3 flex-wrap">
      <v-btn v-if="status !== 'Draft'" size="large" variant="outlined" color="primary" @click="openProfile">
        View
      </v-btn>
      <v-btn v-else-if="!isActive" size="large" variant="outlined" color="primary" @click="openProfile">
        Review now
      </v-btn>
      <v-row v-else>
      <v-btn size="large" variant="outlined" color="primary" @click="openProfile">
        Continue
      </v-btn>
      <v-spacer />
      <p v-if="isActive && status === 'Draft'" class="mt-2 text-support-border-info">
        REVIEW IN PROGRESS: {{ step }}/5
      </p>
      </v-row>
    </v-card-actions>
  </v-card>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";
import type { Components } from "@/types/openapi";
import type { ProgramStage } from "@/types/wizard";
import { formatDate } from "@/utils/format";
import { useRouter } from "vue-router";

export default defineComponent({
  name: "ProgramProfileCard",
  props: {
    program: {
      type: Object as PropType<Components.Schemas.Program>,
      required: true,
    },
  },
  setup() {
    const router = useRouter();
    return { router };
  },
  emits: ["review", "continue", "view"],
  computed: {
    name(): string {
      return this.program.name || "—";
    },
    programType(): string {
      if (!this.program.programTypes || this.program.programTypes.length === 0) {
        return "—";
      }
      return this.program.programTypes.join(", ");
    },
    status(): Components.Schemas.ProgramStatus {
      return this.program.status || "Draft";
    },
    isActive(): boolean {
      return this.program.portalStage !== null && this.program.portalStage !== undefined;
    },
    chipColour(): string | undefined {
      switch (this.status) {
        case "Draft":
          return "warning-border";
        case "UnderReview":
          return "support-border-info"
        case "Approved":
          return "#42814A"
      }
    },
    step(): number {
      if (!this.isActive) return 0;
      
      const stageToStep: Record<ProgramStage, number> = {
        ProgramOverview: 1,
        EarlyChildhood: 2,
        InfantAndToddler: 3,
        SpecialNeeds: 4,
        Review: 5,
      };
      
      return stageToStep[this.program.portalStage as ProgramStage] || 0;
    },
    statusText(): string {
      switch (this.status) {
        case "Draft":
          return "Requires review";
        case "UnderReview":
          return "Under registry review";
        case "Approved":
          return "Registry review complete";
        case "Denied":
          return "Denied";
        case "Inactive":
          return "Inactive";
        default:
          return "Requires review";
      }
    },
  },
  methods: {
    openProfile() {
      this.router.push({ path: "/program/" + this.program.id });
    },
    formatDate
  },
});
</script>
