<template>
  <v-card :rounded="'0'" flat color="background-light-success" class="success-banner">
    <v-container>
      <div class="d-flex">
        <v-icon size="90px" color="#42814A" icon="mdi-check-circle" class="mr-2"></v-icon>
        <h1 class="align-self-center">Application Submitted</h1>
      </div>
    </v-container>
  </v-card>
  <PageContainer>
    <h2>What to expect next</h2>
    <br />
    <p>It's important to keep your contact information up-to-date in your My ECE Registry profile.</p>
    <br />
    <div v-if="draftApplicationHasTranscripts">
      <h3>Transcripts</h3>
      <br />
      <ul class="ml-10">
        <li>Make sure you've requested the transcripts be sent to the ECE Registry directly from the educational institution</li>
        <li>We'll email you once we receive them</li>
      </ul>
    </div>
    <br />
    <div v-if="draftApplicationHasTranscripts && draftApplicationHasEducationNotRecognized">
      <h3>Supporting documents</h3>
      <br />
      <ul class="ml-10">
        <li>
          We'll send you a message soon with a list of additional forms and supporting documentation we need from you because your educational institution is
          not recognized by the ECE Registry
        </li>
        <li>We'll review these documents to help determine whether the course or program is deemed equivalent</li>
      </ul>
      <br />
    </div>
    <h3>References</h3>
    <br />
    <ul class="ml-10">
      <li>We've emailed the people you identified as a reference with a link to an online form</li>
      <li>Your reference must complete the online form to provide a reference for you</li>
      <li>We'll notify you when we receive the reference</li>
      <li>You can view messages in your My ECE Registry account</li>
    </ul>
    <br />
    <h3>Assessment</h3>
    <br />
    <ul class="ml-10">
      <li>We'll assess your application after we've received your transcripts and references</li>
      <li>We assess complete applications in the order they're received</li>
      <li>We'll email you after we've assessed your application</li>
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
      <v-btn class="mt-5" type="" rounded="lg" color="primary">Go to application summary</v-btn>
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
    return { applicationStore };
  },
  computed: {
    draftApplicationHasTranscripts() {
      return (this.applicationStore.draftApplication.transcripts?.length || 0) > 0;
    },
    draftApplicationHasEducationNotRecognized() {
      return this.applicationStore.draftApplication.transcripts?.some((transcript) => transcript.educationRecognition === "NotRecognized");
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
