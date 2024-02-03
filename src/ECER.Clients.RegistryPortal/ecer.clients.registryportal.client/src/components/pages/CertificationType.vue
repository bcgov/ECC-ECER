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
        <ExpandSelect
          :options="certificationTypes"
          :selected="selectedCertificationType?.toString()"
          @selection="handleExpandSelectSelection"
          @sub-selection="handleExpandSelectSubSelection"
        ></ExpandSelect>
        <v-row justify="end" class="mt-12">
          <v-btn rounded="lg" variant="outlined" class="mr-2" @click="$router.back()">Cancel</v-btn>
          <v-btn rounded="lg" color="primary" :disabled="selectedCertificationType === null" @click="submit">Start Application</v-btn>
        </v-row>
      </v-col>
    </v-row>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import ExpandSelect from "@/components/ExpandSelect.vue";
import PageContainer from "@/components/PageContainer.vue";
import certificationTypes from "@/config/certification-types";
import { useAlertStore } from "@/store/alert";
import { useApplicationStore } from "@/store/application";
import type { Components } from "@/types/openapi";

export default defineComponent({
  name: "CertificationType",
  components: { ExpandSelect, PageContainer },
  setup() {
    const applicationStore = useApplicationStore();
    const alertStore = useAlertStore();

    return { certificationTypes, applicationStore, alertStore };
  },
  data() {
    return {
      subSelection: [] as Array<Components.Schemas.CertificationType>,
      selectedCertificationType: null as Components.Schemas.CertificationType | null,
    };
  },
  methods: {
    submit() {
      if (this.selectedCertificationType === null) {
        this.alertStore.setFailureAlert("Select a certification type to continue");
      } else {
        const certificationTypes: Array<Components.Schemas.CertificationType> = [this.selectedCertificationType, ...this.subSelection];

        this.applicationStore.setCertificationTypes(certificationTypes);
        this.applicationStore.newDraftApplication(certificationTypes);

        this.$router.push("/requirements");
      }
    },

    handleExpandSelectSelection(selected: string | null) {
      this.selectedCertificationType = selected as Components.Schemas.CertificationType;
    },
    handleExpandSelectSubSelection(selected: Array<string>) {
      this.subSelection = selected as Components.Schemas.CertificationType[];
    },
  },
});
</script>
