<template>
  <div>
    <v-progress-linear v-if="loading" indeterminate color="primary"></v-progress-linear>
    <ProgramWizard v-else-if="isDraftOrInProgress && program" :program-id="programId" :program="program" />
    <ProgramDetail v-else-if="program" :program="program" />
    <Alert v-else type="error" :closable="false" :prominent="true" variant="text" :rounded="false">
      <h2 class="text-error">Program Not Found</h2>
      <p class="text-grey-dark">The program you are looking for does not exist or you do not have access to it.</p>
    </Alert>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { getPspUserProfile } from "@/api/psp-rep";
import { getPrograms } from "@/api/program";
import { useUserStore } from "@/store/user";
import ProgramWizard from "./ProgramWizard.vue";
import ProgramDetail from "../ProgramDetail.vue";
import Alert from "@/components/Alert.vue";
import type { Components } from "@/types/openapi";

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
    };
  },
  computed: {
    isDraftOrInProgress(): boolean {
      if (!this.program || !this.program.status) {
        return false;
      }
      return this.program.status === "Draft" || this.program.status === "UnderReview";
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
  methods: {
    async loadProgram() {
      this.loading = true;
      try {
        const { data: programs } = await getPrograms(this.programId, ["Draft", "Denied", "Approved", "UnderReview", "ChangeRequestInProgress", "Inactive"]);
        const program = programs && programs.length > 0 ? programs[0] : null;
        this.program = program || null;
      } catch (error) {
        console.error("Error loading program:", error);
        this.program = null;
      } finally {
        this.loading = false;
      }
    },
  },
});
</script>
