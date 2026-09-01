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
        <h1 class="align-self-center">Dispute submitted</h1>
      </div>
    </v-container>
  </v-card>
  <PageContainer>
    <h2>What to expect next</h2>
    <br />
    <p>
      The ECE Registry will contact you to discuss next steps and verify any of
      the provided information.
    </p>
    <br />
    <p>
      It is important to keep your contact information up-to-date in your My ECE
      Registry profile.
    </p>
    <router-link
      :to="{
        name: 'dashboard',
      }"
    >
      <v-btn id="homeBtn" class="mt-10" rounded="lg" color="primary">
        Go to dashboard
      </v-btn>
    </router-link>
  </PageContainer>
</template>
<script lang="ts">
import { defineComponent } from "vue";
import type { PropType } from "vue";

import PageContainer from "@/components/PageContainer.vue";
import type { ReconsiderationType } from "@/types/reconsideration";

export default defineComponent({
  name: "ReconsiderationSubmitted",
  components: { PageContainer },
  props: {
    reconsiderationId: {
      type: String,
      required: true,
    },
    reconsiderationType: {
      type: String as PropType<ReconsiderationType>,
      required: true,
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
