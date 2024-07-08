<template>
  <v-row>
    <v-col v-if="mode == 'add'" md="8" lg="6" xl="4">
      <h2 v-if="!clientId">Reference {{ newClientId }} [Up to 6]</h2>
      <h2 v-if="clientId">Edit {{ previousFullName }}</h2>
      <v-form ref="addWorkExperienceReferenceForm" validate-on="input" class="mt-6">
        <v-text-field
          v-model="lastName"
          :rules="[Rules.required('Enter your reference\'s last name')]"
          label="Reference Last Name"
          variant="outlined"
          color="primary"
          maxlength="100"
        ></v-text-field>
        <v-text-field
          v-model="firstName"
          :rules="[Rules.required('Enter your reference\'s first name')]"
          label="Reference First Name"
          variant="outlined"
          color="primary"
          maxlength="100"
          class="my-8"
        ></v-text-field>
        <v-text-field
          v-model="email"
          :rules="[Rules.required(), Rules.email('Enter your reference\'s email in the format \'name@email.com\'')]"
          label="Reference Email"
          variant="outlined"
          color="primary"
          maxlength="200"
          class="my-8"
        ></v-text-field>
        <v-text-field
          v-model="phoneNumber"
          :rules="[Rules.phoneNumber('Enter your reference\'s 10-digit phone number')]"
          label="Reference Phone Number (Optional)"
          variant="outlined"
          color="primary"
          maxlength="20"
          class="my-8"
          @keypress="isNumber($event)"
        ></v-text-field>
        <v-text-field
          v-model.number="hours"
          :rules="[Rules.required('Enter your work experience hours')]"
          type="number"
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
      <v-col sm="12" md="10" lg="8" xl="6" class="my-6">
        <p class="small">
          You must provide
          <b>500 hours of work experience.</b>
          These hours:
        </p>
        <ul class="small">
          <v-col cols="12">
            <li>
              Are counted from the date you started your education and cannot include your practicum/placement hours (hours that were a part of your education)
            </li>
            <li>Must be completed within the 5 years before your application submission</li>
            <li>Can be work or volunteer hours</li>
          </v-col>
        </ul>
        <p class="small">
          If your 500 hours were completed at more than one location and under the supervision of more than one ECE, you must provide a reference from each
          person who supervised your hours. You may enter up to six references.
        </p>
      </v-col>
      <v-col sm="12" md="10" lg="8" xl="6">
        <Alert type="info">
          <p class="small">
            The Registry will not assess an application until references have been submitted. Please make sure your reference:
            <br />
            <br />
            &#x2022; Directly supervised (observed) the hours they attest to
            <br />
            &#x2022; Can speak to your knowledge, skills, and ability (competencies) as an ECE
            <br />
            &#x2022; Has held valid Canadian ECE certification/registration during the hours they supervised (observed)
            <br />
            &#x2022; Is not the same person you provided as a character reference
          </p>
        </Alert>
      </v-col>
      <v-col v-if="totalHours >= 500" sm="12" md="10" lg="8" xl="6">
        <Alert type="info">
          <p class="small">
            You provided the required 500 hours of work experience. If needed, the Registry will contact you for additional work experience information once
            your references submit their forms.
          </p>
        </Alert>
      </v-col>
      <v-col v-if="count >= 6 && totalHours < 500" sm="12" md="10" lg="8" xl="6">
        <Alert type="warning">
          <p class="small">
            You reached the limit of six work experience references but do not meet the 500-hour requirement. You can still proceed to submit your application.
            The Registry will contact you to provide additional references for the remaining hours.
          </p>
        </Alert>
      </v-col>
      <v-col v-if="count < 6 && totalHours < 500" sm="12" md="10" lg="8" xl="6">
        <Alert type="error">
          <p class="small">You must enter 500 hours of work experience to submit your application.</p>
        </Alert>
      </v-col>
      <v-col v-if="hasDuplicateReferences" sm="12" md="10" lg="8" xl="6">
        <Alert type="error">
          <p class="small">Your work experience reference(s) cannot be the same as your character reference</p>
        </Alert>
      </v-col>
      <v-col sm="12" md="10" lg="8" xl="6" class="my-6">
        <WorkExperienceReferenceProgressBar :references="modelValue" />
      </v-col>
      <v-col sm="12" md="10" lg="8" xl="6">
        <WorkExperienceReferenceList :references="modelValue" @edit="handleEdit" @delete="handleDelete" />
      </v-col>
      <v-col cols="12" class="mt-6">
        <v-row justify="start" class="ml-1">
          <v-btn prepend-icon="mdi-plus" rounded="lg" color="alternate" :disabled="isDisabled" @click="handleAddReference">Add References</v-btn>
        </v-row>
      </v-col>
    </div>
  </v-row>
  <v-input auto-hide="auto" :model-value="modelValue" :rules="[!hasDuplicateReferences, totalHours >= 500 || count >= 6]"></v-input>
</template>

<script lang="ts">
import { mapWritableState } from "pinia";
import { defineComponent } from "vue";

import Alert from "@/components/Alert.vue";
import WorkExperienceReferenceList, { type WorkExperienceReferenceData } from "@/components/WorkExperienceReferenceList.vue";
import WorkExperienceReferenceProgressBar from "@/components/WorkExperienceReferenceProgressBar.vue";
import { useAlertStore } from "@/store/alert";
import { useApplicationStore } from "@/store/application";
import { useWizardStore } from "@/store/wizard";
import type { EceWorkExperienceReferencesProps } from "@/types/input";
import type { Components } from "@/types/openapi";
import { isNumber } from "@/utils/formInput";
import * as Rules from "@/utils/formRules";

export default defineComponent({
  name: "EceEdducation",
  components: { WorkExperienceReferenceList, WorkExperienceReferenceProgressBar, Alert },
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
    const wizardStore = useWizardStore();
    const applicationStore = useApplicationStore();

    return {
      alertStore,
      wizardStore,
      applicationStore,
    };
  },
  data: function () {
    return {
      clientId: "",
      id: null as string | null,
      previousFullName: "",
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
      return this.count >= 6 || this.totalHours >= 500;
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
      this.previousFullName = `${referenceData.reference.firstName} ${referenceData.reference.lastName}`;
      this.firstName = referenceData.reference.firstName ?? "";
      this.lastName = referenceData.reference.lastName ?? "";
      this.email = referenceData.reference.emailAddress ?? "";
      this.phoneNumber = referenceData.reference.phoneNumber ?? "";
      this.hours = referenceData.reference.hours ?? null;
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

      this.alertStore.setSuccessAlert("You have deleted your reference.");
    },
    resetFormData() {
      this.id = null;
      this.previousFullName = "";
      this.firstName = "";
      this.lastName = "";
      this.email = "";
      this.phoneNumber = "";
      this.hours = null;
    },
  },
});
</script>
