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
      <p>PROGRAM PROFILE:</p>
      <p class="font-weight-bold">{{ name }}</p>
      <p class="mt-2">PROGRAM TYPE:</p>
      <p class="font-weight-bold">{{ programType }}</p>
      <v-row>
        <v-col>
          <p class="mt-2">START DATE:</p>
          <p class="font-weight-bold">
            {{ formatDate(program?.startDate || "", "LLLL d, yyyy") }}
          </p>
        </v-col>
        <v-col>
          <p class="mt-2">END DATE:</p>
          <p class="font-weight-bold">
            {{ formatDate(program?.endDate || "", "LLLL d, yyyy") }}
          </p>
        </v-col>
      </v-row>
    </div>

    <v-card-actions class="d-flex flex-row justify-start ga-3 flex-wrap">
      <v-row
        v-if="
          program?.programProfileType === 'ChangeRequest' && status === 'Draft'
        "
      >
        <v-btn
          size="large"
          variant="outlined"
          color="primary"
          @click="openProfile"
        >
          View
        </v-btn>
        <v-spacer />
        <v-btn size="large" variant="flat" color="error" @click="withdraw">
          Withdraw changes
        </v-btn>
      </v-row>
      <v-row v-else-if="status !== 'Draft'">
        <v-btn
          size="large"
          variant="outlined"
          color="primary"
          @click="openProfile"
        >
          View
        </v-btn>
      </v-row>
      <v-row v-else-if="!isActive">
        <v-btn
          size="large"
          variant="outlined"
          color="primary"
          @click="openProfile"
        >
          Review now
        </v-btn>
      </v-row>
      <v-row v-else>
        <v-btn
          size="large"
          variant="outlined"
          color="primary"
          @click="openProfile"
        >
          Continue
        </v-btn>
        <v-spacer />
        <p
          v-if="!isActive && status === 'Draft'"
          class="mt-2 text-support-border-info"
        >
          REVIEW IN PROGRESS: {{ step }}/5
        </p>
      </v-row>
    </v-card-actions>
    <ConfirmationDialog
      :show="showWithdrawConfirmation"
      :loading="isWithdrawing"
      title="Withdraw Change Request"
      accept-button-text="Withdraw"
      cancel-button-text="Cancel"
      @accept="confirmWithdraw"
      @cancel="showWithdrawConfirmation = false"
    >
      <template #confirmation-text>
        <p>
          Are you sure you want to withdraw this change request? All changes
          associated with this request will be deleted.
        </p>
      </template>
    </ConfirmationDialog>
  </v-card>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";
import type { Components } from "@/types/openapi";
import type { ProgramStage } from "@/types/wizard";
import { formatDate } from "@/utils/format";
import { useRouter } from "vue-router";
import ConfirmationDialog from "@/components/ConfirmationDialog.vue";
import { withdrawProgram } from "@/api/program";

export default defineComponent({
  name: "ProgramProfileCard",
  components: { ConfirmationDialog },
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
  emits: ["review", "continue", "view", "withdrawn"],
  data() {
    return {
      showWithdrawConfirmation: false,
      isWithdrawing: false,
    };
  },
  computed: {
    name(): string {
      return this.program.name || "—";
    },
    programType(): string {
      if (
        !this.program.programTypes ||
        this.program.programTypes.length === 0
      ) {
        return "—";
      }
      return this.program.programTypes.join(", ");
    },
    status(): Components.Schemas.ProgramStatus {
      return this.program.status || "Draft";
    },
    isActive(): boolean {
      return (
        this.program.portalStage !== null &&
        this.program.portalStage !== undefined
      );
    },
    chipColour(): string | undefined {
      switch (this.status) {
        case "Draft":
          return "warning";
        case "UnderReview":
          return "primary";
        case "Approved":
          return "success";
        case "Denied":
          return "error";
        default:
          return "grey-darkest";
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
          return "Under ECE Registry review";
        case "Approved":
          return "ECE Registry review complete";
        case "Denied":
          return "Denied";
        case "Inactive":
          return "Inactive";
        case "ChangeRequestInProgress":
          return "Change request in progress";
        case "Withdrawn":
          return "Withdrawn";
        default:
          return "Requires review";
      }
    },
  },
  methods: {
    openProfile() {
      this.router.push({ path: "/program/" + this.program.id });
    },
    withdraw() {
      this.showWithdrawConfirmation = true;
    },
    async confirmWithdraw() {
      this.isWithdrawing = true;
      try {
        const response = await withdrawProgram(this.program);
        if (response.error) {
          console.error("Failed to withdraw program:", response.error);
        } else {
          this.$emit("withdrawn");
        }
      } finally {
        this.isWithdrawing = false;
        this.showWithdrawConfirmation = false;
      }
    },
    formatDate,
  },
});
</script>
