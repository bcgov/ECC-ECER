<template>
  <v-row>
    <v-col>
      <Alert type="info">
        <p>The Registry will not assess an application until references have been submitted. Please make sure your reference:</p>
        <ul class="pl-5">
          <li>Can speak to your character and has known you for at least 6 months.</li>
          <li>Can speak to your ability to educate and care for young children.</li>
          <li>Is NOT a relative, partner, spouse or myself.</li>
          <li>(Recommended) Is a certified ECE who has directly observed you working with young children.</li>
        </ul>
      </Alert>
    </v-col>
  </v-row>
  <v-row>
    <v-col cols="12" md="8" lg="6" xl="4">
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
    <v-col cols="12" md="8" lg="6" xl="4">
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
    <v-col cols="12" md="8" lg="6" xl="4">
      <v-text-field
        v-model="phoneNumber"
        :rules="[Rules.phoneNumber()]"
        label="Phone Number (Optional)"
        variant="outlined"
        color="primary"
        type="number"
        maxlength="100"
        @update:model-value="updateCharacterReference()"
      ></v-text-field>
    </v-col>
  </v-row>
  <v-row>
    <v-col cols="12" md="8" lg="6" xl="4">
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

import Alert from "../Alert.vue";
export default defineComponent({
  name: "EceCharacterReference",
  components: { Alert },
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
