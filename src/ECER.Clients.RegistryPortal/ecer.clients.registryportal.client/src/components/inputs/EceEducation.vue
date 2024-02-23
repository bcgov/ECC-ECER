<template>
  <v-row>
    <v-col v-if="mode == 'add'" md="8" lg="6" xl="4">
      <h3 v-if="!id">Education {{ modelValue.length + 1 }}</h3>
      <h3 v-if="id">Edit {{ previousSchool }}</h3>
      <v-form ref="addEducationForm" validate-on="input" class="mt-6">
        <v-text-field
          v-model="school"
          :rules="[Rules.required()]"
          label="Full Name of Educational Institution"
          variant="outlined"
          color="primary"
          maxlength="100"
        ></v-text-field>
        <v-text-field
          v-model="program"
          :rules="[Rules.required()]"
          label="Name of Program (as it appears on official transcript) "
          variant="outlined"
          color="primary"
          maxlength="100"
          class="my-8"
        ></v-text-field>
        <v-text-field v-model="campusLocation" label="Campus Location (Optional)" variant="outlined" color="primary" maxlength="50" class="my-8"></v-text-field>
        <v-text-field
          v-model="studentName"
          :rules="[Rules.required()]"
          label="Student Name (as it appears on your official transcript)"
          variant="outlined"
          color="primary"
          maxlength="100"
          class="my-8"
        ></v-text-field>
        <v-text-field
          v-model="studentNumber"
          :rules="[Rules.required()]"
          label="Student Number/ ID (as it appears on your official transcript)"
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
          :rules="[Rules.required()]"
          label="Start Date of Program"
          type="date"
          variant="outlined"
          color="primary"
          maxlength="50"
          class="my-8"
        ></v-text-field>
        <v-text-field
          v-model="endYear"
          :rules="[Rules.required()]"
          label="End Date of Program"
          type="date"
          variant="outlined"
          color="primary"
          maxlength="50"
          class="my-8"
        ></v-text-field>
        <v-row justify="start" class="ml-1">
          <v-btn rounded="lg" color="alternate" class="mr-2" @click="handleSubmit">Save Education</v-btn>
          <v-btn rounded="lg" variant="outlined" @click="handleCancel">Cancel</v-btn>
        </v-row>
      </v-form>
    </v-col>
    <div v-else-if="mode == 'list'">
      <v-col>
        <EducationList :educations="modelValue" @edit="handleEdit" @delete="handleDelete" />
      </v-col>
      <v-col cols="12" class="mt-6">
        <v-row justify="start" class="ml-1">
          <v-btn prepend-icon="mdi-plus" rounded="lg" color="alternate" @click="handleAddEducation">Add Education</v-btn>
        </v-row>
      </v-col>
      <v-col class="mt-4" md="8">
        <form ref="checkboxForm">
          <v-checkbox
            v-model="officialTranscriptRequested"
            color="primary"
            label="I have requested the official transcript from my education institution"
          ></v-checkbox>
          <v-checkbox
            v-model="officialTranscriptReceived"
            color="primary"
            label="The ECE Registry has already my official transcript for the course/program relevant to this application and certificate type"
          ></v-checkbox>
        </form>
      </v-col>
    </div>
  </v-row>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import EducationList from "@/components/EducationList.vue";
import { useAlertStore } from "@/store/alert";
import type { EceEducationProps } from "@/types/input";
import * as Rules from "@/utils/formRules";

export default defineComponent({
  name: "EceEdducation",
  components: { EducationList },
  props: {
    props: {
      type: Object as () => EceEducationProps,
      required: true,
    },
    modelValue: {
      type: Object as () => Education[],
      required: true,
    },
  },
  emits: {
    "update:model-value": (_educationData: Education[]) => true,
  },
  setup: () => {
    const alertStore = useAlertStore();

    return {
      alertStore,
    };
  },
  data: function () {
    return {
      mode: "add",
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
  mounted() {
    console.log(this.modelValue);
    if (this.modelValue.length === 0) {
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
        // If modelValue array contains ID, update the education data
        if (this.modelValue.some((e) => e.id === this.id)) {
          this.$emit(
            "update:model-value",
            this.modelValue.map((e) => {
              if (e.id === this.id) {
                return {
                  id: this.id,
                  school: this.school,
                  program: this.program,
                  campusLocation: this.campusLocation,
                  studentName: this.studentName,
                  studentNumber: this.studentNumber,
                  language: this.language,
                  startYear: this.startYear,
                  endYear: this.endYear,
                };
              }
              return e;
            }),
          );

          this.alertStore.setSuccessAlert("You have successfully edited your Education.");

          // Change mode to education list
          this.mode = "list";
        } else {
          // If the form is valid, emit the new education data
          this.$emit("update:model-value", [
            ...this.modelValue,
            {
              id: (this.modelValue.length + 1).toString(),
              school: this.school,
              program: this.program,
              campusLocation: this.campusLocation,
              studentName: this.studentName,
              studentNumber: this.studentNumber,
              language: this.language,
              startYear: this.startYear,
              endYear: this.endYear,
            },
          ]);

          this.alertStore.setSuccessAlert("You have successfully added your Education.");

          // Change mode to education list
          this.mode = "list";
        }
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

      // Change mode to add
      this.mode = "add";
    },
    handleEdit(education: Education) {
      // Set the form fields to the education data
      this.id = education.id;
      this.previousSchool = education.school;
      this.school = education.school;
      this.program = education.program;
      this.campusLocation = education.campusLocation;
      this.studentName = education.studentName;
      this.studentNumber = education.studentNumber;
      this.language = education.language;
      this.startYear = education.startYear;
      this.endYear = education.endYear;
      // Change mode to add
      this.mode = "add";
    },
    handleDelete(education: Education) {
      // Remove the education from the modelValue
      this.$emit(
        "update:model-value",
        this.modelValue.filter((e) => e.id !== education.id),
      );

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
  },
});
</script>
