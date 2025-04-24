<template>
  <v-row>
    <v-col md="10" lg="8" xl="6">
      <p>Make sure you choose a person that:</p>
      <br />
      <ul class="ml-10">
        <li>Can speak to your character</li>
        <li>Can speak to your ability to educate and care for young children</li>
        <li>Has known you for at least 6 months</li>
        <li>Is not your relative, partner, spouse, or yourself</li>
      </ul>
      <br />
      <p>We recommend the person is a certified ECE who has directly observed you working with young children.</p>
      <br />
      <p v-if="hasWorkExperienceReferenceStep">
        The person
        <strong>cannot</strong>
        be any of your work experience references.
      </p>
    </v-col>
  </v-row>
  <v-row>
    <v-col v-if="hasDuplicateReferences" md="10" lg="8" xl="6">
      <Alert type="error">
        <p class="small">Your character reference cannot be the same as your work experience reference(s)</p>
      </Alert>
    </v-col>
  </v-row>
  <v-row>
    <v-col cols="12" md="8" lg="6" xl="4">
      <EceTextField
        id="txtReferenceLastName"
        v-model="lastName"
        :rules="[Rules.required('Enter your reference\'s last name'), Rules.validContactName()]"
        label="Last name"
        maxlength="100"
        @update:model-value="updateCharacterReference()"
        @keypress="validContactNameCharacter"
      ></EceTextField>
    </v-col>
  </v-row>
  <v-row>
    <v-col cols="12" md="8" lg="6" xl="4">
      <EceTextField
        id="txtReferenceFirstName"
        v-model="firstName"
        :rules="[Rules.validContactName()]"
        label="First name"
        maxlength="100"
        @update:model-value="updateCharacterReference()"
        @keypress="validContactNameCharacter"
      ></EceTextField>
    </v-col>
  </v-row>
  <v-row>
    <v-col cols="12" md="8" lg="6" xl="4">
      <EceTextField
        v-model="emailAddress"
        :rules="[
          Rules.required('Enter your reference\'s email in the format \'name@email.com\''),
          Rules.email('Enter your reference\'s email in the format \'name@email.com\''),
        ]"
        label="Email"
        id="txtReferenceEmail"
        maxlength="100"
        @update:model-value="updateCharacterReference()"
      ></EceTextField>
    </v-col>
  </v-row>
  <v-row>
    <v-col cols="12" md="8" lg="6" xl="4">
      <EceTextField
        id="txtReferencePhoneNumber"
        v-model="phoneNumber"
        :rules="[Rules.phoneNumber('Enter your reference\'s valid phone number')]"
        label="Phone number (optional)"
        @update:model-value="updateCharacterReference()"
      ></EceTextField>
    </v-col>
  </v-row>
  <!-- this prevents form from proceeding if there are duplicates -->
  <v-input auto-hide="auto" :model-value="modelValue" :rules="[!hasDuplicateReferences]"></v-input>
  <p>After you submit your application, we'll send an email to this person. The email will have a link to an online form to provide a reference for you.</p>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import EceTextField from "@/components/inputs/EceTextField.vue";
import { useAlertStore } from "@/store/alert";
import { useApplicationStore } from "@/store/application";
import { useWizardStore } from "@/store/wizard";
import type { Components } from "@/types/openapi";
import { isNumber } from "@/utils/formInput";
import { validContactNameCharacter } from "@/utils/formInput";
import * as Rules from "@/utils/formRules";

import Alert from "../Alert.vue";
export default defineComponent({
  name: "EceCharacterReference",
  components: { Alert, EceTextField },
  props: {
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
    const applicationStore = useApplicationStore();
    const wizardStore = useWizardStore();
    const alertStore = useAlertStore();
    return {
      applicationStore,
      alertStore,
      wizardStore,
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
  computed: {
    hasDuplicateReferences() {
      if (!this.wizardStore.wizardData.referenceList || !this.wizardStore.wizardData.characterReferences) {
        //flow doesn't have reference list of character references return false
        return false;
      }
      if (Object.values(this.wizardStore.wizardData.referenceList).length === 0 || this.wizardStore.wizardData.characterReferences.length === 0) {
        return false;
      }

      const refSet = new Set<string>();

      for (const ref of this.wizardStore.wizardData.characterReferences) {
        refSet.add(`${ref.firstName} ${ref.lastName}`);
      }

      for (const ref of Object.values(this.wizardStore.wizardData.referenceList) as [Components.Schemas.WorkExperienceReference]) {
        if (refSet.has(`${ref.firstName} ${ref.lastName}`)) {
          return true;
        }
      }

      return false;
    },
    hasWorkExperienceReferenceStep() {
      this.wizardStore.steps.some((step) => {
        if (step.stage === "WorkReferences") {
          return true;
        }
      });
      return false;
    },
  },
  methods: {
    isNumber,
    async updateCharacterReference() {
      this.$emit("update:model-value", [
        { firstName: this.firstName, lastName: this.lastName, emailAddress: this.emailAddress, phoneNumber: this.phoneNumber },
      ] as Components.Schemas.CharacterReference);
    },
    validContactNameCharacter,
  },
});
</script>
