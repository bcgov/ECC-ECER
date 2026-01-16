<template>
  <v-container>
    <Breadcrumb />
    <h1>My certifications</h1>
    <div class="d-flex flex-column ga-3 my-6">
      <p>Review your certification records. You can also:</p>
      <ul class="ml-10">
        <li>Download a PDF of your active certificate(s)</li>
        <li>Renew your latest certificate</li>
        <li>
          Renew another certificate type (e.g., if you do not meet the
          requirements for higher certification levels)
        </li>
      </ul>
    </div>
    <CertificationList
      :certifications="certificationStore.certifications ?? []"
    />
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
});
</script>
