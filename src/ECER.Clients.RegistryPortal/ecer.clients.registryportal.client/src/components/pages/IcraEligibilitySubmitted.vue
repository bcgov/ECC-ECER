<template>
  <v-card
    :rounded="'0'"
    flat
    color="background-light-success"
    class="success-banner"
  >
    <v-container>
      <div class="d-flex">
        <v-icon
          size="90px"
          color="#42814A"
          icon="mdi-check-circle"
          class="mr-2"
        ></v-icon>
        <h1 id="titleApplicationSubmitted" class="align-self-center">
          Submission received
        </h1>
      </div>
    </v-container>
  </v-card>
  <PageContainer>
    <h2>What to expect next</h2>
    <br />
    <p>
      We will determine your eligibility to apply for ECE Five Year
      certification in British Columbia using professional experience obtained
      outside of Canada and contact you about your application options.
    </p>
    <br />
    <p>
      It is important to keep your contact information up to date in your My ECE
      Registry profile.
    </p>
    <br />
    <h3>International Certification</h3>
    <br />
    <ul class="ml-10">
      <li>
        We will review your information and contact your regulatory authority to
        verify the status of your certification
      </li>
      <li>We will notify you once we confirm your certification</li>
    </ul>
    <br />

    <h3>References</h3>
    <br />
    <ul class="ml-10">
      <li>
        We have emailed the people you identified as a reference with a link to
        an online form
      </li>
      <li>
        Your references must complete the online form to provide a reference for
        you
      </li>
      <li>We will notify you when we receive the references</li>
    </ul>
    <br />

    <h3>Eligibility</h3>
    <br />
    <ul class="ml-10">
      <li>
        We will determine your eligibility to apply for ECE Five Year
        certification in British Columbia using professional experience obtained
        outside of Canada and, if approved, you can continue to apply for
        certification
      </li>
      <li>We review complete submissions in the order they are received</li>
      <li>We will notify you once we have determined your eligibility</li>
    </ul>
    <br />
    <h3>Status</h3>
    <br />
    <ul class="ml-10">
      <li>
        You may view the status of your submission online in the My ECE Registry
      </li>
      <li>
        If needed, you can resend a link to your reference or manage your
        references
      </li>
    </ul>
    <br />

    <router-link
      :to="{
        name: 'manage-icra-eligibility',
        params: { icraEligibilityId: icraEligibilityId },
      }"
    >
      <v-btn
        id="btnIcraEligibilitySummary"
        class="mt-5"
        rounded="lg"
        color="primary"
      >
        Go to submission summary
      </v-btn>
    </router-link>
  </PageContainer>
</template>
<script lang="ts">
import { defineComponent } from "vue";

import PageContainer from "@/components/PageContainer.vue";
import { useIcraStore } from "@/store/icra";

export default defineComponent({
  name: "IcraEligibilitySubmitted",
  components: { PageContainer },
  props: {
    icraEligibilityId: {
      type: String,
      required: true,
    },
  },
  setup: () => {
    const icraStore = useIcraStore();
    // reset the draft application, so user cannot click the back button and submit another application
    icraStore.resetDraftIcraEligibility();
    return { icraStore };
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
