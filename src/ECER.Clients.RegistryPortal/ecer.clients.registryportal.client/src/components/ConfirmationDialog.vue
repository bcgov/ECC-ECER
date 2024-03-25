<template>
  <v-dialog v-model="showDialog" width="auto">
    <template #activator="{ props: activatorProps }">
      <v-btn v-bind="activatorProps" rounded="lg" variant="outlined">
        <slot name="activator">Cancel</slot>
      </v-btn>
    </template>

    <template #default>
      <v-card class="no-scroll">
        <v-card-title>
          <div class="d-flex justify-center align-center">
            <v-icon size="large" icon="mdi-alert-circle" color="warning" class="mr-2"></v-icon>
            <div>
              {{ title }}
            </div>
            <v-spacer></v-spacer>
            <v-btn icon elevation="0" @click="cancel">
              <v-icon>mdi-close</v-icon>
            </v-btn>
          </div>
        </v-card-title>
        <v-card-text>
          <slot name="confirmation-text">
            <p><b>Are you sure you want to proceed?</b></p>
          </slot>
        </v-card-text>
        <v-card-actions>
          <v-row>
            <v-col class="text-right">
              <v-btn rounded="lg" :class="{ 'mb-2': smAndDown }" variant="outlined" @click="cancel">{{ cancelButtonText }}</v-btn>
              <v-btn rounded="lg" color="warning" variant="outlined" @click="accept">{{ acceptButtonText }}</v-btn>
            </v-col>
          </v-row>
        </v-card-actions>
      </v-card>
    </template>
  </v-dialog>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";
import { useDisplay } from "vuetify";

import type { ConfirmationDialogProps } from "@/types/confirmation-dialog";

export default defineComponent({
  name: "ConfirmationDialog",
  props: {
    config: {
      type: Object as PropType<ConfirmationDialogProps>,
      default: () => ({}),
    },
  },
  emits: {
    accept: () => true,
    cancel: () => true,
  },
  setup() {
    const { smAndDown } = useDisplay();
    return { smAndDown };
  },
  data() {
    return {
      showDialog: false,
      cancelButtonText: this.config?.cancelButtonText || "Cancel",
      acceptButtonText: this.config?.acceptButtonText || "Proceed",
      title: this.config?.title || "Please Confirm",
    };
  },
  methods: {
    cancel() {
      this.$emit("cancel");
      this.showDialog = false;
    },
    accept() {
      this.$emit("accept");
      this.showDialog = false;
    },
  },
});
</script>
<style scoped>
.no-scroll {
  /* gets rid of scrollbar on the side */
  overflow-y: hidden !important;
}
</style>
