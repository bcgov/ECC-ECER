<template>
  <v-card elevation="0" rounded="0" class="border-t border-b">
    <v-card-text>
      <div class="d-flex" :class="[smAndUp ? 'space-between align-center' : 'flex-column']">
        <div v-if="status === 'Complete'">{{ text }}</div>
        <a v-else href="#" @click.prevent="buttonClick">{{ text }}</a>

        <v-spacer></v-spacer>
        <v-sheet :width="smAndUp ? '' : '200px'" rounded class="sheet" :class="{ 'mt-2': !smAndUp }" :color="sheetColor">{{ statusText }}</v-sheet>
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
  },
  setup: async () => {
    const { smAndUp } = useDisplay();
    return { smAndUp };
  },
  data() {
    return {};
  },
  computed: {
    text() {
      if (this.type === "transcript") {
        return `Transcript provided by ${this.name}`;
      } else if (this.type === "character") {
        return `Character reference provided by ${this.name}`;
      } else if (this.type === "workExperience") {
        return `Work experience reference provided by ${this.name}`;
      } else {
        return "";
      }
    },
    statusText() {
      if (this.status === "Complete") {
        return "Complete";
      } else {
        return "Incomplete";
      }
    },
    sheetColor() {
      if (this.status === "Complete") {
        return "#f8f8f8";
      } else {
        return "#D9EAFD";
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
<style scoped>
.sheet {
  padding: 1rem 3rem;
  text-align: center;
}
</style>
