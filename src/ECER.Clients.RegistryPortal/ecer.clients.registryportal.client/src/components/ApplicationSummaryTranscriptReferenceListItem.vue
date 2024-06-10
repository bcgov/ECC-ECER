<template>
  <v-card elevation="0" rounded="0" class="border-t border-b">
    <v-card-text>
      <div class="d-flex" :class="[smAndUp ? 'space-between align-center' : 'flex-column']">
        <div v-if="status === 'Complete' || status === 'Rejected'">
          <p>{{ text }}</p>
        </div>
        <a v-else href="#" @click.prevent="buttonClick">
          <p class="text-links">{{ text }}</p>
        </a>

        <v-spacer></v-spacer>
        <v-sheet rounded width="200px" class="py-2 text-center" :class="{ 'mt-2': !smAndUp }" :color="sheetColor">
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
  name: "ApplicationSummaryTranscriptReferenceListItem",
  props: {
    status: {
      type: String as PropType<Components.Schemas.StageStatus | undefined>,
      required: true,
    },
    name: {
      type: String as PropType<String | undefined | null>,
      required: true,
    },
    type: {
      type: String as PropType<"transcript" | "character" | "workExperience" | undefined>,
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
    text() {
      if (this.type === "transcript") {
        return `Transcript provided by ${this.name}`;
      }

      if (this.type === "character") {
        return `Character reference provided by ${this.name}`;
      }

      if (this.type === "workExperience") {
        return `Work experience reference provided by ${this.name}`;
      }

      return "";
    },
    statusText() {
      if (this.status === "Complete") {
        return "Complete";
      }

      if (this.status === "InComplete") {
        return "Incomplete";
      }

      if (this.status === "Rejected" && !this.willProvideReference) {
        return "Cancelled";
      }

      return "Unhandled Status";
    },
    sheetColor() {
      if (this.status === "Complete") {
        return "white-smoke";
      }

      if (this.status === "InComplete") {
        return "hawkes-blue";
      }

      if (this.status === "Rejected" && !this.willProvideReference) {
        return "white-smoke";
      }

      return "";
    },
  },
  methods: {
    buttonClick() {
      this.goTo();
    },
  },
});
</script>
