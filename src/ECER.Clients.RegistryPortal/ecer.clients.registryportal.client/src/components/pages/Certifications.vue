<template>
  <v-container>
    <Breadcrumb :items="items" />
    <h1>My other certifications</h1>
    <div class="d-flex flex-column ga-3 my-6">
      <p>You may wish to renew another certification you hold if you do not meet the requirements for higher certification levels. This may be due to:</p>
      <ul class="ml-10">
        <li>Inability to complete work experience and/or professional development hours within the term of your current certificate</li>
        <li>Working outside of the ECE field due to personal or extenuating circumstances</li>
        <li>Missing a basic and/or specialized ECE educational training program</li>
      </ul>
    </div>
    <CertificationList :certifications="certificationStore.otherCertifications" />
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
  data() {
    return {
      items: [
        {
          title: "Home",
          disabled: false,
          href: "/",
        },
        {
          title: "My other certifications",
          disabled: true,
          href: "/my-other-certifications",
        },
      ],
    };
  },
});
</script>
