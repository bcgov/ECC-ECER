<template>
  <v-col cols="12">
    <h1 class="title-header">Requirements for ECE Assistant Certification</h1>
  </v-col>
  <v-col cols="12">
    <p>Before you proceed, please make sure that you meet the following requirements:</p>
  </v-col>
  <ul>
    <v-col cols="11" offset="1">
      <li>
        Successful completion of at least one early childhood education course in child guidance; child health, safety, and nutrition; or child development,
        completed within the last 5 years at a program
        <a
          href="https://www2.gov.bc.ca/gov/content/education-training/early-learning/teach/training-and-professional-development/become-an-early-childhood-educator/recognized-ece-institutions"
        >
          recognized
        </a>
        by the ECE Registry
      </li>
    </v-col>
    <v-col cols="11" offset="1">
      <li>You can request an official transcript from your educational institution and ask them to send it directly to the ECE Registry</li>
    </v-col>
    <v-col cols="11" offset="1">
      <li>You can provide a character reference who:</li>
    </v-col>
    <v-col cols="10" offset="2">
      <ul>
        <li>Can speak to your character and has known you for at least 6 months</li>
        <li>Can speak to your ability to educate and care for young children</li>
        <li>Is not a relative, partner, spouse, or yourself</li>
        <li>(Recommended) is a certified ECE who has directly observed you working with young children</li>
      </ul>
    </v-col>
  </ul>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "ECEAssistantRequirements",
});
</script>

<style>
.title-header {
  line-height: 1.25;
}
</style>
