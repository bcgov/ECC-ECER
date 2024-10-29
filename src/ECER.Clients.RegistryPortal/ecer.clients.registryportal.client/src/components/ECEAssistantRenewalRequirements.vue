<template>
  <v-col cols="12">
    <h1>Renew your ECE Assistant certification</h1>
  </v-col>
  <v-col cols="12">
    <p>You need the following information to renew your certificate.</p>
  </v-col>
  <v-col cols="12">
    <ECEHeader title="Education information" />
    <div class="d-flex flex-column ga-3 my-6">
      <p>You must have completed a new course that:</p>
      <ul class="ml-10">
        <li>Must have been completed within the last 5 years</li>
        <li>Cannot be one you previously used in an application for an ECE Assistant certification</li>
        <li>
          Be part of an early childhood education training
          <a href="https://www2.gov.bc.ca/gov/content?id=62102395C4F64F068A63E42921E67720">program recognized by the ECE Registry</a>
        </li>
      </ul>
      <p>
        You'll need to request an official transcript from your educational institution. It'll need to be sent directly to the ECE Registry from the educational
        institution.
      </p>
    </div>
  </v-col>
  <v-col cols="12">
    <ECEHeader title="Character reference" />
    <div class="d-flex flex-column ga-3 my-6">
      <p>You will need to provide a character reference. You'll enter their name and email. We'll contact them later after you submit your application.</p>
      <p>The reference must be someone who:</p>
      <ul class="ml-10">
        <li>Can speak to your character</li>
        <li>Can speak to your ability to educate and care for young children</li>
        <li>Has known you for at least 6 months</li>
        <li>Is not your relative, partner, spouse or yourself</li>
      </ul>
      <p>We recommend the person is a certified ECE who has directly observed you working with young children.</p>
    </div>
  </v-col>
  <v-col cols="12">
    <ECEHeader title="Work experience" />
    <div class="d-flex flex-column ga-3 my-6">
      <p>
        You need to have completed 400 hours of work experience. And be able to provide references to attest to the hours. If you worked at multiple locations,
        you can provide multiple references.
      </p>
      <p>The hours must:</p>
      <ul class="ml-10">
        <li>Be related to the field of early childhood education</li>
        <li>Have been completed within the last five years</li>
      </ul>
      <p>The reference must:</p>
      <ul class="ml-10">
        <li>Be able to confirm you've completed the hours</li>
        <li>Be a co-worker, supervisor, or a parent/guardian of a child you worked with</li>
        <li>Not be the same person you provide as a character reference</li>
      </ul>
    </div>
  </v-col>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import ECEHeader from "@/components/ECEHeader.vue";

export default defineComponent({
  name: "ECEAssistantRenewalRequirements",
  components: { ECEHeader },
});
</script>
