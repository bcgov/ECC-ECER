<template>
  <v-card elevation="0" rounded="0" class="border-t border-b">
    <v-card-text>
      <div
        class="d-flex"
        :class="[smAndUp ? 'space-between align-center' : 'flex-column']"
      >
        <p>Transcript provided by {{ name }}</p>

        <v-spacer></v-spacer>
        <v-sheet
          rounded
          width="200px"
          class="py-2 text-center"
          :class="{ 'mt-2': !smAndUp }"
          :color="sheetColor"
        >
          <p>{{ statusText }}</p>
        </v-sheet>
      </div>
    </v-card-text>
  </v-card>
</template>

<script lang="ts">
import type { PropType } from "vue";
import { defineComponent } from "vue";
import { useDisplay } from "vuetify";

import type { Components } from "@/types/openapi";

export default defineComponent({
  name: "ApplicationSummary",
  props: {
    status: {
      type: String as PropType<Components.Schemas.TranscriptStage | undefined>,
      required: true,
    },
    name: {
      type: String as PropType<String | undefined | null>,
      required: true,
    },
  },
  setup: async () => {
    const { smAndUp } = useDisplay();
    return { smAndUp };
  },
  computed: {
    statusText() {
      switch (this.status) {
        case "Accepted":
        case "InProgress":
        case "Submitted":
          return "Received";
        case "ApplicationSubmitted":
        case "Draft":
        case "WaitingforDetails":
        case "Rejected":
          return "Not yet received";
        default:
          return "Unhandled Status";
      }
    },
    sheetColor() {
      switch (this.statusText) {
        case "Received":
          return "white-smoke";
        case "Not yet received":
          return "hawkes-blue";
        default:
          return "";
      }
    },
  },
});
</script>
