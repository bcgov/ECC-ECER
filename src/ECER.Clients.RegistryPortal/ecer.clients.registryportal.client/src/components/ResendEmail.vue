<template>
  <div class="d-flex flex-column ga-3">
    <h3 class="mt-4">Resend email</h3>
    <p>
      We’ll send another email to the reference. It’ll include a link to the My ECE Registry to provide a reference for you. It may take a few minutes to
      receive the email.
      <br />
      If they do not receive the email:
    </p>
    <ul class="ml-10">
      <li>Ask the person to check their spam folder</li>
      <li>Check you provided the correct email address</li>
    </ul>
    <p>If you need to correct any information, choose to use a new reference below.</p>
  </div>
  <v-btn
    :loading="
      loadingStore.isLoading('application_character_reference_resend_invite_post') ||
      loadingStore.isLoading('application_work_experience_reference_resend_invite_post')
    "
    class="mt-6"
    size="large"
    variant="outlined"
    @click="$emit('resend')"
  >
    Resend email
  </v-btn>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import { useLoadingStore } from "@/store/loading";

export default defineComponent({
  name: "ResendEmail",
  emits: ["resend"],
  setup() {
    const loadingStore = useLoadingStore();
    return { loadingStore };
  },
});
</script>
