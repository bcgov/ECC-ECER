<template>
  <v-card
    :rounded="!isRounded ? '0' : ''"
    flat
    color="primary"
    class="h-100 d-flex flex-column justify-space-between h-100"
  >
    <v-card-item class="ma-4">
      <h2 class="text-white">Transfer certification</h2>
      <p class="small text-white mt-4">
        If you are certified in another province or territory in Canada, you may
        be eligible to transfer your certification to B.C.
      </p>
    </v-card-item>

    <div class="d-flex flex-row justify-start ga-3 flex-wrap ma-4">
      <v-card-actions>
        <v-btn
          variant="flat"
          size="large"
          color="warning"
          id="btnTransfer"
          @click="handleTransferApplication"
        >
          <v-icon size="large" icon="mdi-arrow-right" />
          Transfer
        </v-btn>
      </v-card-actions>
    </div>
  </v-card>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useRouter } from "vue-router";

export default defineComponent({
  name: "TransferCard",
  props: {
    isRounded: {
      type: Boolean,
      default: true,
    },
  },

  setup() {
    const router = useRouter();

    return { router };
  },

  methods: {
    handleTransferApplication() {
      this.router.push({ name: "application-transfer" });
    },
  },
});
</script>
