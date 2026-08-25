<template>
  <Loading v-if="loadingStore.isLoading('application_get')"></Loading>
  <template v-else>
    <v-container fluid class="bg-primary">
      <v-container>
        <v-row>
          <v-col>
            <h2 class="text-white">
              Application for
              {{ getCertificationName(application?.certificationTypes || []) }}
              certification assessed
            </h2>
          </v-col>
        </v-row>
      </v-container>
    </v-container>
    <PageContainer :margin-top="false">
      <Breadcrumb />
      <ECEHeader title="Application decision: intent to deny" />
      <p class="mt-3">
        Your application
        {{ getCertificationName(application?.certificationTypes || []) }} was
        denied. Check your messages for more information about the decision. You
        can dispute hte decision by contacting the registry requesting
        <a
          target="_blank"
          href="https://www2.gov.bc.ca/gov/content?id=E0F8FD783E114B22AAAB5CF8BC715B54"
        >
          reconsideration
        </a>
        until
        {{
          formatDate(
            application?.reconsiderationPeriodEndDate || "",
            "LLLL d, yyyy",
          )
        }}.
      </p>
    </PageContainer>
  </template>
</template>
<script lang="ts">
import { defineComponent } from "vue";
import { useRoute } from "vue-router";
import type { Components } from "@/types/openapi";
import { useLoadingStore } from "@/store/loading";
import { getApplications } from "@/api/application";
import { formatDate } from "@/utils/format";

import { getCertificationName } from "@/utils/certification";

import PageContainer from "@/components/PageContainer.vue";
import Breadcrumb from "@/components/Breadcrumb.vue";
import ECEHeader from "@/components/ECEHeader.vue";
import Loading from "@/components/Loading.vue";

export default defineComponent({
  name: "DisputeIntentToDenyMoreInformation",
  components: { PageContainer, Breadcrumb, ECEHeader, Loading },
  setup() {
    const route = useRoute();
    const loadingStore = useLoadingStore();

    return { route, loadingStore };
  },
  props: {
    applicationId: {
      type: String,
      required: true,
    },
  },
  async mounted() {
    this.application = (await getApplications(this.applicationId))?.data?.[0];
  },
  data() {
    return {
      application: undefined as Components.Schemas.Application | undefined,
    };
  },
  methods: {
    getCertificationName,
    formatDate,
  },
});
</script>
