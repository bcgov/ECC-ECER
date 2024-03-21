<template>
  <v-dialog max-width="500" v-model="showDialog">
    <template v-slot:activator="{ props: activatorProps }">
      <v-btn v-bind="activatorProps" rounded="lg" variant="outlined">
        <slot name="activator">Cancel</slot>
      </v-btn>
    </template>

    <template v-slot:default>
      <v-card>
        <v-card-title>
          <v-icon size="large" icon="mdi-alert-circle" color="warning"></v-icon>
          {{ title }}
        </v-card-title>
        <v-card-text>
          <slot name="confirmation-text">
            <p><b>Are you sure you want to proceed?</b></p>
          </slot>
        </v-card-text>

        <v-card-actions>
          <v-spacer></v-spacer>
          <v-btn variant="outlined" @click="cancel">{{ cancelButtonText }}</v-btn>
          <v-btn color="warning" variant="outlined" @click="accept">{{ acceptButtonText }}</v-btn>
        </v-card-actions>
      </v-card>
    </template>
  </v-dialog>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";
import type { ConfirmationDialogProps } from "@/types/confirmation-dialog";

export default defineComponent({
  name: "ConfirmationDialog",
  data() {
    return {
      showDialog: false,
      cancelButtonText: this.config?.cancelButtonText || "Cancel",
      acceptButtonText: this.config?.acceptButtonText || "Proceed",
      title: this.config?.title || "Please Confirm",
    };
  },
  emits: {
    accept: () => true,
    cancel: () => true,
  },
  props: {
    config: {
      type: Object as PropType<ConfirmationDialogProps>,
    },
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
