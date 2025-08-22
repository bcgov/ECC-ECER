<template>
  <v-col v-if="
    certificationStore.certificateStatus(applicationStore.draftApplication.fromCertificate) == 'Active' ||
    (certificationStore.certificateStatus(applicationStore.draftApplication.fromCertificate) == 'Expired' &&
      !certificationStore.expiredMoreThan5Years(applicationStore.draftApplication.fromCertificate))
  " cols="12">
    <Alert title="Do you have 500 hours of supervised work experience?">
      You should
      <a class="cursor-pointer text-decoration-underline" @click="
        applicationStore.draftApplication.certificationTypes = ['FiveYears'];
      applicationStore.draftApplication.applicationType = 'New';
      ">
        apply for ECE Five Year certification
      </a>
      if you have completed 500 hours of work experience.
    </Alert>
  </v-col>
  <v-col cols="12">
    <h1>Renew your ECE One Year certification</h1>
  </v-col>
  <v-col cols="12">
    <p>You need the following information to renew your certificate.</p>
  </v-col>
  <v-col cols="12">
    <div class="d-flex flex-column ga-3 mt-3 mb-6">
      <p>It's important to know that:</p>
      <ul class="ml-10">
        <li>You can only renew this certificate once</li>
        <li>You cannot renew an ECE One Year Certificate if it has been expired for more than 5 years</li>
        <li>After you renew you’ll only be able to apply for an ECE Five Year certificate or an ECE Assistant
          certificate</li>
      </ul>
    </div>
  </v-col>
  <v-col cols="12">
    <ECEHeader title="Reason why you're renewing your ECE One Year certification" />
    <div class="d-flex flex-column ga-3 my-6">
      <p>
        You will need to provide a reason for why you were unable to complete the required 500 hours of supervised work
        experience during the term of your ECE
        One Year Certificate and/or why you were unable to provide a reference from the certified ECE who supervised the
        hours.
      </p>
      <p>
        If you've completed 500 hours, you should apply for
        <a class="cursor-pointer text-decoration-underline" @click="
          applicationStore.draftApplication.certificationTypes = ['FiveYears'];
        applicationStore.draftApplication.applicationType = 'New';
        ">
          ECE Five Year certification
        </a>
        instead.
      </p>
    </div>
  </v-col>
  <v-col cols="12">
    <ECEHeader title="Character reference" />
    <div class="d-flex flex-column ga-3 my-6">
      <p>You will need to provide a character reference. You'll enter their name and email. We'll contact them later
        after you submit your application.</p>
      <p>The reference must be someone who:</p>
      <ul class="ml-10">
        <li>Can speak to your character</li>
        <li>Can speak to your ability to educate and care for young children</li>
        <li>Has known you for at least 6 months</li>
        <li>Is not your relative, partner, spouse or yourself</li>
        <li v-if="expired">Is not the same person as your work experience reference</li>
      </ul>
      <p>We recommend the person is a certified ECE who has directly observed you working with young children.</p>
    </div>
  </v-col>
  <v-col v-if="expired" cols="12">
    <ECEHeader title="Work experience" />
    <div class="d-flex flex-column ga-3 my-6">
      <p>
        You need to have completed 400 hours of work experience and be able to provide references to verify the hours.
        If you worked at multiple locations, you
        can provide multiple references.
      </p>
      <p>The hours must:</p>
      <ul class="ml-10">
        <li>Be related to the field of early childhood education</li>
        <li>Have been completed within the last 5 years</li>
      </ul>
      <p>The reference must:</p>
      <ul class="ml-10">
        <li>Be able to confirm you've completed the hours</li>
        <li>Be a co-worker, supervisor, or a parent/guardian of a child you worked with</li>
        <li>Not be the same person you provide as a character reference</li>
      </ul>
    </div>
  </v-col>
  <v-col v-if="expired" cols="12">
    <ECEHeader title="Professional development" />
    <div class="d-flex flex-column ga-3 my-6">
      <p>To meet the professional development requirement, you need to have completed 40 hours of training.</p>
      <h3>What courses or workshops are eligible?</h3>
      <p>Each course or workshop must:</p>
      <ul class="ml-10">
        <li>Be related to early childhood education</li>
        <li v-if="!expired">
          Have been completed within the dates of your current certificate:
          <strong>{{ formattedLatestCertificationEffectiveDate }}</strong>
          to
          <strong>{{ formattedLatestCertificationExpiryDate }}</strong>
        </li>
        <li v-else>Have been completed within the last 5 years</li>
      </ul>
    </div>
  </v-col>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";

import Alert from "@/components/Alert.vue";
import ECEHeader from "@/components/ECEHeader.vue";
import { useApplicationStore } from "@/store/application";
import { useCertificationStore } from "@/store/certification";
import { formatDate } from "@/utils/format";

export default defineComponent({
  name: "ECEOneYearRenewalRequirements",
  components: { ECEHeader, Alert },
  props: {
    expired: {
      type: Boolean,
      default: false,
    },
  },
  setup() {
    const applicationStore = useApplicationStore();
    const certificationStore = useCertificationStore();
    return { applicationStore, certificationStore };
  },
  computed: {
    formattedLatestCertificationExpiryDate(): string {
      const fromCertificateId = this.applicationStore.draftApplication.fromCertificate;
      if (fromCertificateId) {
        return formatDate(this.certificationStore.certificationExpiryDate(fromCertificateId) ?? "", "LLL d, yyyy");
      }
      return formatDate("", "LLL d, yyyy"); // Default to empty if no fromCertificate is specified
    },
    formattedLatestCertificationEffectiveDate(): string {
      const fromCertificateId = this.applicationStore.draftApplication.fromCertificate;
      if (fromCertificateId) {
        return formatDate(this.certificationStore.certificationEffectiveDate(fromCertificateId) ?? "", "LLL d, yyyy");
      }
      return formatDate("", "LLL d, yyyy"); // Default to empty if no fromCertificate is specified
    },
  },
});
</script>
