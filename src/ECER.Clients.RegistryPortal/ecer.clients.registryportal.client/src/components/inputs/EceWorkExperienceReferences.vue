<template>
  <v-row no-gutters>
    <v-col v-if="mode == 'add'" md="8" lg="6" xl="4">
      <h2>{{ `${id ? "Edit" : "Add"} work experience reference` }}</h2>
      <v-form ref="addWorkExperienceReferenceForm" validate-on="input" class="mt-6">
        <v-row>
          <v-col cols="12">
            <EceTextField
              id="txtWorkReferenceLastName"
              v-model="lastName"
              :rules="[Rules.required('Enter your reference\'s last name')]"
              label="Reference last name"
              maxlength="100"
            ></EceTextField>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12">
            <EceTextField id="txtWorkReferenceFirstName" v-model="firstName" label="Reference first name" maxlength="100"></EceTextField>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12">
            <EceTextField
              id="txtWorkReferenceEmail"
              v-model="email"
              :rules="[Rules.required(), Rules.email('Enter your reference\'s email in the format \'name@email.com\'')]"
              label="Reference email"
              maxlength="200"
            ></EceTextField>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12">
            <EceTextField
              id="txtWorkReferencePhoneNumber"
              v-model="phoneNumber"
              :rules="[Rules.phoneNumber('Enter your reference\'s valid phone number')]"
              label="Reference phone number (Optional)"
            ></EceTextField>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="12">
            <EceTextField
              id="txtWorkReferenceHours"
              v-model.number="hours"
              :rules="[Rules.required('Enter your work experience hours')]"
              type="number"
              label="Work experience hours observed by reference"
              maxlength="8"
              @keypress="isNumber($event)"
            ></EceTextField>
          </v-col>
        </v-row>

        <v-row justify="start" class="ml-1 my-10">
          <v-btn
            id="btnSaveWorkReference"
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
      </v-form>
    </v-col>
    <div v-else-if="mode == 'list'" class="w-100">
      <div class="d-flex flex-column ga-3 my-6">
        <h3 v-if="applicationStore.isDraftApplicationRenewal">
          {{ hoursRequired }} hours of work experience related to the field of early childhood education is required.
        </h3>
        <h3 v-else>{{ hoursRequired }} hours of work experience is required.</h3>
        <p>Your hours:</p>
        <ul v-if="applicationStore.isDraftApplicationRenewal" class="ml-10">
          <li>Be related to the field of early childhood education</li>
          <li v-if="certificationStore.latestCertificateStatus != 'Expired'">
            Have been completed within the term of your current certificate (between
            {{ formatDate(certificationStore.latestCertification?.effectiveDate ?? "", "LLLL d, yyyy") }} and
            {{ formatDate(certificationStore.latestCertification?.expiryDate ?? "", "LLLL d, yyyy") }})
          </li>
          <li v-else>Have been completed within the last 5 years</li>
        </ul>
        <ul v-if="!applicationStore.isDraftApplicationRenewal" class="ml-10">
          <li>Must have been completed after you started your education and within the last 5 years</li>
          <li>Cannot include hours worked as part of your education on your practicum or work placement</li>
          <li>Can be work or volunteer hours</li>
        </ul>
        <p v-if="applicationStore.isDraftApplicationRenewal">If you worked at multiple locations, add a reference for each location.</p>
        <p v-if="!applicationStore.isDraftApplicationRenewal">If your hours were completed at multiple locations under the supervision of multiple ECEs:</p>
        <ul v-if="!applicationStore.isDraftApplicationRenewal" class="ml-10">
          <li>Provide a reference from each person who supervised your hours</li>
          <li>You may enter up to 6 references</li>
        </ul>
      </div>

      <v-col v-if="hasDuplicateReferences" sm="12" md="10" lg="8" xl="6">
        <Alert type="error">
          <p class="small">Your work experience reference(s) cannot be the same as your character reference</p>
        </Alert>
      </v-col>
      <v-col sm="12" md="10" lg="8" xl="6" class="my-6">
        <ProgressBar :total-hours="totalHours" :hours-required="hoursRequired" />
      </v-col>
      <v-col sm="12" md="10" lg="8" xl="6">
        <WorkExperienceReferenceList :references="modelValue" @edit="handleEdit" @delete="handleDelete" />
      </v-col>
      <v-row class="my-6">
        <v-col sm="12" md="10" lg="8" xl="6">
          <Callout v-if="count >= 6 && totalHours < hoursRequired" title="Max limit reached" type="warning" class="mt-10">
            You reached the limit of 6 work experience references but do not meet the {{ hoursRequired }} hour requirement. You can still proceed to submit your
            application. The Registry will contact you to provide additional references for the remaining hours.
          </Callout>
          <p v-else-if="totalHours >= hoursRequired">
            No additional work references may be added. You provided the required hours. After you submit your application, we’ll email the work references to
            complete their reference. If needed, we'll contact you for additional information.
          </p>
          <v-btn
            id="btnAddWorkExperienceReference"
            v-else
            prepend-icon="mdi-plus"
            rounded="lg"
            color="primary"
            :disabled="isDisabled"
            @click="handleAddReference"
            :loading="loadingStore.isLoading('draftapplication_put')"
          >
            Add reference
          </v-btn>
        </v-col>
      </v-row>
      <!-- this prevents form from proceeding if rules are not met -->
      <v-input
        class="mt-6"
        auto-hide="auto"
        :model-value="modelValue"
        :rules="[
          !hasDuplicateReferences,
          totalHours >= hoursRequired ||
            count >= 6 ||
            `You need ${hoursRequired} hours of work experience. You need to add ${hoursRequired - totalHours} more hours.`,
        ]"
      ></v-input>
    </div>
  </v-row>
</template>

<script lang="ts">
import { mapWritableState } from "pinia";
import { defineComponent } from "vue";

import Alert from "@/components/Alert.vue";
import Callout from "@/components/Callout.vue";
import EceTextField from "@/components/inputs/EceTextField.vue";
import WorkExperienceReferenceList, { type WorkExperienceReferenceData } from "@/components/WorkExperienceReferenceList.vue";
import { useAlertStore } from "@/store/alert";
import { useApplicationStore } from "@/store/application";
import { useCertificationStore } from "@/store/certification";
import { useWizardStore } from "@/store/wizard";
import { useLoadingStore } from "@/store/loading";
import type { Components } from "@/types/openapi";
import { WorkExperienceType } from "@/utils/constant";
import { formatDate } from "@/utils/format";
import { isNumber } from "@/utils/formInput";
import * as Rules from "@/utils/formRules";

import ProgressBar from "../ProgressBar.vue";

export default defineComponent({
  name: "EceEdducation",
  components: { WorkExperienceReferenceList, ProgressBar, Alert, Callout, EceTextField },
  props: {
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
    const certificationStore = useCertificationStore();
    const loadingStore = useLoadingStore();

    return {
      alertStore,
      wizardStore,
      applicationStore,
      certificationStore,
      loadingStore,
    };
  },
  data: function () {
    return {
      clientId: "",
      id: null as string | null,
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
      return this.count >= 6 || this.totalHours >= this.hoursRequired;
    },
    hoursRequired() {
      //edge case for renewals > 5 year expired should be 500 hours otherwise all renewals are 400 hours
      if (
        this.applicationStore.isDraftApplicationRenewal &&
        this.certificationStore.latestIsEceFiveYear &&
        this.certificationStore.latestExpiredMoreThan5Years
      ) {
        return 500;
      }
      return this.applicationStore.isDraftApplicationRenewal ? 400 : 500;
    },
    totalHours() {
      return Object.values(this.modelValue).reduce((acc, reference) => {
        return acc + (reference.hours as number);
      }, 0);
    },
    count() {
      return Object.keys(this.modelValue).length;
    },
    newClientId() {
      return Object.keys(this.modelValue).length + 1;
    },
    hasDuplicateReferences() {
      if (Object.values(this.wizardStore.wizardData.referenceList).length === 0 || this.wizardStore.wizardData.characterReferences.length === 0) {
        return false;
      }

      const refSet = new Set<string>();

      for (const ref of this.wizardStore.wizardData.characterReferences) {
        refSet.add(`${ref.firstName} ${ref.lastName}`);
      }

      for (const ref of Object.values(this.wizardStore.wizardData.referenceList) as [Components.Schemas.WorkExperienceReference]) {
        if (refSet.has(`${ref.firstName} ${ref.lastName}`)) {
          return true;
        }
      }

      return false;
    },
  },
  mounted() {
    this.mode = "list";
  },
  methods: {
    formatDate,
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
          hours: parseInt(this.hours!.toString()),
          type: this.hoursRequired === 500 ? WorkExperienceType.IS_500_Hours : WorkExperienceType.IS_400_Hours,
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
        const message = this.modelValue[clientId] ? "You have successfully edited your reference." : "You have successfully added your reference.";

        await this.applicationStore.saveDraft();
        //we need to update wizardData with the latest information to avoid creating duplicate new entries
        await this.wizardStore.initializeWizard(this.applicationStore.applicationConfiguration, this.applicationStore.draftApplication);

        this.alertStore.setSuccessAlert(message);

        // Change mode to list
        this.mode = "list";
      } else {
        this.alertStore.setFailureAlert("You must enter all required fields in the valid format.");
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
      this.firstName = referenceData.reference.firstName ?? "";
      this.lastName = referenceData.reference.lastName ?? "";
      this.email = referenceData.reference.emailAddress ?? "";
      this.phoneNumber = referenceData.reference.phoneNumber ?? "";
      this.hours = referenceData.reference.hours ?? null;
      // Change mode to add
      this.mode = "add";
    },
    async handleDelete(referenceId: string | number) {
      //Remove the entry from the modelValue

      if (referenceId in this.modelValue) {
        // Create a copy of modelValue
        const updatedModelValue = { ...this.modelValue };
        // Delete the entry from the copied object
        delete updatedModelValue[referenceId];
        // Emit the updated modelValue
        this.$emit("update:model-value", updatedModelValue);
      }

      await this.applicationStore.saveDraft();
      //we need to update wizardData with the latest information to avoid creating duplicate new entries
      await this.wizardStore.initializeWizard(this.applicationStore.applicationConfiguration, this.applicationStore.draftApplication);

      await this.alertStore.setSuccessAlert("You have deleted your reference.");
    },
    resetFormData() {
      this.id = null;
      this.firstName = "";
      this.lastName = "";
      this.email = "";
      this.phoneNumber = "";
      this.hours = null;
    },
  },
});
</script>
