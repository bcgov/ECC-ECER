<template>
  <v-row>
    <v-col>
      <h1>
        Transfer Special Needs Educator and Infant and Toddler Educator
        certification
      </h1>
      <p class="mt-6">
        This certification adds specializations to your ECE Five Year
        certificate that allow you to work in licensed child care programs:
      </p>
      <ul class="ml-10">
        <li>
          alone and/or as the primary educator in inclusive settings with
          children 3-5 years of age (SNE)
        </li>
        <li>
          alone and/or as the primary educator with children birth to 5 years
          (ITE)
        </li>
      </ul>
      <p class="mt-6">
        You need the following information to apply to transfer your
        certification to British Columbia.
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
      <ECEHeader title="Character reference" />
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

import ECEHeader from "@/components/ECEHeader.vue";
import SpecializedCertificationOptions from "@/components/SpecializedCertificationOptions.vue";
export default defineComponent({
  name: "ECESneAndIteLabourMobilityRequirements",
  components: { ECEHeader, SpecializedCertificationOptions },
});
</script>
