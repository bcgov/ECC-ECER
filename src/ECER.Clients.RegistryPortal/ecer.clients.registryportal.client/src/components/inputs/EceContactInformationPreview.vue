<template>
  <PreviewCard>
    <v-container>
      <v-row align="center">
        <v-col>
          <h3 class="font-black">Contact Information</h3>
        </v-col>
        <v-col align="end">
          <v-btn v-bind="props" icon="mdi-pencil" color="primary" variant="plain" @click="setWizard('ContactInformation')" />
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Legal Last Name</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">{{ legalLastName }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Legal First Name</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">{{ legalFirstName }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Previous Name</p>
        </v-col>
        <v-col>
          <p class="small font>weight-bold">{{ previousName }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Legal Middle Name</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">{{ legalMiddleName }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Preferred Name</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">{{ preferredName }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Date of Birth</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">{{ dateOfBirth }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Residential Address</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">{{ residentialAddress }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Mailing Address</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">{{ mailingAddress }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Email</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">{{ email }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Primary Phone Number</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">{{ primaryPhoneNumber }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Alternate Phone Number</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">{{ alternatePhoneNumber }}</p>
        </v-col>
      </v-row>
    </v-container>
  </PreviewCard>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import type { AddressesData } from "@/components/inputs/EceAddresses.vue";
import PreviewCard from "@/components/PreviewCard.vue";
import { useApplicationStore } from "@/store/application";
import { useWizardStore } from "@/store/wizard";
import type { EcePreviewProps } from "@/types/input";
import type { Components } from "@/types/openapi";
export default defineComponent({
  name: "EceContactInformationPreview",
  components: {
    PreviewCard,
  },
  props: {
    props: {
      type: Object as () => EcePreviewProps,
      required: true,
    },
  },
  setup: () => {
    const wizardStore = useWizardStore();
    const applicationStore = useApplicationStore();
    return {
      wizardStore,
      applicationStore,
    };
  },
  computed: {
    legalLastName() {
      return this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.profile.form.inputs.legalLastName.id] ?? "—";
    },
    legalFirstName() {
      return this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.profile.form.inputs.legalFirstName.id] ?? "—";
    },
    previousName() {
      return "—";
    },
    legalMiddleName() {
      return this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.profile.form.inputs.legalMiddleName.id] ?? "—";
    },
    preferredName() {
      return this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.profile.form.inputs.preferredName.id] ?? "—";
    },
    dateOfBirth() {
      return this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.profile.form.inputs.dateOfBirth.id] ?? "—";
    },
    residentialAddress() {
      const addresses: AddressesData = this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.profile.form.inputs.addresses.id];
      return `${addresses.residential.line1}, ${addresses.residential.city}, ${addresses.residential.province}, ${addresses.residential.postalCode}, ${addresses.residential.country}`;
    },
    mailingAddress() {
      const addresses: AddressesData = this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.profile.form.inputs.addresses.id];
      return `${addresses.mailing.line1}, ${addresses.mailing.city}, ${addresses.mailing.province}, ${addresses.mailing.postalCode}, ${addresses.mailing.country}`;
    },
    email() {
      return this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.profile.form.inputs.email.id] ?? "—";
    },
    primaryPhoneNumber() {
      return this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.profile.form.inputs.primaryContactNumber.id] ?? "—";
    },
    alternatePhoneNumber() {
      return this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.profile.form.inputs.alternateContactNumber.id] ?? "—";
    },
  },
  methods: {
    setWizard(stage: Components.Schemas.PortalStage) {
      this.wizardStore.setCurrentStep(stage);
      this.applicationStore.draftApplication.stage = this.wizardStore.currentStepStage;
    },
  },
});
</script>
