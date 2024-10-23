<template>
  <v-row>
    <v-col cols>
      <h1 class="title-header">Apply for ECE Assistant certification</h1>
    </v-col>
  </v-row>
  <v-row>
    <v-col>
      <p>You need the following information to apply:</p>
    </v-col>
  </v-row>
  <v-row v-if="isLaborMobility">
    <v-col>
      <ECEHeader title="Certificate information"></ECEHeader>
      <div class="d-flex flex-column ga-3 mt-6">
        <p>After you submit your application, we'll contact you to get your certificate information, including:</p>
        <ul class="ml-10">
          <li>Your certification or registration number</li>
          <li>The type of registration</li>
          <li>The province or territory that you're certified in</li>
        </ul>
      </div>
    </v-col>
  </v-row>
  <v-row v-if="!isLaborMobility">
    <v-col>
      <ECEHeader title="Education information"></ECEHeader>
      <p>You must have completed an early childhood education course in at least one of the following:</p>
      <br />
      <ul class="ml-10">
        <li>Child guidance</li>
        <li>Child health, safety, and nutrition</li>
        <li>Child development</li>
      </ul>
      <br />
      <p>The course must:</p>
      <br />
      <ul class="ml-10">
        <li>Have been completed within the last 5 years</li>
        <li>
          <a
            href="https://www2.gov.bc.ca/gov/content/education-training/early-learning/teach/training-and-professional-development/become-an-early-childhood-educator/recognized-ece-institutions"
          >
            Be part of a program recognized by the ECE Registry
          </a>
        </li>
      </ul>
      <br />
      <p>
        You'll need to request an official transcript from your educational institution. It must be sent directly from the educational institute to the ECE
        Registry.
      </p>
    </v-col>
  </v-row>
  <v-row>
    <v-col>
      <ECEHeader title="Character reference" />
      <p>You will need to provide a character reference. You'll enter their name and email. We'll contact them later after you submit your application.</p>
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
</template>

<script lang="ts">
import { defineComponent } from "vue";

import ECEHeader from "./ECEHeader.vue";

export default defineComponent({
  name: "ECEAssistantRequirements",
  components: { ECEHeader },
  props: {
    isLaborMobility: {
      type: Boolean,
      default: false,
    },
  },
});
</script>
