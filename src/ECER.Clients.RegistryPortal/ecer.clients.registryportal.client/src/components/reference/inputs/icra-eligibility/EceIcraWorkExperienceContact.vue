<template>
  <v-row>
    <v-col cols="12" md="12" lg="12" xl="12">
      <h2>Contact information</h2>
      <div role="doc-subtitle">We may contact you to verify or clarify information you provide.</div>
      <v-row class="mt-5">
        <v-col cols="12" md="8" lg="6" xl="4">
          <EceTextField
            id="lastNameTextInput"
            :model-value="modelValue.lastName"
            :rules="[Rules.required('Enter your last name')]"
            label="Last Name"
            autocomplete="family-name"
            maxlength="100"
            @input="updateField('lastName', $event)"
          />
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <EceTextField
            id="firstNameTextInput"
            :model-value="modelValue.firstName"
            label="First Name"
            autocomplete="given-name"
            maxlength="100"
            @input="updateField('firstName', $event)"
          />
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <EceTextField
            id="emailTextInput"
            :model-value="modelValue.email"
            :rules="[Rules.required(), Rules.email('Enter your email in the format \'name@email.com\'')]"
            label="Email"
            autocomplete="email"
            maxlength="200"
            @input="updateField('email', $event)"
          />
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" md="8" lg="6" xl="4">
          <EceTextField
            id="phoneNumberTextInput"
            :model-value="modelValue.phoneNumber"
            :rules="[Rules.required('Enter a phone number'), Rules.phoneNumber('Enter your valid phone number')]"
            label="Phone Number"
            autocomplete="tel"
            @input="updateField('phoneNumber', $event)"
          ></EceTextField>
        </v-col>
      </v-row>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import EceTextField from "@/components/inputs/EceTextField.vue";
import type { Components } from "@/types/openapi";
import { isNumber } from "@/utils/formInput";
import * as Rules from "@/utils/formRules";

export default defineComponent({
  name: "EceIcraWorkExperienceContact",
  components: { EceTextField },
  props: {
    modelValue: {
      type: Object as () => Components.Schemas.ReferenceContactInformation,
      required: true,
    },
  },
  emits: {
    "update:model-value": (_contactInformationData: Components.Schemas.ReferenceContactInformation) => true,
  },
  setup() {},
  data() {
    return {
      Rules,
    };
  },
  methods: {
    isNumber,
    updateField(fieldName: keyof Components.Schemas.ReferenceContactInformation, value: any) {
      this.$emit("update:model-value", {
        ...this.modelValue,
        [fieldName]: value,
      });
    },
  },
});
</script>
