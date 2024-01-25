<template>
  <PageContainer>
    <v-row class="ga-4">
      <v-col cols="1">
        <v-btn variant="text" @click="$router.back()">
          <v-icon start icon="mdi-arrow-left"></v-icon>
          Back
        </v-btn>
      </v-col>
      <v-col cols="12">
        <h2>What certificate type(s) are you applying for?</h2>
      </v-col>
      <v-col cols="12" md="8" lg="8" xl="8">
        <ExpandSelect
          :options="certificationTypes"
          :selected="selectedCertificationType"
          @selection="handleExpandSelectSelection"
          @sub-selection="handleExpandSelectSubSelection"
        ></ExpandSelect>
        <v-row justify="end" class="mt-12">
          <v-btn variant="outlined" class="mr-2" @click="$router.back()">Cancel</v-btn>
          <v-btn color="primary" :disabled="selectedCertificationType == '-1'" @click="submit">Start Application</v-btn>
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
import { useApplicationStore } from "@/store/application";
import type { Components } from "@/types/openapi";

const certificateTypeMap = new Map<string, Components.Schemas.CertificationType>([
  ["1", 0],
  ["2", 1],
  ["3", 2],
  ["ITE", 3],
  ["SNE", 4],
]);
export default defineComponent({
  name: "CertificationType",
  components: { ExpandSelect, PageContainer },
  setup() {
    const applicationStore = useApplicationStore();
    return { certificationTypes, applicationStore };
  },
  data() {
    return {
      subSelection: [] as Array<string>,
      selectedCertificationType: "-1" as string,
    };
  },
  methods: {
    submit() {
      if (this.selectedCertificationType == "-1") {
        // TODO show snackbar error if no selection when ECER-824 is ready
        return;
      } else {
        // Map selectedCertificationType to corresponding number
        const selectedCertificationTypeNumber = certificateTypeMap.get(this.selectedCertificationType) as Components.Schemas.CertificationType;

        // Map subSelection to corresponding numbers
        const selectedSubNumbers = this.subSelection.map((selected) => certificateTypeMap.get(selected)) as Array<Components.Schemas.CertificationType>;

        const certificationTypes: Array<Components.Schemas.CertificationType> = [selectedCertificationTypeNumber, ...selectedSubNumbers];

        this.applicationStore.newDraftApplication(certificationTypes);
        this.$router.push("/application");
      }
    },

    handleExpandSelectSelection(selected: string) {
      this.selectedCertificationType = selected;
    },
    handleExpandSelectSubSelection(selected: Array<string>) {
      this.subSelection = selected;
    },
  },
});
</script>
