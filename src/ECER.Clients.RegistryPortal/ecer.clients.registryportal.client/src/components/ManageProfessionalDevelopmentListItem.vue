<template>
  <v-card elevation="0" rounded="0" class="border-t border-b">
    <v-card-text>
      <v-row class="d-flex" :class="[smAndUp ? 'justify-space-between align-center' : 'flex-column']">
        <v-col cols="12" sm="4">
          <div>
            <p>{{ courseName }}</p>
          </div>
        </v-col>
        <v-col cols="12" sm="4" :align="smAndUp ? 'center' : ''">
          <p>{{ hoursText }}</p>
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
  name: "ManageProfessionalDevelopmentListItem",
  props: {
    professionalDevelopment: {
      type: Object as PropType<Components.Schemas.ProfessionalDevelopmentStatus>,
      required: true,
    },
  },
  setup: async () => {
    const { smAndUp } = useDisplay();
    return { smAndUp };
  },
  computed: {
    statusText() {
      switch (this.professionalDevelopment.status) {
        case "Approved":
          return "Approved";
        case "Rejected":
          return "Cancelled";
        case "Submitted":
          return "Pending review";
        default:
          return "Unhandled Status";
      }
    },
    hoursText(): string {
      switch (this.statusText) {
        case "Approved":
        case "Pending review":
          return `${this.professionalDevelopment.numberOfHours ?? 0} hours`;
        case "Cancelled":
          return "—";
        default:
          return "0 hours";
      }
    },
    link(): boolean {
      return this.professionalDevelopment.status === "Submitted";
    },
    sheetColor() {
      return this.link ? "hawkes-blue" : "white-smoke";
    },
    courseName(): string {
      return `${this.professionalDevelopment.courseName}`;
    },
  },
});
</script>
