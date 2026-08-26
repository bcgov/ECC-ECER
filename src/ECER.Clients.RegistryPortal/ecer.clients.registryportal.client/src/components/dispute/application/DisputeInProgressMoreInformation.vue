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
      <ECEHeader title="Application decision dispute in progress" />
      <p class="mt-3">
        We have received your request to dispute the decision. We are reviewing
        the decision. We will contact you if we have questions and/or when the
        reconsideration is complete.
      </p>
    </PageContainer>
  </template>
</template>
<script lang="ts">
import { defineComponent } from "vue";
import { useLoadingStore } from "@/store/loading";
import type { Components } from "@/types/openapi";

import { getCertificationName } from "@/utils/certification";
import { getApplications } from "@/api/application";

import Loading from "@/components/Loading.vue";
import PageContainer from "@/components/PageContainer.vue";
import Breadcrumb from "@/components/Breadcrumb.vue";
import ECEHeader from "@/components/ECEHeader.vue";

export default defineComponent({
  name: "DisputeInProgressMoreInformation",
  components: { PageContainer, Breadcrumb, ECEHeader, Loading },
  setup() {
    const loadingStore = useLoadingStore();

    return { loadingStore };
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
  },
});
</script>
