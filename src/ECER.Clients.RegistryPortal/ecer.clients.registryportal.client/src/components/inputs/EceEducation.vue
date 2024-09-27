<template>
  <div ref="addEducationComponent">
    <v-row v-if="mode === 'add'">
      <v-col>
        <h2>{{ clientId ? "Edit" : "Add" }} education</h2>
        <br />
        <p>
          You'll need to request an official transcript from your educational institution for this course or program. It must be sent to us directly from them.
        </p>
        <br />
        <p>When we receive your transcript, we will:</p>
        <ul class="ml-10">
          <li>Attach it to your application</li>
          <li>Email you to let you know we've received it</li>
        </ul>
      </v-col>
    </v-row>
    <v-row>
      <v-col v-if="mode === 'add'" md="8" lg="6" xl="4">
        <v-form ref="addEducationForm" validate-on="input">
          <v-row>
            <v-col>
              <h3>How will you provide your transcript?</h3>
            </v-col>
          </v-row>

          <v-row>
            <v-radio-group v-model="transcriptStatus" :rules="[Rules.required('Indicate the status of your transcript(s)')]" color="primary">
              <v-radio label="I have requested the official transcript from my education institution" value="requested"></v-radio>
              <v-radio
                label="The ECE Registry already has my official transcript for the course/program relevant to this application and certificate type"
                value="received"
              ></v-radio>
            </v-radio-group>
          </v-row>
          <v-row>
            <v-col>
              <h3>What program did you take?</h3>
            </v-col>
          </v-row>

          <v-row>
            <v-col>
              <v-text-field
                v-model="program"
                :rules="[Rules.required('Enter the name of your program or course')]"
                label="Name of program or course"
                variant="outlined"
                color="primary"
                maxlength="100"
              ></v-text-field>
            </v-col>
          </v-row>
          <v-row>
            <v-col>
              <v-text-field
                v-model="startYear"
                :rules="[
                  Rules.required('Enter the start date'),
                  Rules.futureDateNotAllowedRule(),
                  Rules.conditionalWrapper(
                    isDraftApplicationAssistantRenewal,
                    Rules.dateRuleRange(applicationStore.draftApplication.createdOn!, 5, 'Start date must be within the last 5 years'),
                  ),
                ]"
                label="Start date of program or course"
                type="date"
                variant="outlined"
                color="primary"
                maxlength="50"
                :max="today"
              ></v-text-field>
            </v-col>
          </v-row>
          <v-row>
            <v-col>
              <v-text-field
                v-model="endYear"
                :rules="[
                  Rules.required('Enter the end date'),
                  Rules.futureDateNotAllowedRule(),
                  Rules.conditionalWrapper(
                    isDraftApplicationAssistantRenewal,
                    Rules.dateRuleRange(applicationStore.draftApplication.createdOn!, 5, 'End date must be within the last 5 years'),
                  ),
                ]"
                label="End date of program or course"
                type="date"
                variant="outlined"
                color="primary"
                maxlength="50"
                :max="today"
              ></v-text-field>
            </v-col>
          </v-row>
          <v-row>
            <v-col>
              <h3>Where did you take it?</h3>
            </v-col>
          </v-row>
          <v-row>
            <v-col>
              <v-text-field
                v-model="school"
                :rules="[Rules.required('Enter the name of the educational institution')]"
                label="Full name of educational institution"
                variant="outlined"
                color="primary"
                maxlength="100"
              ></v-text-field>
            </v-col>
          </v-row>
          <v-row>
            <v-col>
              <v-text-field v-model="campusLocation" label="Campus Location (Optional)" variant="outlined" color="primary" maxlength="200"></v-text-field>
            </v-col>
          </v-row>
          <v-row>
            <v-col>
              <v-text-field v-model="language" label="Language of institution (optional)" variant="outlined" color="primary" maxlength="100"></v-text-field>
            </v-col>
          </v-row>
          <v-row>
            <v-col>
              <h3>Name and student number on transcript</h3>
              <br />
              <p>Make sure this exactly matches your transcript. It may cause delays if we cannot match a transcript we receive to your application.</p>
            </v-col>
          </v-row>
          <v-row>
            <v-col>
              <v-text-field
                v-model="studentNumber"
                :rules="[Rules.required('Enter your student number or ID')]"
                label="Student number or ID"
                variant="outlined"
                color="primary"
                maxlength="100"
              ></v-text-field>
            </v-col>
          </v-row>
          <br />
          <p>What name is shown on your transcript?</p>
          <br />
          <v-radio-group v-model="previousNameRadio" :rules="[Rules.requiredRadio('Select an option')]" @update:model-value="previousNameRadioChanged">
            <v-radio v-for="(step, index) in ApplicantNameRadioOptions" :key="index" :label="step.label" :value="step.value"></v-radio>
          </v-radio-group>
          <div v-if="previousNameRadio === 'other'">
            <v-row>
              <v-col>
                <v-text-field v-model="studentFirstName" label="First name on transcript" variant="outlined" color="primary" maxlength="100"></v-text-field>
              </v-col>
            </v-row>
            <v-row>
              <v-col>
                <v-text-field
                  v-model="studentMiddleName"
                  label="Middle name(s) on transcript (optional)"
                  variant="outlined"
                  color="primary"
                  maxlength="100"
                ></v-text-field>
              </v-col>
            </v-row>
            <v-row>
              <v-col>
                <v-text-field
                  v-model="studentLastName"
                  :rules="[Rules.required('Enter your last name')]"
                  label="Last name on transcript"
                  variant="outlined"
                  color="primary"
                  maxlength="100"
                ></v-text-field>
              </v-col>
            </v-row>
          </div>
          <v-row justify="start" class="ml-1 mt-3">
            <v-btn rounded="lg" color="alternate" class="mr-2" @click="handleSubmit">Save Education</v-btn>
            <v-btn rounded="lg" variant="outlined" @click="handleCancel">Cancel</v-btn>
          </v-row>
        </v-form>
      </v-col>
      <div v-else-if="mode === 'list'" class="w-100">
        <v-col sm="12" md="10" lg="8" xl="6">
          <EducationList :educations="modelValue" @edit="handleEdit" @delete="handleDelete" />
        </v-col>
        <v-col cols="12" class="mt-6">
          <v-row justify="start" class="ml-1">
            <v-btn v-if="showAddEducationButton" prepend-icon="mdi-plus" rounded="lg" color="alternate" @click="handleAddEducation">Add education</v-btn>
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
  </div>
</template>

<script lang="ts">
import { DateTime } from "luxon";
import { mapWritableState } from "pinia";
import { defineComponent } from "vue";

import EducationList, { type EducationData } from "@/components/EducationList.vue";
import { useAlertStore } from "@/store/alert";
import { useApplicationStore } from "@/store/application";
import { useUserStore } from "@/store/user";
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
  studentNumber: string;
  language: string;
  startYear: string;
  endYear: string;
  transcriptStatus: "received" | "requested" | "";
  previousNameRadio: any;
  Rules: typeof Rules;
  studentFirstName: string;
  studentMiddleName: string;
  studentLastName: string;
  isNameUnverified: boolean;
}

interface RadioOptions {
  value: any;
  label: string;
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
    const userStore = useUserStore();

    return {
      alertStore,
      wizardStore,
      applicationStore,
      userStore,
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
      studentNumber: "",
      language: "",
      startYear: "",
      endYear: "",
      transcriptStatus: "",
      Rules,
      previousNameRadio: undefined,
      studentFirstName: "",
      studentMiddleName: "",
      studentLastName: "",
      isNameUnverified: false,
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
    ApplicantNameRadioOptions(): RadioOptions[] {
      let legalNameRadioOptions: RadioOptions[] = [
        {
          label: this.userStore.legalName,
          value: { firstName: this.userStore.firstName, middleName: this.userStore.middleName, lastName: this.userStore.lastName },
        },
      ];
      return [...legalNameRadioOptions, ...this.previousNameRadioOptions];
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
    isDraftApplicationAssistantRenewal(): boolean {
      return this.applicationStore.isDraftApplicationRenewal && this.applicationStore.isDraftCertificateTypeEceAssistant;
    },
    showAddEducationButton(): boolean {
      //covers case where user has assistant renewal and can only add 1 education. Otherwise allow user to upload as many as needed.
      return this.isDraftApplicationAssistantRenewal ? Object.keys(this.modelValue).length < 1 : true;
    },
    today() {
      return formatDate(DateTime.now().toString());
    },
  },
  watch: {
    mode(newValue) {
      if (newValue === "list") {
        this.scrollToElement(this.$refs.addEducationComponent);
      }
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
          studentFirstName: this.studentFirstName,
          studentMiddleName: this.studentMiddleName,
          studentLastName: this.studentLastName,
          studentNumber: this.studentNumber,
          languageofInstruction: this.language,
          startDate: this.startYear,
          endDate: this.endYear,
          doesECERegistryHaveTranscript: this.transcriptStatus === "received",
          isOfficialTranscriptRequested: this.transcriptStatus === "requested",
          isNameUnverified: this.isNameUnverified,
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
      this.resetFormData();
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
      (this.studentFirstName = educationData.education.studentFirstName ?? ""),
        (this.studentMiddleName = educationData.education.studentMiddleName ?? ""),
        (this.studentLastName = educationData.education.studentLastName ?? ""),
        (this.studentNumber = educationData.education.studentNumber ?? "");
      this.isNameUnverified = educationData.education.isNameUnverified;
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
      //set the radio button for previous names and field buttons correctly
      if (educationData.education.isNameUnverified) {
        let index = this.previousNameRadioOptions.findIndex((option) => option.value === "other");
        this.previousNameRadio = this.previousNameRadioOptions[index].value;
      } else {
        let index = this.previousNameRadioOptions.findIndex(
          (option) =>
            option.value?.firstName === educationData.education.studentFirstName &&
            option.value?.lastName === educationData.education.studentLastName &&
            option.value?.middleName === educationData.education.studentMiddleName,
        );
        this.previousNameRadio = this.previousNameRadioOptions[index].value;
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
    previousNameRadioChanged(option: any) {
      if (option === "other") {
        this.studentFirstName = "";
        this.studentMiddleName = "";
        this.studentLastName = "";
        this.isNameUnverified = true;
      } else {
        this.studentFirstName = option.firstName;
        this.studentMiddleName = option.middleName;
        this.studentLastName = option.lastName;
        this.isNameUnverified = false;
      }
    },
    resetFormData() {
      this.id = "";
      this.previousSchool = "";
      this.school = "";
      this.program = "";
      this.campusLocation = "";
      this.studentNumber = "";
      this.language = "";
      this.startYear = "";
      this.endYear = "";
      this.transcriptStatus = "";
      this.studentFirstName = "";
      this.studentMiddleName = "";
      this.studentLastName = "";
      this.isNameUnverified = true;
      this.previousNameRadio = undefined;
    },
    scrollToElement(element: any) {
      if (element) {
        element.scrollIntoView({ behavior: "smooth" });
      }
    },
    formatDate,
  },
});
</script>
