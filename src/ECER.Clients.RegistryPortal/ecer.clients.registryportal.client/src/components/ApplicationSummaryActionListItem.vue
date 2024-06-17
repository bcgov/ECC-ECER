<template>
  <v-card elevation="0" rounded="0" class="border-t border-b">
    <v-card-text>
      <div class="d-flex" :class="[smAndUp ? 'space-between align-center' : 'flex-column']">
        <div v-if="!active">
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
import { defineComponent } from "vue";
import { useDisplay } from "vuetify";

export default defineComponent({
  name: "ApplicationSummaryActionListItem",
  props: {
    text: {
      type: String,
      required: true,
    },
    active: {
      type: Boolean,
      required: false,
      default: true,
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
      return this.active ? "Incomplete" : "Complete";
    },
    sheetColor() {
      return this.active ? "hawkes-blue" : "white-smoke";
    },
  },
  methods: {
    buttonClick() {
      this.goTo();
    },
  },
});
</script>
