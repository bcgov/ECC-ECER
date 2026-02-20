<template>
  <div>
    <v-progress-linear
      v-if="loading"
      indeterminate
      color="primary"
    ></v-progress-linear>
    <v-row v-else-if="program && updateInProgress" justify="center">
      <v-col cols="12" md="8" lg="6">
        <v-card flat rounded="lg" class="pa-8 text-center">
          <v-progress-circular
            indeterminate
            color="primary"
            size="56"
            class="mb-4"
          />
          <h3 class="mb-3">Preparing your change request</h3>
          <p class="ma-0">
            Your request has been initiated. Please wait a few minutes while we
            prepare it for review. When ready, it will appear here and will also
            be available in your dashboard.
          </p>
        </v-card>
      </v-col>
    </v-row>
    <ProgramWizard
      v-else-if="isDraftOrInProgress && program"
      :program-id="programId"
      :program="program"
    />
    <ProgramDetail v-else-if="program" :program="program" />
    <Alert
      v-else
      type="error"
      :closable="false"
      :prominent="true"
      variant="text"
      :rounded="false"
    >
      <h2 class="text-error">Program Not Found</h2>
      <p class="text-grey-dark">
        The program you are looking for does not exist or you do not have access
        to it.
      </p>
    </Alert>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { getPspUserProfile } from "@/api/psp-rep";
import { getPrograms } from "@/api/program";
import { useUserStore } from "@/store/user";
import ProgramWizard from "../program-profile/ProgramWizard.vue";
import ProgramDetail from "../ProgramDetail.vue";
import Alert from "@/components/Alert.vue";
import type { Components } from "@/types/openapi";

const POLL_INTERVAL_MS = 10000;

export default defineComponent({
  name: "Program",
  components: {
    ProgramWizard,
    ProgramDetail,
    Alert,
  },
  props: {
    programId: {
      type: String,
      required: true,
    },
  },
  data() {
    return {
      loading: true,
      program: null as Components.Schemas.Program | null,
      pollTimeoutId: null as ReturnType<typeof setTimeout> | null,
    };
  },
  computed: {
    isDraftOrInProgress(): boolean {
      if (!this.program || !this.program.status) {
        return false;
      }
      return this.program.status === "Draft";
    },
    updateInProgress(): boolean {
      return (
        this.program?.programProfileType === "ChangeRequest" &&
        this.program?.status === "Draft" &&
        !this.program?.readyForReview
      );
    },
  },
  async setup() {
    const userStore = useUserStore();

    // Refresh userProfile from the server
    const userProfile = await getPspUserProfile();
    if (userProfile !== null) {
      userStore.setPspUserProfile(userProfile);
    }

    return {
      userStore,
    };
  },
  async mounted() {
    await this.loadProgram();
  },
  beforeUnmount() {
    this.clearPoll();
  },
  methods: {
    clearPoll() {
      if (this.pollTimeoutId != null) {
        clearTimeout(this.pollTimeoutId);
        this.pollTimeoutId = null;
      }
    },
    async fetchProgram() {
      this.clearPoll();
      try {
        const { data: response } = await getPrograms(this.programId, [
          "Draft",
          "Denied",
          "Approved",
          "UnderReview",
          "ChangeRequestInProgress",
          "Inactive",
        ]);
        const program =
          response?.programs && response.programs.length > 0
            ? response.programs[0]
            : null;
        this.program = program || null;

        if (this.updateInProgress) {
          this.pollTimeoutId = setTimeout(
            () => this.fetchProgram(),
            POLL_INTERVAL_MS,
          );
        }
      } catch (error) {
        console.error("Error loading program:", error);
        this.program = null;
      }
    },
    async loadProgram() {
      this.loading = true;
      await this.fetchProgram();
      this.loading = false;
    },
  },
});
</script>
