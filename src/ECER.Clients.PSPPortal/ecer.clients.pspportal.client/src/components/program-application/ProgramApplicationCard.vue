<template>
  <v-card
    :rounded="true"
    :border="true"
    flat
    class="pa-6 border-primary border-opacity-100 w-100"
  >
    <div class="d-flex justify-end">
      <v-chip :color="chipColour" variant="flat" size="small">
        {{ statusText }}
      </v-chip>
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
        <v-btn
          size="large"
          variant="outlined"
          color="primary"
        >
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
import { mapProgramStatus, mapApplicationType, mapDeliveryType, mapProgramType } from "@/api/program-application";

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

    };
  },
  computed: {
    applicationType(): string {
      return this.programApplication.programApplicationType 
      ? mapApplicationType(this.programApplication.programApplicationType) 
      : "-";
    },
    programName(): string {
      return this.programApplication.programApplicationName || "—";
    },
    deliveryType() : string {
      return this.programApplication.deliveryType 
      ? mapDeliveryType(this.programApplication.deliveryType)
      : "—";
    },
    programType(): string {
      return this.programApplication.programType 
      ? mapProgramType(this.programApplication.programType) 
      : "-"
    },
    status(): string {
      return this.programApplication.status || "Draft";
    },
    buttonText(): string {
      if(this.programApplication.status === "Draft" || this.programApplication.status === "RFAI") {
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
  },
});
</script>
