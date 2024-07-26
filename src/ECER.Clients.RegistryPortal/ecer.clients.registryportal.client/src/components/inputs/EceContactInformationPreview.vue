<template>
  <PreviewCard title="Contact information" portal-stage="ContactInformation">
    <template #content>
      <v-row>
        <v-col cols="4">
          <p class="small">Legal last name</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">{{ legalLastName }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Legal first name</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">{{ legalFirstName }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Previous name</p>
        </v-col>
        <v-col>
          <p class="small font>weight-bold">{{ previousName }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Legal middle name</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">{{ legalMiddleName }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Preferred first name</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">{{ preferredName }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Date of birth</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">{{ dateOfBirth }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Home address</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">{{ residentialAddress }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Mailing address</p>
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
          <p class="small">Primary phone</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">{{ primaryPhoneNumber }}</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <p class="small">Alternate phone</p>
        </v-col>
        <v-col>
          <p class="small font-weight-bold">{{ alternatePhoneNumber }}</p>
        </v-col>
      </v-row>
    </template>
  </PreviewCard>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import PreviewCard from "@/components/PreviewCard.vue";
import { useWizardStore } from "@/store/wizard";
import { formatDate } from "@/utils/format";
import type { EcePreviewProps } from "@/types/input";
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
    return {
      wizardStore,
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
      return formatDate(this.wizardStore.wizardData[this.wizardStore.wizardConfig.steps.profile.form.inputs.dateOfBirth.id], "LLLL d, yyyy") ?? "—";
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
});
</script>
