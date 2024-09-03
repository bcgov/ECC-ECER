<template>
  <!-- add view -->
  <div v-if="mode === 'add'">
    <v-row v-if="mode === 'add'">
      <v-col>
        <h3>{{ `${id ? "Edit" : "Add"} course or workshop` }}</h3>
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
          <v-text-field
            v-model="courseName"
            variant="outlined"
            label="Name of course or workshop"
            maxlength="100"
            :rules="[Rules.required('Enter your course or workshop name')]"
          ></v-text-field>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <v-text-field
            v-model="numberOfHours"
            variant="outlined"
            label="How many hours was it?"
            :rules="[Rules.required('Enter your course of workshop hours')]"
            @keypress="isNumber($event)"
          ></v-text-field>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <v-text-field
            v-model="organizationName"
            variant="outlined"
            label="Name of place that hosted the course or workshop"
            maxlength="300"
            :rules="[Rules.required('Enter the name of the place that hosted the course or workshop')]"
          ></v-text-field>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="4">
          <v-text-field
            variant="outlined"
            label="Website with description of course or workshop (optional)"
            maxlength="500"
            :rules="[Rules.website()]"
          ></v-text-field>
        </v-col>
      </v-row>
      <v-row>
        <v-col>
          <h3>When did you take it?</h3>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="2">
          <v-text-field
            v-model="startDate"
            :rules="[Rules.required('Enter the start date of your course or workshop')]"
            label="Start date"
            type="date"
            variant="outlined"
            color="primary"
          ></v-text-field>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="2">
          <v-text-field
            v-model="endDate"
            :rules="[Rules.required('Enter the start date of your course or workshop')]"
            label="End date"
            type="date"
            variant="outlined"
            color="primary"
          ></v-text-field>
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
          {{ selection }}
          <v-checkbox
            v-model="selection"
            label="Phone number for instructor of course or workshop"
            :hide-details="true"
            density="compact"
            value="phone"
            @input="selectionChanged"
          ></v-checkbox>
          <v-checkbox
            v-model="selection"
            label="Email address for instructor of course or workshop"
            :hide-details="true"
            value="email"
            density="compact"
            @input="selectionChanged"
          ></v-checkbox>
          <v-checkbox
            v-model="selection"
            label="A certificate or document that shows I completed the course"
            hide-details="auto"
            value="file"
            density="compact"
            :rules="[Rules.atLeastOneOptionRequired()]"
            @input="selectionChanged"
          ></v-checkbox>
        </v-col>
      </v-row>
      <v-row v-if="showInstructorNameInput">
        <v-col cols="4">
          <v-text-field
            v-model="instructorName"
            variant="outlined"
            label="Instructor name"
            maxlength="100"
            :rules="[Rules.required('Enter the instructor name of your course or workshop')]"
          ></v-text-field>
        </v-col>
      </v-row>
      <v-row v-if="showPhoneNumberInput">
        <v-col cols="4">
          <v-text-field
            v-model="organizationContactInformation"
            label="Phone number"
            :rules="[
              Rules.required('Enter the phone number for your course or workshop contact'),
              Rules.phoneNumber('Enter your reference\'s 10-digit phone number'),
            ]"
            variant="outlined"
            color="primary"
            maxlength="10"
            @keypress="isNumber($event)"
          ></v-text-field>
        </v-col>
      </v-row>
      <v-row v-if="showEmailInput">
        <v-col cols="4">
          <v-text-field
            v-model="organizationEmailAddress"
            variant="outlined"
            label="Email address"
            :rules="[Rules.required('Enter the email address of your course or workshop contact'), Rules.email()]"
          ></v-text-field>
        </v-col>
      </v-row>
      <v-row v-if="showFileInput">
        <v-col>
          <FileUploader :userFiles="newFilesWithData" @update:files="handleFileUpdate" @delete:file="handleFileDelete" />
        </v-col>
      </v-row>
    </v-form>
    <v-row class="mt-10">
      <v-col>
        <v-row justify="start" class="ml-1">
          <v-btn rounded="lg" color="alternate" class="mr-2" @click="handleSubmit">Save Reference</v-btn>
          <v-btn rounded="lg" variant="outlined" @click="handleCancel">Cancel</v-btn>
        </v-row>
      </v-col>
    </v-row>
  </div>
  <!-- List view -->
  <div v-else>
    <v-row>
      <v-col>
        <p>
          You must have completed at least 40 hours of professional development relevant to early childhood education. Add the courses or workshops you’ve
          taken.
        </p>
      </v-col>
    </v-row>
    <v-row>
      <v-col>
        <ProgressBar :hours-required="hoursRequired" :total-hours="totalHours"></ProgressBar>
      </v-col>
    </v-row>
    <v-row>
      <v-col>
        {{ modelValue }}
      </v-col>
    </v-row>
    <v-row>
      <v-col v-if="modelValue.length > 0">
        <ProfessionalDevelopmentCard
          v-for="(professionalDevelopment, index) in modelValue"
          :key="index"
          :professionalDevelopment="professionalDevelopment"
          @edit="handleEdit"
          @delete="(professionalDevelopment) => handleDelete(professionalDevelopment, index)"
        />
      </v-col>
      <v-col v-else><p></p></v-col>
    </v-row>
    <v-row>
      <v-col>
        <v-btn
          v-if="totalHours < hoursRequired"
          prepend-icon="mdi-plus"
          rounded="lg"
          color="alternate"
          :disabled="isDisabled"
          @click="handleAddProfessionalDevelopment"
        >
          Add course or workshop
        </v-btn>
      </v-col>
    </v-row>
  </div>

  <!-- TODO -->
  <!-- <v-input auto-hide="auto" :model-value="modelValue" :rules="[totalHours >= 500]"></v-input> -->
</template>

<script lang="ts">
import { mapWritableState } from "pinia";
import { defineComponent } from "vue";
import type { VForm } from "vuetify/components";

import Alert from "@/components/Alert.vue";
import type { FileItem } from "@/components/UploadFileItem.vue";
import { useAlertStore } from "@/store/alert";
import { useApplicationStore } from "@/store/application";
import { useWizardStore } from "@/store/wizard";
import type { Components } from "@/types/openapi";
import { isNumber } from "@/utils/formInput";
import * as Rules from "@/utils/formRules";

import FileUploader from "../FileUploader.vue";
import ProgressBar from "../ProgressBar.vue";
import ProfessionalDevelopmentCard from "../ProfessionalDevelopmentCard.vue";
import { formatDate } from "@/utils/format";
import { removeElementByIndex, replaceElementByIndex } from "@/utils/functions";

interface ProfessionalDevelopmentData {
  selection: ("phone" | "email" | "file")[];
  areAttachedFilesValid: boolean;
  isFileUploadInProgress: boolean;
  hoursRequired: number;
}

export interface ProfessionalDevelopmentExtended extends Components.Schemas.ProfessionalDevelopment {
  newFilesWithData: FileItem[];
}

export default defineComponent({
  name: "EceProfessionalDevelopment",
  components: { ProgressBar, Alert, FileUploader, ProfessionalDevelopmentCard },
  props: {
    props: {
      type: Object as () => {},
      required: true,
    },
    modelValue: {
      type: Object as () => Components.Schemas.ProfessionalDevelopment[],
      required: true,
    },
  },
  emits: {
    "update:model-value": (_professionalDevelopmentData: Components.Schemas.ProfessionalDevelopment[]) => true,
  },
  setup: () => {
    const alertStore = useAlertStore();
    const wizardStore = useWizardStore();
    const applicationStore = useApplicationStore();

    return {
      alertStore,
      wizardStore,
      applicationStore,
      Rules,
    };
  },
  data(): ProfessionalDevelopmentData & ProfessionalDevelopmentExtended {
    return {
      //Professional Development
      id: "",
      courseName: "",
      numberOfHours: undefined,
      organizationName: "",
      startDate: undefined,
      courseorWorkshopLink: "",
      endDate: undefined,
      instructorName: "",
      organizationContactInformation: "",
      organizationEmailAddress: "",
      files: [],
      newFiles: [],
      deletedFiles: [],
      //professional development extended to handle files
      newFilesWithData: [],
      //other data
      selection: [],
      isFileUploadInProgress: false,
      areAttachedFilesValid: true,
      hoursRequired: 40,
    };
  },
  unmounted() {
    this.mode = "list";
  },
  computed: {
    ...mapWritableState(useWizardStore, { mode: "listComponentMode" }),
    isDisabled() {
      return false;
    },
    totalHours() {
      return this.modelValue.reduce((acc, professionalDevelopment) => {
        return acc + Number(professionalDevelopment.numberOfHours!);
      }, 0);
    },
    showInstructorNameInput() {
      return this.selection.includes("email") || this.selection.includes("phone");
    },
    showPhoneNumberInput() {
      return this.selection.includes("phone");
    },
    showEmailInput() {
      return this.selection.includes("email");
    },
    showFileInput() {
      return this.selection.includes("file");
    },
  },
  mounted() {
    this.mode = "list";
  },
  methods: {
    isNumber,
    handleCancel() {
      this.resetFormData();
      // Change mode to list
      this.mode = "list";
      window.scroll(0, 0);
    },
    handleAddProfessionalDevelopment() {
      // Reset the form fields
      this.resetFormData();
      this.mode = "add";
    },
    handleEdit(professionalDevelopment: ProfessionalDevelopmentExtended) {
      // Set the form fields to component data
      this.id = professionalDevelopment.id;
      this.courseName = professionalDevelopment.courseName;
      this.numberOfHours = professionalDevelopment.numberOfHours;
      this.organizationName = professionalDevelopment.organizationName;
      this.startDate = professionalDevelopment.startDate ? formatDate(professionalDevelopment.startDate) : undefined;
      this.courseorWorkshopLink = professionalDevelopment.courseorWorkshopLink;
      this.endDate = professionalDevelopment.endDate ? formatDate(professionalDevelopment.endDate) : undefined;
      this.instructorName = professionalDevelopment.instructorName;
      this.organizationContactInformation = professionalDevelopment.organizationContactInformation;
      this.organizationEmailAddress = professionalDevelopment.organizationEmailAddress;
      this.files = professionalDevelopment.files;
      this.newFiles = professionalDevelopment.newFiles;
      this.deletedFiles = professionalDevelopment.deletedFiles;
      this.newFilesWithData = professionalDevelopment.newFilesWithData;
      this.selection = [];
      //selection
      if (professionalDevelopment.organizationContactInformation) {
        this.selection.push("phone");
      }
      if (professionalDevelopment.organizationEmailAddress) {
        this.selection.push("email");
      }
      if (
        (professionalDevelopment?.files?.length && professionalDevelopment?.files?.length > 0) ||
        (professionalDevelopment?.newFilesWithData?.length && professionalDevelopment.newFilesWithData.length > 0)
      ) {
        this.selection.push("file");
      }

      // Change mode to add
      this.mode = "add";
    },
    handleDelete(_professionalDevelopment: ProfessionalDevelopmentExtended, index: number) {
      this.$emit("update:model-value", removeElementByIndex(this.modelValue, index));
      this.alertStore.setSuccessAlert("You have deleted your professional development.");
    },
    async handleSubmit() {
      const { valid } = await (this.$refs.professionalDevelopmentForm as VForm).validate();

      if (valid) {
        const newProfessionalDevelopment: ProfessionalDevelopmentExtended = {
          id: this.id, //empty if we are adding
          courseName: this.courseName,
          numberOfHours: this.numberOfHours,
          organizationName: this.organizationName,
          startDate: this.startDate,
          courseorWorkshopLink: this.courseorWorkshopLink,
          endDate: this.endDate,
          instructorName: this.instructorName,
          organizationContactInformation: this.organizationContactInformation,
          organizationEmailAddress: this.organizationEmailAddress,
          files: this.files,
          newFiles: this.newFiles,
          deletedFiles: this.deletedFiles,
          newFilesWithData: this.newFilesWithData,
        };
        let updatedModelValue = this.modelValue.slice(); //create a copy of the array

        if (newProfessionalDevelopment.id) {
          //we are editing if id exists
          const indexOfEditedProfessionalDevelopment = updatedModelValue.findIndex(
            (professionalDevelopment) => professionalDevelopment.id === newProfessionalDevelopment.id,
          );
          updatedModelValue = replaceElementByIndex(updatedModelValue, indexOfEditedProfessionalDevelopment, newProfessionalDevelopment);
        } else {
          //otherwise we are adding
          updatedModelValue.push(newProfessionalDevelopment);
        }

        this.resetFormData();
        this.mode = "list";
        this.alertStore.setSuccessAlert(
          newProfessionalDevelopment.id
            ? "You have successfully edited your professional development."
            : "You have successfully added your professional development.",
        );

        this.$emit("update:model-value", updatedModelValue);
        window.scroll(0, 0);
      } else {
        this.alertStore.setFailureAlert("You must enter all required fields in the valid format.");
      }
    },
    handleFileUpdate(filesArray: FileItem[]) {
      this.areAttachedFilesValid = true;
      this.isFileUploadInProgress = false;
      this.newFilesWithData = []; // Reset attachments
      if (filesArray && filesArray.length > 0) {
        for (let i = 0; i < filesArray.length; i++) {
          const file = filesArray[i];

          // Check for file errors
          if (file.fileErrors && file.fileErrors.length > 0) {
            this.areAttachedFilesValid = false;
          }

          // Check if file is still uploading
          else if (file.progress < 101) {
            this.isFileUploadInProgress = true;
          }

          // If file is valid and fully uploaded, add to attachments
          if (this.areAttachedFilesValid && !this.isFileUploadInProgress) {
            this.newFilesWithData.push(file);
          }
        }
      }
    },
    handleFileDelete(fileItem: FileItem) {
      console.log("deleted");
      console.log(fileItem);
    },
    resetFormData() {
      //professional development
      this.id = "";
      this.courseName = "";
      this.numberOfHours = undefined;
      this.organizationName = "";
      this.startDate = undefined;
      this.courseorWorkshopLink = "";
      this.endDate = undefined;
      this.instructorName = "";
      this.organizationContactInformation = "";
      this.organizationEmailAddress = "";
      this.files = [];
      this.newFiles = [];
      this.deletedFiles = [];
      this.newFilesWithData = [];
      //selection
      this.selection = [];
    },
    selectionChanged() {
      if (!this.selection.includes("phone") && !this.selection.includes("email")) {
        //no email or phone clear out instructor name
        this.instructorName = "";
      }

      if (!this.selection.includes("phone")) {
        this.organizationContactInformation = "";
      }

      if (!this.selection.includes("email")) {
        this.organizationEmailAddress = "";
      }

      if (!this.selection.includes("file")) {
        console.log("TODO how to handle a mix of new files and old files");
      }
    },
  },
});
</script>
