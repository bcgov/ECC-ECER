<template>
  <v-col cols="12">
    <h1>Renew your ECE Five Year certification</h1>
  </v-col>
  <v-col cols="12">
    <p>You need the following information to renew your certificate.</p>
  </v-col>
  <v-col v-if="expired && !expiredMoreThan5Years" cols="12">
    <ECEHeader title="Reason why you're late renewing your ECE Five Year certification" />
    <div class="d-flex flex-column ga-3 my-6">
      <p>You need to provide the reason why you did not submit an application before your certificate expired.</p>
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
        <li>Is not the same person as your work experience reference</li>
      </ul>
      <p>We recommend the person is a certified ECE who has directly observed you working with young children.</p>
    </div>
  </v-col>
  <v-col cols="12">
    <ECEHeader title="Work experience" />
    <div v-if="!expired" class="d-flex flex-column ga-3 my-6">
      <p>
        You need to have completed 400 hours of work experience and be able to provide references to verify the hours. If you worked at multiple locations, you
        can provide multiple references.
      </p>
      <p>The hours must:</p>
      <ul class="ml-10">
        <li>Be related to the field of early childhood education</li>
        <li>
          Have been completed within the term of your current certificate (between {{ formattedLatestCertificationEffectiveDate }} and
          {{ formattedLatestCertificationExpiryDate }})
        </li>
      </ul>
      <p>The reference must:</p>
      <ul class="ml-10">
        <li>Be able to confirm you've completed the hours</li>
        <li>Be a co-worker, supervisor, or a parent/guardian of a child you worked with</li>
        <li>Not be the same person you provide as a character reference</li>
      </ul>
    </div>
    <div v-if="expired && !expiredMoreThan5Years" class="d-flex flex-column ga-3 my-6">
      <p>You need to have completed 400 hours of work experience. And be able to provide references to attest to all the hours.</p>
      <p>Important information about calculating hours:</p>
      <ul class="ml-10">
        <li>Only include hours you worked once you began your early childhood education training program</li>
        <li>Only include hours completed within the last 5 years</li>
        <li>Cannot include hours that were part of your education (practicum or placement hours)</li>
        <li>Can include hours you volunteered</li>
      </ul>
      <p>The reference must be someone who:</p>
      <ul class="ml-10">
        <li>Has directly supervised or observed you working with children from birth to 5 years old</li>
        <li>Can speak to your knowledge, skills, and abilities (competencies) as an ECE</li>
        <li>Held valid Canadian ECE certification during the hours they supervised or observed you</li>
        <li>The person cannot be your character reference.</li>
      </ul>
    </div>
    <div v-if="expired && expiredMoreThan5Years" class="d-flex flex-column ga-3 my-6">
      <p>You need to have completed 500 hours of work experience and be able to provide references to verify the hours.</p>
      <p>Important information about calculating hours:</p>
      <ul class="ml-10">
        <li>Only include hours you worked once you began your early childhood education training program</li>
        <li>Only include hours completed within the last 5 years</li>
        <li>Cannot include hours that were part of your education (practicum or placement hours)</li>
        <li>Can include hours you volunteered</li>
      </ul>
      <p>The reference must be someone who:</p>
      <ul class="ml-10">
        <li>Has directly supervised or observed you working with children from birth to 5 years old</li>
        <li>Can speak to your knowledge, skills, and abilities (competencies) as an ECE</li>
        <li>Held valid Canadian ECE certification during the hours they supervised or observed you</li>
        <li>The person cannot be your character reference.</li>
      </ul>
    </div>
  </v-col>
  <v-col cols="12">
    <ECEHeader title="Professional development" />
    <div class="d-flex flex-column ga-3 my-6">
      <p>You must have completed at least 40 hours of professional development.</p>
      <p>The course or workshop must:</p>
      <ul class="ml-10">
        <li>Be relevant to the field of early childhood education</li>
        <li v-if="!expired">
          Have been completed within the term of your current certificate (between {{ formattedLatestCertificationEffectiveDate }} and
          {{ formattedLatestCertificationExpiryDate }})
        </li>
        <li v-else>Have been completed within the last 5 years</li>
      </ul>
      <p>You'll need to provide the following information about each course or workshop:</p>
      <ul class="ml-10">
        <li>Name of the course or workshop</li>
        <li>Name of the place where you took it</li>
        <li>Dates when you started and completed it</li>
        <li>How many hours it was</li>
        <li>Contact information for the facilitator/instructor or a document to show you've completed the course</li>
      </ul>
    </div>
  </v-col>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import ECEHeader from "@/components/ECEHeader.vue";
import { useCertificationStore } from "@/store/certification";
import { formatDate } from "@/utils/format";

export default defineComponent({
  name: "ECEFiveYearRenewalRequirements",
  components: { ECEHeader },
  setup() {
    const certificationStore = useCertificationStore();
    return { certificationStore };
  },
  props: {
    expired: {
      type: Boolean,
      default: false,
    },
    expiredMoreThan5Years: {
      type: Boolean,
      default: false,
    },
  },
  computed: {
    formattedLatestCertificationExpiryDate(): string {
      return formatDate(this.certificationStore.latestCertification?.expiryDate ?? "", "LLL d, yyyy");
    },
    formattedLatestCertificationEffectiveDate(): string {
      return formatDate(this.certificationStore.latestCertification?.effectiveDate ?? "", "LLL d, yyyy");
    },
  },
});
</script>
