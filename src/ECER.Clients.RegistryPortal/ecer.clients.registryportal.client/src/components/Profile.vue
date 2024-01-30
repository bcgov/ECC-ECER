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
import { defineComponent, onMounted } from "vue";

import { getProfile, putProfile } from "@/api/profile";
import EceForm from "@/components/Form.vue";
import profileInformationForm from "@/config/profile-information-form";
import { useFormStore } from "@/store/form";
import { useUserStore } from "@/store/user";

import { AddressType } from "./inputs/EceAddresses.vue";

export default defineComponent({
  name: "Profile",
  components: { EceForm },
  setup: () => {
    const formStore = useFormStore();
    const userStore = useUserStore();

    formStore.initializeForm({
      [profileInformationForm.inputs.legalFirstName.id]: userStore.oidcUserInfo.firstName,
      [profileInformationForm.inputs.legalLastName.id]: userStore.oidcUserInfo.lastName,
      [profileInformationForm.inputs.dateOfBirth.id]: userStore.oidcUserInfo.dateOfBirth,
      [profileInformationForm.inputs.addresses.id]: { [AddressType.RESIDENTIAL]: userStore.oidcAddress, [AddressType.MAILING]: userStore.oidcAddress },
      [profileInformationForm.inputs.email.id]: userStore.oidcUserInfo.email,
      [profileInformationForm.inputs.primaryContactNumber.id]: userStore.oidcUserInfo.phone,
    });

    onMounted(async () => {
      const userProfile = await getProfile();
      if (userProfile !== null) {
        formStore.initializeForm({
          [profileInformationForm.inputs.legalFirstName.id]: userProfile.firstName,
          [profileInformationForm.inputs.legalLastName.id]: userProfile.lastName,
          [profileInformationForm.inputs.dateOfBirth.id]: "1972-04-13",
          [profileInformationForm.inputs.addresses.id]: {
            [AddressType.RESIDENTIAL]: userProfile.residentialAddress || userStore.oidcAddress,
            [AddressType.MAILING]: userProfile.mailingAddress || userStore.oidcAddress,
          },
          [profileInformationForm.inputs.email.id]: userProfile.email,
          [profileInformationForm.inputs.primaryContactNumber.id]: userProfile.phone,
        });
      }
    });

    return { profileInformationForm, formStore };
  },
  data: () => ({
    isFormValid: null as boolean | null,
  }),
  methods: {
    async saveProfile() {
      if (this.isFormValid) {
        const success = await putProfile(this.formStore.formData);
        if (success) {
          alert("Profile saved successfully");
        } else {
          alert("Profile save failed");
        }
      } else {
        alert("Please fill out all required fields");
      }
    },
  },
});
</script>
