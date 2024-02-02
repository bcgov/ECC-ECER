<template>
  <PageContainer>
    <v-row class="ga-4">
      <v-col cols="1" offset="0" offset-lg="2" offset-md="2" offset-xl="2">
        <v-btn variant="text" @click="$router.back()">
          <v-icon start icon="mdi-arrow-left"></v-icon>
          Back
        </v-btn>
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="12" md="8" lg="8" xl="8" class="mx-auto">
        <v-row class="ga-4">
          <div v-for="certificationType in applicationStore.currentApplication.certificationTypes" :key="certificationType">
            <template v-if="certificationType === 'EceAssistant'">
              <ECEAssistantRequirements />
            </template>
            <template v-if="certificationType === 'OneYear'">
              <ECEOneYearRequirements />
            </template>
            <template v-if="certificationType === 'FiveYears'">
              <ECEFiveYearRequirements />
            </template>
            <template v-if="certificationType === 'Sne'">
              <SneRequirements />
            </template>
            <template v-if="certificationType === 'Ite'">
              <IteRequirements />
            </template>
          </div>
        </v-row>
        <v-row justify="end" class="mt-12">
          <v-btn rounded="lg" variant="outlined" class="mr-2" @click="alertStore.setSuccessAlert('Save as Draft')">Save As Draft</v-btn>
          <v-btn rounded="lg" color="primary" @click="$router.push('/application')">Start Application</v-btn>
        </v-row>
      </v-col>
    </v-row>
  </PageContainer>
</template>
<script lang="ts">
import { defineComponent } from "vue";

import ECEAssistantRequirements from "@/components/ECEAssistantRequirements.vue";
import ECEFiveYearRequirements from "@/components/ECEFiveYearRequirements.vue";
import ECEOneYearRequirements from "@/components/ECEOneYearRequirements.vue";
import IteRequirements from "@/components/IteRequirements.vue";
import PageContainer from "@/components/PageContainer.vue";
import SneRequirements from "@/components/SneRequirements.vue";
import { useAlertStore } from "@/store/alert";
import { useApplicationStore } from "@/store/application";

export default defineComponent({
  name: "Requirements",
  components: {
    ECEAssistantRequirements,
    ECEOneYearRequirements,
    ECEFiveYearRequirements,
    SneRequirements,
    IteRequirements,
    PageContainer,
  },
  setup() {
    const applicationStore = useApplicationStore();
    const alertStore = useAlertStore();
    return { applicationStore, alertStore };
  },
});
</script>
