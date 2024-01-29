<template>
  <h1>Profile</h1>
  <v-row>
    <v-col cols="12" md="10" lg="6" xl="4">
      <EceForm :form="profileInformationForm" :form-data="formStore.formData" @updated-form-data="formStore.setFormData" />
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import EceForm from "@/components/Form.vue";
import profileInformationForm from "@/config/profile-information-form";
import { useFormStore } from "@/store/form";

export default defineComponent({
  name: "Profile",
  components: { EceForm },
  setup: () => {
    const formStore = useFormStore();

    formStore.initializeForm({
      [profileInformationForm.inputs.legalFirstName.id]: "John",
      [profileInformationForm.inputs.legalLastName.id]: "Doe",
      [profileInformationForm.inputs.dateOfBirth.id]: "1991-01-01",
    });

    return { profileInformationForm, formStore };
  },
});
</script>
