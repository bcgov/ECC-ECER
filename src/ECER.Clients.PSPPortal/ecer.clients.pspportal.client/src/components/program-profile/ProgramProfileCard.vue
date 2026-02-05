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
        <v-col
          v-if="
            program.programProfileType == 'ChangeRequest' && !isReadyForReview()
          "
          class="d-flex justify-end"
        >
          <p>Loading...</p>
          <v-progress-circular
            indeterminate
            color="primary"
            size="24"
          ></v-progress-circular>
        </v-col>
        <v-col class="d-flex justify-end">
          <v-chip :color="chipColour" variant="flat" size="small">
            {{ statusText }}
          </v-chip>
        </v-col>
      </v-row>
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
import { getPrograms, withdrawProgram } from "@/api/program";

const INTERVAL_TIME = 30000;

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
      readyForReview: false as boolean,
      updatedProgram: this.program as Components.Schemas.Program,
      pollInterval: 0 as any,
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
  async mounted() {
    if (this.program.programProfileType == "ChangeRequest") {
      if (!this.program.readyForReview) {
        /* Poll the backend until the ready for review flag is set */
        this.pollInterval = setInterval(() => {
          this.fetchProgram();
        }, INTERVAL_TIME);
        setTimeout(() => {
          clearInterval(this.pollInterval);
        }, INTERVAL_TIME * 10);
      }
    }
  },
  methods: {
    async fetchProgram() {
      const programId = this.program.id;
      if (programId) {
        const { data: response } = await getPrograms(programId);
        if (response?.programs && response.programs[0]) {
          this.updatedProgram = response.programs[0];
          if (this.updatedProgram.readyForReview) {
            /* Ready for review flag has been set. Stop polling. */
            clearInterval(this.pollInterval);
          }
        }
      }
    },
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
    isReadyForReview(): boolean {
      if (this.updatedProgram.readyForReview) {
        return true;
      } else {
        return false;
      }
    },
  },
});
</script>
