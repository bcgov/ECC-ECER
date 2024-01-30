<template>
  <h1>Profile</h1>
  <v-row class="ma-4">
    <v-col cols="12" md="10" lg="6" xl="4">
      <EceForm
        :form="profileInformationForm"
        :form-data="formStore.formData"
        @updated-form-data="formStore.setFormData"
        @updated-validation="isFormValid = $event"
      />
    </v-col>
    <v-col cols="12">
      <v-row justify="end">
        <v-btn rounded="lg" color="primary" @click="saveProfile">Save</v-btn>
      </v-row>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import EceForm from "@/components/Form.vue";
import profileInformationForm from "@/config/profile-information-form";
import { useFormStore } from "@/store/form";

import type { AddressData } from "./inputs/EceAddress.vue";

export default defineComponent({
  name: "Profile",
  components: { EceForm },
  setup: () => {
    const formStore = useFormStore();

    /*
    TODO:
      - Fetch UserProfile from Server
      - Initialize form with UserProfile data
    */

    formStore.initializeForm({
      [profileInformationForm.inputs.legalFirstName.id]: "John",
      [profileInformationForm.inputs.legalLastName.id]: "Doe",
      [profileInformationForm.inputs.dateOfBirth.id]: "1991-01-01",
      [profileInformationForm.inputs.residentialAddress.id]: {
        line1: "123 Main Street",
        city: "Toronto",
        province: "Ontario",
        postalCode: "M1M 1M1",
        country: "Canada",
      } as AddressData,
      [profileInformationForm.inputs.mailingAddress.id]: {
        line1: "123 Main Street",
        city: "Toronto",
        province: "Ontario",
        postalCode: "M1M 1M1",
        country: "Canada",
      } as AddressData,
    });

    return { profileInformationForm, formStore };
  },
  data: () => ({
    isFormValid: null as boolean | null,
  }),
  methods: {
    saveProfile() {
      /*
      TODO:
        - Validate form (use isFormValid)
        - Save updated profile data
        - Toast user with success/error message
      */
    },
  },
});
</script>
