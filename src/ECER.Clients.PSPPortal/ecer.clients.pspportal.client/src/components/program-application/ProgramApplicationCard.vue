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
        <v-col class="d-flex justify-end">
          <v-chip :color="chipColour" variant="flat" size="small">
            <div :class="isStillLoading && status === 'Draft' ? 'pr-2' : ''">
              {{ statusText }}
            </div>
            <v-progress-circular
              v-if="isStillLoading && status === 'Draft'"
              indeterminate
              color="primary"
              size="18"
            />
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
        <v-btn
          size="large"
          variant="outlined"
          color="primary"
          @click="handleButtonAction"
        >
          {{ buttonText }}
        </v-btn>
        <v-spacer />
        <v-btn
          v-if="programApplication?.status === 'Draft'"
          size="large"
          variant="flat"
          color="error"
          @click="showWithdrawConfirmation = !showWithdrawConfirmation"
        >
          Withdraw
        </v-btn>
      </v-row>
    </v-card-actions>

    <ConfirmationDialog
      :show="showWithdrawConfirmation"
      :loading="isWithdrawing"
      title="Withdraw application"
      accept-button-text="Withdraw application"
      cancel-button-text="Cancel"
      @accept="confirmWithdraw"
      @cancel="showWithdrawConfirmation = false"
    >
      <template #confirmation-text>
        <p class="font-weight-bold">
          Are you sure you want to withdraw the application {{ programName }}?
        </p>
        <p>
          Once withdrawn, the application will no longer be considered. This
          action cannot be undone.
        </p>
      </template>
    </ConfirmationDialog>
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
  withdrawProgramApplication,
} from "@/api/program-application";

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
  emits: ["refresh-application-list"],
  data() {
    return {
      showWithdrawConfirmation: false,
      isWithdrawing: false,
    };
  },
  computed: {
    isStillLoading(): boolean {
      return this.programApplication.componentsGenerationCompleted !== true;
    },
    isRFAI(): boolean {
      return this.programApplication.statusReasonDetail === "RFAIrequested";
    },
    applicationType(): string {
      return this.programApplication.programApplicationType
        ? mapApplicationType(this.programApplication.programApplicationType)
        : "-";
    },
    programName(): string {
      return this.programApplication.programApplicationName || "—";
    },
    deliveryType(): string {
      return this.programApplication.deliveryType
        ? mapDeliveryType(this.programApplication.deliveryType)
        : "—";
    },
    programType(): string {
      const types = this.programApplication.programTypes;
      if (!types?.length) return "—";
      return types.map(mapProgramType).join(", ");
    },
    status(): string {
      return this.programApplication.status || "Draft";
    },
    buttonText(): string {
      if (this.status === "Draft" || this.isRFAI) {
        return "Edit";
      }
      return "View";
    },
    chipColour(): string | undefined {
      if (this.isRFAI) return "warning";
      switch (this.status) {
        case "Draft":
          return "warning";
        case "Approved":
          return "success";
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
        case "PendingDecision":
          return "primary";
        default:
          return "grey-darkest";
      }
    },
    statusText(): string {
      if (this.isRFAI) return "Additional information requested";
      let status = mapProgramStatus(this.status);
      if (this.isStillLoading && status === "Draft") {
        status = "Initiating";
      }
      return status;
    },
  },
  methods: {
    formatDate,
    async confirmWithdraw() {
      this.isWithdrawing = true;
      try {
        const response = await withdrawProgramApplication(
          this.programApplication,
        );
        if (response.error) {
          console.error("Failed to withdraw program:", response.error);
        } else {
          this.$emit("refresh-application-list");
        }
      } finally {
        this.isWithdrawing = false;
        this.showWithdrawConfirmation = false;
      }
    },

    handleButtonAction(): void {
      if (this.buttonText === "Edit") {
        this.editApplication();
      }
    },

    editApplication(): void {
      this.$router.push({
        name: "programApplication",
        params: { programApplicationId: this.programApplication.id },
      });
    },
  },
});
</script>
