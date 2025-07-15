<template>
  <v-container>
    <Breadcrumb :items="items" />
    <h1 class="mb-3">What type of certification do you want to apply for?</h1>
    <p class="mb-6">Start an application for a new certification.</p>
    <ApplicationCardList @apply-now="handleApplyNow" :certifications="certificationStore.certifications || []" />
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import Breadcrumb from "@/components/Breadcrumb.vue";

import { useCertificationStore } from "@/store/certification";
import ApplicationCardList from "@/components/ApplicationCardList.vue";
import { useRouter } from "vue-router";
import type { Components } from "@/types/openapi";
import { useApplicationStore } from "@/store/application";

export default defineComponent({
  name: "CertificationType",
  components: {
    Breadcrumb,
    ApplicationCardList,
  },

  setup: () => {
    const certificationStore = useCertificationStore();
    const applicationStore = useApplicationStore();
    const router = useRouter();

    return { certificationStore, router, applicationStore };
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
          title: "Apply for new certification",
          disabled: true,
          href: "/application/certification",
        },
      ],
    };
  },
  methods: {
    handleApplyNow(type: Components.Schemas.CertificationType[]) {
      this.applicationStore.$patch({ draftApplication: { certificationTypes: [...type] } });

      // Registrant pathways, associate the most recent 5 year certificate
      if (type.includes("Ite") || type.includes("Sne") || type.length === 0) {
        if (this.certificationStore.activeEceFiveYearCertification) {
          this.applicationStore.$patch({ draftApplication: { fromCertificate: this.certificationStore.activeEceFiveYearCertification.id } });
        }
      }

      if (!type.includes("FiveYears")) {
        this.applicationStore.$patch({ draftApplication: { workExperienceReferences: [] } });
      }
      this.router.push({ name: "application-requirements" });
    },
  },
});
</script>
