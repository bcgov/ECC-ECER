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
          <v-text-field v-model="lookupCertificationStore.firstName" hide-details="auto" variant="outlined" label="First name"></v-text-field>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" sm="8" lg="4">
          <v-text-field v-model="lookupCertificationStore.lastName" hide-details="auto" variant="outlined" label="Last name"></v-text-field>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" sm="8" lg="4">
          <v-text-field
            v-model="lookupCertificationStore.registrationNumber"
            hide-details="auto"
            variant="outlined"
            label="ECE registration number"
            maxlength="6"
            @keypress="isNumber($event)"
          ></v-text-field>
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
    <v-row v-if="lookupCertificationStore.certificationSearchResults?.length === 0">
      <v-col>
        <h2>No records found</h2>
        <p>If you cannot find an ECE, it may be because the:</p>
        <br />
        <ul class="ml-10">
          <li>Name was entered incorrectly - check the spelling and that you're entering their legal name</li>
          <li>Person is not certified</li>
          <li>Person is still in the process of applying for certification</li>
        </ul>
      </v-col>
    </v-row>
    <v-row v-else-if="lookupCertificationStore.certificationSearchResults && lookupCertificationStore.certificationSearchResults?.length > 0">
      <v-col>
        <h2>
          {{
            `${lookupCertificationStore.certificationSearchResults.length} record${lookupCertificationStore.certificationSearchResults.length === 1 ? "" : "s"} found`
          }}
        </h2>
        <v-data-table-virtual :headers="headers" :items="lookupCertificationStore.certificationSearchResults" :items-per-page="-1" :mobile="mobile" must-sort>
          <template #item.name="{ item }">
            <a href="#" @click.prevent="() => applicantClick(item)">{{ item.name }}</a>
          </template>
          <template #item.statusCode="{ item }">
            <span>{{ `${item.statusCode}${item.hasConditions ? " with Terms and Conditions" : ""}` }}</span>
          </template>
          <template #item.expiryDate="{ item }">
            <span>{{ formatDate(item.expiryDate, "LLLL d, yyyy") }}</span>
          </template>
        </v-data-table-virtual>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useRouter } from "vue-router";
import { useDisplay } from "vuetify";
import type { VDataTable, VForm } from "vuetify/components";

import { useAlertStore } from "@/store/alert";
import { useLookupCertificationStore } from "@/store/lookupCertification";
import { formatDate } from "@/utils/format";
import { isNumber } from "@/utils/formInput";

import * as Rules from "../utils/formRules";
import EceRecaptcha from "./inputs/EceRecaptcha.vue";

interface LookupCertificationData {
  recaptchaToken: string;
  headers: ReadonlyHeaders;
}

type ReadonlyHeaders = VDataTable["$props"]["headers"];

export default defineComponent({
  name: "LookupCertification",
  components: { EceRecaptcha },
  setup() {
    const alertStore = useAlertStore();
    const lookupCertificationStore = useLookupCertificationStore();
    const { mobile } = useDisplay();
    const router = useRouter();

    return { alertStore, Rules, mobile, lookupCertificationStore, router, isNumber, formatDate };
  },
  data(): LookupCertificationData {
    return {
      recaptchaToken: "",
      headers: [
        { title: "Name", key: "name" },
        { title: "Registration number", key: "registrationNumber" },
        { title: "Registration status", key: "statusCode" },
        { title: "Certification", key: "levelName" },
        { title: "Certificate expiry date", key: "expiryDate" },
      ],
    };
  },
  computed: {},
  mounted() {
    // TODO REmove
  },
  methods: {
    customAtLeastOneRule() {
      return () =>
        !!(this.lookupCertificationStore.firstName && this.lookupCertificationStore.firstName?.trim()) ||
        !!(this.lookupCertificationStore.lastName && this.lookupCertificationStore.lastName?.trim()) ||
        !!(this.lookupCertificationStore.registrationNumber && this.lookupCertificationStore.registrationNumber?.trim());
    },
    async handleSubmit() {
      try {
        const { valid, errors } = await (this.$refs.lookupForm as VForm).validate();

        if (!valid) {
          if (errors.some((error) => error.id === "inputFieldsError")) {
            this.alertStore.setFailureAlert("You must enter at least one option");
          }
        } else {
          if (this.lookupCertificationStore.registrationNumber) {
            this.lookupCertificationStore.registrationNumber = this.lookupCertificationStore.registrationNumber.padStart(6, "0");
          }

          //make api call
          this.lookupCertificationStore.certificationSearchResults = [
            {
              name: "first withConditions",
              registrationNumber: "1234",
              statusCode: "Active",
              levelName: "ECE Assistant",
              expiryDate: "2024-11-24",
              hasConditions: true,
            },
            {
              name: "first noConditions",
              registrationNumber: "1234",
              statusCode: "Expired",
              levelName: "ECE Five Year",
              expiryDate: "2025-11-24",
              hasConditions: false,
            },
          ];
        }
      } catch (e) {
        console.error(e);
      }
    },
    applicantClick(item) {
      this.lookupCertificationStore.setCertificationRecord(item);
      this.router.push({ name: "lookup-certification-record" });
    },
  },
});
</script>
