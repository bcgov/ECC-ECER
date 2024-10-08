<template>
  <v-sheet class="bg-primary">
    <v-container><h1>Validate an ECE certificate</h1></v-container>
  </v-sheet>
  <v-container>{{ test }}</v-container>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";
import EceRecaptcha from "./inputs/EceRecaptcha.vue";

export default defineComponent({
  name: "LookupCertification",
  components: { EceRecaptcha },
  props: {
    test: {
      // TODO create type for prop
      type: String as PropType<any>,
      required: true,
    },
  },
  setup() {
    return {};
  },

  methods: {},
});
</script>
