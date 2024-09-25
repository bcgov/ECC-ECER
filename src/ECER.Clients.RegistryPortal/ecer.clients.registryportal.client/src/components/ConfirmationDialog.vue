<template>
  <v-dialog :model-value="show" width="650" :disabled="disabled" @click:outside="cancel">
    <template #default>
      <v-container>
        <v-card class="no-scroll">
          <v-card-title>
            <div class="d-flex justify-center align-center text-wrap">
              <h2>
                {{ title }}
              </h2>
              <v-spacer></v-spacer>
              <v-btn icon elevation="0" @click="cancel">
                <v-icon>mdi-close</v-icon>
              </v-btn>
            </div>
          </v-card-title>
          <v-card-text class="pb-12">
            <slot name="confirmation-text">
              <p><b>Are you sure you want to proceed?</b></p>
            </slot>
          </v-card-text>
          <v-card-actions>
            <v-row>
              <v-col class="text-right d-flex flex-row justify-end flex-wrap ga-2">
                <v-btn class="ma-0" variant="outlined" @click="cancel">{{ cancelButtonText }}</v-btn>
                <v-btn class="ma-0" color="primary" variant="flat" @click="accept">{{ acceptButtonText }}</v-btn>
              </v-col>
            </v-row>
          </v-card-actions>
        </v-card>
      </v-container>
    </template>
  </v-dialog>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";
import { VBtn } from "vuetify/components";
type TVariant = VBtn["$props"]["variant"];

export default defineComponent({
  name: "ConfirmationDialog",
  props: {
    show: {
      type: Boolean,
      default: false,
    },
    hasActivator: {
      type: Boolean,
      default: true,
    },
    title: {
      type: String,
      default: "Please Confirm",
    },
    cancelButtonText: {
      type: String,
      default: "Cancel",
    },
    acceptButtonText: {
      type: String,
      default: "Proceed",
    },
    customButtonVariant: {
      type: String as PropType<TVariant>,
      default: "outlined",
    },
    disabled: {
      type: Boolean,
      default: false,
    },
  },
  emits: {
    accept: () => true,
    cancel: () => true,
  },

  methods: {
    cancel() {
      this.$emit("cancel");
    },
    accept() {
      this.$emit("accept");
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
