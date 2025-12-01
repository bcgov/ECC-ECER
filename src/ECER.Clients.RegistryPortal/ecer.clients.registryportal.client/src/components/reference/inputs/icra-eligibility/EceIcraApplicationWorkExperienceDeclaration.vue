<template>
  <v-row>
    <v-col cols="12" v-for="systemMessage in configStore.systemMessages">
      <Alert v-if="systemMessage.portalTags && systemMessage.portalTags.includes('REFERENCES')" title="Alert">
        {{ systemMessage.message ? systemMessage.message : "" }}
      </Alert>
    </v-col>
    <v-col cols="12" md="12" lg="12" xl="12">
      <v-row no-gutters>
        <v-col>
          <p>
            <b>{{ `${cleanPreferredName(wizardStore.wizardData.applicantFirstName, wizardStore.wizardData.applicantLastName)}` }}</b>
            is requesting a
            <strong>competencies assessment</strong>
            for ECE Five Year certification. We will review your competencies assessment when assessing if the applicant is eligible for certification.
          </p>
          <br />
          <p>As part of the process, you are asked to complete two reference forms:</p>
          <br />
          <v-row align="center">
            <v-col cols="auto">
              <v-chip color="success" size="large" variant="flat">Completed</v-chip>
            </v-col>
            <v-col>Step 1 of 2: Employment Verification - Confirm the applicant's employment details</v-col>
          </v-row>
          <v-row align="center">
            <v-col cols="auto">
              <v-chip color="warning" size="large" variant="flat"><strong>Complete now</strong></v-chip>
            </v-col>
            <v-col>
              <div>
                <strong>Step 2 of 2: Competencies Assessment - Evaluate the applicant's knowledge, skills and abilities as an early childhood educator</strong>
              </div>
            </v-col>
          </v-row>
          <br />

          <p>
            <b>
              This reference request is for {{ cleanPreferredName(wizardStore.wizardData.referenceFirstName, wizardStore.wizardData.referenceLastName) }}. If
              you are not {{ cleanPreferredName(wizardStore.wizardData.referenceFirstName, wizardStore.wizardData.referenceLastName) }}, please select "No"
              below and “Other” on the following page.
            </b>
          </p>
          <br />
          <h2 class="mb-5">Information you'll need</h2>
          <p>
            It should take about 5 minutes to complete your competencies assessment. Make sure you get together all the information you need before you
            continue, as you cannot save the form. If you are not ready now, you can come back later using the link in your email.
          </p>
          <br />
          <p class="mb-3">You will be asked to provide:</p>
          <ul class="ml-10">
            <li>Your contact information</li>
            <li>An assessment of the applicant's competency in:</li>
            <ul class="ml-3 list-style-dash">
              <li>Child development, guidance, and health safety and nutrition</li>
              <li>Developing and implementing early childhood education curriculum</li>
              <li>Fostering positive relationships with children under their care, the families of children and with co-workers</li>
            </ul>
          </ul>
          <br />
          <!-- prettier-ignore -->
          <div>
            All personal information is collected by the Ministry of Education and Child Care under the authority of the Freedom of Information and Protection
            of Privacy Act s. 26(a), and will be used to determine if the applicant has the experience and other qualifications required by the regulations. If
            you have any questions about the collection, use or disclosure of this information, contact the Early Childhood Educator (ECE) Registry, PO Box
            9961, STN PROV GOVT, Victoria BC V8W 9R4, Phone toll free:
            <a style="text-decoration: underline" href="tel:18883386622">1-888-338-6622</a>, or email
            <a style="text-decoration: underline" href="mailto:ECERegistry@gov.bc.ca">ECERegistry@gov.bc.ca</a>
          </div>
          <br />
          <h2 class="mb-5">Requirements to be a reference</h2>
          <div>
            <p class="mb-3">You need to be:</p>
            <ul class="ml-10">
              <li>Able to verify the applicant's employment history</li>
              <li>Able to speak to the applicant's knowledge, skills, and abilities (competencies) as an ECE</li>
              <li>A former or current co-worker or employer</li>
            </ul>
          </div>
        </v-col>
      </v-row>
      <v-row>
        <v-col>
          <p>Will you provide a reference?</p>
          <v-radio-group
            id="willProvideReferenceRadio"
            :rules="[Rules.requiredRadio()]"
            hide-details="auto"
            @update:model-value="(value) => $emit('update:model-value', value as boolean)"
          >
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
import * as Rules from "@/utils/formRules";
import Alert from "@/components/Alert.vue";
import { cleanPreferredName } from "@/utils/functions";
export default defineComponent({
  name: "EceIcraApplicationWorkExperienceDeclaration",
  components: { Alert },
  emits: {
    "update:model-value": (_willProvideReference: boolean) => true,
  },
  setup: () => {
    const wizardStore = useWizardStore();
    const configStore = useConfigStore();
    return { wizardStore, configStore };
  },
  data() {
    return {
      selection: undefined,
      cleanPreferredName,
      Rules,
    };
  },
  computed: {},
});
</script>
<style scoped>
ul > li {
  margin-bottom: 10px;
}

.list-style-dash {
  list-style-type: "- ";
}
</style>
