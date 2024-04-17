<template>
  <v-dialog :model-value="show" width="auto" :disabled="disabled">
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
  setup() {
    const { smAndDown } = useDisplay();
    return { smAndDown };
  },

  methods: {
    cancel() {
      this.$emit("cancel");
    },
    accept() {
      this.$emit("accept");
      /* creating a delay before emitting accept - helps prevent warning dialog overlay in print preview */
      // setTimeout(this.$emit, 500, "accept");
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
