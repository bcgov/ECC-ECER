<template>
  <v-card :rounded="!isRounded ? '0' : ''" flat color="primary">
    <v-card-item class="ma-4">
      <h2 class="text-white">Transfer certification</h2>
      <p class="small text-white mt-4">
        If you are certified in another province or territory in Canada, you may be eligible to transfer your certification in B.C.
      </p>
    </v-card-item>
    <div class="d-flex flex-row justify-start ga-3 flex-wrap ma-4">
      <v-card-actions>
        <v-btn class="pl-8 pr-8" size="large" variant="flat" color="warning" @click="router.push('/transfer')">Transfer</v-btn>
      </v-card-actions>
    </div>
  </v-card>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useApplicationStore } from "@/store/application";
import { useRouter } from "vue-router";

export default defineComponent({
  name: "TransferCard",
  props: {
    isRounded: {
      type: Boolean,
      default: true,
    },
  },
  emits: ["cancel-application"],
  setup() {
    const applicationStore = useApplicationStore();
    const router = useRouter();

    return {
      applicationStore,
      router,
    };
  },
  computed: {},
  methods: {},
});
</script>
