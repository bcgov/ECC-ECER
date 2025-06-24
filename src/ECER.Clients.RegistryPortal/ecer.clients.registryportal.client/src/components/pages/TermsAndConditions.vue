<template>
  <v-container>
    <Loading v-if="showLoading" />
    <div v-else>
      <Breadcrumb :items="items" />
      <h1>Certificate terms and conditions</h1>
      <div class="d-flex flex-column ga-4 my-6">
        <h2><certification-title :certification="certification" /></h2>
        <div>
          <certification-chip :certification="certification" />
        </div>
        <h3>{{ certification.name }}</h3>
        <div>
          <certification-dates :certification="certification" :is-bold="false" />
        </div>
      </div>
      <ECEHeader title="Terms and conditions" />

      <ul class="ml-10 d-flex flex-column ga-4 mt-6 mb-16">
        <li v-for="(condition, index) in certification.certificateConditions" :key="index">
          {{ condition.details }}
        </li>
      </ul>

      <v-btn id="backToHome" size="large" color="primary" @click="handleBackToHome">
        <v-icon size="small" icon="mdi-arrow-left" class="mr-2"></v-icon>
        Back to home
      </v-btn>
    </div>
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import type { Components } from "@/types/openapi";

import { getCertificationsById } from "@/api/certification";
import Breadcrumb from "@/components/Breadcrumb.vue";
import CertificationChip from "@/components/CertificationChip.vue";
import CertificationDates from "@/components/CertificationDates.vue";
import CertificationTitle from "@/components/CertificationTitle.vue";
import ECEHeader from "@/components/ECEHeader.vue";
import Loading from "@/components/Loading.vue";
import { useLoadingStore } from "@/store/loading";
import { useAlertStore } from "@/store/alert";

export default defineComponent({
  name: "TermsAndConditions",
  components: {
    Breadcrumb,
    CertificationChip,
    CertificationDates,
    CertificationTitle,
    ECEHeader,
    Loading,
  },
  props: {
    certificationId: {
      type: String,
      required: true,
    },
  },
  setup() {
    const loadingStore = useLoadingStore();
    const alertStore = useAlertStore();
    return {
      loadingStore,
      alertStore,
    };
  },

  data() {
    return {
      certification: {} as Components.Schemas.Certification,
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
  async mounted() {
    await this.loadCertification();
  },
  computed: {
    showLoading(): boolean {
      return this.loadingStore.isLoading("certification_get");
    },
  },
  methods: {
    async loadCertification() {
      try {
        const response = await getCertificationsById(this.certificationId);
        if (response.data && response.data.length > 0) {
          this.certification = response.data[0];
        }
      } catch (error) {
        this.alertStore.setFailureAlert("Could not load certification. Please try again later.");
        this.$router.push("/");
      }
    },
    handleBackToHome() {
      this.$router.push("/");
    },
  },
});
</script>
