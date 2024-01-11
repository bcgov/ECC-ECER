<template>
  <v-radio-group>
    <v-radio label="Radio One" value="one"></v-radio>
    <v-radio label="Radio Two" value="two"></v-radio>
    <v-radio label="Radio Three" value="three"></v-radio>
  </v-radio-group>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "EceRadio",
});
</script>
