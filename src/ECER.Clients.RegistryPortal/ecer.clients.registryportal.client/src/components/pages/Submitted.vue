<template>
  <PageContainer>
    <v-col>
      <v-row class="pa-10 mt-5">
        <div>
          <div class="text-center mb-10">
            <v-icon size="106px" color="#42814A" icon="mdi-check-circle" class="mr-2"></v-icon>

            <h3>Application Submitted Successfully</h3>
            <p class="small">Thank you for submitting your application</p>
          </div>
          <p>What to expect next?</p>
          <ul class="mt-10">
            <li>
              You may be contacted for additional information/documentation to support your application. If your contact information changes during the
              application process, you must update this in your dashboard profile.
            </li>
            <br />
            <li>Applications can only be assessed once all documentation is received. Complete applications are assessed in date order.</li>
            <br />
            <li>
              Your application will remain open for one year (12 months) from the date your first application document is received by the ECE Registry.
              Incomplete applications are closed after 12 months.
            </li>
            <br />
            <li>You may view the status of your application in the dashboard. The ECE Registry will notify you of the outcome of your application.</li>
          </ul>
        </div>
      </v-row>
      <router-link :to="'/my-certifications/in-progress'">
        <v-btn class="mt-5" type="" rounded="lg" color="primary">Go to Dashboard</v-btn>
      </router-link>
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
