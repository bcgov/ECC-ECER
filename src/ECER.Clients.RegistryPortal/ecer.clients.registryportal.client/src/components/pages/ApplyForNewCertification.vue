<template>
  <v-container>
    <Breadcrumb :items="items" />
    <h1 class="mb-3">What type of certification do you want to apply for?</h1>
    <p class="mb-6">Start an application for a new certification.</p>
    <PureApplicationCardList
      @apply-now="handleApplyNow"
      :has-ece-assistant-certification="certificationStore.holdsEceAssistantCertification"
      :has-ece-one-year-certification="certificationStore.holdsEceOneYearCertification"
      :has-ece-five-year-certification="certificationStore.holdsEceFiveYearCertification"
    />
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import Breadcrumb from "@/components/Breadcrumb.vue";

import { useCertificationStore } from "@/store/certification";
import PureApplicationCardList from "@/components/PureApplicationCardList.vue";
import { useRouter } from "vue-router";
import type { Components } from "@/types/openapi";
import { useApplicationStore } from "@/store/application";

export default defineComponent({
  name: "CertificationType",
  components: {
    Breadcrumb,
    PureApplicationCardList,
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
    handleApplyNow(type: Components.Schemas.CertificationType) {
      this.applicationStore.$patch({ draftApplication: { certificationTypes: [type] } });

      if (type !== "FiveYears") {
        this.applicationStore.$patch({ draftApplication: { workExperienceReferences: [] } });
      }
      this.router.push({ name: "application-requirements" });
    },
  },
});
</script>
