<template>
  <ReferencePreviewCard :is-valid="true" title="Contact information" reference-stage="ContactInformation">
    <template #content>
      <v-row>
        <v-col cols="4">
          <p class="small">Last Name</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">{{ reference.lastName }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">First Name</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">{{ reference.firstName }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Email</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">{{ reference.email }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Phone Number</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">{{ reference.phoneNumber }}</p>
        </v-col>
      </v-row>
      <v-row v-if="reference.certificateNumber">
        <v-col cols="4">
          <p class="small">ECE Certification/Registration Number</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">{{ reference.certificateNumber }}</p>
        </v-col>
      </v-row>
      <v-row v-if="reference.certificateProvinceId">
        <v-col cols="4">
          <p class="small">Province/Territory Certified/Registered In</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">{{ reference.certificateProvinceId }}</p>
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
import type { EcePreviewProps } from "@/types/input";
import type { Components } from "@/types/openapi";

export default defineComponent({
  name: "EceReferenceContactPreview",
  components: {
    ReferencePreviewCard,
  },
  props: {
    props: {
      type: Object as () => EcePreviewProps,
      required: true,
    },
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
      const provinceName = this.configStore.provinceName(
        this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.contactInformation.form.inputs.referenceContactInformation.id]?.certificateProvinceId,
      );
      return {
        firstName: this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.contactInformation.form.inputs.referenceContactInformation.id]?.firstName,
        lastName: this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.contactInformation.form.inputs.referenceContactInformation.id]?.lastName,
        email: this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.contactInformation.form.inputs.referenceContactInformation.id]?.email,
        phoneNumber:
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.contactInformation.form.inputs.referenceContactInformation.id]?.phoneNumber,
        certificateProvinceId: provinceName,
        certificateProvinceOther:
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.contactInformation.form.inputs.referenceContactInformation.id]
            ?.certificateProvinceOther,
        certificateNumber:
          this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.contactInformation.form.inputs.referenceContactInformation.id]?.certificateNumber,
      };
    },
  },
});
</script>
