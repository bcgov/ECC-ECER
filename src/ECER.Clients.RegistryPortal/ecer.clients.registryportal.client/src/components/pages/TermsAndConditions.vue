<template>
  <v-container>
    <Breadcrumb :items="items" />
    <h1>Certificate terms and conditions</h1>
    <h2><certification-title :certification="certification" /></h2>
    <certification-chip :certification="certification" />
    <h3>{{}}</h3>

    <div class="d-flex flex-column ga-3 my-6">
      <ul class="ml-10">
        <li>Inability to complete work experience and/or professional development hours within the term of your current certificate</li>
        <li>Working outside of the ECE field due to personal or extenuating circumstances</li>
        <li>Missing a basic and/or specialized ECE educational training program</li>
      </ul>
    </div>

    <v-btn id="backToHome" size="large" color="primary" @click="handleBackToHome">
      <v-icon size="small" icon="mdi-arrow-left" class="mr-2"></v-icon>
      Back to home
    </v-btn>
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import Breadcrumb from "@/components/Breadcrumb.vue";
import CertificationChip from "@/components/CertificationChip.vue";
import CertificationDates from "@/components/CertificationDates.vue";
import CertificationTitle from "@/components/CertificationTitle.vue";

import { useCertificationStore } from "@/store/certification";

export default defineComponent({
  name: "TermsAndConditions",
  components: {
    Breadcrumb,
    CertificationChip,
    CertificationDates,
    CertificationTitle,
  },
  props: {
    certificationId: {
      type: String,
      required: true,
    },
  },
  setup() {
    const certificationStore = useCertificationStore();
    return {
      certificationStore,
    };
  },
  data() {
    return {
      certification: {},
      items: [
        {
          title: "Home",
          disabled: false,
          href: "/",
        },
        {
          title: "Certificate terms and conditions",
          disabled: true,
          href: "/certificate-terms-and-conditions",
        },
      ],
    };
  },
  methods: {
    handleBackToHome() {
      this.$router.push("/");
    },
  },
});
</script>
