<template>
  <!-- add view -->
  <div v-if="icraWorkExperienceEligibilityFormMode === 'edit' || icraWorkExperienceEligibilityFormMode === 'add'">
    <p>{{ icraWorkExperienceEligibilityFormMode }}</p>
    <ECEHeader title="Employment experience references" />
    <br />
    <p>
      The ECE Registry will contact your references to verify your employment. Once we receive your submission, we will send an email to these people containing
      a link to an online reference form.
    </p>
    <br />
    <p>
      If you are eligible for this application pathway, your references will be asked to complete a competencies assessment to verify that you have the
      knowledge, skills and abilities to work as an ECE in British Columbia.
    </p>
    <v-form ref="icraWorkExperienceEligibilityForm" class="mt-6">
      <v-row>
        <v-col md="8" lg="6" xl="4">
          <EceTextField
            id="txtWorkReferenceLastName"
            v-model="lastName"
            :rules="[Rules.required('Enter your reference\'s last name')]"
            label="Last name"
            maxlength="100"
          ></EceTextField>
        </v-col>
      </v-row>
      <v-row>
        <v-col md="8" lg="6" xl="4">
          <EceTextField id="txtWorkReferenceFirstName" v-model="firstName" label="first name" maxlength="100"></EceTextField>
        </v-col>
      </v-row>
      <v-row>
        <v-col md="8" lg="6" xl="4">
          <EceTextField
            id="txtWorkReferenceEmail"
            v-model="emailAddress"
            :rules="[Rules.required(), Rules.email('Enter your reference\'s email in the format \'name@email.com\'')]"
            label="Reference email"
            maxlength="200"
          ></EceTextField>
        </v-col>
      </v-row>
      <v-row>
        <v-col md="8" lg="6" xl="4">
          <EceTextField
            id="txtWorkReferencePhoneNumber"
            v-model="phoneNumber"
            :rules="[Rules.phoneNumber('Enter your reference\'s valid phone number')]"
            label="Phone number (optional)"
          ></EceTextField>
        </v-col>
      </v-row>
    </v-form>
    <v-row class="mt-10">
      <v-col>
        <v-row justify="start" class="ml-1">
          <v-btn
            id="btnSaveReference"
            rounded="lg"
            color="primary"
            class="mr-2"
            @click="handleSubmit"
            :loading="loadingStore.isLoading('draftapplication_put')"
          >
            Save reference
          </v-btn>
          <v-btn rounded="lg" variant="outlined" @click="handleCancel" :loading="loadingStore.isLoading('draftapplication_put')">Cancel</v-btn>
        </v-row>
      </v-col>
    </v-row>
  </div>
  <!-- List view -->
  <div v-else>
    <v-row>
      <v-col>
        <h2>Provide proof of 2 years of independent ECE practice</h2>
        <br />
        <p>Your employment experience must:</p>
        <ul class="ml-10">
          <li>Have been completed within the last 5 years</li>
          <li>Not overlap (any overlapping employment experience will only be counted once)</li>
          <li>Have been completed while holding valid ECE certification</li>
        </ul>
        <br />
        <p>If your employment experience was completed at multiple locations:</p>
        <ul class="ml-10">
          <li>Provide a reference from each person</li>
          <li>You may enter up to 6 references</li>
        </ul>
      </v-col>
    </v-row>
    <v-row v-if="modelValue?.length > 0" v-for="(reference, index) in modelValue">
      <v-col sm="12" md="10" lg="8" xl="6">
        <EceIcraWorkExperienceEligibilityCard
          :key="index"
          :reference="reference"
          @edit="handleEdit"
          @delete="(internationalCertification) => handleDelete(internationalCertification, index)"
        />
      </v-col>
    </v-row>
    <v-row v-else-if="modelValue?.length === 0">
      <v-col sm="12" md="10" lg="8" xl="6">
        <p>No work experience reference added yet</p>
      </v-col>
    </v-row>
    <!-- this prevents form from proceeding if rules are not met -->
    <v-input class="mt-6" :model-value="modelValue" :rules="[]" auto-hide="auto"></v-input>
    <v-row v-if="showAddInternationalCertificationButton">
      <v-col sm="12" md="10" lg="8" xl="6">
        <v-btn
          id="btnAddInternationalCertification"
          prepend-icon="mdi-plus"
          rounded="lg"
          color="primary"
          :disabled="isDisabled"
          @click="handleAddInternationalCertification"
          :loading="loadingStore.isLoading('draftapplication_put')"
        >
          Add reference
        </v-btn>
      </v-col>
    </v-row>
    <!-- callouts and optional messages -->
    <v-row>
      <v-col>
        <p v-if="modelValue?.length < 4">
          No additional certifications may be added. You provided the required certifications. After you submit your application, the Registry will review and
          verify your certifications and contact you for additional information if needed.
        </p>
        <Callout v-if="modelValue?.length >= 4" type="warning" title="Max limit reached">
          You have reached the limit of 4 certifications. You can proceed to submit your application. The Registry will contact you to provide additional
          certifications if needed.
        </Callout>
      </v-col>
    </v-row>
  </div>
</template>

<script lang="ts">
import { DateTime } from "luxon";
import { mapWritableState } from "pinia";
import { defineComponent, type Component } from "vue";
import type { VForm, VInput } from "vuetify/components";

import EceDateInput from "@/components/inputs/EceDateInput.vue";
import EceTextField from "@/components/inputs/EceTextField.vue";
import Callout from "@/components/Callout.vue";
import type { FileItem } from "@/components/UploadFileItem.vue";
import { useAlertStore } from "@/store/alert";
import { useApplicationStore } from "@/store/application";
import { useCertificationStore } from "@/store/certification";
import { useWizardStore } from "@/store/wizard";
import { useLoadingStore } from "@/store/loading";
import { useConfigStore } from "@/store/config";
import { useUserStore } from "@/store/user";
import type { Components, Country } from "@/types/openapi";
import { formatDate } from "@/utils/format";
import { isNumber } from "@/utils/formInput";
import * as Rules from "@/utils/formRules";
import { parseHumanFileSize, removeElementByIndex, replaceElementByIndex } from "@/utils/functions";

import ECEHeader from "../ECEHeader.vue";
import FileUploader from "../FileUploader.vue";
import EceIcraWorkExperienceEligibilityCard from "../EceIcraWorkExperienceEligibilityCard.vue";
import ProgressBar from "../ProgressBar.vue";
import type { CpuInfo } from "os";

interface RadioOptions {
  value: any;
  label: string;
}

interface IcraWorkExperienceEligibilityData extends Components.Schemas.WorkExperienceReference {
  //other fields
  icraWorkExperienceEligibilityFormMode?: "add" | "edit" | undefined; //TODO not supposed to be optional
  modelValue?: Components.Schemas.WorkExperienceReference[];
}

export default defineComponent({
  name: "EceIcraWorkExperienceEligibility",
  components: { ProgressBar, FileUploader, EceIcraWorkExperienceEligibilityCard, EceDateInput, EceTextField, Callout, ECEHeader },
  props: {
    modelValue2: {
      type: Object as () => Components.Schemas.WorkExperienceReference[],
      required: false, //to switch to true
      default: [
        // {
        //   certificationStatus: "expired",
        //   certificationTitle: "certificate one",
        //   country: { countryId: "93dd2dc5-3d8b-ef11-8a6a-000d3af46d37", countryName: "Albania", countryCode: "AL" },
        //   expiryDate: "2025-08-20",
        //   firstName: "one",
        //   id: "1",
        //   isNameUnverified: true,
        //   issueDate: "2025-08-12",
        //   lastName: "three",
        //   middleName: "two",
        //   regulatoryAuthorityEmail: "test@gmail.com",
        //   regulatoryAuthorityName: "one",
        //   regulatoryAuthorityPhoneNumber: "1231424124",
        //   regulatoryAuthorityValidation: "online verification",
        //   regulatoryAuthorityWebsite: "https://www.google.com",
        // },
        // {
        //   certificationStatus: "expired",
        //   certificationTitle: "certificate two",
        //   country: { countryId: "93dd2dc5-3d8b-ef11-8a6a-000d3af46d37", countryName: "Albania", countryCode: "AL" },
        //   expiryDate: "2025-08-20",
        //   firstName: "one",
        //   id: "2",
        //   isNameUnverified: true,
        //   issueDate: "2025-08-12",
        //   lastName: "three",
        //   middleName: "two",
        //   regulatoryAuthorityEmail: "test@gmail.com",
        //   regulatoryAuthorityName: "two",
        //   regulatoryAuthorityPhoneNumber: "1231424124",
        //   regulatoryAuthorityValidation: "online verification",
        //   regulatoryAuthorityWebsite: "https://www.google.com",
        // },
        // {
        //   certificationStatus: "expired",
        //   certificationTitle: "certificate three",
        //   country: { countryId: "93dd2dc5-3d8b-ef11-8a6a-000d3af46d37", countryName: "Albania", countryCode: "AL" },
        //   expiryDate: "2025-08-20",
        //   firstName: "one",
        //   id: "3",
        //   isNameUnverified: true,
        //   issueDate: "2025-08-12",
        //   lastName: "three",
        //   middleName: "two",
        //   regulatoryAuthorityEmail: "test@gmail.com",
        //   regulatoryAuthorityName: "three",
        //   regulatoryAuthorityPhoneNumber: "1231424124",
        //   regulatoryAuthorityValidation: "online verification",
        //   regulatoryAuthorityWebsite: "https://www.google.com",
        // },
        // {
        //   certificationStatus: "expired",
        //   certificationTitle: "certificate four",
        //   country: { countryId: "93dd2dc5-3d8b-ef11-8a6a-000d3af46d37", countryName: "Albania", countryCode: "AL" },
        //   expiryDate: "2025-08-20",
        //   firstName: "one",
        //   id: "4",
        //   isNameUnverified: true,
        //   issueDate: "2025-08-12",
        //   lastName: "three",
        //   middleName: "two",
        //   regulatoryAuthorityEmail: "test@gmail.com",
        //   regulatoryAuthorityName: "four",
        //   regulatoryAuthorityPhoneNumber: "1231424124",
        //   regulatoryAuthorityValidation: "online verification",
        //   regulatoryAuthorityWebsite: "https://www.google.com",
        // },
      ],
    },
  },
  emits: {
    "update:model-value": (_icraWorkExperienceEligibility: Components.Schemas.WorkExperienceReference[]) => true,
  },
  setup: () => {
    const alertStore = useAlertStore();
    const wizardStore = useWizardStore();
    const applicationStore = useApplicationStore();
    const certificationStore = useCertificationStore();
    const loadingStore = useLoadingStore();
    const configStore = useConfigStore();
    const userStore = useUserStore();

    return {
      alertStore,
      wizardStore,
      applicationStore,
      certificationStore,
      loadingStore,
      configStore,
      userStore,
      formatDate,
      Rules,
    };
  },
  data(): IcraWorkExperienceEligibilityData {
    return {
      //international certification
      modelValue: undefined,
      id: "",
      lastName: "",
      firstName: "",
      emailAddress: "",
      phoneNumber: "",
      //other data
      icraWorkExperienceEligibilityFormMode: undefined,
    };
  },
  computed: {
    ...mapWritableState(useWizardStore, { mode: "listComponentMode" }),
    isDisabled() {
      return false;
    },
    showAddInternationalCertificationButton() {
      return this.modelValue && this.modelValue?.length < 6;
    },
  },

  unmounted() {
    this.mode = "list";
  },
  mounted() {
    this.modelValue = this.modelValue2 || [];
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
      this.icraWorkExperienceEligibilityFormMode = "add";
      window.scroll(0, 0);
    },
    handleEdit(reference: Components.Schemas.WorkExperienceReference) {
      // Set the form fields to component data
      this.id = reference.id;
      this.lastName = reference.lastName;
      this.firstName = reference.firstName;
      this.emailAddress = reference.emailAddress;
      this.phoneNumber = reference.phoneNumber;

      this.mode = "add";
      this.icraWorkExperienceEligibilityFormMode = "edit";
    },
    async handleDelete(_reference: Components.Schemas.WorkExperienceReference, index: number) {
      // this.$emit("update:model-value", removeElementByIndex(this.modelValue, index));
      this.modelValue = removeElementByIndex(this.modelValue, index); //TODO remove
      console.log(this.modelValue);

      // await this.applicationStore.saveDraft();
      //we need to update wizardData with the latest information to avoid creating duplicate new entries
      // await this.wizardStore.initializeWizard(this.applicationStore.applicationConfiguration, this.applicationStore.draftApplication);

      this.alertStore.setSuccessAlert("You have deleted your reference.");
    },
    async handleSubmit() {
      const { valid } = await (this.$refs.icraWorkExperienceEligibilityForm as VForm).validate();

      if (valid) {
        const newIcraWorkExperienceEligibilityReference: Components.Schemas.WorkExperienceReference = {
          id: this.id, //empty if we are adding
          lastName: this.lastName,
          firstName: this.firstName,
          emailAddress: this.emailAddress,
          phoneNumber: this.phoneNumber,
        };
        let updatedModelValue = this.modelValue.slice(); //create a copy of the array

        if (this.icraWorkExperienceEligibilityFormMode === "edit") {
          const indexOfEditedReference = updatedModelValue.findIndex((reference) => reference.id === newIcraWorkExperienceEligibilityReference.id);
          updatedModelValue = replaceElementByIndex(updatedModelValue, indexOfEditedReference, newIcraWorkExperienceEligibilityReference);
        } else if (this.icraWorkExperienceEligibilityFormMode === "add") {
          updatedModelValue.push(newIcraWorkExperienceEligibilityReference);
        }

        this.$emit("update:model-value", updatedModelValue);
        this.modelValue = updatedModelValue; // TODO remove this

        // await this.applicationStore.saveDraft();

        this.alertStore.setSuccessAlert(
          this.icraWorkExperienceEligibilityFormMode === "edit"
            ? "You have successfully edited your reference."
            : "You have successfully added your reference.",
        );

        this.resetFormData();

        this.mode = "list";

        window.scroll(0, 0);
      } else {
        this.alertStore.setFailureAlert("You must enter all required fields in the valid format.");
      }
    },
    resetFormData() {
      this.id = "";
      this.lastName = "";
      this.firstName = "";
      this.emailAddress = "";
      this.phoneNumber = "";

      this.icraWorkExperienceEligibilityFormMode = undefined;
    },
  },
});
</script>
