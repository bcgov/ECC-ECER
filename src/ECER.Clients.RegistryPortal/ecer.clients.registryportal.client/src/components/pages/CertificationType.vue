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
        <ExpandSelect :options="certificationTypes" :selected="selected" @selection="handleExpandSelectSelection"></ExpandSelect>
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
      multiselect: false,
      selected: "",
    };
  },
  methods: {
    handleExpandSelectSelection(selected: string) {
      this.selected = selected;
    },
  },
});
</script>
