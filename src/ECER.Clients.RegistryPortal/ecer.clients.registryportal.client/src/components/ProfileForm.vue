<template>
  <v-col cols="12">
    <EceForm
      :form="profileInformationForm"
      :form-data="formStore.formData"
      @updated-form-data="formStore.setFormData"
      @updated-validation="isFormValid = $event"
    />
    <v-row justify="end">
      <v-btn :form="profileInformationForm.id" type="submit" rounded="lg" color="primary" @click="saveProfile">Save</v-btn>
    </v-row>
  </v-col>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import { getProfile, putProfile } from "@/api/profile";
import EceForm from "@/components/Form.vue";
import profileInformationForm from "@/config/profile-information-form";
import { useAlertStore } from "@/store/alert";
import { useFormStore } from "@/store/form";
import { useUserStore } from "@/store/user";

import { AddressType } from "./inputs/EceAddresses.vue";

export default defineComponent({
  name: "ProfileForm",
  components: { EceForm },
  setup: async () => {
    const formStore = useFormStore();
    const userStore = useUserStore();
    const alertStore = useAlertStore();

    const userProfile = await getProfile();
    if (userProfile !== null) {
      formStore.initializeForm({
        [profileInformationForm.inputs.legalFirstName.id]: userProfile.firstName,
        [profileInformationForm.inputs.legalLastName.id]: userProfile.lastName,
        [profileInformationForm.inputs.dateOfBirth.id]: userProfile.dateOfBirth,
        [profileInformationForm.inputs.addresses.id]: {
          [AddressType.RESIDENTIAL]: userProfile.residentialAddress || userStore.oidcAddress,
          [AddressType.MAILING]: userProfile.mailingAddress || userStore.oidcAddress,
        },
        [profileInformationForm.inputs.email.id]: userProfile.email,
        [profileInformationForm.inputs.legalMiddleName.id]: userProfile.middleName,
        [profileInformationForm.inputs.preferredName.id]: userProfile.preferredName,
        [profileInformationForm.inputs.alternateContactNumber.id]: userProfile.alternateContactPhone,
        [profileInformationForm.inputs.primaryContactNumber.id]: userProfile.phone,
      });
    } else {
      formStore.initializeForm({
        [profileInformationForm.inputs.legalFirstName.id]: userStore.oidcUserInfo.firstName,
        [profileInformationForm.inputs.legalLastName.id]: userStore.oidcUserInfo.lastName,
        [profileInformationForm.inputs.dateOfBirth.id]: userStore.oidcUserInfo.dateOfBirth,
        [profileInformationForm.inputs.addresses.id]: { [AddressType.RESIDENTIAL]: userStore.oidcAddress, [AddressType.MAILING]: userStore.oidcAddress },
        [profileInformationForm.inputs.email.id]: userStore.oidcUserInfo.email,
        [profileInformationForm.inputs.primaryContactNumber.id]: userStore.oidcUserInfo.phone,
      });
    }

    return { profileInformationForm, formStore, alertStore, userStore };
  },
  data: () => ({
    isFormValid: null as boolean | null,
  }),
  methods: {
    async saveProfile() {
      if (!this.isFormValid) {
        this.alertStore.setFailureAlert("You must enter all required fields in the valid format.");
      } else {
        const success = await putProfile({
          firstName: this.formStore.formData[profileInformationForm.inputs.legalFirstName.id],
          middleName: this.formStore.formData[profileInformationForm.inputs.legalMiddleName.id],
          preferredName: this.formStore.formData[profileInformationForm.inputs.preferredName.id],
          lastName: this.formStore.formData[profileInformationForm.inputs.legalLastName.id],
          dateOfBirth: this.formStore.formData[profileInformationForm.inputs.dateOfBirth.id],
          residentialAddress: this.formStore.formData[profileInformationForm.inputs.addresses.id][AddressType.RESIDENTIAL],
          mailingAddress: this.formStore.formData[profileInformationForm.inputs.addresses.id][AddressType.MAILING],
          email: this.formStore.formData[profileInformationForm.inputs.email.id],
          phone: this.formStore.formData[profileInformationForm.inputs.primaryContactNumber.id],
          alternateContactPhone: this.formStore.formData[profileInformationForm.inputs.alternateContactNumber.id],
        });

        if (success) {
          this.alertStore.setSuccessAlert("Profile saved successfully");
          this.userStore.setUserInfo({
            firstName: this.formStore.formData[profileInformationForm.inputs.legalFirstName.id],
            lastName: this.formStore.formData[profileInformationForm.inputs.legalLastName.id],
            email: this.formStore.formData[profileInformationForm.inputs.email.id],
            phone: this.formStore.formData[profileInformationForm.inputs.primaryContactNumber.id],
            dateOfBirth: this.formStore.formData[profileInformationForm.inputs.dateOfBirth.id],
          });
        } else {
          this.alertStore.setFailureAlert("Profile save failed");
        }
      }
    },
  },
});
</script>
