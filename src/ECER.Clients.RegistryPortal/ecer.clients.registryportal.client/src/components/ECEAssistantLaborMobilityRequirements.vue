<template>
  <v-row>
    <v-col cols>
      <h1 class="title-header">Transfer to ECE Assistant certification</h1>
    </v-col>
  </v-row>
  <v-row>
    <v-col>
      <p>
        You need the following information to apply to transfer your
        certification to B.C.
      </p>
    </v-col>
  </v-row>
  <v-row>
    <v-col>
      <ECEHeader title="Certificate information"></ECEHeader>
      <div class="d-flex flex-column ga-3 mt-6">
        <p>You will need to provide:</p>
        <ul class="ml-10">
          <li>
            Your certification or registration number (if applicable to your
            province)
          </li>
          <li>The name on your certificate</li>
        </ul>
      </div>
    </v-col>
  </v-row>
  <v-row>
    <v-col>
      <ECEHeader title="Character reference"></ECEHeader>
      <p>
        You will need to provide a character reference. You will enter their
        name and email. We will contact them later after you submit your
        application.
      </p>
      <br />
      <p>Make sure you choose a person that:</p>
      <br />
      <ul class="ml-10">
        <li>Can speak to your character</li>
        <li>
          Can speak to your ability to educate and care for young children
        </li>
        <li>Has known you for at least 6 months</li>
        <li>Is not your relative, partner, spouse or yourself</li>
      </ul>
      <br />
      <p>
        We recommend the person is a certified ECE who has directly observed you
        working with young children.
      </p>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import ECEHeader from "./ECEHeader.vue";

export default defineComponent({
  name: "ECEAssistantLaborMobilityRequirements",
  components: { ECEHeader },
});
</script>
