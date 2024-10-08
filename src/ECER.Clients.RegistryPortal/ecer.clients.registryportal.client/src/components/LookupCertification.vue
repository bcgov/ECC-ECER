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
    <v-row v-if="certificationSearchResults?.length === 0">
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
    <v-row v-else-if="certificationSearchResults && certificationSearchResults?.length > 0">
      <!-- desktop/tablet list view -->
      <v-col v-if="!mobile">
        <h2>{{ `${certificationSearchResults.length} record${certificationSearchResults.length === 1 ? "" : "s"} found` }}</h2>
        <v-data-table-virtual :headers="headers" :items="certificationSearchResults" :items-per-page="-1">
          <template #item="{ item }">
            <tr>
              <td>
                <a href="#" @click.prevent="() => applicantClick(item)">{{ `${item.firstName} ${item.lastName}` }}</a>
              </td>
              <td>{{ item.registrationNumber }}</td>
              <td>{{ item.certification }}</td>
              <td>{{ item.status }}</td>
            </tr>
          </template>
        </v-data-table-virtual>
      </v-col>
      <!-- mobile list view -->
      <v-col v-if="mobile">
        <h2>{{ `${certificationSearchResults.length} record${certificationSearchResults.length === 1 ? "" : "s"} found` }}</h2>
        <v-card v-for="(result, index) in certificationSearchResults" :key="index" elevation="0">
          <v-card-text>
            <div v-for="(header, index) in headers" :key="`${header.key}${index}}`">
              <p class="font-weight-bold">{{ header.key }}</p>
              <a v-if="header.key === 'name'" href="#" @click.prevent="() => applicantClick(result)">{{ header.key }}</a>
              <!-- render link only for name header -->
              <p v-else>{{ result[header.key] }}</p>
            </div>
          </v-card-text>
        </v-card>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import ECEHeader from "./ECEHeader.vue";
import type { VForm, VDataTable } from "vuetify/components";
import { useAlertStore } from "@/store/alert";
import EceRecaptcha from "./inputs/EceRecaptcha.vue";
import * as Rules from "../utils/formRules";
import { useDisplay } from "vuetify";
import { useLookupCertificationStore } from "@/store/lookupCertification";

interface LookupCertificationData {
  firstName: string;
  lastName: string;
  registrationNumber: string;
  recaptchaToken: string;
  certificationSearchResults: undefined | [];
  headers: ReadonlyHeaders;
}

type ReadonlyHeaders = VDataTable["$props"]["headers"];

export default defineComponent({
  name: "LookupCertification",
  components: { ECEHeader, EceRecaptcha },
  setup() {
    const alertStore = useAlertStore();
    const lookupCertificationStore = useLookupCertificationStore();
    const { mobile } = useDisplay();

    return { alertStore, Rules, mobile, lookupCertificationStore };
  },
  data(): LookupCertificationData {
    return {
      firstName: "",
      lastName: "",
      registrationNumber: "",
      recaptchaToken: "",
      certificationSearchResults: [
        { name: "first last", registrationNumber: "1234", certification: "nothing", status: "fake" },
        { name: "first2 last2", registrationNumber: "1234", certification: "1nothing", status: "2fake" },
      ],
      headers: [
        { title: "Name", key: "name", sortable: false },
        { title: "Registration number", key: "registrationNumber", sortable: false },
        { title: "Certification", key: "certification", sortable: false },
        { title: "Registration status", key: "status", sortable: false },
      ],
    };
  },
  mounted() {
    this.lookupCertificationStore.setCertificationSearchResults([
      { name: "first last", registrationNumber: "1234", certification: "nothing", status: "fake" },
      { name: "first2 last2", registrationNumber: "1234", certification: "1nothing", status: "2fake" },
    ]);
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
      try {
        const { valid, errors } = await (this.$refs.lookupForm as VForm).validate();

        if (!valid) {
          if (errors.some((error) => error.id === "inputFieldsError")) {
            this.alertStore.setFailureAlert("You must enter at least one option");
          }
        } else {
          //make api call
          this.certificationSearchResults = [
            { firstName: "first", lastName: "last", registrationNumber: "1234", certification: "nothing", status: "fake" },
            { firstName: "first2", lastName: "last2", registrationNumber: "1234", certification: "1nothing", status: "2fake" },
          ];
        }
      } catch (e) {
        console.error(e);
        this.alertStore.setFailureAlert("You must enter at least one option");
      }
    },
    applicantClick(item) {
      this.alertStore.setSuccessAlert(item);
      this.lookupCertificationStore.setCertificationRecord(item);
      

    },
  },
});
</script>
