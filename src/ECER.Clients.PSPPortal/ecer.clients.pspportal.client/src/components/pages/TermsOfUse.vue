<template>
  <PageContainer>
    <v-row>
      <v-col cols="12">
        <h1>Terms of Use</h1>
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="12">
        <p>
          Access to the Post-Secondary Programs portal is restricted to
          registered users authorized by the Province of British Columbia who
          hold a valid Business BCeID as an authorized representative of an
          educational institution. By accessing the Post-Secondary Programs
          portal, you confirm that you are the registered authorized user
          associated with the electronic login credentials provided to gain
          access to the Post-Secondary Programs portal, and acknowledge that you
          are bound by the terms of your Business BCeID Terms of Use Agreement.
        </p>
      </v-col>
      <v-col cols="12">
        <p>
          You acknowledge that sharing your password or failing to log out of
          your session may compromise business and personal information, and may
          be construed as compromising, altering, or rendering ineffective the
          BCeID system. You agree to log out using the log out button when you
          are finished your session in the Post-Secondary Programs portal.
        </p>
      </v-col>
      <v-col cols="12">
        <p>
          The Province uses the Post-Secondary Programs portal as a preferred
          means of communication with registered users. You consent to the
          provision, receipt and retention of records and information in
          electronic form through the Post-Secondary Programs portal and related
          communications.
        </p>
      </v-col>
      <v-col cols="12">
        <p>
          You agree that your electronic signature shall have the same force and
          effect as a written signature for any functions requiring an
          electronic acknowledgement or electronic signature. You agree to be
          bound by your electronic signature in the same legal manner as a
          handwritten signature. The Province reserves the right to suspend or
          revoke access to the Post-Secondary Programs.
        </p>
      </v-col>
    </v-row>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import PageContainer from "@/components/PageContainer.vue";
export default defineComponent({
  name: "TermsOfUse",
  components: { PageContainer },
});
</script>
