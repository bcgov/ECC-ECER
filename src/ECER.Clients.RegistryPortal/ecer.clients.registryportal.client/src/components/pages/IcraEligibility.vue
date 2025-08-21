<template>
  <v-container>
    <div>
      <Breadcrumb />
      <h1 class="pb-9">Apply with international certification</h1>

      <ECEHeader title="Is this pathway right for you?" />
      <p>
        If you are certified as an Early Childhood Educator (ECE) in another country, you may be eligible to apply for ECE Five Year certification in British
        Columbia using professional experience you have outside of Canada.
      </p>
      <p class="mt-5 mb-8">
        <b>Note:</b>
        This application is only for
        <b>ECE Five Year certification.</b>
        It does not apply to ECE Assistant of ECE One Year certification.
      </p>
      <ECEHeader title="The eligibility process" />
      <p class="mt-3">1. Submit your required documentation</p>
      <p class="mt-1">
        2. We will review your submission and determine if you are eligible to apply for ECE Five Year certification using professional experience you have
        outside of Canada
      </p>
      <p class="mt-1">
        3. If you are eligible, you may continue with your ECE Five Year application. If you are not eligible, we will contact you with other application
        options
      </p>

      <Callout class="mt-10" type="warning" title="Do you have 500 hours of work experience supervised by a Canadian-certified ECE?">
        If yes, you do not need to confirm your eligibility. Please
        <router-link class="mt-4" to="/">apply for certification directly here.</router-link>
      </Callout>

      <h3 class="mt-13">When you are ready, you can begin your submission.</h3>

      <v-btn class="mt-10" id="backToHome" size="large" color="primary" @click="handleBackToHome">
        <v-icon size="small" icon="mdi-arrow-left" class="mr-2"></v-icon>
        Back to home
      </v-btn>
    </div>
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import type { Components } from "@/types/openapi";
import Breadcrumb from "@/components/Breadcrumb.vue";
import ECEHeader from "@/components/ECEHeader.vue";
import Callout from "../Callout.vue";

export default defineComponent({
  name: "IcraEligibility",
  components: {
    Breadcrumb,
    ECEHeader,
    Callout,
  },
  methods: {
    handleBackToHome() {
      this.$router.push("/");
    },
  },
});
</script>
