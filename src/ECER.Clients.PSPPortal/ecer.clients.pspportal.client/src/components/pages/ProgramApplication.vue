<template>
  <PageContainer>
    <v-row>
      <v-col cols="12">
        <Breadcrumb />
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="12">
        <h1 class="mb-4">Program application</h1>
      </v-col>
    </v-row>

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
          <h3 class="mb-3">Preparing your application</h3>
          <p class="ma-0">
            Your request has been initiated. Please wait a few minutes while we
            prepare it for review. When ready, it will appear here and will also
            be available in your dashboard.
          </p>
        </v-card>
      </v-col>
    </v-row>

    <template v-else-if="application?.componentsGenerationCompleted">
      <v-row>
        <v-col cols="12">
          <p>
            Program application:
            {{ application.programApplicationName ?? application.id }}
          </p>
          <v-btn
            class="mt-4"
            variant="text"
            color="primary"
            :to="{ name: 'program-applications' }"
          >
            Back to program applications
          </v-btn>
        </v-col>
      </v-row>
    </template>

    <v-row v-else-if="loadError">
      <v-col cols="12">
        <Callout title="Error" type="error">
          <p>
            Unable to load this program application. It may not exist or you may
            not have access.
          </p>
          <v-btn
            class="mt-2"
            variant="text"
            color="primary"
            :to="{ name: 'program-applications' }"
          >
            Back to program applications
          </v-btn>
        </Callout>
      </v-col>
    </v-row>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import PageContainer from "@/components/PageContainer.vue";
import Breadcrumb from "@/components/Breadcrumb.vue";
import Callout from "@/components/common/Callout.vue";
import Loading from "@/components/Loading.vue";
import { getProgramApplicationById } from "@/api/program-application";
import type { Components } from "@/types/openapi";

const POLL_INTERVAL_MS = 10000;

export default defineComponent({
  name: "ProgramApplication",
  components: { PageContainer, Breadcrumb, Callout, Loading },
  props: {
    programApplicationId: {
      type: String,
      required: true,
    },
  },
  data() {
    return {
      application: null as Components.Schemas.ProgramApplication | null,
      initialLoad: true,
      loadError: false,
      pollTimeoutId: null as ReturnType<typeof setTimeout> | null,
    };
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
  },
});
</script>
<style scoped>
.v-progress-linear__indeterminate .long,
.v-progress-linear__indeterminate .short {
  animation-duration: 4s;
}
</style>
