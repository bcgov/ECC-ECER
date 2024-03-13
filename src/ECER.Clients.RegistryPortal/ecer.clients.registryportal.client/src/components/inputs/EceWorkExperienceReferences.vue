<template>
  <v-row>
    <v-col v-if="mode == 'add'" md="8" lg="6" xl="4">
      <h3 v-if="!clientId">Reference {{ newClientId }}</h3>
      <h3 v-if="clientId">Edit {{ previousFullName }}</h3>
      <v-form ref="addWorkExperienceReferenceForm" validate-on="input" class="mt-6">
        <v-text-field
          v-model="lastName"
          :rules="[Rules.required()]"
          label="Reference Last Name"
          variant="outlined"
          color="primary"
          maxlength="100"
        ></v-text-field>
        <v-text-field
          v-model="firstName"
          :rules="[Rules.required()]"
          label="Reference First Name"
          variant="outlined"
          color="primary"
          maxlength="100"
          class="my-8"
        ></v-text-field>
        <v-text-field
          v-model="email"
          :rules="[Rules.required(), Rules.email()]"
          label="Reference Email"
          variant="outlined"
          color="primary"
          maxlength="200"
          class="my-8"
        ></v-text-field>
        <v-text-field
          v-model="phoneNumber"
          :rules="[Rules.phoneNumber()]"
          label="Reference Phone Number (Optional)"
          variant="outlined"
          color="primary"
          maxlength="20"
          class="my-8"
        ></v-text-field>
        <v-text-field
          v-model="hours"
          label="Work Experience Hours"
          variant="outlined"
          color="primary"
          maxlength="8"
          class="my-8"
          @keypress="isNumber($event)"
        ></v-text-field>

        <v-row justify="start" class="ml-1">
          <v-btn rounded="lg" color="alternate" class="mr-2" @click="handleSubmit">Save Reference</v-btn>
          <v-btn rounded="lg" variant="outlined" @click="handleCancel">Cancel</v-btn>
        </v-row>
      </v-form>
    </v-col>
    <div v-else-if="mode == 'list'" class="w-100">
      <v-col cols="12">
        <WorkExperienceReferenceProgressBar :references="modelValue" />
      </v-col>
      <v-col sm="12" md="8" lg="6">
        <WorkExperienceReferenceList :references="modelValue" @edit="handleEdit" @delete="handleDelete" />
      </v-col>
      <v-col cols="12" class="mt-6">
        <v-row justify="start" class="ml-1">
          <v-btn prepend-icon="mdi-plus" rounded="lg" color="alternate" @click="handleAddReference">Add Reference</v-btn>
        </v-row>
      </v-col>
    </div>
  </v-row>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import WorkExperienceReferenceList, { type WorkExperienceReferenceData } from "@/components/WorkExperienceReferenceList.vue";
import { useAlertStore } from "@/store/alert";
import type { EceWorkExperienceReferencesProps } from "@/types/input";
import type { Components } from "@/types/openapi";
import { isNumber } from "@/utils/formInput";
import * as Rules from "@/utils/formRules";

import WorkExperienceReferenceProgressBar from "../WorkExperienceReferenceProgressBar.vue";

export default defineComponent({
  name: "EceEdducation",
  components: { WorkExperienceReferenceList, WorkExperienceReferenceProgressBar },
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

    return {
      alertStore,
    };
  },
  data: function () {
    return {
      clientId: "",
      mode: "add",
      id: null as string | null,
      previousFullName: "",
      firstName: "",
      lastName: "",
      email: "",
      phoneNumber: "",
      hours: "",
      Rules,
    };
  },
  computed: {
    fullName() {
      return `${this.firstName} ${this.lastName}`;
    },
    newClientId() {
      return Object.keys(this.modelValue).length + 1;
    },
  },
  mounted() {
    if (Object.keys(this.modelValue).length === 0) {
      this.mode = "add";
    } else {
      this.mode = "list";
    }
  },
  methods: {
    isNumber,
    async handleSubmit() {
      // Validate the form
      const { valid } = await (this.$refs.addWorkExperienceReferenceForm as any).validate();

      if (valid) {
        // Prepare the new or updated WorkExperienceReference data
        const newReference: Components.Schemas.WorkExperienceReference = {
          id: this.id,
          firstName: this.firstName,
          lastName: this.lastName,
          emailAddress: this.email,
          phoneNumber: this.phoneNumber,
          hours: this.hours,
        };

        // see if we already have a clientId (which is edit), if not use the newClientId (which is add)
        const clientId = this.clientId ? this.clientId : this.newClientId;
        // Update the modelValue dictionary
        const updatedModelValue = {
          ...this.modelValue,
          [clientId]: newReference,
        };

        // Emit the updated modelValue
        this.$emit("update:model-value", updatedModelValue);

        // Set success alert message
        const message = this.modelValue[clientId] ? "You have successfully edited your Reference." : "You have successfully added your Reference.";
        this.alertStore.setSuccessAlert(message);

        // Change mode to list
        this.mode = "list";
      } else {
        this.alertStore.setFailureAlert("Please fill out all required fields");
      }
    },
    handleCancel() {
      // Change mode to list
      this.mode = "list";
    },
    handleAddReference() {
      // Reset the form fields
      this.resetFormData();
      this.clientId = "";
      this.mode = "add";
    },
    handleEdit(referenceData: WorkExperienceReferenceData) {
      // Set the form fields to component data
      this.id = referenceData.reference.id ?? null;
      this.clientId = referenceData.referenceId.toString();
      this.previousFullName = this.fullName ?? "";
      this.firstName = referenceData.reference.firstName ?? "";
      this.lastName = referenceData.reference.lastName ?? "";
      this.email = referenceData.reference.emailAddress ?? "";
      this.phoneNumber = referenceData.reference.phoneNumber ?? "";
      this.hours = referenceData.reference.hours ?? "";
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

      this.alertStore.setWarningAlert("You have Deleted your Reference.");
    },
    resetFormData() {
      this.id = null;
      this.firstName = "";
      this.lastName = "";
      this.email = "";
      this.phoneNumber = "";
      this.hours = "";
    },
  },
});
</script>
