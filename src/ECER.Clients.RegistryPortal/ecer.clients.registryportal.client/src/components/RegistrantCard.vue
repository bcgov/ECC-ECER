<template>
  <v-col v-for="(flow, index) in registrantFlows" :key="index" cols="12" sm="6" lg="4">
    <ActionCard :title="flow.title">
      <template #content>
        <div class="d-flex flex-column ga-3">
          {{ flow.text }}
        </div>
      </template>
      <template #action>
        <v-btn variant="text" @click="handleLearnMore">
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

interface RegistrantFlow {
  type: string;
  title: string;
  text: string;
}

const assistantRegistrantFlow: RegistrantFlow = {
  type: "ECE Assistant",
  title: "Apply for ECE Assistant certification",
  text: "You’ll be able to work alongside other ECE's in a licensed child care program for children birth to 5 years of age.",
};

const oneYearRegistrantFlow: RegistrantFlow = {
  type: "ECE One Year",
  title: "Apply for ECE One Year certification",
  text: "Work alone or be the primary educator. You'll need a basic early childhood education program.",
};

const fiveYearRegistrantFlow: RegistrantFlow = {
  type: "ECE Five Year",
  title: "Apply for ECE Five Year certification",
  text: "Work alone or be the primary educator. You'll need a basic early childhood education program. And have 500 hours of supervised work experience.",
};

const sneRegistrantFlow: RegistrantFlow = {
  type: "SNE",
  title: "Apply for Special Needs Educator (SNE) certification",
  text: "If you've completed an SNE program you can add this to your certificate. It will also renew your ECE Five Year certificate.",
};

const iteRegistrantFlow: RegistrantFlow = {
  type: "ITE",
  title: "Apply for Infant and Toddler Educator (ITE) certification",
  text: "If you've completed an ITE program you can add this certification to your certificate. This will also renew your ECE Five Year certificate.",
};

const specializationRegistrantFlow: RegistrantFlow = {
  type: "Specialization",
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

    return {
      certificationStore,
      applicationStore,
    };
  },
  computed: {
    registrantFlows(): RegistrantFlow[] {
      let types = [];
      if (this.certificationStore.latestIsEceAssistant) {
        types.push(oneYearRegistrantFlow, assistantRegistrantFlow);
      }
      if (this.certificationStore.latestIsEceOneYear) {
        types.push(fiveYearRegistrantFlow);
      }
      if (this.certificationStore.latestIsEceFiveYear) {
        if (this.certificationStore.latestHasSNE && !this.certificationStore.latestHasITE) {
          types.push(iteRegistrantFlow);
        } else if (this.certificationStore.latestHasITE && !this.certificationStore.latestHasSNE) {
          types.push(sneRegistrantFlow);
        } else if (!this.certificationStore.latestHasITE && !this.certificationStore.latestHasSNE) types.push(specializationRegistrantFlow);
      }
      return types;
    },
  },
  methods: {
    handleLearnMore() {
      this.$router.push({
        name: "certification-requirements",
        query: { certificationTypes: this.certificationStore.latestCertificationTypes, isRegistant: "true" },
      });
    },
  },
});
</script>
