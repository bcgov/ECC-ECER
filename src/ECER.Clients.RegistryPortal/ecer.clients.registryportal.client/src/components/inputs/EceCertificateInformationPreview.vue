<template>
  <PreviewCard title="Certificate information" portal-stage="CertificateInformation">
    <template #content>
      <v-row>
        <v-col cols="4">
          <p class="small">
            <b>Transfer to: {{ applicationStore.certificateName }}</b>
          </p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Province</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">{{ certificateInformation.labourMobilityProvince?.provinceName }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Certificate type in province</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">{{ certificateInformation.existingCertificationType }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Registration number</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">{{ certificateInformation.currentCertificationNumber || "—" }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Name on certificate</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">{{ nameOnCertificate }}</p>
        </v-col>
      </v-row>
    </template>
  </PreviewCard>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import PreviewCard from "@/components/PreviewCard.vue";
import { useApplicationStore } from "@/store/application";
import { useWizardStore } from "@/store/wizard";
import type { Components } from "@/types/openapi";
export default defineComponent({
  name: "EceCertificateInformationPreview",
  components: {
    PreviewCard,
  },
  setup: () => {
    const applicationStore = useApplicationStore();
    const wizardStore = useWizardStore();

    return {
      applicationStore,
      wizardStore,
    };
  },
  computed: {
    certificateInformation(): Components.Schemas.CertificateInformation {
      return this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.certificateInformation.form.inputs.certificateInformation.id];
    },
    nameOnCertificate(): string {
      const firstName = this.certificateInformation.legalFirstName || "";
      const middleName = this.certificateInformation.legalMiddleName ? ` ${this.certificateInformation.legalMiddleName}` : "";
      const lastName = this.certificateInformation.legalLastName || "";
      return `${firstName}${middleName} ${lastName}`;
    },
  },
});
</script>
