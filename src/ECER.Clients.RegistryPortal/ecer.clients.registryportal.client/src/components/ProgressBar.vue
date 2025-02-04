<template>
  <v-tooltip :text="`${hoursRemaining} Hours Remaining`" location="top">
    <template #activator="{ props }">
      <v-progress-linear v-bind="props" v-model="percentHours" rounded="lg" height="25" color="#67cb7b" bg-color="#f0f2f4" bg-opacity="1">
        <strong>{{ `${totalHours.toFixed(decimalPlaces)}/${hoursRequired} hours` }}</strong>
      </v-progress-linear>
    </template>
  </v-tooltip>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "ProgressBar",
  props: {
    totalHours: {
      type: Number,
      required: true,
    },
    hoursRequired: {
      type: Number,
      required: true,
    },
    decimalPlaces: {
      type: Number,
      default: 0,
    },
  },
  computed: {
    percentHours(): number {
      return (this.totalHours / this.hoursRequired) * 100;
    },
    hoursRemaining(): number {
      return Number((this.hoursRequired - this.totalHours).toFixed(this.decimalPlaces));
    },
  },
});
</script>
