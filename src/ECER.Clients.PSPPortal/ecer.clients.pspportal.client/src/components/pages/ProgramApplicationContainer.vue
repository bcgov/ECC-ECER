<template>
  <Loading v-if="initialLoad && !application" />

  <v-row
    v-else-if="application && !application.componentsGenerationCompleted"
    justify="center"
  >
    <v-col cols="12" md="8" lg="6">
      <v-card flat rounded="lg" class="pa-8 text-center">
        <v-progress-circular
          indeterminate
          color="primary"
          size="56"
          class="mb-4"
        />
        <h3 class="mb-3">Preparing profile for review</h3>
        <p class="ma-0">
          Please wait a few minutes while we prepare it for review. When ready,
          it will appear here and will also be available in your dashboard.
        </p>
      </v-card>
    </v-col>
  </v-row>

  <template v-else-if="application?.componentsGenerationCompleted">
    <ProgramApplication
      :programApplicationId="programApplicationId"
      :program-application="application"
    ></ProgramApplication>
  </template>

  <v-row v-else-if="loadError">
    <v-col cols="12">
      <Callout title="Error" type="error">
        <p>
          Unable to load this program application. It may not exist or you may
          not have access.
        </p>
      </Callout>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useDisplay } from "vuetify";
import PageContainer from "@/components/PageContainer.vue";
import Breadcrumb from "@/components/Breadcrumb.vue";
import Callout from "@/components/common/Callout.vue";
import Loading from "@/components/Loading.vue";
import { getProgramApplicationById } from "@/api/program-application";
import type { Components } from "@/types/openapi";
import ProgramApplication from "@/components/program-application/ProgramApplication.vue";

const POLL_INTERVAL_MS = 10000;

export default defineComponent({
  name: "ProgramApplicationContainer",
  components: {
    PageContainer,
    Breadcrumb,
    Callout,
    Loading,
    ProgramApplication,
  },
  props: {
    programApplicationId: {
      type: String,
      required: true,
    },
  },
  setup() {
    const { mobile } = useDisplay();

    return {
      mobile,
    };
  },
  data() {
    return {
      application: null as Components.Schemas.ProgramApplication | null,
      initialLoad: true,
      loadError: false,
      pollTimeoutId: null as ReturnType<typeof setTimeout> | null,
    };
  },
  computed: {
    programName(): string {
      return this.application?.programApplicationName || "—";
    },
  },
  mounted() {
    this.fetchApplication();
  },
  beforeUnmount() {
    if (this.pollTimeoutId != null) {
      clearTimeout(this.pollTimeoutId);
      this.pollTimeoutId = null;
    }
  },
  methods: {
    async fetchApplication() {
      if (!this.programApplicationId) {
        this.initialLoad = false;
        this.loadError = true;
        return;
      }
      const result = await getProgramApplicationById(this.programApplicationId);
      this.initialLoad = false;
      if (result.error || result.data == null) {
        this.loadError = true;
        this.application = null;
        return;
      }
      this.loadError = false;
      this.application = result.data;
      if (this.application.componentsGenerationCompleted !== true) {
        this.pollTimeoutId = setTimeout(this.checkReady, POLL_INTERVAL_MS);
      }
    },
    async checkReady() {
      if (!this.programApplicationId) return;
      const result = await getProgramApplicationById(this.programApplicationId);
      if (result.error) return;
      if (result.data?.componentsGenerationCompleted === true) {
        this.application = result.data;
        return;
      }
      this.pollTimeoutId = setTimeout(this.checkReady, POLL_INTERVAL_MS);
    },
    saveAndExit() {},
  },
});
</script>
<style scoped>
.v-progress-linear__indeterminate .long,
.v-progress-linear__indeterminate .short {
  animation-duration: 4s;
}
</style>
