<template>
  <v-row v-if="mode == 'add'">
    <v-col>
      <h2>{{ clientId ? "Edit" : "Add" }} education</h2>
      <p>Must match your official transcript. It’s important to check your transcript to make sure the information you enter matches what’s on it.</p>
    </v-col>
  </v-row>
  <v-row>
    <v-col v-if="mode == 'add'" md="8" lg="6" xl="4">
      <v-form ref="addEducationForm" validate-on="input" class="mt-6">
        <v-text-field
          v-model="school"
          :rules="[Rules.required('Enter the full name of your educational institution')]"
          label="Full name of educational institution"
          variant="outlined"
          color="primary"
          maxlength="100"
        ></v-text-field>
        <v-text-field
          v-model="program"
          :rules="[Rules.required('Enter the name of your program or course')]"
          label="Name of program or course"
          variant="outlined"
          color="primary"
          maxlength="100"
          class="my-8"
        ></v-text-field>
        <v-text-field v-model="campusLocation" label="Campus Location (Optional)" variant="outlined" color="primary" maxlength="50" class="my-8"></v-text-field>
        <v-text-field
          v-model="studentName"
          :rules="[Rules.required('Enter your student name shown on transcript')]"
          label="Student name shown on transcript"
          variant="outlined"
          color="primary"
          maxlength="100"
          class="my-8"
        ></v-text-field>
        <v-text-field
          v-model="studentNumber"
          :rules="[Rules.required('Enter your student number or ID')]"
          label="Student number or ID"
          variant="outlined"
          color="primary"
          maxlength="100"
          class="my-8"
        ></v-text-field>
        <v-text-field
          v-model="language"
          label="Language of institution (optional)"
          variant="outlined"
          color="primary"
          maxlength="100"
          class="my-8"
        ></v-text-field>
        <v-text-field
          v-model="startYear"
          :rules="[Rules.required('Enter the start date of your program or course')]"
          label="Start date of program or course"
          type="date"
          variant="outlined"
          color="primary"
          maxlength="50"
          class="my-8"
        ></v-text-field>
        <v-text-field
          v-model="endYear"
          :rules="[Rules.required('Enter the end date of your program or course')]"
          label="End date of program or course"
          type="date"
          variant="outlined"
          color="primary"
          maxlength="50"
          class="my-8"
        ></v-text-field>
        <v-row class="mb-10">
          <v-radio-group v-model="transcriptStatus" :rules="[Rules.required('Indicate the status of your transcript(s)')]" color="primary">
            <v-radio label="I have requested the official transcript from my education institution" value="requested"></v-radio>
            <v-radio
              label="The ECE Registry already has my official transcript for the course/program relevant to this application and certificate type"
              value="received"
            ></v-radio>
          </v-radio-group>
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
      <v-col>
        <!-- this prevents form from proceeding if rules are not met -->
        <v-input
          :model-value="modelValue"
          :rules="[
            (v) => Object.keys(v).length > 0 || 'education required',
            (v) => Object.keys(v).length >= numOfEducationRequired || `at least ${numOfEducationRequired} transcripts required`,
          ]"
          auto-hide="auto"
        ></v-input>
      </v-col>
    </div>
  </v-row>
</template>

<script lang="ts">
import { mapWritableState } from "pinia";
import { defineComponent } from "vue";

import EducationList, { type EducationData } from "@/components/EducationList.vue";
import { useAlertStore } from "@/store/alert";
import { useApplicationStore } from "@/store/application";
import { useWizardStore } from "@/store/wizard";
import type { EceEducationProps } from "@/types/input";
import type { Components } from "@/types/openapi";
import { formatDate } from "@/utils/format";
import * as Rules from "@/utils/formRules";

interface EceEducationData {
  clientId: string;
  id: string;
  previousSchool: string;
  school: string;
  program: string;
  campusLocation: string;
  studentName: string;
  studentNumber: string;
  language: string;
  startYear: string;
  endYear: string;
  transcriptStatus: "received" | "requested" | "";
  Rules: typeof Rules;
}

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
    const applicationStore = useApplicationStore();

    return {
      alertStore,
      wizardStore,
      applicationStore,
    };
  },
  data: function (): EceEducationData {
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
      transcriptStatus: "",
      Rules,
    };
  },
  computed: {
    ...mapWritableState(useWizardStore, { mode: "listComponentMode" }),
    newClientId() {
      return Object.keys(this.modelValue).length + 1;
    },
    numOfEducationRequired() {
      let numOfEducationRequired = 1;

      this.applicationStore.isDraftCertificateTypeSne && numOfEducationRequired++;
      this.applicationStore.isDraftCertificateTypeIte && numOfEducationRequired++;

      return numOfEducationRequired;
    },
  },

  mounted() {
    this.mode = "list";
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
          doesECERegistryHaveTranscript: this.transcriptStatus === "received",
          isOfficialTranscriptRequested: this.transcriptStatus === "requested",
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
        this.alertStore.setFailureAlert("You must enter all required fields in the valid format.");
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
      if (educationData.education.isOfficialTranscriptRequested) {
        this.transcriptStatus = "requested";
      } else if (educationData.education.doesECERegistryHaveTranscript) {
        this.transcriptStatus = "received";
      } else {
        this.transcriptStatus = "";
      }
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

      this.alertStore.setSuccessAlert("You have deleted your education.");
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
      this.transcriptStatus = "";
    },
    formatDate,
  },
});
</script>
