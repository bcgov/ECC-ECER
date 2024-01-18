<template>
  <PageContainer>
    <v-col>
      <v-btn v-if="hasBackButton" class="mb-10" variant="text" @click="goBack">
        <v-icon size="small" color="secondary" icon="mdi-arrow-left" class="mr-2"></v-icon>
        <a class="small">Back</a>
      </v-btn>
      <v-row justify="space-between">
        <h3>Terms of Use</h3>
        <v-btn variant="text" onclick="window.print()">
          <v-row align="center" justify="end">
            <v-icon size="small" color="secondary" icon="mdi-printer-outline" class="mr-2"></v-icon>
            <a class="small">Print Terms of Use</a>
          </v-row>
        </v-btn>
      </v-row>
      <v-row>
        <v-card variant="outlined" color="grey-lightest" class="pa-10 mt-5">
          <p class="small">
            <b>Notification of Collection</b>
            <br />
            <br />
            The personal information collected through My Early Childhood Educator Registry is collected under the authority of Section 26(c) of
            <a href="https://bclaws.gov.bc.ca/civix/document/id/complete/statreg/96165_00">the Freedom of Information and Protection of Privacy Act</a>
            and is subject to all of the provisions of that Act. Your information will be used under Section 32(a) of the Act to communicate with you, determine
            or audit your eligibility.
            <br />
            <br />
            If you have any specific questions concerning the collection, use or disclosure of personal information, please contact the Early Childhood Educator
            Registry at
            <a href="tel:18883386622">1 (888) 338-6622</a>
            .
            <br />
            <br />
            <b>User Agreement</b>
            <br />
            <br />
            Access to My Early Childhood Educator Registry is restricted to registered users authorized by the Province of British Columbia, who are validly
            holding a basic BCeID or BC Services Card. By accessing My Early Childhood Educator Registry, you confirm that you are the registered authorized
            user associated with the electronic login information provided to gain access to My Early Childhood Educator Registry, and acknowledge that you are
            bound by the terms of your
            <a href="https://www.bceid.ca/aboutbceid/tou_basic.aspx#:~:text=1)%20You%20agree%20to%20abide,using%20your%20Basic%20BCeID%20account.">
              BCeID Terms of Use Agreement
            </a>
            or the
            <a href="https://www2.gov.bc.ca/gov/content/governments/government-id/bcservicescardapp/terms-of-use">BC Login Service Terms of Use</a>
            .
            <br />
            <br />
            You acknowledge that sharing your password or failing to log out of your session may compromise your personal information, and may be construed as
            compromising, altering, or rendering ineffective the BCeID system or BC Services Card systems. You acknowledge that you are responsible for all
            activities that occur using your BCeID or BC Services Card and your Digital Identity, whether those activities were performed by you or someone
            else. You agree to log out using the log out button when you are finished your session in My Early Childhood Educator Registry. The Province uses My
            Early Childhood Educator Registry as a preferred means of communication with registered users. You consent to the provision, receipt and retention
            of records and information in electronic form through My Early Childhood Educator Registry and related communications.
            <br />
            <br />
            You agree that your electronic signature shall have the same force and effect as a written signature for any functions requiring an electronic
            acknowledgement or electronic signature. You agree to be bound by your electronic signature in the same legal manner as a handwritten signature.
          </p>
        </v-card>
      </v-row>
    </v-col>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import PageContainer from "@/components/PageContainer.vue";

export default defineComponent({
  name: "TermsOfUse",
  components: { PageContainer },
  props: {
    hasBackButton: {
      type: Boolean,
      default: false,
    },
  },
  methods: {
    goBack() {
      this.$router.go(-1);
    },
  },
});
</script>
