<template>
  <v-row class="ma-4">
    <v-col cols="12">
      <ul class="small">
        <v-col cols="12">
          <li>I understand that the ECE Registry require additional information (including supporting documents) in connection with this application</li>
        </v-col>
        <v-col cols="12">
          <li>
            I confirm that the information provided in this application is complete and accurate. I understand if inaccurate information is submitted it may
            results in the denial of certification
          </li>
        </v-col>
        <v-col cols="12">
          <li>
            I understand that information in this application or subsequently provided information may be reviewed, audited, and verified for the purpose of
            determining or auditing my eligibility for ECE Certificate in British Columbia
          </li>
        </v-col>
        <v-col cols="12">
          <li>
            I understand that the ECE Registry May take disciplinary actions against ,e. including action to cancel my certification, if I have, by omission or
            commission, knowingly given false ot misleading information in the course of completing this application
          </li>
        </v-col>
        <v-col cols="12">
          <li>
            I consent the work experience and character references indicated i this application to provide the ECE registry with this reference and other
            information as part of my application for certification
          </li>
        </v-col>
      </ul>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "DeclarationStepContent",
});
</script>
