<template>
  <v-card :rounded="!isRounded ? '0' : ''" flat color="primary">
    <v-card-item class="ma-4">
      <h2 class="text-white">{{ title }}</h2>
      <p class="small text-white mt-4">
        {{ subTitle }}
      </p>
    </v-card-item>

    <div class="d-flex flex-row justify-start ga-3 flex-wrap ma-4">
      <!-- ICRA eligibility status Draft -->
      <v-card-actions v-if="icraStore.icraEligibilityStatus === 'Draft' || icraStore.icraEligibilityStatus === 'Active'">
        <v-btn size="large" variant="flat" color="warning" @click="router.push('/icra-eligibility')">
          <v-icon size="large" icon="mdi-arrow-right" />
          Continue submission
        </v-btn>
      </v-card-actions>

      <!-- ICRA Eligibility status Submitted, Ready, In Progress, Pending Queue -->
      <v-card-actions
        v-if="
          icraStore.icraEligibilityStatus === 'Submitted' ||
          icraStore.icraEligibilityStatus === 'ReadyforReview' ||
          icraStore.icraEligibilityStatus === 'InReview'
        "
      >
        <v-btn variant="flat" size="large" color="warning" @click="handleManageIcraEligibility">
          <v-icon size="large" icon="mdi-arrow-right" />
          Manage ICRA eligibility
        </v-btn>
      </v-card-actions>
    </div>
  </v-card>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import { useIcraStore } from "@/store/icra";
import { useRouter } from "vue-router";
import { formatDate } from "@/utils/format";

export default defineComponent({
  name: "IcraEligibilityCard",
  props: {
    isRounded: {
      type: Boolean,
      default: true,
    },
  },
  // emits: ["cancel-application"],
  setup() {
    const icraStore = useIcraStore();
    const router = useRouter();

    return {
      icraStore,
      router,
    };
  },
  computed: {
    title(): string {
      switch (this.icraStore.icraEligibilityStatus) {
        case "Draft":
        case "Submitted":
        case "InReview":
        case "ReadyforReview":
        case "Active":
          return "Your submission to determine eligibility to apply with international certification is in progress";
        default:
          return "Apply with international certification";
      }
    },
    subTitle(): string {
      switch (this.icraStore.icraEligibilityStatus) {
        case "Draft":
        case "Active":
        case "Submitted":
        case "InReview":
        case "ReadyforReview":
          return `Started ${formatDate(this.icraStore.icraEligibility?.createdOn || "", "LLLL d, yyyy")}`;
        default:
          return "Apply for ECE Five Year Certification if you are internationally certified in a country that regulates the ECE profession and do not have 500 hours work experience supervised by a Canadian-certified ECE.";
      }
    },
  },
  methods: {
    handleManageIcraEligibility() {
      this.router.push({ name: "manage-icra-eligibility", params: { icraEligibilityId: this.icraStore?.icraEligibility?.id } });
    },
  },
});
</script>
