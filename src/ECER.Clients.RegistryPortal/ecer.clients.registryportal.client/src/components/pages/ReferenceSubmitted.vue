<template>
    <PageContainer>
      <v-col class="pa-10 mt-5">
     
          <div>
            <div class="text-center mb-10">
              <v-icon size="106px" color="#42814A" icon="mdi-check-circle" class="mr-2"></v-icon>
              <h3>Thank you</h3>
              <br />
              <p>We will notify the applicant of you've responded to their request for a reference.</p>
            </div>
          </div>
      
      </v-col>
    </PageContainer>
  </template>
<script lang="ts">
import { defineComponent } from "vue";

import PageContainer from "@/components/PageContainer.vue";

export default defineComponent({
  name: "Submitted",
  components: { PageContainer },
  props: {},
});
</script>
  