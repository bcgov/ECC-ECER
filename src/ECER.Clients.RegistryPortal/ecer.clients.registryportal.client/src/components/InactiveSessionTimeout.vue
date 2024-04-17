<template>
  <ConfirmationDialog
    :show="showTimeoutDialog"
    :cancel-button-text="'Keep me logged in'"
    :accept-button-text="'Logout'"
    :title="'Inactive Session'"
    @accept="oidcStore.logout"
    @cancel="showTimeoutDialog = false"
  >
    <template #confirmation-text>
      <p>Your session will expire in {{ timer }} seconds</p>
    </template>
  </ConfirmationDialog>
</template>

<script lang="ts">
import { useIdle, useIntervalFn } from "@vueuse/core";
import { defineComponent, ref, watch } from "vue";

import ConfirmationDialog from "@/components/ConfirmationDialog.vue";
import { useOidcStore } from "@/store/oidc";

export default defineComponent({
  name: "InactiveSessionTimeout",
  components: { ConfirmationDialog },
  setup() {
    const oidcStore = useOidcStore();
    const showTimeoutDialog = ref(false);
    const timer = ref(60);
    const idle = useIdle(2 * 1 * 1000); // 2 minutes

    const { pause, resume } = useIntervalFn(
      () => {
        if (timer.value > 0) {
          timer.value--;
        } else {
          pause();
          oidcStore.logout();
        }
      },
      1000,
      { immediate: false },
    );

    // Watch for idle state to trigger the timeout dialog
    watch(idle.idle, async (newIdle) => {
      if (newIdle && showTimeoutDialog.value === false) {
        const user = await oidcStore.getUser();
        if (user) {
          showTimeoutDialog.value = true;
        }
      }
    });

    // Control the interval based on dialog visibility
    watch(showTimeoutDialog, (newValue) => {
      if (!newValue) {
        pause();
      } else {
        timer.value = 60;
        resume();
      }
    });

    return {
      oidcStore,
      showTimeoutDialog,
      timer,
    };
  },
});
</script>
