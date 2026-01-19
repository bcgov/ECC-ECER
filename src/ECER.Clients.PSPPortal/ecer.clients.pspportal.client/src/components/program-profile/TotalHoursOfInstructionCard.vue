<template>
  <v-card
    flat
    rounded="lg"
    :border="true"
    :color="cardBackgroundColor"
    :style="{
      borderColor: progressColor,
      borderWidth: '1px',
      borderStyle: 'solid',
    }"
  >
    <v-card-text>
      <v-card-title class="pl-0 pt-0">
        <strong>Total hours of instruction</strong>
      </v-card-title>
      <div class="mb-4 d-flex align-center">
        <v-progress-linear
          :model-value="progressPercentage"
          height="25"
          rounded
          rounded-bar
          :color="progressColor"
          bg-color="#f0f2f4"
          class="flex-grow-1"
        >
          <template v-slot:default="{ value }">
            <span>
              <strong>{{ totalHours }}</strong>
              total hours /
              <strong>{{ requiredHours }}</strong>
              required hours
            </span>
          </template>
        </v-progress-linear>
        <v-icon
          v-if="isCompleted"
          icon="mdi-check"
          :color="progressColor"
          size="28"
          class="ml-2"
        ></v-icon>
      </div>
      <p class="text-center font-weight-medium">
        The required hours of instruction have
        <span v-if="!isCompleted">NOT</span>
        been met.
      </p>
    </v-card-text>
  </v-card>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "TotalHoursOfInstructionCard",
  props: {
    totalHours: {
      type: Number,
      required: true,
      default: 0,
    },
    requiredHours: {
      type: Number,
      required: true,
      default: 0,
    },
  },
  computed: {
    progressPercentage(): number {
      if (this.requiredHours === 0) return 0;
      return Math.min((this.totalHours / this.requiredHours) * 100, 100);
    },
    isCompleted(): boolean {
      return this.progressPercentage >= 100;
    },
    progressColor(): string {
      return this.isCompleted ? "#66CB7B" : "#FACC75";
    },
    cardBackgroundColor(): string {
      return this.isCompleted ? "#F0F9F4" : "#FEFBF3";
    },
  },
});
</script>
