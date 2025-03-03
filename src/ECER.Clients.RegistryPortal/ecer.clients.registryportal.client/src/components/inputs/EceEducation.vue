<template>
  <v-row v-if="mode === 'add'">
    <v-col>
      <h2>{{ clientId ? "Edit" : "Add" }} education</h2>
      <br />
      <p>
        You'll need to request an official transcript from your educational institution for this course or program. It must be sent to us directly from them.
      </p>
      <br />
      <p>When we receive your transcript, we will:</p>
      <br />
      <ul class="ml-10">
        <li>Attach it to your application</li>
        <li>Email you to let you know we've received it</li>
      </ul>
    </v-col>
  </v-row>
  <v-row>
    <v-col v-if="mode === 'add'">
      <v-form ref="addEducationForm" validate-on="input">
        <v-row>
          <v-col>
            <h3>How will you provide your transcript?</h3>
          </v-col>
        </v-row>

        <v-row>
          <v-radio-group v-model="transcriptStatus" :rules="[Rules.required('Indicate the status of your transcript(s)')]" color="primary">
            <v-radio
              label="I have requested the official transcript to be sent to the ECE Registry from my educational institution"
              value="requested"
            ></v-radio>
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
          <v-col md="8" lg="6" xl="4">
            <EceTextField
              v-model="program"
              :rules="[Rules.required('Enter the name of your program or course')]"
              label="Name of program or course"
              maxlength="100"
            ></EceTextField>
          </v-col>
        </v-row>
        <v-row>
          <v-col md="8" lg="6" xl="4">
            <EceDateInput
              v-model="startYear"
              :rules="[
                Rules.required('Enter the start date'),
                Rules.futureDateNotAllowedRule(),
                Rules.conditionalWrapper(
                  isDraftApplicationAssistant,
                  Rules.dateRuleRange(applicationStore.draftApplication.createdOn!, 5, 'Start date must be within the last 5 years'),
                ),
              ]"
              label="Start date of program or course"
              maxlength="50"
              placeholder=""
              :max="today"
            ></EceDateInput>
          </v-col>
        </v-row>
        <v-row>
          <v-col md="8" lg="6" xl="4">
            <EceDateInput
              v-model="endYear"
              :rules="[
                Rules.required('Enter the end date'),
                Rules.futureDateNotAllowedRule(),
                Rules.dateBeforeRule(startYear || ''),
                Rules.conditionalWrapper(
                  isDraftApplicationAssistant,
                  Rules.dateRuleRange(applicationStore.draftApplication.createdOn!, 5, 'End date must be within the last 5 years'),
                ),
              ]"
              label="End date of program or course"
              maxlength="50"
              placeholder=""
              :max="today"
            ></EceDateInput>
          </v-col>
        </v-row>
        <v-row>
          <v-col>
            <h3>Where did you take it?</h3>
          </v-col>
        </v-row>
        <v-row>
          <v-col md="8" lg="6" xl="4">
            Country
            <v-select
              class="pt-2"
              :items="configStore.countryList"
              variant="outlined"
              label=""
              v-model="country"
              item-title="countryName"
              item-value="countryId"
              :rules="[Rules.required('Select your country', 'countryId')]"
              @update:modelValue="onCountryChange"
              return-object
            ></v-select>
          </v-col>
        </v-row>
        <v-row v-if="country?.countryId == configStore.canada?.countryId">
          <v-col md="8" lg="6" xl="4">
            Province or territory
            <v-select
              class="pt-2"
              :items="configStore.provinceList"
              variant="outlined"
              label=""
              v-model="province"
              item-title="provinceName"
              item-value="provinceId"
              :rules="[Rules.required('Select your province or territory', 'provinceId')]"
              @update:modelValue="onProvinceChange"
              return-object
            ></v-select>
          </v-col>
        </v-row>
        <v-row>
          <v-col v-if="country?.countryId == configStore.canada?.countryId" md="8" lg="6" xl="4">
            Educational institution
            <v-select
              class="pt-2"
              :items="postSecondaryInstitutionByProvince"
              variant="outlined"
              item-title="name"
              item-value="id"
              v-model="postSecondaryInstitution"
              :rules="[Rules.required('Select your post secondary institution', 'id')]"
              return-object
            ></v-select>
          </v-col>
          <v-col v-else md="8" lg="6" xl="4">
            <EceTextField
              v-model="school"
              :rules="[Rules.required('Enter the name of the educational institution')]"
              label="Name of educational institution"
              maxlength="100"
            ></EceTextField>
          </v-col>
        </v-row>
        <v-row>
          <v-col md="8" lg="6" xl="4">
            <EceTextField v-model="campusLocation" label="Campus location (optional)" variant="outlined" color="primary" maxlength="200"></EceTextField>
          </v-col>
        </v-row>

        <!-- If not Canada or no recognized institution selected: -->
        <template v-if="country?.countryId !== configStore.canada?.countryId || postSecondaryInstitution === undefined">
          <v-row>
            <v-col>
              <callout type="warning">
                <h3>You will need to provide supporting documents as part of your application.</h3>
                <p class="mt-3">
                  The ECE Registry does not recognize the program or course from the educational institution you entered. We will need additional information to
                  assess if your education is considered equivalent.
                </p>
                <h3 class="mt-3">
                  You may continue your application. After you submit, you can indicate how you will provide supporting documents in the application summary
                  page.
                </h3>
              </callout>
            </v-col>
          </v-row>
          <v-row>
            <v-col>
              <div class="d-flex flex-column ga-3">
                <h3>Detailed course outlines or syllabi</h3>
                <p>You will need to request course outlines or syllabi from your educational institution.</p>
                <p>They must:</p>
                <ul class="ml-10">
                  <li>Include detailed descriptions of course content, learning goals, outcomes and expectations</li>
                  <li>Be created by the educational institution</li>
                  <li>Be for the year(s) you completed the course(s)</li>
                  <li>
                    Be in English – if they are not, you must have them
                    <a
                      href="https://www2.gov.bc.ca/gov/content/education-training/early-learning/teach/training-and-professional-development/become-an-early-childhood-educator/pathways/international#prepare-your-application"
                    >
                      translated by a professional translator
                    </a>
                  </li>
                </ul>
              </div>
            </v-col>
          </v-row>
          <v-row>
            <v-col>
              <div class="d-flex flex-column ga-3">
                <h3>Program confirmation form</h3>
                <p>You will need to:</p>
                <ul class="ml-10">
                  <li>
                    Download the
                    <a href="https://www2.gov.bc.ca/assets/download/1DD5579B6A474ED2B095FD13B3268DA0">Program Confirmation Form (16KB, PDF)</a>
                  </li>
                  <li>Complete Section 1 of the form</li>
                  <li>Ask your educational institution to complete the rest of the form</li>
                  <li>
                    If they cannot complete the form in English, you will need to have it
                    <a
                      href="https://www2.gov.bc.ca/gov/content/education-training/early-learning/teach/training-and-professional-development/become-an-early-childhood-educator/pathways/international#prepare-your-application"
                    >
                      translated by a professional translator
                    </a>
                  </li>
                </ul>
              </div>
            </v-col>
          </v-row>
        </template>
        <!-- If not Canada -->
        <template v-if="country?.countryId !== configStore.canada?.countryId">
          <v-row>
            <v-col>
              <div class="d-flex flex-column ga-3">
                <h3>Comprehensive Evaluation Report</h3>
                <p>
                  You will need to request a Comprehensive Evaluation Report from BCIT’s International Credential Evaluation Service. This is needed for any
                  program or course completed outside of Canada.
                </p>
                <p>
                  You may be eligible for a fee waiver to cover the costs of the report.
                  <b>
                    If you wish to apply for a fee waiver, you can indicate this in your application summary (once you submit this application) before you
                    request a report from BCIT.
                  </b>
                  The fee waiver is paid out directly to BCIT from the ECE Registry and cannot be used to reimburse the applicant.
                </p>
                <p>
                  <a
                    href="https://www2.gov.bc.ca/gov/content/education-training/early-learning/teach/training-and-professional-development/become-an-early-childhood-educator/pathways/international#education-requirements-equivalency-process:~:text=Apply%20for%20an%20International%20Credential%20Evaluation%20Service%20Comprehensive%20Report%C2%A0"
                  >
                    Learn more about Comprehensive Evaluation Report
                  </a>
                </p>
              </div>
            </v-col>
          </v-row>
        </template>

        <v-row>
          <v-col>
            <h3>Name and student number on transcript</h3>
            <br />
            <p>Make sure this exactly matches your transcript. It may cause delays if we cannot match a transcript we receive to your application.</p>
          </v-col>
        </v-row>
        <v-row>
          <v-col md="8" lg="6" xl="4">
            <EceTextField
              v-model="studentNumber"
              :rules="[Rules.required('Enter your student number or ID')]"
              label="Student number or ID"
              maxlength="100"
            ></EceTextField>
          </v-col>
        </v-row>
        <br />
        <p>What name is shown on your transcript?</p>
        <br />
        <v-radio-group v-model="previousNameRadio" :rules="[Rules.requiredRadio('Select an option')]" @update:model-value="previousNameRadioChanged">
          <v-radio v-for="(step, index) in applicantNameRadioOptions" :key="index" :label="step.label" :value="step.value"></v-radio>
        </v-radio-group>
        <div v-if="previousNameRadio === 'other'">
          <v-row>
            <v-col md="8" lg="6" xl="4">
              <EceTextField v-model="studentFirstName" label="First name on transcript" variant="outlined" color="primary" maxlength="100"></EceTextField>
            </v-col>
          </v-row>
          <v-row>
            <v-col md="8" lg="6" xl="4">
              <EceTextField v-model="studentMiddleName" label="Middle name(s) on transcript (optional)" maxlength="100"></EceTextField>
            </v-col>
          </v-row>
          <v-row>
            <v-col md="8" lg="6" xl="4">
              <EceTextField
                v-model="studentLastName"
                :rules="[Rules.required('Enter your last name')]"
                label="Last name on transcript"
                maxlength="100"
              ></EceTextField>
            </v-col>
          </v-row>
        </div>
        <v-row justify="start" class="ml-1 mt-10">
          <v-btn rounded="lg" color="primary" class="mr-2" @click="handleSubmit" :loading="loadingStore.isLoading('draftapplication_put')">
            Save education
          </v-btn>
          <v-btn rounded="lg" variant="outlined" @click="handleCancel" :loading="loadingStore.isLoading('draftapplication_put')">Cancel</v-btn>
        </v-row>
      </v-form>
    </v-col>
    <div v-else-if="mode === 'list'" class="w-100">
      <v-col>
        <EducationList :educations="modelValue" @edit="handleEdit" @delete="handleDelete" />
      </v-col>
      <v-col cols="12" class="mt-6">
        <v-row justify="start" class="ml-1">
          <v-btn
            v-if="showAddEducationButton"
            prepend-icon="mdi-plus"
            rounded="lg"
            color="primary"
            @click="handleAddEducation"
            :loading="loadingStore.isLoading('draftapplication_put')"
          >
            Add education
          </v-btn>
        </v-row>
      </v-col>
      <v-col>
        <!-- this prevents form from proceeding if rules are not met -->
        <v-input
          :model-value="modelValue"
          :rules="[(v) => Object.keys(v).length > 0 || 'You must enter at least 1 education entry']"
          auto-hide="auto"
        ></v-input>
      </v-col>
    </div>
  </v-row>
</template>

<script lang="ts">
import { DateTime } from "luxon";
import { mapWritableState } from "pinia";
import { defineComponent } from "vue";
import EceTextField from "@/components/inputs/EceTextField.vue";
import EceDateInput from "@/components/inputs/EceDateInput.vue";
import EducationList, { type EducationData } from "@/components/EducationList.vue";
import { useAlertStore } from "@/store/alert";
import { useApplicationStore } from "@/store/application";
import { useUserStore } from "@/store/user";
import { useWizardStore } from "@/store/wizard";
import { useLoadingStore } from "@/store/loading";
import type { Components, Country, PostSecondaryInstitution, Province, Transcript } from "@/types/openapi";
import { formatDate } from "@/utils/format";
import * as Rules from "@/utils/formRules";
import { useConfigStore } from "@/store/config";
import { educationRecognitionRadio, educationOriginRadio } from "@/utils/constant";
import Callout from "../Callout.vue";

interface EceEducationData {
  clientId: string;
  id: string;
  previousSchool: string;
  school: string;
  program: string;
  country: Country | undefined;
  province: Province | undefined;
  postSecondaryInstitution: PostSecondaryInstitution | undefined;
  campusLocation: string;
  studentNumber: string;
  startYear: string;
  endYear: string;
  transcriptStatus: "received" | "requested" | "";
  previousNameRadio: any;
  Rules: typeof Rules;
  studentFirstName: string | null;
  studentMiddleName: string | null;
  studentLastName: string;
  isNameUnverified: boolean;
  educationRecognition: Components.Schemas.EducationRecognition | undefined;
  educationOrigin: Components.Schemas.EducationOrigin | undefined;
}

interface RadioOptions {
  value: any;
  label: string;
}

export default defineComponent({
  name: "EceEducation",
  components: { EducationList, EceTextField, EceDateInput, Callout },
  props: {
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
    const configStore = useConfigStore();
    const loadingStore = useLoadingStore();

    return {
      alertStore,
      wizardStore,
      applicationStore,
      userStore,
      configStore,
      loadingStore,
      educationRecognitionRadio,
      educationOriginRadio,
    };
  },
  data: function (): EceEducationData {
    return {
      clientId: "",
      id: "",
      previousSchool: "",
      school: "",
      program: "",
      country: undefined,
      province: undefined,
      postSecondaryInstitution: undefined,
      campusLocation: "",
      studentNumber: "",
      startYear: "",
      endYear: "",
      transcriptStatus: "",
      Rules,
      previousNameRadio: undefined,
      studentFirstName: "",
      studentMiddleName: "",
      studentLastName: "",
      isNameUnverified: false,
      educationRecognition: undefined,
      educationOrigin: undefined,
    };
  },
  computed: {
    ...mapWritableState(useWizardStore, { mode: "listComponentMode" }),
    newClientId() {
      return Object.keys(this.modelValue).length + 1;
    },
    postSecondaryInstitutionByProvince() {
      return this.configStore.postSecondaryInstitutionList.filter((item) => item.provinceId === this.province?.provinceId);
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
    isDraftApplicationAssistant(): boolean {
      return this.applicationStore.isDraftCertificateTypeEceAssistant;
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
        window.scrollTo({
          top: 0,
          behavior: "smooth",
        });
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
          country: this.country,
          province: this.province,
          postSecondaryInstitution: this.postSecondaryInstitution,
          studentNumber: this.studentNumber,
          startDate: this.startYear,
          endDate: this.endYear,
          doesECERegistryHaveTranscript: this.transcriptStatus === "received",
          isOfficialTranscriptRequested: this.transcriptStatus === "requested",
          isNameUnverified: this.isNameUnverified,
          educationRecognition: this.educationRecognition!,
          educationOrigin: this.educationOrigin!,
        };

        // Remove undefined properties before sending
        Object.keys(newTranscript).forEach((key) => {
          const transcriptRecord = newTranscript as Record<string, Transcript>;

          if ((transcriptRecord[key] as any) === undefined) {
            delete transcriptRecord[key];
          }
        });

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

        await this.applicationStore.saveDraft();
        //we need to update wizardData with the latest information to avoid creating duplicate new entries
        await this.wizardStore.initializeWizard(this.applicationStore.applicationConfiguration, this.applicationStore.draftApplication);

        this.alertStore.setSuccessAlert(message);

        // Change mode to education list
        this.mode = "list";
      } else {
        this.alertStore.setFailureAlert("You must enter all required fields in the valid format.");
      }
    },
    onProvinceChange() {
      this.postSecondaryInstitution = undefined;
    },
    onCountryChange() {
      this.province = undefined;
      this.postSecondaryInstitution = undefined;
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
      (this.studentFirstName = educationData.education.studentFirstName ?? null),
        (this.studentMiddleName = educationData.education.studentMiddleName ?? null),
        (this.studentLastName = educationData.education.studentLastName ?? ""),
        (this.studentNumber = educationData.education.studentNumber ?? "");
      this.isNameUnverified = educationData.education.isNameUnverified ?? false;
      this.startYear = formatDate(educationData.education.startDate || "") ?? "";
      this.endYear = formatDate(educationData.education.endDate || "") ?? "";
      this.educationRecognition = educationData.education.educationRecognition;
      this.educationOrigin = educationData.education.educationOrigin;
      this.country = educationData.education.country;
      this.province = educationData.education.province;
      this.postSecondaryInstitution = educationData.education.postSecondaryInstitution;
      if (educationData.education.isOfficialTranscriptRequested) {
        this.transcriptStatus = "requested";
      } else if (educationData.education.doesECERegistryHaveTranscript) {
        this.transcriptStatus = "received";
      } else {
        this.transcriptStatus = "";
      }
      //set the radio button for previous names and field buttons correctly
      if (educationData.education.isNameUnverified) {
        let index = this.applicantNameRadioOptions.findIndex((option) => option.value === "other");
        this.previousNameRadio = this.applicantNameRadioOptions[index].value;
      } else {
        let index = this.applicantNameRadioOptions.findIndex(
          (option) =>
            option.value?.firstName === educationData.education.studentFirstName &&
            option.value?.lastName === educationData.education.studentLastName &&
            option.value?.middleName === educationData.education.studentMiddleName,
        );
        this.previousNameRadio = this.applicantNameRadioOptions[index].value;
      }
      // Change mode to add
      this.mode = "add";
    },
    async handleDelete(educationId: string | number) {
      //Remove the education from the modelValue

      if (educationId in this.modelValue) {
        // Create a copy of modelValue
        const updatedModelValue = { ...this.modelValue };
        // Delete the education entry from the copied object
        delete updatedModelValue[educationId];
        // Emit the updated modelValue
        this.$emit("update:model-value", updatedModelValue);
      }

      await this.applicationStore.saveDraft();
      //we need to update wizardData with the latest information to avoid creating duplicate new entries
      await this.wizardStore.initializeWizard(this.applicationStore.applicationConfiguration, this.applicationStore.draftApplication);

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
      this.country = this.configStore.canada;
      this.province = undefined;
      this.postSecondaryInstitution = undefined;
      this.startYear = "";
      this.endYear = "";
      this.transcriptStatus = "";
      this.studentFirstName = "";
      this.studentMiddleName = "";
      this.studentLastName = "";
      this.isNameUnverified = true;
      this.previousNameRadio = undefined;
      this.educationRecognition = undefined;
      this.educationOrigin = undefined;
    },
    formatDate,
  },
});
</script>
