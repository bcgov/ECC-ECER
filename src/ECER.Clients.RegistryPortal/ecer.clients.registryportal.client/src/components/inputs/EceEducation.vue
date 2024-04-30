<template>
  <v-row>
    <v-col v-if="mode == 'add'" md="8" lg="6" xl="4">
      <h3 v-if="!clientId">Education {{ newClientId }}</h3>
      <h3 v-if="clientId">Edit {{ previousSchool }}</h3>
      <v-form ref="addEducationForm" validate-on="input" class="mt-6">
        <v-text-field
          v-model="school"
          :rules="[Rules.required('Enter the full name of your educational institution')]"
          label="Full Name of Educational Institution"
          variant="outlined"
          color="primary"
          maxlength="100"
        ></v-text-field>
        <v-text-field
          v-model="program"
          :rules="[Rules.required(`Enter the name of your ${getLabelOnCertificateType.toLowerCase()}`)]"
          :label="`Name of ${getLabelOnCertificateType}`"
          variant="outlined"
          color="primary"
          maxlength="100"
          class="my-8"
        ></v-text-field>
        <v-text-field v-model="campusLocation" label="Campus Location (Optional)" variant="outlined" color="primary" maxlength="50" class="my-8"></v-text-field>
        <v-text-field
          v-model="studentName"
          :rules="[Rules.required('Enter your student name as it appears on your official transcript')]"
          label="Student Name (as it appears on your official transcript)"
          variant="outlined"
          color="primary"
          maxlength="100"
          class="my-8"
        ></v-text-field>
        <v-text-field
          v-model="studentNumber"
          :rules="[Rules.required('Enter your student ID as it appears on your official transcript')]"
          label="Student ID"
          variant="outlined"
          color="primary"
          maxlength="100"
          class="my-8"
        ></v-text-field>
        <v-text-field
          v-model="language"
          label="Language of Institution (Optional)"
          variant="outlined"
          color="primary"
          maxlength="100"
          class="my-8"
        ></v-text-field>
        <v-text-field
          v-model="startYear"
          :rules="[Rules.required(`Enter the start date of your ${getLabelOnCertificateType.toLowerCase()}`)]"
          :label="`Start Date of ${getLabelOnCertificateType}`"
          type="date"
          variant="outlined"
          color="primary"
          maxlength="50"
          class="my-8"
        ></v-text-field>
        <v-text-field
          v-model="endYear"
          :rules="[Rules.required(`Enter the end date of your ${getLabelOnCertificateType.toLowerCase()}`)]"
          :label="`End Date of ${getLabelOnCertificateType}`"
          type="date"
          variant="outlined"
          color="primary"
          maxlength="50"
          class="my-8"
        ></v-text-field>
        <v-row>
          <v-checkbox
            ref="refOfficialTranscriptRequested"
            v-model="officialTranscriptRequested"
            :rules="[atLeastOneCheckedRule]"
            color="primary"
            label="I have requested the official transcript from my education institution"
          ></v-checkbox>
          <v-checkbox
            ref="refOfficialTranscriptReceived"
            v-model="officialTranscriptReceived"
            :rules="[atLeastOneCheckedRule]"
            color="primary"
            label="The ECE Registry already has my official transcript for the course/program relevant to this application and certificate type"
          ></v-checkbox>
          <div v-if="!atLeastOneChecked" class="v-messages error-message mb-5" role="alert">Indicate the status of your transcript(s)</div>
        </v-row>
        <v-row justify="start" class="ml-1">
          <v-btn rounded="lg" color="alternate" class="mr-2" @click="handleSubmit">Save Education</v-btn>
          <v-btn rounded="lg" variant="outlined" @click="handleCancel">Cancel</v-btn>
        </v-row>
      </v-form>
    </v-col>
    <div v-else-if="mode == 'list'" class="w-100">
      <v-col sm="12" md="10" lg="8" xl="6">
        <EducationList :educations="modelValue" @edit="handleEdit" @delete="handleDelete" />
      </v-col>
      <v-col cols="12" class="mt-6">
        <v-row justify="start" class="ml-1">
          <v-btn prepend-icon="mdi-plus" rounded="lg" color="alternate" @click="handleAddEducation">Add Education</v-btn>
        </v-row>
      </v-col>
    </div>
  </v-row>
</template>

<script lang="ts">
import { mapWritableState } from "pinia";
import { defineComponent } from "vue";

import EducationList, { type EducationData } from "@/components/EducationList.vue";
import { useAlertStore } from "@/store/alert";
import { useWizardStore } from "@/store/wizard";
import type { EceEducationProps } from "@/types/input";
import type { Components } from "@/types/openapi";
import { formatDate } from "@/utils/format";
import * as Rules from "@/utils/formRules";
export default defineComponent({
  name: "EceEducation",
  components: { EducationList },
  props: {
    props: {
      type: Object as () => EceEducationProps,
      required: true,
    },
    modelValue: {
      type: Object as () => { [id: string]: Components.Schemas.Transcript },
      required: true,
    },
  },
  emits: {
    "update:model-value": (_educationData: { [id: string]: Components.Schemas.Transcript }) => true,
  },
  setup: () => {
    const alertStore = useAlertStore();
    const wizardStore = useWizardStore();

    return {
      alertStore,
      wizardStore,
    };
  },
  data: function () {
    return {
      clientId: "",
      id: "",
      previousSchool: "",
      school: "",
      program: "",
      campusLocation: "",
      studentName: "",
      studentNumber: "",
      language: "",
      startYear: "",
      endYear: "",
      officialTranscriptRequested: false,
      officialTranscriptReceived: false,
      Rules,
    };
  },
  computed: {
    ...mapWritableState(useWizardStore, { mode: "listComponentMode" }),
    newClientId() {
      return Object.keys(this.modelValue).length + 1;
    },

    atLeastOneChecked(): boolean {
      return this.officialTranscriptRequested == true || this.officialTranscriptReceived == true;
    },
    getLabelOnCertificateType() {
      if (this.wizardStore.wizardData.certificationSelection.includes("FiveYears")) {
        return "Program";
      } else {
        return "Course";
      }
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
    async handleSubmit() {
      // Validate the form
      const { valid } = await (this.$refs.addEducationForm as any).validate();

      if (valid) {
        // Prepare the new or updated Transcript data
        const newTranscript: Components.Schemas.Transcript = {
          id: this.id,
          educationalInstitutionName: this.school,
          programName: this.program,
          campusLocation: this.campusLocation,
          studentName: this.studentName,
          studentNumber: this.studentNumber,
          languageofInstruction: this.language,
          startDate: this.startYear,
          endDate: this.endYear,
          doesECERegistryHaveTranscript: this.officialTranscriptReceived,
          isOfficialTranscriptRequested: this.officialTranscriptRequested,
        };

        // see if we already have a clientId (which is edit), if not use the newClientId (which is add)
        const clientId = this.clientId ? this.clientId : this.newClientId;
        // Update the modelValue dictionary
        const updatedModelValue = {
          ...this.modelValue,
          [clientId]: newTranscript,
        };

        // Emit the updated modelValue
        this.$emit("update:model-value", updatedModelValue);

        // Set success alert message
        const message = this.modelValue[clientId] ? "You have successfully edited your Education." : "You have successfully added your Education.";
        this.alertStore.setSuccessAlert(message);

        // Change mode to education list
        this.mode = "list";
      } else {
        this.alertStore.setFailureAlert("Please fill out all required fields");
      }
    },
    handleCancel() {
      // Change mode to education list
      this.mode = "list";
    },
    handleAddEducation() {
      // Reset the form fields
      this.resetFormData();
      this.clientId = "";
      this.mode = "add";
    },
    handleEdit(educationData: EducationData) {
      // Set the form fields to the education data
      this.id = educationData.education.id ?? "";
      this.clientId = educationData.educationId.toString();
      this.previousSchool = educationData.education.educationalInstitutionName ?? "";
      this.school = educationData.education.educationalInstitutionName ?? "";
      this.program = educationData.education.programName ?? "";
      this.campusLocation = educationData.education.campusLocation ?? "";
      this.studentName = educationData.education.studentName ?? "";
      this.studentNumber = educationData.education.studentNumber ?? "";
      this.language = educationData.education.languageofInstruction ?? "";
      this.startYear = formatDate(educationData.education.startDate) ?? "";
      this.endYear = formatDate(educationData.education.endDate) ?? "";
      this.officialTranscriptRequested = educationData.education.isOfficialTranscriptRequested ?? false;
      this.officialTranscriptReceived = educationData.education.doesECERegistryHaveTranscript ?? false;
      // Change mode to add
      this.mode = "add";
    },
    handleDelete(educationId: string | number) {
      //Remove the education from the modelValue

      if (educationId in this.modelValue) {
        // Create a copy of modelValue
        const updatedModelValue = { ...this.modelValue };
        // Delete the education entry from the copied object
        delete updatedModelValue[educationId];
        // Emit the updated modelValue
        this.$emit("update:model-value", updatedModelValue);
      }

      this.alertStore.setWarningAlert("You have Deleted your Education.");
    },
    resetFormData() {
      this.id = "";
      this.previousSchool = "";
      this.school = "";
      this.program = "";
      this.campusLocation = "";
      this.studentName = "";
      this.studentNumber = "";
      this.language = "";
      this.startYear = "";
      this.endYear = "";
    },
    formatDate,
    atLeastOneCheckedRule() {
      if (!this.officialTranscriptRequested && !this.officialTranscriptReceived) {
        return "";
      }
      return true;
    },
  },
});
</script>
