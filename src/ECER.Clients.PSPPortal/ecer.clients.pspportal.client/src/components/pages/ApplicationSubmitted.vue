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
        <h1 id="programApplicationSubmitted" class="align-self-center">
          Application submitted
        </h1>
      </div>
    </v-container>
  </v-sheet>
  <v-container>
    <h2>What to expect next</h2>
    <br />
    <p>
      It is important to keep your educational institution and user contact
      information updated in your ECE Post-Secondary Programs portal profile.
    </p>
    <br />
    <h2>Assessment</h2>
    <br />
    <ul class="ml-10">
      <li>
        We will assess your application and follow up with any requests for
        additional information, if needed
      </li>
      <li>We will email you after we have assessed your application</li>
    </ul>
    <br />
    <h2>Status</h2>
    <br />
    <p>
      You may view the status of your application online in the ECE
      Post-Secondary Programs portal dashboard.
    </p>
    <br />
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
import { useRouter } from "vue-router";

export default defineComponent({
  name: "Submitted",
  props: {
    programApplicationId: {
      type: String,
      required: true,
    },
  },
  setup: () => {
    const router = useRouter();
    return { router };
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
