<template>
  <v-row>
    <v-col v-if="mode == 'add'" md="8" lg="6" xl="4">
      <v-form ref="education-form" validate-on="input" title="hello">
        <v-text-field
          :model-value="school"
          :rules="[Rules.required()]"
          label="Full Name of Educational Institution"
          variant="outlined"
          color="primary"
          maxlength="150"
        ></v-text-field>
        <v-text-field
          :model-value="program"
          :rules="[Rules.required()]"
          label="Name of Program (as it appears on official transcript) "
          variant="outlined"
          color="primary"
          maxlength="50"
          class="my-8"
        ></v-text-field>
        <v-text-field
          :model-value="campusLocation"
          label="Campus Location (Optional)"
          variant="outlined"
          color="primary"
          maxlength="50"
          class="my-8"
        ></v-text-field>
        <v-text-field
          :model-value="studentName"
          :rules="[Rules.required()]"
          label="Student Name (as it appears on your official transcript)"
          variant="outlined"
          color="primary"
          maxlength="7"
          class="my-8"
        ></v-text-field>
        <v-text-field
          :model-value="studentNumber"
          :rules="[Rules.required()]"
          label="Student Number/ ID (as it appears on your official transcript)"
          variant="outlined"
          color="primary"
          maxlength="50"
          class="my-8"
        ></v-text-field>
        <v-text-field
          :model-value="language"
          label="Language of Institution (Optional)"
          variant="outlined"
          color="primary"
          maxlength="50"
          class="my-8"
        ></v-text-field>
        <v-text-field
          :model-value="startYear"
          :rules="[Rules.required()]"
          label="Start Date of Program"
          type="date"
          variant="outlined"
          color="primary"
          maxlength="50"
          class="my-8"
        ></v-text-field>
        <v-text-field
          :model-value="endYear"
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
        <EducationList :educations="modelValue" />
      </v-col>
      <v-col cols="12">
        <v-row justify="start" class="ml-1">
          <v-btn prepend-icon="mdi-plus" rounded="lg" color="alternate" @click="handleAddEducation">Add Education</v-btn>
        </v-row>
      </v-col>
      <v-col class="mt-4" md="8">
        <form ref="education-step-form">
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
  data: function () {
    return {
      mode: "add",
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
  methods: {
    handleSubmit() {
      // Validate the form
      // If the form is valid, emit the new education data
      // Change mode to education list
      this.mode = "list";
    },
    handleCancel() {
      // Reset the form
      // Change mode to education list
      this.mode = "list";
    },
    handleAddEducation() {
      // Change mode to add
      this.mode = "add";
    },
  },
});
</script>
