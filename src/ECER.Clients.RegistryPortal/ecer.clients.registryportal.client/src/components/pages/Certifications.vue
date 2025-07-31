<template>
  <v-container>
    <Breadcrumb />
    <h1>My other certifications</h1>
    <div class="d-flex flex-column ga-3 my-6">
      <p>You may wish to renew another certification you hold if you do not meet the requirements for higher certification levels. This may be due to:</p>
      <ul class="ml-10">
        <li>Inability to complete work experience and/or professional development hours within the term of your current certificate</li>
        <li>Working outside of the ECE field due to personal or extenuating circumstances</li>
        <li>Missing a basic and/or specialized ECE educational training program</li>
      </ul>
    </div>
    <CertificationList :certifications="getOtherCertifications()" />
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import Breadcrumb from "@/components/Breadcrumb.vue";
import CertificationList from "@/components/CertificationList.vue";
import Alert from "@/components/Alert.vue";
import { useCertificationStore } from "@/store/certification";

export default defineComponent({
  name: "Certifications",
  components: {
    Breadcrumb,
    CertificationList,
    Alert,
  },
  setup() {
    const certificationStore = useCertificationStore();
    return {
      certificationStore,
    };
  },
  methods: {
    getOtherCertifications() {
      // Get all certifications except the current one
      if (!this.certificationStore.certifications || this.certificationStore.certifications.length <= 1) {
        return [];
      }
      const currentCertification = this.certificationStore.currentCertification;
      if (!currentCertification) return [];
      return this.certificationStore.certifications.filter((cert) => cert.id !== currentCertification.id);
    },
  },
});
</script>
