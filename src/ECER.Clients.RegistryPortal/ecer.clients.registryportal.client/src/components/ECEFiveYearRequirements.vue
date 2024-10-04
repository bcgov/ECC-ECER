<template>
  <v-row>
    <v-col cols>
      <h1 class="title-header">{{ generateTitle() }}</h1>
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
      <p>
        You must have completed a basic early childhood education program. It must be
        <a
          href="https://www2.gov.bc.ca/gov/content/education-training/early-learning/teach/training-and-professional-development/become-an-early-childhood-educator/recognized-ece-institutions"
        >
          a program recognized by the ECE Registry
        </a>
      </p>
      <br />
      <p>
        You'll need to request an official transcript from your educational institution. It must be sent directly from the educational institute to the ECE
        Registry.
      </p>
    </v-col>
  </v-row>
  <v-row v-if="applicationStore.isDraftCertificateTypeIte">
    <v-col>
      <ECEHeader title="Education for Infant and Toddler Educator (ITE)"></ECEHeader>
      <p>
        You must have completed an infant and toddler educator training program. It must be a
        <a
          href="https://www2.gov.bc.ca/gov/content/education-training/early-learning/teach/training-and-professional-development/become-an-early-childhood-educator/recognized-ece-institutions"
        >
          a program recognized by the ECE Registry
        </a>
      </p>
      <br />
      <p>
        You'll need to request an official transcript from your educational institution. It must be sent directly from the educational institute to the ECE
        Registry.
      </p>
    </v-col>
  </v-row>
  <v-row v-if="applicationStore.isDraftCertificateTypeSne">
    <v-col>
      <ECEHeader title="Education for Special Needs Educator (SNE)"></ECEHeader>
      <p>
        You must have completed a special needs early childhood educator training. It must be a program recognized by the ECE Registry.
        <a
          href="https://www2.gov.bc.ca/gov/content/education-training/early-learning/teach/training-and-professional-development/become-an-early-childhood-educator/recognized-ece-institutions"
        >
          a program recognized by the ECE Registry
        </a>
      </p>
      <br />
      <p>
        You'll need to request an official transcript from your educational institution. It must be sent directly from the educational institute to the ECE
        Registry.
      </p>
    </v-col>
  </v-row>
  <v-row>
    <v-col>
      <ECEHeader title="Work experience" />
      <p>You need to have completed 500 hours of work experience. And be able to provide references to attest to all the hours.</p>
      <br />
      <p>Important information about calculating hours:</p>
      <br />
      <ul class="ml-10">
        <li>Only include hours you worked after you started your education</li>
        <li>Only include hours completed within the 5 year</li>
        <li>Cannot include hours you work as part of your practicum</li>
        <li>Can include hours you volunteered</li>
      </ul>
      <br />
      <p>The reference must be someone who:</p>
      <br />
      <ul class="ml-10">
        <li>Have directly supervised or observed the hours they can attest to</li>
        <li>Can speak to your knowledge, skills, and ability as an ECE</li>
        <li>Has held valid Canadian ECE certificate during the hours they supervised or observed you</li>
      </ul>
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

import { useApplicationStore } from "@/store/application";

import ECEHeader from "./ECEHeader.vue";

export default defineComponent({
  name: "ECEFiveYearRequirements",
  components: { ECEHeader },
  setup() {
    const applicationStore = useApplicationStore();
    return { applicationStore };
  },
  methods: {
    generateTitle() {
      if (this.applicationStore.isDraftCertificateTypeIte && this.applicationStore.isDraftCertificateTypeSne) {
        return "Apply for ECE Five Year Certification, SNE and ITE";
      } else if (this.applicationStore.isDraftCertificateTypeIte) {
        return "Apply for ECE Five Year Certification and ITE";
      } else if (this.applicationStore.isDraftCertificateTypeSne) {
        return "Apply for ECE Five Year Certification and SNE";
      } else {
        return "Apply for ECE Five Year Certification";
      }
    },
  },
});
</script>
