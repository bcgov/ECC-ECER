<template>
  <PageContainer>
    <v-row class="ga-4">
      <v-col cols="1" offset="0" offset-lg="2" offset-md="2" offset-xl="2">
        <v-btn variant="text" @click="$router.back()">
          <v-icon start icon="mdi-arrow-left"></v-icon>
          Back
        </v-btn>
      </v-col>
      <v-col cols="12">
        <h2 class="text-center">What certificate type(s) are you applying for?</h2>
      </v-col>
      <v-col cols="12" md="8" lg="8" xl="8" class="mx-auto">
        <ExpandSelect :options="certificationTypes"></ExpandSelect>
        <v-row justify="end" class="mt-12">
          <v-btn rounded="lg" variant="outlined" class="mr-2" @click="$router.back()">Cancel</v-btn>
          <v-btn rounded="lg" color="primary" :disabled="certificationTypeStore.certificationTypes.length === 0" @click="submit">Start Application</v-btn>
        </v-row>
      </v-col>
    </v-row>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import { createOrUpdateDraftApplication } from "@/api/application";
import ExpandSelect from "@/components/ExpandSelect.vue";
import PageContainer from "@/components/PageContainer.vue";
import certificationTypes from "@/config/certification-types";
import { useAlertStore } from "@/store/alert";
import { useApplicationStore } from "@/store/application";
import { useCertificationTypeStore } from "@/store/certificationType";
import type { Components } from "@/types/openapi";

export default defineComponent({
  name: "CertificationType",
  components: { ExpandSelect, PageContainer },
  setup() {
    const applicationStore = useApplicationStore();
    const alertStore = useAlertStore();
    const certificationTypeStore = useCertificationTypeStore();

    return { certificationTypes, applicationStore, alertStore, certificationTypeStore };
  },

  methods: {
    async submit() {
      if (this.certificationTypeStore.certificationTypes.length > 0) {
        const application: Components.Schemas.DraftApplication = {
          id: null,
          certificationTypes: this.certificationTypeStore.certificationTypes,
          stage: "ContactInformation",
        };
        const response = await createOrUpdateDraftApplication(application);
        if (response) {
          const data = response as Components.Schemas.DraftApplicationResponse;
          this.applicationStore.currentApplication.id = data.applicationId;
          this.$router.push("/requirements");
        }
      } else {
        this.alertStore.setFailureAlert("Select a certification type to continue");
      }
    },
  },
});
</script>
