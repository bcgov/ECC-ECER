<template>
  <v-row>
    <v-col cols="12" md="12" lg="12" xl="12">
      <v-row no-gutters>
        <v-col>
          <p>
            <b>{{ `${wizardStore.wizardData.applicantFirstName} ${wizardStore.wizardData.applicantLastName}` }}</b>
            is requesting a character reference for an
            <b>ECE Certificate</b>
            . We'll review your reference when assessing if the applicant is eligible for certification
          </p>
          <br />

          <h2>Information you'll need</h2>
          <p>
            It should take about 5 minutes to enter your reference. Make sure you get together all the information you need for you continue. If you're not
            ready now, you can come back later using the link in your email.
          </p>
          <br />

          <p>You'll be asked to provide:</p>
          <ul class="ml-10">
            <li>Your contact information</li>
            <li>Your ECE certification Information (optional)</li>
            <li>How long you've known the applicant</li>
            <li>Your relationship with the applicant</li>
            <li>Your personal observations of the applicant</li>
          </ul>
          <br />
          <div>
            All personal information is collection by the Ministry of Education and Child Care under the authority of hte Freedom of Information and Protection
            of Privacy Act s. 26(a), and will be used to determine if the applicant has the, experience and other qualifications required by the regulations. If
            you have any questions about the collection, use or disclosure of this information, contact the Early Childhood Educator (ECE) Registry, PO Box
            9961, STN PROV GOVT, Victoria BC V8W 9R4, Phone toll free: 1-888-338-6622, or email ECERegistry@gov.bc.ca.
          </div>
          <br />
          <h2>Requirements to be a reference</h2>
          <p>You need to:</p>
          <ul class="ml-10">
            <li>Have known the applicant for at least 6 months</li>
            <li>Be able to speak to the character of the applicant</li>
            <li>Be able to speak to the applicant's ability to educate and care for young children</li>
            <li>Not be a relative, partner, or spouse of the applicant</li>
          </ul>
        </v-col>
      </v-row>
      <v-row>
        <v-col>
          <p>Will you provide a reference?</p>
          <v-radio-group :rules="[Rules.requiredRadio()]" @update:model-value="(value) => $emit('update:model-value', value as boolean)">
            <v-radio label="Yes" :value="true"></v-radio>
            <v-radio label="No" :value="false"></v-radio>
          </v-radio-group>
        </v-col>
      </v-row>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import { useWizardStore } from "@/store/wizard";
import * as Rules from "@/utils/formRules";

export default defineComponent({
  name: "EceReferenceIntroduction",
  components: {},
  emits: {
    "update:model-value": (_reference: boolean) => true,
  },
  setup: () => {
    const wizardStore = useWizardStore();
    return { wizardStore };
  },
  data() {
    return {
      selection: undefined,
      Rules,
    };
  },
});
</script>
