<template>
  <ReferencePreviewCard :is-valid="true" title="Your contact information" reference-stage="ContactInformation">
    <template #content>
      <v-row>
        <v-col cols="4">
          <p class="small">Last Name</p>
        </v-col>
        <v-col cols="8">
          <p class="small font-weight-bold">{{ reference.lastName }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">First Name</p>
        </v-col>
        <v-col cols="8">
          <p class="small font-weight-bold">{{ reference.firstName }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Email</p>
        </v-col>
        <v-col cols="8">
          <p class="small font-weight-bold">{{ reference.email }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Phone Number</p>
        </v-col>
        <v-col cols="8">
          <p class="small font-weight-bold">{{ reference.phoneNumber }}</p>
        </v-col>
      </v-row>
    </template>
  </ReferencePreviewCard>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import ReferencePreviewCard from "@/components/reference/inputs/ReferencePreviewCard.vue";
import { useConfigStore } from "@/store/config";
import { useWizardStore } from "@/store/wizard";
import type { Components } from "@/types/openapi";
import { formatDate } from "@/utils/format";

export default defineComponent({
  name: "EceIcraEligibilityWorkExperienceContactPreview",
  components: {
    ReferencePreviewCard,
  },
  setup: () => {
    const wizardStore = useWizardStore();
    const configStore = useConfigStore();
    return {
      wizardStore,
      configStore,
    };
  },
  computed: {
    reference(): Components.Schemas.ReferenceContactInformation {
      return {
        firstName:
          this.wizardStore.wizardData[
            this.wizardStore?.wizardConfig?.steps?.contactInformation?.form?.inputs?.icraEligibilityWorkExperienceContactInformation?.id || ""
          ]?.firstName,
        lastName:
          this.wizardStore.wizardData[
            this.wizardStore?.wizardConfig?.steps?.contactInformation?.form?.inputs?.icraEligibilityWorkExperienceContactInformation?.id || ""
          ]?.lastName,
        email:
          this.wizardStore.wizardData[
            this.wizardStore?.wizardConfig?.steps?.contactInformation?.form?.inputs?.icraEligibilityWorkExperienceContactInformation?.id || ""
          ]?.email,
        phoneNumber:
          this.wizardStore.wizardData[
            this.wizardStore?.wizardConfig?.steps?.contactInformation?.form?.inputs?.icraEligibilityWorkExperienceContactInformation?.id || ""
          ]?.phoneNumber,
      };
    },
  },
  methods: {
    formatDate,
  },
});
</script>
