<template>
  <v-responsive min-height="210">
    <v-card height="100%" class="custom-card-styling">
      <v-row class="fill-height" no-gutters>
        <v-col cols="3" class="icon-container d-flex align-center justify-center">
          <v-icon v-if="icon" :icon="icon" size="40"></v-icon>
        </v-col>
        <v-col>
          <div class="d-flex flex-column justify-center fill-height pa-6 ga-4">
            <h3>{{ title }}</h3>
            <p>{{ text }}</p>
          </div>
        </v-col>
      </v-row>
    </v-card>
  </v-responsive>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "IconCard",
  props: {
    title: {
      type: String,
      required: true,
    },
    icon: {
      type: String,
      required: true,
    },
    text: {
      type: String,
      required: false,
      default: "",
    },
  },
});
</script>

<style scoped lang="scss">
.custom-card-styling {
  border-radius: 10px;
  border: 1px solid #d9d9d9;

  .icon-container {
    height: 100%;
    background-color: #f5f5f5;
    border-right: 1px solid #d9d9d9;
  }
}
</style>
