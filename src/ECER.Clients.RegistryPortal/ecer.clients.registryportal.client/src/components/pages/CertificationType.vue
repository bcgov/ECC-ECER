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
        <ExpandSelect :options="options" :multiselect="multiselect" :selected="selectedOptions"></ExpandSelect>
      </v-col>
    </v-row>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import ECEAssistantContent from "@/components/ECEAssistantContent.vue";
import ECEFiveYearContent from "@/components/ECEFiveYearContent.vue";
import ECEOneYearContent from "@/components/ECEOneYearContent.vue";
import ExpandSelect from "@/components/ExpandSelect.vue";
import PageContainer from "@/components/PageContainer.vue";
import type { ExpandSelectOption } from "@/types/expand-select";

export default defineComponent({
  name: "CertificationType",
  components: { ExpandSelect, PageContainer },
  data() {
    return {
      multiselect: false,
      options: [
        {
          id: "1",
          title: "ECE Assistant",
          contentComponent: ECEAssistantContent,
        },
        {
          id: "2",
          title: "ECE One Year",
          contentComponent: ECEOneYearContent,
        },
        {
          id: "3",
          title: "ECE Five Year",
          contentComponent: ECEFiveYearContent,
        },
      ] as ExpandSelectOption[],
      selectedOptions: [] as string[],
    };
  },
});
</script>
