<template>
  <v-row>
    <v-col cols>
      <h1 class="title-header">Transfer to ECE Five Year certification</h1>
    </v-col>
  </v-row>
  <v-row>
    <v-col>
      <p>You need the following information to apply to transfer your certification to B.C.</p>
    </v-col>
  </v-row>
  <v-row>
    <v-col>
      <ECEHeader title="Certificate information"></ECEHeader>
      <div class="d-flex flex-column ga-3 mt-6">
        <p>You will need to provide:</p>
        <ul class="ml-10">
          <li>Your certification or registration number (if applicable to your province)</li>
          <li>The name on your certificate</li>
        </ul>
      </div>
    </v-col>
  </v-row>
  <v-row>
    <v-col>
      <ECEHeader title="Work experience"></ECEHeader>
      <p>
        You need to have completed 500 hours of work experience and be able to provide references to verify the hours. If you worked at multiple locations, you
        can provide multiple references.
      </p>
      <br />
      <p>Important information about calculating hours:</p>
      <br />
      <ul class="ml-10">
        <li>Only include hours you worked once you began your early childhood education training program</li>
        <li>Only include hours completed within the last 5 years</li>
        <li>You cannot include hours that were part of your education (practicum or placement hours)</li>
        <li>You can include hours you volunteered</li>
      </ul>
      <br />
      <p>The reference must be someone who:</p>
      <br />
      <ul class="ml-10">
        <li>Has directly supervised or observed you working with children from birth to 5 years old</li>
        <li>Can speak to your knowledge, skills, and abilities (competencies) as an ECE</li>
        <li>Held valid Canadian ECE certification during the hours they supervised or observed you</li>
        <li>Is not the same person as your character reference</li>
      </ul>
    </v-col>
  </v-row>
  <v-row>
    <v-col>
      <ECEHeader title="Character reference"></ECEHeader>
      <p>You will need to provide a character reference. You will enter their name and email. We will contact them later after you submit your application.</p>
      <br />
      <p>Make sure you choose a person that:</p>
      <br />
      <ul class="ml-10">
        <li>Can speak to your character</li>
        <li>Can speak to your ability to educate and care for young children</li>
        <li>Has known you for at least 6 months</li>
        <li>Is not your relative, partner, spouse or yourself</li>
        <li>Is not the same person as your work experience reference</li>
      </ul>
      <br />
      <p>We recommend the person is a certified ECE who has directly observed you working with young children.</p>
    </v-col>
  </v-row>
  <v-row v-if="isPostBasic">
    <v-col cols="12">
      <SpecializedCertificationOptions ref="SpecializedCertificationOptions" :pre-selected="['Ite', 'Sne']" :is-labor-mobility="true" />
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import ECEHeader from "@/components/ECEHeader.vue";
import SpecializedCertificationOptions from "@/components/SpecializedCertificationOptions.vue";

export default defineComponent({
  name: "ECEFiveYearLaborMobilityRequirements",
  components: { ECEHeader, SpecializedCertificationOptions },
  props: {
    isPostBasic: {
      type: Boolean,
      required: false,
    },
  },
});
</script>
