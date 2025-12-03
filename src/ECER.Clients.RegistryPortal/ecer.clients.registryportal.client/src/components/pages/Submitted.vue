<template>
  <v-card :rounded="'0'" flat color="background-light-success" class="success-banner">
    <v-container>
      <div class="d-flex">
        <v-icon size="90px" color="#42814A" icon="mdi-check-circle" class="mr-2"></v-icon>
        <h1 id="titleApplicationSubmitted" class="align-self-center">Application Submitted</h1>
      </div>
    </v-container>
  </v-card>
  <PageContainer>
    <h2>What to expect next</h2>
    <br />
    <p>{{ certificationText }}</p>
    <br />
    <p>It is important to keep your contact information up-to-date in your My ECE Registry profile.</p>
    <br />
    <div v-if="applicationHasTranscripts">
      <h3>Transcripts</h3>
      <br />
      <ul class="ml-10">
        <li>
          <strong>Make sure the ECE Registry will receive your transcripts in English</strong>
        </li>
        <li>We will email you once we receive them</li>
      </ul>
    </div>
    <br />
    <div v-if="applicationHasTranscripts && applicationHasEducationNotRecognized">
      <h3>Supporting documents</h3>
      <br />
      <ul class="ml-10">
        <li>
          <strong>
            Your application summary lists the supporting documents you need to submit as your educational institution is not recognized by the ECE Registry
          </strong>
        </li>
        <li>
          Once submitted, we will review these documents to determine whether the {{ applicationIsEceAssistant ? "course" : "program" }} is considered
          equivalent
        </li>
      </ul>
      <br />
    </div>
    <h3>References</h3>
    <br />
    <ul class="ml-10">
      <li>We have emailed the people you identified as a reference with a link to an online form</li>
      <li>Your reference must complete the online form to provide a reference for you</li>
      <li>We will notify you when we receive the reference</li>
      <li>You can view messages in your My ECE Registry account</li>
    </ul>
    <br />
    <h3>Assessment</h3>
    <br />
    <ul class="ml-10">
      <li v-if="applicationHasTranscripts">We will assess your application after we have received your transcripts and references</li>
      <li v-else>We will assess your application after we have received your references</li>
      <li>We assess complete applications in the order they are received</li>
      <li>We will email you after we have assessed your application</li>
    </ul>
    <br />
    <h3>Status</h3>
    <br />
    <ul class="ml-10">
      <li>You may view the status of your application online in the My ECE Registry</li>
      <li>If needed, you can resend a link to your reference or manage your references</li>
    </ul>
    <br />

    <router-link :to="{ name: 'manageApplication', params: { applicationId: applicationId } }">
      <v-btn id="btnApplicationSummary" class="mt-5" rounded="lg" color="primary">Go to application summary</v-btn>
    </router-link>
  </PageContainer>
</template>
<script lang="ts">
import { defineComponent } from "vue";

import PageContainer from "@/components/PageContainer.vue";
import { useApplicationStore } from "@/store/application";

export default defineComponent({
  name: "Submitted",
  components: { PageContainer },
  props: {
    applicationId: {
      type: String,
      required: true,
    },
  },
  setup: () => {
    const applicationStore = useApplicationStore();
    // reset the draft application, so user cannot click the back button and submit another application
    applicationStore.resetDraftApplication();
    return { applicationStore };
  },
  computed: {
    applicationHasTranscripts() {
      return (this.applicationStore.application?.transcripts?.length || 0) > 0;
    },
    applicationHasEducationNotRecognized() {
      return this.applicationStore.application?.transcripts?.some(
        (transcript) => transcript.educationRecognition === "NotRecognized"
      );
    },
    applicationIsEceAssistant() {
      return this.applicationStore.application?.certificationTypes?.includes("EceAssistant");
    },
    certificationText() {
      const types = this.applicationStore.application?.certificationTypes || [];

      const hasITE = types.includes("Ite");
      const hasSNE = types.includes("Sne");

      if (hasITE && hasSNE) {
        return "We will assess your application for ECE Five Year, Infant and Toddler Educator, and Special Needs Educator certification.";
      }

      if (hasITE) {
        return "We will assess your application for ECE Five Year and Infant and Toddler Educator certification.";
      }

      if (hasSNE) {
        return "We will assess your application for ECE Five Year and Special Needs Educator certification.";
      }

      // 5-YR only (FiveYears)
      return "We will assess your application for ECE Five Year certification.";
    },
  },
});
</script>
<style scoped>
.success-banner {
  min-height: 200px;
  display: flex;
  align-items: center;
}
</style>
