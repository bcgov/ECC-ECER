<template>
  <v-container>
    <Breadcrumb />
    <h1 class="mb-5">Parent or guardian consent will be required</h1>
    <div class="d-flex flex-column ga-3 my-6">
      <p>
        Applicants under 19 years old need consent from a parent or legal
        guardian to apply for certification.
      </p>
      <p>To provide consent, you will need to:</p>
      <ol class="ml-10">
        <li>Choose the type of certification you want to apply for</li>
        <li>Submit your application form</li>
        <li>
          Get a parent or guardian to sign the consent form we will send you in
          a message (it will include your application details)
        </li>
        <li>Send us a photo or scanned copy of the signed consent form</li>
      </ol>
    </div>
    <v-btn
      class="mt-6"
      rounded="lg"
      color="primary"
      @click="continueClick"
      id="btnContinue"
    >
      Continue
    </v-btn>
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import Breadcrumb from "@/components/Breadcrumb.vue";

import { useRouter } from "vue-router";

export default defineComponent({
  name: "ConsentRequired",
  components: { Breadcrumb },
  async setup() {
    const router = useRouter();

    return { router };
  },

  methods: {
    async continueClick() {
      this.router.push({ name: "application-requirements" });
    },
  },
});
</script>
