<template>
  <v-row>
    <v-col cols>
      <h1 class="title-header">Apply for ECE One Year certification</h1>
    </v-col>
  </v-row>
  <v-row>
    <v-col>
      <p>You need the following information to apply:</p>
    </v-col>
  </v-row>
  <v-row>
    <v-col>
      <ECEHeader title="Education information"></ECEHeader>
      <p>You must have completed a basic early childhood education training program.</p>
      <br />
      <p>The program must be either:</p>
      <br />
      <ul class="ml-10">
        <li>
          <a
            target="_blank"
            href="https://www2.gov.bc.ca/gov/content/education-training/early-learning/teach/training-and-professional-development/become-an-early-childhood-educator/recognized-ece-institutions"
          >
            Recognized
          </a>
          by the ECE Registry
        </li>
        <li>Considered equivalent by the ECE Registry</li>
      </ul>
      <br />
      <p>
        You'll need to request an official transcript from your educational institution. It must be sent directly from the educational institution to the ECE
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
  name: "ECEOneYearRequirements",
  components: { ECEHeader },
});
</script>
