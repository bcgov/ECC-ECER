<template>
  <v-sheet class="bg-primary">
    <v-container><h1>Validate an ECE certificate</h1></v-container>
  </v-sheet>
  <v-container>
    <v-row v-for="systemMessage in configStore.systemMessages" class="mt-10 mb-10">
      <v-col v-if="systemMessage.portalTags && systemMessage.portalTags.includes('LOOKUP')" cols="12">
        <Banner type="info" :title="systemMessage.message ? systemMessage.message : ''" />
      </v-col>
    </v-row>
    <v-row>
      <v-col class="text-break">
        To work as an Early Childhood Educator (ECE) or ECE Assistant in a licensed child care facility in B.C. people must be certified by the ECE Registry.
        Visit our website
        <!-- prettier-ignore -->
        <a target="_blank" href="https://www2.gov.bc.ca/gov/content/education-training/early-learning/teach/training-and-professional-development/become-an-early-childhood-educator">www.gov.bc.ca/earlychildhoodeducators</a>
        to learn more about the ECE Registry, certificate types, or status information.
        <p class="mt-3">
          To find an ECE, you can search by name or registration number. The registration number is on an ECEs certificate that's posted on the wall where they
          work.
        </p>
      </v-col>
    </v-row>
    <v-form ref="lookupForm" class="mt-10">
      <v-row>
        <v-col cols="12" sm="8" lg="4">
          <EceTextField v-model="lookupCertificationStore.firstName" label="First name"></EceTextField>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" sm="8" lg="4">
          <EceTextField v-model="lookupCertificationStore.lastName" label="Last name"></EceTextField>
        </v-col>
      </v-row>
      <v-row>
        <v-col cols="12" sm="8" lg="4">
          <EceTextField
            v-model="lookupCertificationStore.registrationNumber"
            label="ECE registration number"
            maxlength="6"
            @keypress="isNumber($event)"
          ></EceTextField>
        </v-col>
      </v-row>
      <v-row>
        <v-col>
          <EceRecaptcha
            :model-value="recaptchaToken"
            :rules="[Rules.required('Check to confirm you are not a robot')]"
            recaptchaElementId="recaptchaLookup"
            @update:model-value="(value: string) => (recaptchaToken = value)"
          ></EceRecaptcha>
        </v-col>
      </v-row>
      <!-- this is to check if all fields are blank without making one input box red -->
      <v-input id="inputFieldsError" :rules="[customAtLeastOneRule()]"></v-input>
      <v-row>
        <v-col>
          <v-btn rounded="lg" color="primary" @click="handleSubmit" :loading="loadingStore.isLoading('certifications_lookup_post')">Search</v-btn>
        </v-col>
      </v-row>
    </v-form>

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
        <v-data-table-virtual
          :headers="headers"
          :loading="loadingStore.isLoading('certifications_lookup_post')"
          :items="lookupCertificationStore.certificationSearchResults"
          :items-per-page="-1"
          :mobile="mobile"
          style="font-size: 16px"
          must-sort
        >
          <template #item.name="{ item }">
            <a href="#" @click.prevent="() => applicantClick(item)">{{ item.name }}</a>
          </template>
          <template #item.statusCode="{ item }">
            <span>{{ `${item.statusCode}${item.hasConditions ? " with Terms and Conditions" : ""}` }}</span>
          </template>
          <template #item.levels="{ item }">
            <span>{{ lookupCertificationStore.generateCertificateLevelName(item.levels || []) }}</span>
          </template>
          <template #item.expiryDate="{ item }">
            <span>{{ formatDate(item.expiryDate || "", "LLLL d, yyyy") }}</span>
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
import EceTextField from "@/components/inputs/EceTextField.vue";
import { useAlertStore } from "@/store/alert";
import { useLookupCertificationStore } from "@/store/lookupCertification";
import { useLoadingStore } from "@/store/loading";
import { formatDate } from "@/utils/format";
import { isNumber } from "@/utils/formInput";
import { postLookupCertificate } from "@/api/certification";
import { useConfigStore } from "@/store/config";
import Banner from "@/components/Banner.vue";
import * as Rules from "../utils/formRules";
import EceRecaptcha from "./inputs/EceRecaptcha.vue";
import type { Components } from "@/types/openapi";

interface LookupCertificationData {
  recaptchaToken: string;
  headers: ReadonlyHeaders;
}

type ReadonlyHeaders = VDataTable["$props"]["headers"];

export default defineComponent({
  name: "LookupCertification",
  components: { EceRecaptcha, EceTextField, Banner },
  setup() {
    const alertStore = useAlertStore();
    const lookupCertificationStore = useLookupCertificationStore();
    const { mobile } = useDisplay();
    const configStore = useConfigStore();
    const router = useRouter();
    const loadingStore = useLoadingStore();

    return { alertStore, configStore, Rules, mobile, lookupCertificationStore, loadingStore, router, isNumber, formatDate };
  },
  data(): LookupCertificationData {
    return {
      recaptchaToken: "",
      headers: [
        { title: "Name", key: "name" },
        { title: "Registration number", key: "registrationNumber" },
        { title: "Registration status", key: "statusCode" },
        { title: "Certification", key: "levels" },
        { title: "Certificate expiry date", key: "expiryDate" },
      ],
    };
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
          const { data } = await postLookupCertificate({
            firstName: this.lookupCertificationStore.firstName,
            lastName: this.lookupCertificationStore.lastName,
            registrationNumber: this.lookupCertificationStore.registrationNumber,
            recaptchaToken: this.recaptchaToken,
          });

          this.lookupCertificationStore.setCertificationSearchResults(data);

          //reset grecaptcha after success, token cannot be reused
          this.recaptchaToken = "";
          window.grecaptcha.reset();
          await this.$nextTick();
          (this.$refs.lookupForm as VForm).resetValidation();
        }
      } catch (e) {
        console.error(e);
      }
    },
    applicantClick(item: Components.Schemas.CertificationLookupResponse) {
      this.lookupCertificationStore.setCertificationRecord(item);
      this.router.push({ name: "lookup-certification-record" });
    },
  },
});
</script>
