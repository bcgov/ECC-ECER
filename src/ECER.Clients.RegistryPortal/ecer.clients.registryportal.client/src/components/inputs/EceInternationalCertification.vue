<template>
  <!-- add view -->
  <div v-if="internationalCertificationFormMode === 'edit' || internationalCertificationFormMode === 'add'">
    <ECEHeader title="Regulatory authority information" />
    <br />
    <p>Your international certification must be issued by a country or region that regulates the practice of early childhood education.</p>
    <v-form ref="internationalCertificationForm" class="mt-6">
      <v-row>
        <v-col md="8" lg="6" xl="4">
          Country
          <v-select
            class="pt-2"
            :items="configStore.countryList"
            variant="outlined"
            label=""
            v-model="countryId"
            item-title="countryName"
            item-value="countryId"
            :rules="[Rules.required('Select your country of certification', 'countryId')]"
          ></v-select>
        </v-col>
      </v-row>
      <v-row>
        <v-col md="8" lg="6" xl="4">
          <EceTextField
            id="txtRegulatoryAuthorityName"
            v-model="nameOfRegulatoryAuthority"
            label="Name of regulatory authority"
            maxlength="100"
            :rules="[Rules.required('Enter the name of your regulatory authority')]"
          ></EceTextField>
        </v-col>
      </v-row>
      <v-row>
        <v-col md="8" lg="6" xl="4">
          <EceTextField
            id="txtRegulatoryAuthorityEmail"
            v-model="emailOfRegulatoryAuthority"
            label="Email of regulatory authority"
            maxlength="100"
            :rules="[Rules.required('Enter the email of your regulatory authority'), Rules.email()]"
          ></EceTextField>
        </v-col>
      </v-row>
      <v-row>
        <v-col md="8" lg="6" xl="4">
          <EceTextField
            id="txtRegulatoryAuthorityPhoneNumber"
            v-model="phoneOfRegulatoryAuthority"
            label="Phone number of regulatory authority (optional)"
            :rules="[Rules.phoneNumber()]"
          ></EceTextField>
        </v-col>
      </v-row>
      <v-row>
        <v-col md="8" lg="6" xl="4">
          <EceTextField
            id="txtRegulatoryAuthorityWebsite"
            v-model="websiteOfRegulatoryAuthority"
            label="Website of regulatory authority (optional)"
            :rules="[Rules.website()]"
          ></EceTextField>
        </v-col>
      </v-row>
      <v-row>
        <v-col md="8" lg="6" xl="4">
          <EceTextField
            id="txtRegulatoryAuthorityValidation"
            v-model="onlineCertificateValidationToolOfRegulatoryAuthority"
            label="Online certificate validation tool of regulatory authority (optional)"
            :rules="[]"
          ></EceTextField>
        </v-col>
      </v-row>
      <br />
      <ECEHeader title="Certification information" />
      <br />
      <v-row>
        <v-col md="8" lg="6" xl="4">
          <p>What is your certification status?</p>
          <v-radio-group id="certificateStatusRadio" v-model="certificateStatus" :rules="[Rules.requiredRadio('Select the status of your certification')]">
            <v-radio label="Valid" :value="'valid'"></v-radio>
            <v-radio label="Expired" :value="'expired'"></v-radio>
          </v-radio-group>
        </v-col>
      </v-row>
      <v-row>
        <v-col md="8" lg="6" xl="4">
          <EceTextField
            id="certificateTitle"
            v-model="certificateTitle"
            label="Certificate title"
            :rules="[Rules.required('Enter the title of your certificate')]"
          ></EceTextField>
        </v-col>
      </v-row>
      <v-row>
        <v-col md="8" lg="6" xl="4">
          <EceDateInput
            id="certificationIssueDate"
            ref="issueDateInput"
            v-model="issueDate"
            :rules="[Rules.required('Enter the issue date of your certificate'), Rules.futureDateNotAllowedRule()]"
            label="Issue date"
            :max="today"
            @update:model-value="validateDates"
            append-inner-icon="mdi-calendar"
          ></EceDateInput>
        </v-col>
      </v-row>
      <v-row>
        <v-col md="8" lg="6" xl="4">
          <EceDateInput
            id="certificationExpiryDate"
            ref="expiryDateInput"
            v-model="expiryDate"
            :rules="[Rules.futureDateNotAllowedRule(), Rules.dateBeforeRule(issueDate || '')]"
            label="Expiry date"
            :max="today"
            @update:model-value="validateDates"
            append-inner-icon="mdi-calendar"
          ></EceDateInput>
        </v-col>
      </v-row>
      <v-row>
        <v-col>
          <p>What name is shown on your certificate?</p>
        </v-col>
      </v-row>
      <v-row>
        <v-col md="8" lg="6" xl="4">
          <v-radio-group
            id="radioNameOnInternationalCertification"
            v-model="previousNameRadio"
            :rules="[Rules.requiredRadio('Enter your name as it appears on your certificate')]"
            @update:model-value="previousNameRadioChanged"
          >
            <v-radio v-for="(step, index) in applicantNameRadioOptions" :key="index" :label="step.label" :value="step.value"></v-radio>
          </v-radio-group>
        </v-col>
      </v-row>
      <div v-if="previousNameRadio === 'other'">
        <v-row>
          <v-col md="8" lg="6" xl="4">
            <EceTextField v-model="otherFirstName" label="First name on transcript" variant="outlined" color="primary" maxlength="100"></EceTextField>
          </v-col>
        </v-row>
        <v-row>
          <v-col md="8" lg="6" xl="4">
            <EceTextField v-model="otherMiddleName" label="Middle name(s) on transcript (optional)" maxlength="100"></EceTextField>
          </v-col>
        </v-row>
        <v-row>
          <v-col md="8" lg="6" xl="4">
            <EceTextField
              v-model="otherLastName"
              :rules="[Rules.required('Enter your last name')]"
              label="Last name on transcript"
              maxlength="100"
            ></EceTextField>
          </v-col>
        </v-row>
      </div>
      <v-row>
        <v-col>
          <h3>Upload a copy of your certificate</h3>
        </v-col>
      </v-row>
      <v-row>
        <v-col>
          <FileUploader
            :allow-multiple-files="false"
            :max-number-of-files="1"
            :show-add-file-button="generateUserFileArray.length === 0"
            :user-files="generateUserFileArray"
            :delete-file-from-temp-when-removed="false"
            @update:files="handleFileUpdate"
            @delete:file="handleFileDelete"
          />
        </v-col>
        <!-- :rules="[Rules.atLeastOneOptionRequired('Upload a certificate or document that shows you completed the course or workshop')]" -->
      </v-row>
    </v-form>

    <v-row class="mt-10">
      <v-col>
        <v-row justify="start" class="ml-1">
          <v-btn
            id="btnSaveInternationalCertification"
            rounded="lg"
            color="primary"
            class="mr-2"
            @click="handleSubmit"
            :loading="loadingStore.isLoading('icra_put')"
          >
            Save certification
          </v-btn>
          <v-btn rounded="lg" variant="outlined" @click="handleCancel" :loading="loadingStore.isLoading('icra_put')">Cancel</v-btn>
        </v-row>
      </v-col>
    </v-row>
  </div>
  <!-- List view -->
  <div v-else>
    else
    <v-row>
      <v-col>
        <p>Provide evidence of your international certification. Your certificate(s) must:</p>
        <br />
        <ul class="ml-10">
          <li>Be issued by a country or region that regulates the early childhood education profession</li>
          <li>Be in good standing</li>
          <li>Be valid (you can provide expired certification, but you must also provide at least one valid certification) </li>
        </ul>
        <br />
        <p>
          Note: The ECE Registry will contact the international regulatory authority to verify the information you have provided and confirm that your
          certificate(s) are in good standing.
        </p>
      </v-col>
    </v-row>
    <v-row v-if="modelValue && modelValue.length > 0" v-for="(internationalCertification, index) in modelValue">
      <v-col sm="12" md="10" lg="8" xl="6">
        <InternationalCertificationCard
          :key="index"
          :international-certification="internationalCertification"
          @edit="handleEdit"
          @delete="(internationalCertification) => handleDelete(internationalCertification, index)"
        />
      </v-col>
    </v-row>
    <!-- this prevents form from proceeding if rules are not met -->
    <v-input
      class="mt-6"
      :model-value="modelValue"
      :rules="[
        (v) =>
          //if user has 4 certificates and they are all expired they cannot proceed
          hasValidCertificate ||
          v.length < MAX_NUM_CERTIFICATIONS ||
          `You have entered the maximum number of certifications. You must replace one of your entries with a valid certificate to proceed.`,
        (v) =>
          (v && v.some((certificate: any) => certificate.certificateStatus === 'Valid')) ||
          `You provided an expired certificate. To continue, you must also provide a valid certificate.`,
      ]"
      auto-hide="auto"
    ></v-input>
    <v-row v-if="showAddInternationalCertificationButton">
      <v-col sm="12" md="10" lg="8" xl="6">
        <v-btn
          id="btnAddInternationalCertification"
          prepend-icon="mdi-plus"
          rounded="lg"
          color="primary"
          @click="handleAddInternationalCertification"
          :loading="loadingStore.isLoading('icra_put')"
        >
          Add certification
        </v-btn>
      </v-col>
    </v-row>
    <!-- callouts and optional messages -->
    <v-row>
      <v-col>
        <p v-if="hasValidCertificate && modelValue && modelValue.length < MAX_NUM_CERTIFICATIONS">
          No additional certifications may be added. You provided the required certifications. After you submit your application, the Registry will review and
          verify your certifications and contact you for additional information if needed.
        </p>
        <Callout v-if="modelValue && modelValue.length >= MAX_NUM_CERTIFICATIONS && hasValidCertificate" type="warning" title="Max limit reached">
          {{
            `You have reached the limit of ${MAX_NUM_CERTIFICATIONS} certifications. You can proceed to submit your application. The Registry will contact you to provide additional
          certifications if needed.`
          }}
        </Callout>
      </v-col>
    </v-row>
  </div>
</template>

<script lang="ts">
import { DateTime } from "luxon";
import { mapWritableState } from "pinia";
import { defineComponent } from "vue";
import type { VForm, VInput } from "vuetify/components";

import EceDateInput from "@/components/inputs/EceDateInput.vue";
import EceTextField from "@/components/inputs/EceTextField.vue";
import Callout from "@/components/Callout.vue";
import type { FileItem } from "@/components/UploadFileItem.vue";
import { useAlertStore } from "@/store/alert";
import { useIcraStore } from "@/store/icra";
import { useCertificationStore } from "@/store/certification";
import { useWizardStore } from "@/store/wizard";
import { useLoadingStore } from "@/store/loading";
import { useConfigStore } from "@/store/config";
import { useUserStore } from "@/store/user";
import type { Components } from "@/types/openapi";
import { formatDate } from "@/utils/format";
import { isNumber } from "@/utils/formInput";
import * as Rules from "@/utils/formRules";
import { parseHumanFileSize, removeElementByIndex, replaceElementByIndex } from "@/utils/functions";

import ECEHeader from "../ECEHeader.vue";
import FileUploader from "../FileUploader.vue";
import InternationalCertificationCard from "../InternationalCertificationCard.vue";
import ProgressBar from "../ProgressBar.vue";

const MAX_NUM_CERTIFICATIONS = 4;

interface RadioOptions {
  value: any;
  label: string;
}

interface InternationalCertificationData {
  //name fields
  previousNameRadio?: any; //TODO not supposed to be optional
  newFilesWithData?: FileItem[];
  //other fields
  internationalCertificationFormMode?: "add" | "edit" | undefined; //TODO not supposed to be optional
}

export interface InternationalCertificationExtended extends Components.Schemas.InternationalCertification {
  newFilesWithData?: FileItem[];
}

export default defineComponent({
  name: "EceInternationalCertification",
  components: { ProgressBar, FileUploader, InternationalCertificationCard, EceDateInput, EceTextField, Callout, ECEHeader },
  props: {
    modelValue: {
      type: Object as () => InternationalCertificationExtended[],
      required: true,
    },
  },
  emits: {
    "update:model-value": (_internationalCertificationData: Components.Schemas.InternationalCertification[]) => true, //TODO change type to InternationalCertificationExtended[]
  },
  setup: () => {
    const alertStore = useAlertStore();
    const wizardStore = useWizardStore();
    const icraStore = useIcraStore();
    const certificationStore = useCertificationStore();
    const loadingStore = useLoadingStore();
    const configStore = useConfigStore();
    const userStore = useUserStore();

    return {
      alertStore,
      wizardStore,
      icraStore,
      certificationStore,
      loadingStore,
      configStore,
      userStore,
      formatDate,
      Rules,
      MAX_NUM_CERTIFICATIONS,
    };
  },
  data(): InternationalCertificationData & InternationalCertificationExtended {
    return {
      //international certification
      id: "",
      countryId: undefined,
      nameOfRegulatoryAuthority: "",
      emailOfRegulatoryAuthority: "",
      phoneOfRegulatoryAuthority: "",
      websiteOfRegulatoryAuthority: "",
      onlineCertificateValidationToolOfRegulatoryAuthority: "",
      certificateStatus: "Expired",
      certificateTitle: "",
      issueDate: "",
      expiryDate: "",
      //name fields
      previousNameRadio: undefined,
      otherFirstName: "",
      otherMiddleName: "",
      otherLastName: "",
      // isNameUnverified: false, TODO needs name verification
      //other data

      internationalCertificationFormMode: undefined,
    };
  },
  computed: {
    ...mapWritableState(useWizardStore, { mode: "listComponentMode" }),

    today() {
      return formatDate(DateTime.now().toString());
    },
    applicantNameRadioOptions(): RadioOptions[] {
      let legalNameRadioOptions: RadioOptions[] = [
        {
          label: this.userStore.legalName,
          value: { firstName: this.userStore.firstName || null, middleName: this.userStore.middleName || null, lastName: this.userStore.lastName || null },
        },
      ];
      return [...legalNameRadioOptions, ...this.previousNameRadioOptions];
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
    previousNameRadioOptions(): RadioOptions[] {
      let radioOptions: RadioOptions[] = this.userStore.verifiedPreviousNames.map((previousName) => {
        let displayLabel = previousName.firstName ?? "";
        if (previousName.middleName) {
          displayLabel += ` ${previousName.middleName}`;
        }
        displayLabel += ` ${previousName.lastName}`;
        return { label: displayLabel, value: { firstName: previousName.firstName, middleName: previousName.middleName, lastName: previousName.lastName } };
      });

      radioOptions.push({ label: "Other name", value: "other" });
      return radioOptions;
    },
    showAddInternationalCertificationButton() {
      return this.modelValue && this.modelValue.length < MAX_NUM_CERTIFICATIONS && !this.hasValidCertificate;
    },
    hasValidCertificate() {
      return this.modelValue && this.modelValue.some((certificate) => certificate.certificateStatus === "Valid");
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
    handleAddInternationalCertification() {
      // Reset the form fields
      this.resetFormData();
      this.mode = "add";
      this.internationalCertificationFormMode = "add";
      window.scroll(0, 0);
    },
    handleEdit(internationalCertification: InternationalCertificationExtended) {
      // Set the form fields to component data
      this.id = internationalCertification.id;
      this.countryId = internationalCertification.countryId;
      this.nameOfRegulatoryAuthority = internationalCertification.nameOfRegulatoryAuthority;
      this.emailOfRegulatoryAuthority = internationalCertification.emailOfRegulatoryAuthority;
      this.phoneOfRegulatoryAuthority = internationalCertification.phoneOfRegulatoryAuthority;
      this.websiteOfRegulatoryAuthority = internationalCertification.websiteOfRegulatoryAuthority;
      this.onlineCertificateValidationToolOfRegulatoryAuthority = internationalCertification.onlineCertificateValidationToolOfRegulatoryAuthority;
      this.certificateStatus = internationalCertification.certificateStatus;
      this.certificateTitle = internationalCertification.certificateTitle;
      this.issueDate = internationalCertification.issueDate;
      this.expiryDate = internationalCertification.expiryDate;
      this.otherFirstName = internationalCertification.otherFirstName;
      this.otherMiddleName = internationalCertification.otherMiddleName;
      this.otherLastName = internationalCertification.otherLastName;
      this.isNameUnverified = internationalCertification.isNameUnverified;

      //set the radio button for previous names and field buttons correctly
      if (internationalCertification.isNameUnverified) {
        let index = this.applicantNameRadioOptions.findIndex((option) => option.value === "other");
        this.previousNameRadio = this.applicantNameRadioOptions[index]?.value;
      } else {
        let index = this.applicantNameRadioOptions.findIndex(
          (option) =>
            option.value?.firstName === internationalCertification.otherFirstName &&
            option.value?.lastName === internationalCertification.otherLastName &&
            option.value?.middleName === internationalCertification.otherMiddleName,
        );
        this.previousNameRadio = this.applicantNameRadioOptions[index]?.value;
      }

      this.mode = "add";
      this.internationalCertificationFormMode = "edit";
    },
    async handleDelete(_internationalCertification: InternationalCertificationExtended, index: number) {
      this.$emit("update:model-value", removeElementByIndex(this.modelValue, index));

      // await this.icraStore.saveDraft();
      //we need to update wizardData with the latest information to avoid creating duplicate new entries
      // await this.wizardStore.initializeWizard(this.icraStore.applicationConfiguration, this.icraStore.draftApplication);

      this.alertStore.setSuccessAlert("You have deleted your international certification.");
    },
    async handleSubmit() {
      const { valid } = await (this.$refs.internationalCertificationForm as VForm).validate();

      if (valid) {
        const newInternationalCertification: InternationalCertificationExtended = {
          id: this.id, //empty if we are adding
          countryId: this.countryId,
          nameOfRegulatoryAuthority: this.nameOfRegulatoryAuthority,
          emailOfRegulatoryAuthority: this.emailOfRegulatoryAuthority,
          phoneOfRegulatoryAuthority: this.phoneOfRegulatoryAuthority,
          websiteOfRegulatoryAuthority: this.websiteOfRegulatoryAuthority,
          onlineCertificateValidationToolOfRegulatoryAuthority: this.onlineCertificateValidationToolOfRegulatoryAuthority,
          certificateStatus: this.certificateStatus,
          certificateTitle: this.certificateTitle,
          issueDate: this.issueDate,
          expiryDate: this.expiryDate,
          otherFirstName: this.otherFirstName,
          otherMiddleName: this.otherMiddleName,
          otherLastName: this.otherLastName,
          isNameUnverified: this.isNameUnverified,
        };
        let updatedModelValue = this.modelValue?.slice() || []; //create a copy of the array

        if (this.internationalCertificationFormMode === "edit") {
          const indexOfEditedInternationalCertification = updatedModelValue.findIndex((certification) => certification.id === newInternationalCertification.id);
          updatedModelValue = replaceElementByIndex(updatedModelValue, indexOfEditedInternationalCertification, newInternationalCertification);
        } else if (this.internationalCertificationFormMode === "add") {
          updatedModelValue.push(newInternationalCertification);
        }

        this.$emit("update:model-value", updatedModelValue);

        await this.icraStore.saveDraft();
        //we need to update wizardData with the latest information to avoid creating duplicate new entries
        // await this.wizardStore.initializeWizard(this.icraStore.applicationConfiguration, this.icraStore.draftApplication);

        this.resetFormData();

        this.mode = "list";
        this.alertStore.setSuccessAlert(
          newInternationalCertification.id
            ? "You have successfully edited your international certification."
            : "You have successfully added your international certification.",
        );

        window.scroll(0, 0);
      } else {
        this.alertStore.setFailureAlert("You must enter all required fields in the valid format.");
      }
    },
    handleFileUpdate(filesArray: FileItem[]) {
      this.newFilesWithData = []; // Reset attachments
      if (filesArray && filesArray.length > 0) {
        for (let i = 0; i < filesArray.length; i++) {
          let areAttachedFilesValid = true;
          let isFileUploadInProgress = false;
          const file = filesArray[i] as FileItem;

          // Check for file errors
          if (file.fileErrors && file.fileErrors.length > 0) {
            areAttachedFilesValid = false;
          }

          // Check if file is still uploading
          else if (file.progress < 101) {
            isFileUploadInProgress = true;
          }

          // If file is valid and fully uploaded, add to attachments
          if (areAttachedFilesValid && !isFileUploadInProgress && file.storageFolder === "temporary") {
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
      //international certification
      this.id = "";
      this.countryId = undefined;
      this.nameOfRegulatoryAuthority = "test";
      this.emailOfRegulatoryAuthority = "test@gmail.com";
      this.phoneOfRegulatoryAuthority = "60464646846468";
      this.websiteOfRegulatoryAuthority = "";
      this.onlineCertificateValidationToolOfRegulatoryAuthority = "";
      this.certificateStatus = "Expired";
      this.certificateTitle = "aweoigwaogi";
      this.issueDate = "";
      this.expiryDate = "";
      //name fields
      this.previousNameRadio = undefined;
      this.otherFirstName = "";
      this.otherMiddleName = "";
      this.otherLastName = "";
      this.isNameUnverified = false;
      //selection

      this.internationalCertificationFormMode = undefined;
    },
    async validateDates() {
      if (this.issueDate && this.expiryDate) {
        await (this.$refs.issueDateInput as VInput).validate();
        await (this.$refs.expiryDateInput as VInput).validate();
      }
    },
    previousNameRadioChanged(option: any) {
      if (option === "other") {
        this.otherFirstName = "";
        this.otherMiddleName = "";
        this.otherLastName = "";
        this.isNameUnverified = true;
      } else {
        this.otherFirstName = option.firstName;
        this.otherMiddleName = option.middleName;
        this.otherLastName = option.lastName;
        this.isNameUnverified = false;
      }
    },
  },
});
</script>
