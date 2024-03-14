<template>
  <v-row>
    <v-col>
      <v-text-field
        v-model="firstName"
        :rules="[Rules.required()]"
        label="First Name"
        variant="outlined"
        color="primary"
        maxlength="100"
        @update:model-value="updateCharacterReference()"
      ></v-text-field>
    </v-col>
  </v-row>
  <v-row>
    <v-col>
      <v-text-field
        v-model="lastName"
        :rules="[Rules.required()]"
        label="Last Name"
        variant="outlined"
        color="primary"
        maxlength="100"
        @update:model-value="updateCharacterReference()"
      ></v-text-field>
    </v-col>
  </v-row>
  <v-row>
    <v-col>
      <v-text-field
        v-model="phoneNumber"
        :rules="[Rules.phoneNumber()]"
        label="Phone Number (Optional)"
        variant="outlined"
        color="primary"
        maxlength="100"
        @update:model-value="updateCharacterReference()"
      ></v-text-field>
    </v-col>
  </v-row>
  <v-row>
    <v-col>
      <v-text-field
        v-model="emailAddress"
        :rules="[Rules.required(), Rules.email()]"
        label="Email"
        variant="outlined"
        color="primary"
        maxlength="100"
        @update:model-value="updateCharacterReference()"
      ></v-text-field>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import { useAlertStore } from "@/store/alert";
import type { EceCharacterReferenceProps } from "@/types/input";
import type { Components } from "@/types/openapi";
import * as Rules from "@/utils/formRules";
export default defineComponent({
  name: "EceCharacterReference",
  //   props: {
  //     props: {
  //       type: Object as () => EceCharacterReferenceProps,
  //       required: true,
  //     },
  //     modelValue: {
  //       type: Object,
  //       required: true,
  //     },
  //   },
  props: {
    props: {
      type: Object as () => EceCharacterReferenceProps,
      required: true,
    },
    modelValue: {
      type: Object as () => Components.Schemas.CharacterReference[],
      required: true,
    },
  },
  emits: {
    "update:model-value": (_characterReferencesData: Components.Schemas.CharacterReference) => true,
    updatedValidation: (_errorState: boolean) => true,
  },
  setup: () => {
    const alertStore = useAlertStore();
    return {
      alertStore,
    };
  },
  data() {
    return {
      firstName: this.modelValue?.[0]?.firstName ? this.modelValue?.[0]?.firstName : "",
      lastName: this.modelValue?.[0]?.lastName ? this.modelValue?.[0]?.lastName : "",
      emailAddress: this.modelValue?.[0]?.emailAddress ? this.modelValue?.[0]?.emailAddress : "",
      phoneNumber: this.modelValue?.[0]?.phoneNumber ? this.modelValue?.[0]?.phoneNumber : "",
      Rules,
    };
  },
  created() {},
  methods: {
    async updateCharacterReference() {
      this.$emit("update:model-value", [
        { firstName: this.firstName, lastName: this.lastName, emailAddress: this.emailAddress, phoneNumber: this.phoneNumber },
      ] as Components.Schemas.CharacterReference);
    },
  },
});
</script>
