<template>
  <!-- add view -->
  <div v-if="mode === 'add'">
    <v-row v-if="mode === 'add'">
      <v-col>
        <h3>{{ `${mode} course or workshop` }}</h3>
        <br />
        <p>The course or workshop must:</p>
        <br />
        <ul class="ml-10">
          <li>Be relevant to the field of early childhood education</li>
          <li>Have been completed within the term of your current certificate (Between TODO DATES TO BE CALCULATED HERE)</li>
        </ul>
      </v-col>
    </v-row>
    <v-form ref="professionalDevelopmentForm">
      <v-row>
        <v-col>
          <h3>Course or workshop information</h3>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <v-text-field variant="outlined" label="Name of course or workshop" :rules="[Rules.required()]"></v-text-field>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <v-text-field variant="outlined" label="How many hours was it?" :rules="[Rules.required()]" @keypress="isNumber($event)"></v-text-field>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <v-text-field variant="outlined" label="Name of place that hosted the course or workshop" :rules="[Rules.required()]"></v-text-field>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <v-text-field variant="outlined" label="Website with description of course or workshop (optional)" :rules="[Rules.required()]"></v-text-field>
        </v-col>
      </v-row>
      <v-row>
        <v-col>
          <h3>When did you take it?</h3>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="2">
          <v-text-field :rules="[Rules.required()]" label="Start date" type="date" variant="outlined" color="primary"></v-text-field>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="2">
          <v-text-field :rules="[Rules.required()]" label="End date" type="date" variant="outlined" color="primary"></v-text-field>
        </v-col>
      </v-row>
    </v-form>
    <v-row>
      <v-col>
        <v-row justify="start" class="ml-1">
          <v-btn rounded="lg" color="alternate" class="mr-2" @click="handleSubmit">Save Reference</v-btn>
          <v-btn rounded="lg" variant="outlined" @click="handleCancel">Cancel</v-btn>
        </v-row>
      </v-col>
    </v-row>
    <v-row>
      <v-col>
        <h3>Proof of course or workshop</h3>
        <br />
        <p>We may need to verify you took this course. You'll need to provide at least one option below</p>
        <br />
        <p>What can you provide? Choose all that apply</p>
      </v-col>
    </v-row>
    <v-row>
      <v-col>
        <v-checkbox label="Phone number for instructor of course or workshop" :hide-details="true" density="compact"></v-checkbox>
        <v-checkbox label="Email address for instructor of course or workshop" :hide-details="true" density="compact"></v-checkbox>
        <v-checkbox label="A certificate or document that shows I completed the course" :hide-details="true" density="compact"></v-checkbox>
        <!-- TODO figure out how to ensure at least one of the above is selected -->
      </v-col>
    </v-row>
  </div>
  <!-- List view -->
  <div v-else>
    <v-row>
      <v-col>list</v-col>
    </v-row>
    <v-row>
      <v-col>
        <v-btn prepend-icon="mdi-plus" rounded="lg" color="alternate" :disabled="isDisabled" @click="handleAddProfessionalDevelopment">
          Add course or workshop
        </v-btn>
      </v-col>
    </v-row>
  </div>

  <v-input auto-hide="auto" :model-value="modelValue" :rules="[totalHours >= 500]"></v-input>
</template>

<script lang="ts">
import { mapWritableState } from "pinia";
import { defineComponent } from "vue";

import Alert from "@/components/Alert.vue";
import WorkExperienceReferenceList, { type WorkExperienceReferenceData } from "@/components/WorkExperienceReferenceList.vue";
import WorkExperienceReferenceProgressBar from "@/components/WorkExperienceReferenceProgressBar.vue";
import { useAlertStore } from "@/store/alert";
import { useApplicationStore } from "@/store/application";
import { useWizardStore } from "@/store/wizard";
import type { EceWorkExperienceReferencesProps } from "@/types/input";
import type { Components } from "@/types/openapi";
import { isNumber } from "@/utils/formInput";
import * as Rules from "@/utils/formRules";

export default defineComponent({
  name: "EceProfessionalDevelopment",
  components: { WorkExperienceReferenceList, WorkExperienceReferenceProgressBar, Alert },
  props: {
    props: {
      type: Object as () => EceWorkExperienceReferencesProps,
      required: true,
    },
    modelValue: {
      type: Object as () => { [id: string]: Components.Schemas.WorkExperienceReference },
      required: true,
    },
  },
  emits: {
    "update:model-value": (_referenceData: { [id: string]: Components.Schemas.WorkExperienceReference }) => true,
  },
  setup: () => {
    const alertStore = useAlertStore();
    const wizardStore = useWizardStore();
    const applicationStore = useApplicationStore();

    return {
      alertStore,
      wizardStore,
      applicationStore,
    };
  },
  data: function () {
    return {
      clientId: "",
      id: null as string | null,
      previousFullName: "",
      firstName: "",
      lastName: "",
      email: "",
      phoneNumber: "",
      hours: null as number | null | undefined,
      Rules,
    };
  },
  computed: {
    ...mapWritableState(useWizardStore, { mode: "listComponentMode" }),
    isDisabled() {
      return false;
    },
    totalHours() {
      // return Object.values(this.modelValue).reduce((acc, reference) => {
      //   return acc + (reference.hours as number);
      // }, 0);
      return 100;
    },
  },
  mounted() {
    this.mode = "list";
  },
  methods: {
    isNumber,
    handleCancel() {
      // Change mode to list
      this.mode = "list";
    },
    handleAddProfessionalDevelopment() {
      // Reset the form fields
      this.resetFormData();
      this.clientId = "";
      this.mode = "add";
    },
    handleEdit(referenceData: WorkExperienceReferenceData) {
      // Set the form fields to component data
      this.id = referenceData.reference.id ?? null;
      this.clientId = referenceData.referenceId.toString();
      this.previousFullName = `${referenceData.reference.firstName} ${referenceData.reference.lastName}`;
      this.firstName = referenceData.reference.firstName ?? "";
      this.lastName = referenceData.reference.lastName ?? "";
      this.email = referenceData.reference.emailAddress ?? "";
      this.phoneNumber = referenceData.reference.phoneNumber ?? "";
      this.hours = referenceData.reference.hours ?? null;
      // Change mode to add
      this.mode = "add";
    },
    handleDelete(referenceId: string | number) {
      //Remove the entry from the modelValue

      if (referenceId in this.modelValue) {
        // Create a copy of modelValue
        const updatedModelValue = { ...this.modelValue };
        // Delete the entry from the copied object
        delete updatedModelValue[referenceId];
        // Emit the updated modelValue
        this.$emit("update:model-value", updatedModelValue);
      }

      this.alertStore.setSuccessAlert("You have deleted your reference.");
    },
    handleSubmit() {
      console.log("nope");
    },
    resetFormData() {
      this.id = null;
      this.previousFullName = "";
      this.firstName = "";
      this.lastName = "";
      this.email = "";
      this.phoneNumber = "";
      this.hours = null;
    },
  },
});
</script>
