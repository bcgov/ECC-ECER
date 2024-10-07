<template>
  <v-sheet class="bg-primary">
    <v-container><h1>Validate an ECE certificate</h1></v-container>
  </v-sheet>
  <v-container>
    <v-row>
      <v-col class="text-break">
        To work as an Early Childhood Educator (ECE) or ECE Assistant in a licensed child care facility in B.C. people must be certified by the ECE Registry.
        Visit our website
        <a
          target="_blank"
          href="https://www2.gov.bc.ca/gov/content/education-training/early-learning/teach/training-and-professional-development/become-an-early-childhood-educator"
        >
          www.gov.bc.ca/earlychildhoodeducators
        </a>
        to learn more about the ECE Registry, certificate types, or status information. To find an ECE, you can search by name or registration number. The
        registration number is on an ECE's certificate that's posted on the wall where they work.
      </v-col>
    </v-row>
    <v-form ref="lookupForm" class="mt-10">
      <v-row>
        <v-col cols="12" sm="8" lg="4">
          <v-text-field v-model="firstName" hide-details="auto" variant="outlined" label="First name"></v-text-field>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" sm="8" lg="4">
          <v-text-field v-model="lastName" hide-details="auto" variant="outlined" label="Last name"></v-text-field>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" sm="8" lg="4">
          <v-text-field v-model="registrationNumber" hide-details="auto" variant="outlined" label="ECE registration number"></v-text-field>
        </v-col>
      </v-row>
      <v-row>
        <v-col>
          <EceRecaptcha
            :model-value="recaptchaToken"
            :props="{ rules: [Rules.required('Check to confirm you are not a robot')], recaptchaElementId: 'recaptchaLookup' }"
            @update:model-value="(value: string) => (recaptchaToken = value)"
          ></EceRecaptcha>
        </v-col>
      </v-row>
      <!-- this is to check if all fields are blank without making one input box red -->
      <v-input id="inputFieldsError" :rules="[customAtLeastOneRule()]"></v-input>
    </v-form>
    <v-row>
      <v-col>
        <v-btn rounded="lg" color="primary" @click="handleSubmit">Search</v-btn>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import ECEHeader from "./ECEHeader.vue";
import type { VForm } from "vuetify/components";
import { useAlertStore } from "@/store/alert";
import EceRecaptcha from "./inputs/EceRecaptcha.vue";
import * as Rules from "../utils/formRules";

export default defineComponent({
  name: "LookupCertification",
  components: { ECEHeader, EceRecaptcha },
  setup() {
    const alertStore = useAlertStore();

    return { alertStore, Rules };
  },
  data() {
    return {
      firstName: "",
      lastName: "",
      registrationNumber: "",
      recaptchaToken: "",
    };
  },
  computed: {},
  methods: {
    customAtLeastOneRule() {
      return () =>
        !!(this.firstName && this.firstName?.trim()) ||
        !!(this.lastName && this.lastName?.trim()) ||
        !!(this.registrationNumber && this.registrationNumber?.trim());
    },
    async handleSubmit() {
      const { valid, errors } = await (this.$refs.lookupForm as VForm).validate();
      console.log(valid);
      console.log(errors);

      if (!valid) {
        if (errors.some((error) => error.id === "inputFieldsError")) {
          this.alertStore.setFailureAlert("You must enter at least one option");
        }
      }
    },
  },
});
</script>
