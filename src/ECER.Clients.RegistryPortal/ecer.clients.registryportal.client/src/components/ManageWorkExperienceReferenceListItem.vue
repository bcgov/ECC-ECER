<template>
  <v-card elevation="0" rounded="0" class="border-t border-b">
    <v-card-text>
      <v-row class="d-flex" :class="[smAndUp ? 'justify-space-between align-center' : 'flex-column']">
        <v-col cols="12" sm="4">
          <div v-if="statusText !== 'Not yet received'">
            <p>{{ referenceFullName }}</p>
          </div>
          <a v-else href="#" @click.prevent="buttonClick">
            <p class="text-links">{{ referenceFullName }}</p>
          </a>
        </v-col>
        <v-col cols="12" sm="4" :align="smAndUp ? 'center' : ''">
          <p>{{ hours }} hours</p>
        </v-col>
        <v-col cols="12" sm="4" :align="smAndUp ? 'right' : ''">
          <v-sheet rounded width="200px" class="py-2 text-center" :class="{ 'mt-2': !smAndUp }" :color="sheetColor">
            <p>{{ statusText }}</p>
          </v-sheet>
        </v-col>
      </v-row>
    </v-card-text>
  </v-card>
</template>

<script lang="ts">
import type { PropType } from "vue";
import { defineComponent } from "vue";
import { useDisplay } from "vuetify";

import type { Components } from "@/types/openapi";

export default defineComponent({
  name: "ManageWorkExperienceReferenceListItem",
  props: {
    reference: {
      type: Object as PropType<Components.Schemas.WorkExperienceReferenceStatus>,
      required: true,
    },
    applicationStatus: {
      type: String as PropType<Components.Schemas.ApplicationStatus>,
      required: true,
    },
    goTo: {
      type: Function,
      required: true,
    },
  },
  setup: async () => {
    const { smAndUp } = useDisplay();
    return { smAndUp };
  },
  computed: {
    statusText() {
      switch (this.reference.status) {
        case "ApplicationSubmitted":
          return "Not yet received";
        case "Approved":
          return "Approved";
        case "Rejected":
          return "Cancelled";
        case "InProgress":
        case "UnderReview":
        case "WaitingforResponse":
        case "Submitted":
          if (this.applicationStatus === "Submitted") {
            return "Received";
          } else {
            return "Received, pending review";
          }
        default:
          return "Unhandled Status";
      }
    },
    hours(): string | number {
      switch (this.statusText) {
        case "Not yet received":
          return this.reference.totalNumberofHoursAnticipated ?? 0;
        case "Approved":
          return this.reference.totalNumberofHoursApproved ?? 0;
        case "Cancelled":
          return "—";
        case "Received":
        case "Received, pending review":
          return this.reference.totalNumberofHoursObserved ?? 0;
        default:
          return 0;
      }
    },
    link(): boolean {
      return this.reference.status === "ApplicationSubmitted";
    },
    sheetColor() {
      return this.link ? "hawkes-blue" : "white-smoke";
    },
    referenceFullName(): string {
      return `${this.reference.firstName} ${this.reference.lastName}`;
    },
  },
  methods: {
    buttonClick() {
      this.goTo();
    },
  },
});
</script>
