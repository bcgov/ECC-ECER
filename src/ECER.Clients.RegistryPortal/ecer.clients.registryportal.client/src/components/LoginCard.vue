<template>
  <v-card v-if="isPreferred" class="rounded-lg" width="466px" height="550px" color="warning">
    <div class="d-flex flex-column align-center mt-2" :style="{ height: '24px' }">
      <p class="white"><b>Preferred Login Method</b></p>
    </div>
    <v-card width="450px" height="500px" color="white" class="mt-3 ml-2 rounded-lg"><slot></slot></v-card
  ></v-card>
  <v-card v-else color="white" width="450px" height="500px" class="mt-11 rounded-lg">
    <slot> </slot>
  </v-card>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "LoginCard",
  props: {
    isPreferred: Boolean,
  },
});
</script>
