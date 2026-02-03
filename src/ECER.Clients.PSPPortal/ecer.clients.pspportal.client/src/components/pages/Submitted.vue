<template>
  <Loading v-if="loadingStore.isLoading('program_get') || !programProfile" />
  <template v-else>
    <v-sheet
      :rounded="'0'"
      flat
      color="background-light-success"
      class="success-banner"
    >
      <v-container>
        <div class="d-flex">
          <v-icon
            size="90px"
            color="#42814A"
            icon="mdi-check-circle"
            class="mr-2"
          ></v-icon>
          <h1 id="programProfileSubmitted" class="align-self-center">
            Submitted
          </h1>
        </div>
      </v-container>
    </v-sheet>

    <v-container>
      <template
        v-if="
          programProfile.changesMade &&
          programProfile.programProfileType === 'AnnualReview'
        "
      >
        <h2>What to expect next</h2>
        <br />
        <p>
          It is important to keep your educational institution and user contact
          information updated in your PSP Portal profile.
        </p>
        <br />
        <h2>Review</h2>
        <br />
        <ul class="ml-10">
          <li>
            We will review your submission and follow up with any requests for
            additional information, if needed
          </li>
          <li>We will email you after we have reviewed your submission</li>
        </ul>
        <br />
        <h2>Status</h2>
        <br />
        <ul class="ml-10">
          <li>
            You may view the status of your submission online in the PSP Portal
            dashboard
          </li>
        </ul>
        <br />
      </template>
      <template
        v-else-if="
          !programProfile.changesMade &&
          programProfile.programProfileType === 'AnnualReview'
        "
      >
        <h2>What to expect next</h2>
        <br />
        <p>
          You have submitted no changes to your program profile. Your program
          profile review is complete.
        </p>
        <br />
        <p>No further action is required.</p>
        <br />
      </template>
      <v-btn
        id="btnHome"
        rounded="lg"
        color="primary"
        @click="router.push({ name: 'dashboard' })"
      >
        Home
      </v-btn>
    </v-container>
  </template>
</template>
<script lang="ts">
import { defineComponent } from "vue";
import { getPrograms } from "@/api/program";
import type { Components } from "@/types/openapi";

import { useProgramStore } from "@/store/program";
import { useRouter } from "vue-router";
import { useLoadingStore } from "@/store/loading";
import { useAlertStore } from "@/store/alert";

import Loading from "../Loading.vue";

export default defineComponent({
  name: "Submitted",
  components: {
    Loading,
  },
  props: {
    programProfileId: {
      type: String,
      required: true,
    },
  },
  setup: () => {
    const programStore = useProgramStore();
    const loadingStore = useLoadingStore();
    const router = useRouter();
    const alertStore = useAlertStore();

    programStore.resetDraftProgram();
    return { programStore, router, loadingStore, alertStore };
  },
  data() {
    return {
      programProfile: undefined as Components.Schemas.Program | undefined,
    };
  },
  async mounted() {
    const { data } = await getPrograms(this.programProfileId, []);
    this.programProfile = data?.programs?.[0];

    if (!this.programProfile) {
      this.alertStore.setFailureAlert(
        `Program profile not found for program profile id: ${this.programProfileId}`,
      );
    }
  },
});
</script>
<style scoped>
.success-banner {
  min-height: 200px;
  display: flex;
  align-items: center;
}
</style>
