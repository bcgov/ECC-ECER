<template>
  <v-col v-for="(flow, index) in registrantFlows" :key="index" cols="12" sm="6" lg="4">
    <ActionCard :title="flow.title">
      <template #content>
        <div class="d-flex flex-column ga-3">
          {{ flow.text }}
        </div>
      </template>
      <template #action>
        <v-btn variant="text" @click="handleLearnMore(flow)">
          <a href="#" @click.prevent>Learn more</a>
        </v-btn>
      </template>
    </ActionCard>
  </v-col>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import ActionCard from "@/components/ActionCard.vue";
import { useApplicationStore } from "@/store/application";
import { useCertificationStore } from "@/store/certification";
import type { Components } from "@/types/openapi";
import { useRouter } from "vue-router";

interface RegistrantFlow {
  types: Components.Schemas.CertificationType[];
  title: string;
  text: string;
}

const assistantRegistrantFlow: RegistrantFlow = {
  types: ["EceAssistant"],
  title: "Apply for ECE Assistant certification",
  text: "You'll be able to work alongside other ECE's in a licensed child care program for children birth to 5 years of age.",
};

const oneYearRegistrantFlow: RegistrantFlow = {
  types: ["OneYear"],
  title: "Apply for ECE One Year certification",
  text: "Work alone or be the primary educator. You'll need a basic early childhood education program.",
};

const fiveYearRegistrantFlow: RegistrantFlow = {
  types: ["FiveYears"],
  title: "Apply for ECE Five Year certification",
  text: "Work alone or be the primary educator. You'll need a basic early childhood education program. And have 500 hours of supervised work experience.",
};

const sneRegistrantFlow: RegistrantFlow = {
  types: ["Sne"],
  title: "Apply for Special Needs Educator (SNE) certification",
  text: "If you've completed an SNE program you can add this to your certificate. It will also renew your ECE Five Year certificate.",
};

const iteRegistrantFlow: RegistrantFlow = {
  types: ["Ite"],
  title: "Apply for Infant and Toddler Educator (ITE) certification",
  text: "If you've completed an ITE program you can add this certification to your certificate. This will also renew your ECE Five Year certificate.",
};

const specializationRegistrantFlow: RegistrantFlow = {
  types: [],
  title: "Add your specialized certification",
  text: "If you've completed additional training, you can apply to add an Infant and Toddler Educator (ITE) or Special Needs Educator (SNE) to your certificate. This will also renew your ECE Five Year certificate.",
};

export default defineComponent({
  name: "RegistrantCard",
  components: {
    ActionCard,
  },
  setup() {
    const certificationStore = useCertificationStore();
    const applicationStore = useApplicationStore();
    const router = useRouter();

    return {
      certificationStore,
      applicationStore,
      router,
    };
  },
  computed: {
    registrantFlows(): RegistrantFlow[] {
      let types = [];
      if (this.certificationStore.latestIsEceAssistant) {
        types.push(oneYearRegistrantFlow, fiveYearRegistrantFlow);
      }
      if (this.certificationStore.latestIsEceOneYear) {
        if (this.certificationStore.latestCertificateStatus === "Expired") types.push(assistantRegistrantFlow);
        types.push(fiveYearRegistrantFlow);
      }
      if (this.certificationStore.latestIsEceFiveYear) {
        if (this.certificationStore.latestHasSNE && !this.certificationStore.latestHasITE) {
          if (this.certificationStore.latestCertificateStatus === "Expired") types.push(assistantRegistrantFlow);
          else types.push(iteRegistrantFlow);
        } else if (this.certificationStore.latestHasITE && !this.certificationStore.latestHasSNE) {
          if (this.certificationStore.latestCertificateStatus === "Expired") types.push(assistantRegistrantFlow);
          else types.push(sneRegistrantFlow);
        } else if (!this.certificationStore.latestHasITE && !this.certificationStore.latestHasSNE) {
          if (this.certificationStore.latestCertificateStatus === "Expired") types.push(assistantRegistrantFlow);
          else types.push(specializationRegistrantFlow);
        }
      }
      return types;
    },
  },
  methods: {
    handleLearnMore(flow: RegistrantFlow) {
      this.applicationStore.$patch({ draftApplication: { applicationType: "New", certificationTypes: flow.types } });

      this.router.push({ name: "application-requirements" });
    },
  },
});
</script>
