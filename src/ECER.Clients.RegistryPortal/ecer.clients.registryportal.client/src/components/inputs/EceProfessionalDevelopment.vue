<template>
  <!-- add view -->
  <div v-if="mode === 'add'">
    <v-row v-if="mode === 'add'">
      <v-col>
        <h3>{{ `${professionalDevelopmentFormMode === "add" ? "Add" : "Edit"} course or workshop` }}</h3>
        <br />
        <p>The course or workshop must:</p>
        <br />
        <ul class="ml-10">
          <li>Be relevant to the field of early childhood education</li>
          <li v-if="certificationStore.latestCertificateStatus === 'Active'">
            {{
              `Have been completed within the term of your current certificate (Between ${formatDate(certificationStore?.latestCertification?.effectiveDate || "", "LLLL d, yyyy")} to ${formatDate(certificationStore?.latestCertification?.expiryDate || "", "LLLL d, yyyy")})`
            }}
          </li>
          <li v-else-if="certificationStore.latestCertificateStatus === 'Expired'">Have been completed within the last 5 years</li>
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
        <v-col md="8" lg="6" xl="4">
          <EceTextField
            v-model="courseName"
            label="Name of course or workshop"
            maxlength="100"
            :rules="[Rules.required('Enter your course or workshop name')]"
          ></EceTextField>
        </v-col>
      </v-row>
      <v-row>
        <v-col md="8" lg="6" xl="4">
          <EceTextField
            v-model="numberOfHours"
            label="How many hours was it?"
            :rules="[Rules.required('Enter your course of workshop hours')]"
            @keypress="isNumber($event)"
          ></EceTextField>
        </v-col>
      </v-row>
      <v-row>
        <v-col md="8" lg="6" xl="4">
          <EceTextField
            v-model="organizationName"
            label="Name of place that hosted the course or workshop"
            maxlength="300"
            :rules="[Rules.required('Enter the name of the place that hosted the course or workshop')]"
          ></EceTextField>
        </v-col>
      </v-row>
      <v-row>
        <v-col md="8" lg="6" xl="4">
          <EceTextField label="Website with description of course or workshop (optional)" maxlength="500" :rules="[Rules.website()]"></EceTextField>
        </v-col>
      </v-row>
      <v-row>
        <v-col>
          <h3>When did you take it?</h3>
        </v-col>
      </v-row>
      <v-row>
        <v-col sm="6" md="4" lg="2">
          <EceDateInput
            ref="startDateInput"
            v-model="startDate"
            :rules="[
              Rules.required('Enter the start date of your course or workshop'),
              Rules.futureDateNotAllowedRule(),
              Rules.conditionalWrapper(
                certificationStore.latestCertificateStatus === 'Active',
                Rules.dateBetweenRule(
                  certificationStore?.latestCertification?.effectiveDate || '',
                  certificationStore?.latestCertification?.expiryDate || '',
                  'The start date of your course or workshop must be within the term of your current certificate',
                ),
              ),
              Rules.conditionalWrapper(
                certificationStore.latestCertificateStatus === 'Expired',
                Rules.dateRuleRange(
                  applicationStore.draftApplication.createdOn || '',
                  5,
                  'The start date of your course or workshop must be within the last five years',
                ),
              ),
            ]"
            label="Start date"
            :max="today"
            @input="validateDates"
          ></EceDateInput>
        </v-col>
      </v-row>
      <v-row>
        <v-col sm="6" md="4" lg="2">
          <EceDateInput
            ref="endDateInput"
            v-model="endDate"
            :rules="[
              Rules.required('Enter the end date of your course or workshop'),
              Rules.futureDateNotAllowedRule(),
              Rules.dateBeforeRule(startDate || ''),
              Rules.conditionalWrapper(
                certificationStore.latestCertificateStatus === 'Active',
                Rules.dateBetweenRule(
                  certificationStore?.latestCertification?.effectiveDate || '',
                  certificationStore?.latestCertification?.expiryDate || '',
                  'The end date of your course or workshop must be within the term of your current certificate',
                ),
              ),
              Rules.conditionalWrapper(
                certificationStore.latestCertificateStatus === 'Expired',
                Rules.dateRuleRange(
                  applicationStore.draftApplication.createdOn || '',
                  5,
                  'The end date of your course or workshop must be within the last five years',
                ),
              ),
            ]"
            label="End date"
            :max="today"
            @input="validateDates"
          ></EceDateInput>
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
        <v-col md="8" lg="6" xl="4">
          <EceTextField
            v-model="instructorName"
            label="Instructor name"
            maxlength="100"
            :rules="[Rules.required('Enter the instructor name of your course or workshop')]"
          ></EceTextField>
        </v-col>
      </v-row>
      <v-row v-if="showPhoneNumberInput">
        <v-col md="8" lg="6" xl="4">
          <EceTextField
            v-model="organizationContactInformation"
            label="Phone number"
            :rules="[
              Rules.required('Enter the phone number for your course or workshop contact'),
              Rules.phoneNumber('Enter your reference\'s 10-digit phone number'),
            ]"
            color="primary"
            maxlength="10"
            @keypress="isNumber($event)"
          ></EceTextField>
        </v-col>
      </v-row>
      <v-row v-if="showEmailInput">
        <v-col md="8" lg="6" xl="4">
          <EceTextField
            v-model="organizationEmailAddress"
            label="Email address"
            :rules="[Rules.required('Enter the email address of your course or workshop contact'), Rules.email()]"
          ></EceTextField>
        </v-col>
      </v-row>
      <v-row v-if="showFileInput">
        <v-col>
          <FileUploader
            :allow-multiple-files="false"
            :show-add-file-button="generateUserFileArray.length === 0 && !isFileUploadInProgress"
            :user-files="generateUserFileArray"
            :rules="[Rules.atLeastOneOptionRequired('Upload a certificate or document that shows you completed the course or workshop')]"
            :delete-file-from-temp-when-removed="false"
            @update:files="handleFileUpdate"
            @delete:file="handleFileDelete"
          />
        </v-col>
      </v-row>
    </v-form>
    <v-row class="mt-10">
      <v-col>
        <v-row justify="start" class="ml-1">
          <v-btn rounded="lg" color="primary" class="mr-2" @click="handleSubmit">Save course or workshop</v-btn>
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
      <v-col v-if="modelValue.length > 0">
        <ProfessionalDevelopmentCard
          v-for="(professionalDevelopment, index) in modelValue"
          :key="index"
          :professional-development="professionalDevelopment"
          @edit="handleEdit"
          @delete="(professionalDevelopment) => handleDelete(professionalDevelopment, index)"
        />
      </v-col>
      <!-- empty list -->
      <v-col v-else><p>No courses or workshops added yet.</p></v-col>
    </v-row>

    <v-row>
      <v-col>
        <p v-if="totalHours >= hoursRequired">
          No additional professional development may be added. You provided the required 40 hours. After you submit your application, the registry will review
          and verify the professional development added. If needed, the registry will contact you for additional information.
        </p>
        <v-btn v-else prepend-icon="mdi-plus" rounded="lg" color="primary" :disabled="isDisabled" @click="handleAddProfessionalDevelopment">
          Add course or workshop
        </v-btn>
      </v-col>
    </v-row>
    <!-- this prevents form from proceeding if rules are not met -->
    <v-input
      class="mt-6"
      :model-value="modelValue"
      :rules="[
        () =>
          totalHours >= hoursRequired ||
          `You need ${hoursRequired} hours of professional development. You need to add ${hoursRequired - totalHours} more hours.`,
      ]"
      auto-hide="auto"
    ></v-input>
  </div>
</template>

<script lang="ts">
import { DateTime } from "luxon";
import { mapWritableState } from "pinia";
import { defineComponent } from "vue";
import type { VForm, VInput } from "vuetify/components";

import EceDateInput from "@/components/inputs/EceDateInput.vue";
import EceTextField from "@/components/inputs/EceTextField.vue";
import type { FileItem } from "@/components/UploadFileItem.vue";
import { useAlertStore } from "@/store/alert";
import { useApplicationStore } from "@/store/application";
import { useCertificationStore } from "@/store/certification";
import { useWizardStore } from "@/store/wizard";
import type { Components } from "@/types/openapi";
import { formatDate } from "@/utils/format";
import { isNumber } from "@/utils/formInput";
import * as Rules from "@/utils/formRules";
import { parseHumanFileSize, removeElementByIndex, replaceElementByIndex } from "@/utils/functions";

import FileUploader from "../FileUploader.vue";
import ProfessionalDevelopmentCard from "../ProfessionalDevelopmentCard.vue";
import ProgressBar from "../ProgressBar.vue";

interface ProfessionalDevelopmentData {
  selection: ("phone" | "email" | "file")[];
  areAttachedFilesValid: boolean;
  isFileUploadInProgress: boolean;
  hoursRequired: number;
  professionalDevelopmentFormMode: "add" | "edit";
}

export interface ProfessionalDevelopmentExtended extends Components.Schemas.ProfessionalDevelopment {
  newFilesWithData?: FileItem[];
}

export default defineComponent({
  name: "EceProfessionalDevelopment",
  components: { ProgressBar, FileUploader, ProfessionalDevelopmentCard, EceDateInput, EceTextField },
  props: {
    modelValue: {
      type: Object as () => ProfessionalDevelopmentExtended[],
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
    const certificationStore = useCertificationStore();

    return {
      alertStore,
      wizardStore,
      applicationStore,
      certificationStore,
      formatDate,
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
      professionalDevelopmentFormMode: "add",
    };
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
    today() {
      return formatDate(DateTime.now().toString());
    },
    generateUserFileArray() {
      const userFileList: FileItem[] = [];

      if (this.files) {
        for (let file of this.files) {
          const newFileItem: FileItem = {
            fileId: file.id!,
            fileErrors: [],
            fileSize: parseHumanFileSize(file.size!),
            fileName: file.name!,
            progress: 101,
            file: new File([], file.name!),
            storageFolder: "permanent",
          };

          userFileList.push(newFileItem);
        }
      }

      if (this.newFilesWithData) {
        for (let each of this.newFilesWithData) {
          userFileList.push(each);
        }
      }

      return userFileList;
    },
  },
  unmounted() {
    this.mode = "list";
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
      this.professionalDevelopmentFormMode = "add";
      window.scroll(0, 0);
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
      this.newFilesWithData = professionalDevelopment.newFilesWithData || [];
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

      this.mode = "add";
      this.professionalDevelopmentFormMode = "edit";
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

        if (this.professionalDevelopmentFormMode === "edit") {
          const indexOfEditedProfessionalDevelopment = updatedModelValue.findIndex(
            (professionalDevelopment) => professionalDevelopment.id === newProfessionalDevelopment.id,
          );
          updatedModelValue = replaceElementByIndex(updatedModelValue, indexOfEditedProfessionalDevelopment, newProfessionalDevelopment);
        } else if (this.professionalDevelopmentFormMode === "add") {
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
          if (this.areAttachedFilesValid && !this.isFileUploadInProgress && file.storageFolder === "temporary") {
            this.newFilesWithData.push(file);
          }
        }
      }
    },
    handleFileDelete(fileItem: FileItem) {
      if (fileItem.storageFolder === "permanent") {
        //we need to add it to the list of deleted files for the backend to remove.
        this.deletedFiles?.push(fileItem.fileId);
        let index = this.files?.findIndex((file) => file.id === fileItem.fileId);
        if (index) {
          this.files = removeElementByIndex(this.modelValue, index);
        }
      }
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
      this.professionalDevelopmentFormMode = "add";
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
        //if user unchecks file we remove all files. All previously saved files need to be put onto the deleted files list
        this.deletedFiles = this.files?.map((file) => file.id || "") || [];
        this.files = [];
        this.newFilesWithData = [];
      }
    },
    validateDates() {
      if (this.startDate && this.endDate) {
        (this.$refs.startDateInput as VInput).validate();
        (this.$refs.endDateInput as VInput).validate();
      }
    },
  },
});
</script>
