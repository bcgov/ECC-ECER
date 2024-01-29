<template>
  <h1>Profile</h1>
  <v-row>
    <v-col cols="12" md="10" lg="6" xl="4">
      <EceForm :form="profileInformationForm" />
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import EceForm from "@/components/EceForm.vue";
import profileInformationForm from "@/config/profile-information-form";

export default defineComponent({
  name: "Profile",
  components: { EceForm },
  setup: () => {
    return { profileInformationForm };
  },
});
</script>
