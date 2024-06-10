<template>
  <v-card elevation="0" rounded="0" class="border-t border-b">
    <v-card-text>
      <div class="d-flex" :class="[smAndUp ? 'space-between align-center' : 'flex-column']">
        <a href="#" @click.prevent="buttonClick">{{ text }}</a>
        <v-spacer></v-spacer>
        <v-sheet rounded width="200px" class="py-2 text-center" :class="{ 'mt-2': !smAndUp }" color="hawkes-blue">Incomplete</v-sheet>
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
    goTo: {
      type: Function,
      required: true,
    },
  },
  setup: async () => {
    const { smAndUp } = useDisplay();
    return { smAndUp };
  },
  methods: {
    buttonClick() {
      this.goTo();
    },
  },
});
</script>
