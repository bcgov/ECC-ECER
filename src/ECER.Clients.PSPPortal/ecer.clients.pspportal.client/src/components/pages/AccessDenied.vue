<template>
  <v-container>
    <div class="d-flex flex-column ga-10 mt-10">
      <h1>Access denied</h1>
      <h2 class="mt-20">Your business BCeid account is not associated with this institution.</h2>
      <div class="d-flex flex-column ga-3">
        <p>you may have followed an invalid link.</p>
        <p>Contact the ECE Registry if you require help.</p>
      </div>
      <v-btn
        class="mt-8 align-self-start"
        @click="router.push('/')"
        :size="smAndDown ? 'default' : 'large'"
        color="primary"
      >
        Try again
      </v-btn>
    </div>
  </v-container>
</template>

<script lang="ts">
import { useRouter } from "vue-router";
import { useDisplay } from "vuetify";
export default {
  name: "AccessDenied",
  setup() {
    const router = useRouter();
    const { smAndDown } = useDisplay();
    return { router, smAndDown };
  },
};
</script>
