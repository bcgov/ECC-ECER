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

export default defineComponent({
  name: "CertificationType",
  components: { ExpandSelect, PageContainer },
  setup() {
    return { certificationTypes };
  },
  data() {
    return {
      subSelection: [] as Array<string>,
      selectedCertificationType: "-1" as string,
    };
  },
  methods: {
    submit() {
      // TODO handle passing data to application wizard
      this.$router.push("/application");
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
