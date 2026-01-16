<template>
  <v-sheet
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
        <h1 id="programProfileSubmitted" class="align-self-center">
          Submitted
        </h1>
      </div>
    </v-container>
  </v-sheet>

  <v-container>
    <v-btn
      id="btnHome"
      rounded="lg"
      color="primary"
      @click="router.push({ name: 'dashboard' })"
    >
      Home
    </v-btn>
  </v-container>
</template>
<script lang="ts">
import { defineComponent } from "vue";

import { useProgramStore } from "@/store/program";
import { useRouter } from "vue-router";

export default defineComponent({
  name: "Submitted",
  setup: () => {
    const programStore = useProgramStore();
    const router = useRouter();

    programStore.resetDraftProgram();
    return { programStore, router };
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
