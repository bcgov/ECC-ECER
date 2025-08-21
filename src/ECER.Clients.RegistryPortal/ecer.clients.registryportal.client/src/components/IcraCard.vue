<template>
  <v-card :rounded="!isRounded ? '0' : ''" variant="outlined" class="h-100 d-flex flex-column justify-space-between h-100">
    <v-card-item class="ma-4">
      <h2>Apply with international certification</h2>
      <p class="small mt-4">
        Apply for
        <b>ECE Five Year Certification</b>
        if you are internationally certified in a country that regulates the ECE profession and do not have 500 hours work experience supervised by a
        Canadian-certified ECE.
      </p>
    </v-card-item>

    <div class="d-flex flex-row justify-start ga-3 flex-wrap ma-4">
      <v-card-actions>
        <router-link :to="{ name: 'icra-eligibility' }">Learn more</router-link>
      </v-card-actions>
    </div>
  </v-card>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useRouter } from "vue-router";

export default defineComponent({
  name: "IcraCard",
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
});
</script>
