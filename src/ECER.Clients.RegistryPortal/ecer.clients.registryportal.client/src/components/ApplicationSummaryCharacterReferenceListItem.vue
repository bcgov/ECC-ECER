<template>
  <v-card elevation="0" rounded="0" class="border-t border-b">
    <v-card-text>
      <div
        class="d-flex"
        :class="[smAndUp ? 'space-between align-center' : 'flex-column']"
      >
        <div v-if="statusText === 'Received' || statusText === 'Cancelled'">
          <p>Character reference provided by {{ name }}</p>
        </div>
        <a v-else href="#" @click.prevent="buttonClick">
          <p class="text-links">Character reference provided by {{ name }}</p>
        </a>

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
  name: "ApplicationSummaryCharacterReferenceListItem",
  props: {
    status: {
      type: String as PropType<
        Components.Schemas.CharacterReferenceStage | undefined
      >,
      required: true,
    },
    name: {
      type: String as PropType<String | undefined | null>,
      required: true,
    },
    goTo: {
      type: Function,
      required: true,
    },
    willProvideReference: {
      type: Boolean,
      required: false,
      default: true,
    },
  },
  setup: async () => {
    const { smAndUp } = useDisplay();
    return { smAndUp };
  },
  computed: {
    statusText() {
      switch (this.status) {
        case "Approved":
        case "InProgress":
        case "UnderReview":
        case "WaitingResponse":
        case "Submitted":
          return "Received";
        case "ApplicationSubmitted":
        case "Draft":
          return "Not yet received";
        case "Rejected":
          return this.willProvideReference ? "" : "Cancelled";
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
        case "Cancelled":
          return "white-smoke";
        default:
          return "";
      }
    },
  },
  methods: {
    buttonClick() {
      this.goTo();
    },
  },
});
</script>
