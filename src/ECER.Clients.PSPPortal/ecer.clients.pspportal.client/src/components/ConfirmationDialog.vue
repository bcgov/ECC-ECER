<template>
  <v-dialog
    :model-value="visible"
    width="650"
    :disabled="disabled"
    @click:outside="clickOutside"
  >
    <template #default>
      <v-container>
        <v-card class="no-scroll">
          <v-card-title>
            <div class="d-flex justify-center align-center text-wrap">
              <h2>
                {{ title }}
              </h2>
              <v-spacer></v-spacer>
              <v-btn
                size="default"
                icon="mdi-close"
                elevation="0"
                @click="exit"
              />
            </div>
          </v-card-title>
          <v-card-text class="pb-12">
            <slot name="confirmation-text">
              <p><b>Are you sure you want to proceed?</b></p>
            </slot>
          </v-card-text>
          <v-card-actions>
            <v-row>
              <v-col
                class="text-right d-flex flex-row justify-end flex-wrap ga-2"
              >
                <v-btn
                  class="ma-0"
                  :loading="loading"
                  color="primary"
                  variant="flat"
                  @click="accept"
                >
                  {{ acceptButtonText }}
                </v-btn>
                <v-btn
                  v-if="hasCancelButton"
                  :loading="loading"
                  class="ma-0"
                  variant="outlined"
                  @click="cancel"
                >
                  {{ cancelButtonText }}
                </v-btn>
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

export type ConfirmationDialogResult =
  | "accept"
  | "cancel"
  | "exit"
  | "close"
  | "clickOutside";

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
    hasCancelButton: {
      type: Boolean,
      default: true,
    },
    loading: {
      type: Boolean,
      default: false,
    },
  },
  data() {
    return {
      internalShow: false,
      resolvePromise: null as
        | ((value: ConfirmationDialogResult) => void)
        | null,
    };
  },
  emits: {
    accept: () => true,
    cancel: () => true,
    exit: () => true,
    clickOutside: () => true,
  },
  methods: {
    cancel() {
      this.$emit("cancel");
      if (this.resolvePromise) {
        this.resolvePromise("cancel");
      }
    },
    accept() {
      this.$emit("accept");
      if (this.resolvePromise) {
        this.resolvePromise("accept");
      }
    },
    clickOutside() {
      this.$emit("clickOutside");
      if (this.resolvePromise) {
        this.resolvePromise("clickOutside");
      }
    },
    // user clicks the "x" button to close dialog
    exit() {
      this.$emit("exit");
      if (this.resolvePromise) {
        this.resolvePromise("exit");
      }
    },
    open() {
      this.internalShow = true;
      return new Promise<ConfirmationDialogResult>((resolve) => {
        this.resolvePromise = resolve;
      });
    },
    close() {
      this.internalShow = false;
      if (this.resolvePromise) {
        this.resolvePromise = null;
      }
    },
  },
  computed: {
    visible() {
      return this.show || this.internalShow;
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
