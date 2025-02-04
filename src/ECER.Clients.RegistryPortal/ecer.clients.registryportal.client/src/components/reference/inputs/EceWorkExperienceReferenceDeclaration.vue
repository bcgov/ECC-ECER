<template>
  <v-row>
    <v-col cols="12" md="12" lg="12" xl="12">
      <v-row no-gutters>
        <v-col cols="12" v-for="(systemMessage, index) in configStore.systemMessages" :index="index">
          <Banner v-if="systemMessage.portalTags && systemMessage.portalTags.includes('REFERENCES')" type="info" :title="systemMessage.message ? systemMessage.message : ''" />
        </v-col>
        <v-col>
          <p>
            <b>{{ `${wizardStore.wizardData.applicantFirstName} ${wizardStore.wizardData.applicantLastName}` }}</b>
            is requesting a work experience reference for
            {{ certificationType }}. We'll review your reference when assessing if the applicant is eligible for certification.
          </p>
          <br />

          <h2 class="mb-5">Information you'll need</h2>
          <p>
            It should take about 5 minutes to enter your reference. Make sure you get together all the information you need before you continue. If you're not
            ready now, you can come back later using the link in your email.
          </p>
          <br />

          <p class="mb-3">You'll be asked to provide:</p>
          <ul v-if="wizardStore.wizardData.workExperienceType === WorkExperienceType.IS_500_Hours" class="ml-10">
            <li>Your contact information</li>
            <li>Your ECE certification information</li>
            <li>An assessment if you think the applicant is competent in a variety of areas - if not, we'll ask you to provide more details</li>
            <li>Total number of hours you directly supervised the applicant working</li>
            <li>Name and type of child care program</li>
            <li>Age of children the applicant cared for</li>
            <li>Dates the applicant worked the hours</li>
          </ul>
          <ul v-if="wizardStore.wizardData.workExperienceType === WorkExperienceType.IS_400_Hours" class="ml-10">
            <li>Your contact information</li>
            <li>Your ECE certification information(if applicable)</li>
            <li>Total number of hours the applicant worked</li>
            <li>Where the applicant worked and their role</li>
            <li>Age of children the applicant cared for (if applicable)</li>
            <li>Dates the applicant worked the hours</li>
          </ul>
          <br />
          <div>
            All personal information is collected by the Ministry of Education and Child Care under the authority of the Freedom of Information and Protection
            of Privacy Act s. 26(a), and will be used to determine if the applicant has the, experience and other qualifications required by the regulations. If
            you have any questions about the collection, use or disclosure of this information, contact the Early Childhood Educator (ECE) Registry, PO Box
            9961, STN PROV GOVT, Victoria BC V8W 9R4, Phone toll free: 1-888-338-6622, or email
            <a style="text-decoration: underline" href="mailto:ECERegistry@gov.bc.ca">ECERegistry@gov.bc.ca</a>
          </div>
          <br />
          <h2 class="mb-5">Requirements to be a reference</h2>
          <div v-if="wizardStore.wizardData.workExperienceType === WorkExperienceType.IS_500_Hours">
            <p class="mb-3">You need to:</p>
            <ul class="ml-10">
              <li>Have directly supervised (observed) all hours during which the applicant worked or volunteered</li>
              <li>Have held a valid Canadian ECE certification/registration during the hours you supervised the applicant</li>
              <li>Be able to speak to the applicant's knowledge, skills, and ability (competencies) as an ECE</li>
            </ul>
          </div>
          <div v-if="wizardStore.wizardData.workExperienceType === WorkExperienceType.IS_400_Hours">
            <p class="mb-3">You need to be:</p>
            <ul class="ml-10">
              <li>Able to confirm the applicant completed work experience hours</li>
              <li>A co-worker, supervisor, or parent/guardian of a child you cared for</li>
            </ul>
          </div>
        </v-col>
      </v-row>
      <v-row>
        <v-col>
          <p>Will you provide a reference?</p>
          <v-radio-group :rules="[Rules.requiredRadio()]" hide-details="auto" @update:model-value="(value) => $emit('update:model-value', value as boolean)">
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
import { useConfigStore } from "@/store/config";
import { useWizardStore } from "@/store/wizard";
import { CertificationType, WorkExperienceType } from "@/utils/constant";
import * as Rules from "@/utils/formRules";
import Banner from "../Banner.vue";

export default defineComponent({
  name: "EceReferenceIntroduction",
  components: {Banner},
  emits: {
    "update:model-value": (_reference: boolean) => true,
  },
  setup: () => {
    const wizardStore = useWizardStore();
    const configStore = useConfigStore();
    return { wizardStore, configStore, WorkExperienceType };
  },
  data() {
    return {
      selection: undefined,
      Rules,
    };
  },
  computed: {
    certificationType() {
      let certificationType = "Certificate type not found";
      if (this.wizardStore.wizardData.certificationTypes?.includes(CertificationType.ECE_ASSISTANT)) {
        certificationType = "ECE Assistant certificate";
      } else if (this.wizardStore.wizardData.certificationTypes?.includes(CertificationType.ONE_YEAR)) {
        certificationType = "ECE One Year certificate";
      } else if (this.wizardStore.wizardData.certificationTypes?.includes(CertificationType.FIVE_YEAR)) {
        certificationType = "ECE Five Year certificate";
      }
      return certificationType;
    },
  },
});
</script>
<style scoped>
ul > li {
  margin-bottom: 10px;
}
</style>
