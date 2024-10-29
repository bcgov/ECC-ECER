<template>
  <v-sheet class="bg-primary">
    <v-container><h1>Validate an ECE certificate</h1></v-container>
  </v-sheet>
  <v-container>
    <!-- prettier-ignore -->
    <a href="#" @click.prevent="router.push({ name: 'lookup-certification' })">
      <v-icon size='x-large'>mdi-chevron-left</v-icon>Back to search
    </a>
    <div>
      <h2 class="mt-7">{{ lookupCertificationStore.certificationRecord?.name }}</h2>
      <p class="font-weight-bold mt-4">ECE registration number</p>
      <span>{{ lookupCertificationStore.certificationRecord?.registrationNumber }}</span>
      <p class="font-weight-bold mt-4">Registration status</p>
      <v-row class="mt-2" no-gutters>
        <v-col cols="12" md="auto" class="mr-2 mb-2">
          <v-chip :color="chipColor" variant="flat" size="small">{{ chipText }}</v-chip>
        </v-col>
        <v-col cols="12" md="auto">
          <v-chip v-if="lookupCertificationStore.certificationRecord?.hasConditions" color="grey-darkest" variant="outlined" size="small">
            Has Terms and Conditions
          </v-chip>
        </v-col>
      </v-row>
      <p class="font-weight-bold mt-4">Certification</p>
      <span>{{ lookupCertificationStore.generateCertificateLevelName(lookupCertificationStore.certificationRecord?.levels || []) }}</span>
      <p class="font-weight-bold mt-4">Certificate expiry date</p>
      <span>{{ formatDate(lookupCertificationStore.certificationRecord?.expiryDate || "", "LLLL d, yyyy") }}</span>
      <div v-if="lookupCertificationStore.certificationRecord?.hasConditions">
        <p class="font-weight-bold mt-7 mb-2">Certificate terms and conditions</p>
        <ul class="ml-10">
          <li v-for="(condition, index) in sortedCertificateConditions" :key="index">
            {{ condition.details }}
          </li>
        </ul>
      </div>
    </div>
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useDisplay } from "vuetify";
import { useRouter } from "vue-router";

import { useLookupCertificationStore } from "@/store/lookupCertification";
import { formatDate } from "@/utils/format";

export default defineComponent({
  name: "LookupCertification",
  setup() {
    const lookupCertificationStore = useLookupCertificationStore();
    const router = useRouter();
    const { mobile } = useDisplay();
    return { lookupCertificationStore, router, formatDate, mobile };
  },
  mounted() {
    //refresh safety
    if (this.lookupCertificationStore.certificationRecord === undefined) {
      this.router.push({ name: "lookup-certification" });
    }
  },
  computed: {
    sortedCertificateConditions() {
      // Sort the conditions based on displayOrder
      return this.lookupCertificationStore.certificationRecord?.certificateConditions?.sort((a, b) => a.displayOrder! - b.displayOrder!) || [];
    },
    chipText() {
      // "Active" | "Cancelled" | "Expired" | "Inactive" | "Reprinted" | "Suspended"
      switch (this.lookupCertificationStore.certificationRecord?.statusCode) {
        case "Active":
        case "Reprinted":
          return "Active";
        case "Renewed":
          return "Renewed";
        case "Expired":
        case "Inactive":
          return "Expired";
        case "Cancelled":
          return "Cancelled";
        case "Suspended":
          return "Suspended";
        default:
          return "Expired";
      }
    },
    chipColor(): string {
      // "success" | "error" | "warning" | "info"
      switch (this.chipText) {
        case "Active":
        case "Renewed":
          return "success";
        case "Expired":
          return "error";
        case "Cancelled":
        case "Suspended":
          return "grey-darkest";
        default:
          return "grey-darkest";
      }
    },
  },
  methods: {},
});
</script>
