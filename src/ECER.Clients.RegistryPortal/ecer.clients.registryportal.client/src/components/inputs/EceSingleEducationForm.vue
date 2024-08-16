<template>
  <v-row>
    <v-col>
      <p>You must have completed a new course in an early childhood education training program.</p>
      <br />
      <p>The course must:</p>
      <br />
      <ul class="ml-10">
        <li>Have been completed within the last 5 years</li>
        <li>Be part of a program recognized by the ECE Registry</li>
        <li>Be a new course - it cannot be one you previously used in an application for an ECE Assistant certification</li>
      </ul>
    </v-col>
  </v-row>
  <v-row>
    <v-col>
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
        @update:model-value="updateTranscript"
      ></v-text-field>
    </v-col>
  </v-row>
  <v-row>
    <v-col>
      <v-text-field
        v-model="startYear"
        :rules="[Rules.required('Enter the start date of your program or course')]"
        label="Start date of program or course"
        type="date"
        variant="outlined"
        color="primary"
        maxlength="50"
        @update:model-value="updateTranscript"
      ></v-text-field>
    </v-col>
  </v-row>
  <v-row>
    <v-col>
      <v-text-field
        v-model="endYear"
        :rules="[Rules.required('Enter the end date of your program or course')]"
        label="End date of program or course"
        type="date"
        variant="outlined"
        color="primary"
        maxlength="50"
        @update:model-value="updateTranscript"
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
        :rules="[Rules.required('Enter the full name of your educational institution')]"
        label="Full name of educational institution"
        variant="outlined"
        color="primary"
        maxlength="100"
        @update:model-value="updateTranscript"
      ></v-text-field>
    </v-col>
  </v-row>
  <v-row>
    <v-col>
      <v-text-field
        v-model="campusLocation"
        label="Campus Location (Optional)"
        variant="outlined"
        color="primary"
        maxlength="50"
        @update:model-value="updateTranscript"
      ></v-text-field>
    </v-col>
  </v-row>
  <v-row>
    <v-col>
      <v-text-field
        v-model="language"
        label="Language of institution (optional)"
        variant="outlined"
        color="primary"
        maxlength="100"
        @update:model-value="updateTranscript"
      ></v-text-field>
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
        @update:model-value="updateTranscript"
      ></v-text-field>
    </v-col>
  </v-row>
  <br />
  <p>What name is shown on your transcript?</p>
  <br />
  <v-radio-group v-model="previousNameRadio" :rules="[Rules.requiredRadio('Select an option')]" @update:model-value="previousNameRadioChanged">
    <v-radio v-for="(step, index) in previousNameRadioOptions" :key="index" :label="step.label" :value="step.value"></v-radio>
  </v-radio-group>
  <div v-if="previousNameRadio === 'other'">
    <v-row>
      <v-col>
        <v-text-field
          v-model="studentFirstName"
          :rules="[Rules.required('Enter your first name')]"
          label="First name on transcript"
          variant="outlined"
          color="primary"
          maxlength="100"
          @update:model-value="updateTranscript"
        ></v-text-field>
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
          @update:model-value="updateTranscript"
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
          @update:model-value="updateTranscript"
        ></v-text-field>
      </v-col>
    </v-row>
  </div>
</template>

<script lang="ts">
import { mapWritableState } from "pinia";
import { defineComponent } from "vue";

import EducationList, { type EducationData } from "@/components/EducationList.vue";
import { useAlertStore } from "@/store/alert";
import { useApplicationStore } from "@/store/application";
import { useUserStore } from "@/store/user";
import { useWizardStore } from "@/store/wizard";
import type { EceEducationProps } from "@/types/input";
import type { Components } from "@/types/openapi";
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
  name: "EceSingleEducationForm",
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
  },
  mounted() {},
  methods: {
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
    updateTranscript() {
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

      // Update the modelValue dictionary
      const updatedModelValue = {
        [1]: newTranscript,
      };
      this.$emit("update:model-value", updatedModelValue);
    },
  },
});
</script>
