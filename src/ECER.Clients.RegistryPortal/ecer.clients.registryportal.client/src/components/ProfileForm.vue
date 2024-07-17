<template>
  <v-col cols="12">
    <EceForm ref="profileForm" :form="profileInformationForm" :form-data="formStore.formData" @updated-form-data="formStore.setFormData" />
    <v-row justify="end">
      <v-btn rounded="lg" color="primary" @click="saveProfile">Save</v-btn>
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
import { useOidcStore } from "@/store/oidc";
import { useUserStore } from "@/store/user";
import { AddressType } from "@/utils/constant";

export default defineComponent({
  name: "ProfileForm",
  components: { EceForm },
  setup: async () => {
    const formStore = useFormStore();
    const userStore = useUserStore();
    const alertStore = useAlertStore();
    const oidcStore = useOidcStore();

    const oidcUserInfo = await oidcStore.oidcUserInfo();
    const oidcAddress = await oidcStore.oidcAddress();

    const userProfile = await getProfile();
    if (userProfile !== null) {
      formStore.initializeForm({
        [profileInformationForm.inputs.legalFirstName.id]: userProfile.firstName,
        [profileInformationForm.inputs.legalLastName.id]: userProfile.lastName,
        [profileInformationForm.inputs.dateOfBirth.id]: userProfile.dateOfBirth,
        [profileInformationForm.inputs.addresses.id]: {
          [AddressType.RESIDENTIAL]: userProfile.residentialAddress || oidcAddress,
          [AddressType.MAILING]: userProfile.mailingAddress || oidcAddress,
        },
        [profileInformationForm.inputs.email.id]: userProfile.email,
        [profileInformationForm.inputs.legalMiddleName.id]: userProfile.middleName,
        [profileInformationForm.inputs.preferredName.id]: userProfile.preferredName,
        [profileInformationForm.inputs.alternateContactNumber.id]: userProfile.alternateContactPhone,
        [profileInformationForm.inputs.primaryContactNumber.id]: userProfile.phone,
      });
    } else {
      formStore.initializeForm({
        [profileInformationForm.inputs.legalFirstName.id]: oidcUserInfo.firstName,
        [profileInformationForm.inputs.legalLastName.id]: oidcUserInfo.lastName,
        [profileInformationForm.inputs.dateOfBirth.id]: oidcUserInfo.dateOfBirth,
        [profileInformationForm.inputs.addresses.id]: { [AddressType.RESIDENTIAL]: oidcAddress, [AddressType.MAILING]: oidcAddress },
        [profileInformationForm.inputs.email.id]: oidcUserInfo.email,
        [profileInformationForm.inputs.primaryContactNumber.id]: oidcUserInfo.phone,
      });
    }

    return { profileInformationForm, formStore, alertStore, userStore };
  },
  methods: {
    async saveProfile() {
      const { valid } = await (this.$refs.profileForm as typeof EceForm).$refs[profileInformationForm.id].validate();

      if (!valid) {
        this.alertStore.setFailureAlert("You must enter all required fields in the valid format.");
      } else {
        const { error } = await putProfile({
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

        if (!error) {
          this.alertStore.setSuccessAlert("You have successfully edited your profile information.");
          this.userStore.setUserInfo({
            firstName: this.formStore.formData[profileInformationForm.inputs.legalFirstName.id],
            lastName: this.formStore.formData[profileInformationForm.inputs.legalLastName.id],
            email: this.formStore.formData[profileInformationForm.inputs.email.id],
            phone: this.formStore.formData[profileInformationForm.inputs.primaryContactNumber.id],
            dateOfBirth: this.formStore.formData[profileInformationForm.inputs.dateOfBirth.id],
          });

          this.userStore.setUserProfile({
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
        } else {
          this.alertStore.setFailureAlert("Profile save failed");
        }
      }
    },
  },
});
</script>
