<template>
  <v-container>
    <Breadcrumb />
    <v-row>
      <v-col>
        <h1>
          Check if you are eligible to apply with international certification
        </h1>
      </v-col>
    </v-row>

    <v-row>
      <v-col>
        <ECEHeader title="International certification" />
        <p>Your certificates(s) must:</p>
        <ul class="ml-10">
          <li>
            Be issued by a country or region that regulates the early childhood
            education profession
          </li>
          <li>Be in good standing</li>
          <li>
            Be valid (you can provide expired certification, but you must also
            provide at least one valid certification)
          </li>
        </ul>
      </v-col>
    </v-row>
    <v-row>
      <v-col>
        <ECEHeader title="Employment experience" />
        <p>
          You need to have completed 2 years of independent ECE practice and be
          able to provide references to verify your employment experience. If
          you worked at multiple locations, you can provide multiple references.
        </p>
      </v-col>
    </v-row>
    <v-row>
      <v-col>
        <p>Important information about employment experience:</p>
        <ul class="ml-10">
          <li>
            Only include employment experience completed within the last 5 years
          </li>
          <li>
            Must have held valid ECE certification at the time the employment
            experience was completed
          </li>
        </ul>
      </v-col>
    </v-row>
    <v-row>
      <v-col>
        <!-- prettier-ignore -->
        <p>
          If you do not meet these requirements, you can
          <a href="/">apply for certification through another pathway</a>.
        </p>
      </v-col>
    </v-row>
    <v-row>
      <v-col>
        <v-btn
          class="mt-6"
          rounded="lg"
          color="primary"
          @click="router.push({ name: 'icra-eligibility-declaration' })"
          id="btnContinue"
        >
          Continue
        </v-btn>
      </v-col>
    </v-row>
  </v-container>
</template>

<script>
import Breadcrumb from "@/components/Breadcrumb.vue";
import ECEHeader from "@/components/ECEHeader.vue";
import { useRouter } from "vue-router";

export default {
  name: "IcraEligibilityRequirements",
  components: { Breadcrumb, ECEHeader },
  setup() {
    const router = useRouter();

    return { router };
  },
};
</script>
