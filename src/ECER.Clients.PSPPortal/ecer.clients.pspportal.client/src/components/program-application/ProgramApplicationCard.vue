<template>
  <v-card
    :rounded="true"
    :border="true"
    flat
    class="pa-6 border-primary border-opacity-100 w-100"
  >
    <div class="d-flex justify-end">
      <v-row>
        <v-col></v-col>
        <v-col v-if="isStillLoading" class="d-flex justify-end">
          <p>Loading...</p>
          <v-progress-circular indeterminate color="primary" size="24" />
        </v-col>
        <v-col class="d-flex justify-end">
          <v-chip :color="chipColour" variant="flat" size="small">
            {{ statusText }}
          </v-chip>
        </v-col>
      </v-row>
    </div>

    <div class="mb-4">
      <h3 class="font-weight-bold mt-n3">Application</h3>
      <p class="font-weight-bold">{{ applicationType }}</p>
      <p class="mt-2">PROGRAM NAME:</p>
      <p class="font-weight-bold">{{ programName }}</p>
      <p class="mt-2">PROGRAM TYPE:</p>
      <p class="font-weight-bold">{{ programType }}</p>
      <p class="mt-2">DELIVERY TYPE:</p>
      <p class="font-weight-bold">{{ deliveryType }}</p>
    </div>

    <v-card-actions class="d-flex flex-row justify-start ga-3 flex-wrap">
      <v-row>
        <v-btn size="large" variant="outlined" color="primary">
          {{ buttonText }}
        </v-btn>
      </v-row>
    </v-card-actions>
  </v-card>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";
import type { Components } from "@/types/openapi";
import { formatDate } from "@/utils/format";
import { useRouter } from "vue-router";
import ConfirmationDialog from "@/components/ConfirmationDialog.vue";
import {
  mapProgramStatus,
  mapApplicationType,
  mapDeliveryType,
  mapProgramType,
  getProgramApplicationById,
} from "@/api/program-application";

const POLL_INTERVAL_MS = 10000;

export default defineComponent({
  name: "ProgramApplicationCard",
  components: { ConfirmationDialog },
  props: {
    programApplication: {
      type: Object as PropType<Components.Schemas.ProgramApplication>,
      required: true,
    },
  },
  setup() {
    const router = useRouter();
    return { router };
  },
  emits: [],
  data() {
    return {
      pollTimeoutId: null as ReturnType<typeof setTimeout> | null,
      localApplication: null as Components.Schemas.ProgramApplication | null, //for managing is loading
    };
  },
  mounted() {
    if (this.isStillLoading && this.programApplication?.id) {
      this.pollTimeoutId = setTimeout(this.checkReady, POLL_INTERVAL_MS);
    }
  },
  beforeUnmount() {
    if (this.pollTimeoutId != null) {
      clearTimeout(this.pollTimeoutId);
      this.pollTimeoutId = null;
    }
  },
  computed: {
    displayedApplication(): Components.Schemas.ProgramApplication {
      return this.localApplication ?? this.programApplication;
    },
    isStillLoading(): boolean {
      return this.displayedApplication.componentsGenerationCompleted !== true;
    },
    applicationType(): string {
      return this.displayedApplication.programApplicationType
        ? mapApplicationType(this.displayedApplication.programApplicationType)
        : "-";
    },
    programName(): string {
      return this.displayedApplication.programApplicationName || "—";
    },
    deliveryType(): string {
      return this.displayedApplication.deliveryType
        ? mapDeliveryType(this.displayedApplication.deliveryType)
        : "—";
    },
    programType(): string {
      const types = this.displayedApplication.programTypes;
      if (!types?.length) return "—";
      return types.map(mapProgramType).join(", ");
    },
    status(): string {
      return this.displayedApplication.status || "Draft";
    },
    buttonText(): string {
      if (
        this.displayedApplication.status === "Draft" ||
        this.displayedApplication.status === "RFAI"
      ) {
        return "Edit";
      }
      return "View";
    },
    chipColour(): string | undefined {
      switch (this.status) {
        case "Draft":
          return "warning";
        case "RFAI":
          return "warning";
        case "InterimRecognition":
          return "success";
        case "OnGoingRecognition":
          return "success";
        case "Submitted":
          return "primary";
        case "PendingReview":
          return "primary";
        case "ReviewAnalysis":
          return "primary";
        default:
          return "grey-darkest";
      }
    },
    statusText(): string {
      return mapProgramStatus(this.status);
    },
  },
  methods: {
    formatDate,
    async checkReady() {
      const id = this.programApplication?.id;
      if (!id) return;
      const result = await getProgramApplicationById(id);
      if (result.error) return;
      if (result.data?.componentsGenerationCompleted === true) {
        this.localApplication = {
          ...this.programApplication,
          componentsGenerationCompleted: true,
        };
        return;
      }
      this.pollTimeoutId = setTimeout(this.checkReady, POLL_INTERVAL_MS);
    },
  },
});
</script>
