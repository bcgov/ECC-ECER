<template>
  <v-card :rounded="false" class="w-100" flat color="links">
    <v-container>
      <v-card-item class="ma-4">
        <h1 class="text-white">Apply for ECE certification in B.C.</h1>
        <p class="small text-white mt-4">There are two options to get certified in British Columbia.</p>
      </v-card-item>

      <v-row justify="center" class="mb-4">
        <v-col cols="12" sm="6" lg="4">
          <v-responsive :aspect-ratio="16 / 9">
            <v-card class="rounded-lg" color="primary" height="100%">
              <v-card-item class="ma-4">
                <h2 class="text-white">Get certified</h2>
                <p class="small text-white mt-4">Your options will vary depending on your education and work experience.</p>
              </v-card-item>
              <div class="d-flex flex-row justify-start ga-3 flex-wrap ma-4">
                <v-card-actions>
                  <v-btn variant="flat" size="large" color="warning" id="btnApplyNow" @click="handleStartNewApplication">Apply now</v-btn>
                </v-card-actions>
              </div>
            </v-card>
          </v-responsive>
        </v-col>
        <v-col cols="12" sm="6" lg="4">
          <v-responsive :aspect-ratio="16 / 9">
            <v-card class="rounded-lg" color="primary" height="100%">
              <v-card-item class="ma-4">
                <h2 class="text-white">Transfer certification</h2>
                <p class="small text-white mt-4">
                  If you are certified in another province or territory in Canada, you may be eligible to transfer your certification to B.C.
                </p>
              </v-card-item>
              <div class="d-flex flex-row justify-start ga-3 flex-wrap ma-4">
                <v-card-actions>
                  <v-btn variant="flat" color="warning" @click="handleStartNewTransfer">Transfer</v-btn>
                </v-card-actions>
              </div>
            </v-card>
          </v-responsive>
        </v-col>
      </v-row>
    </v-container>
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
    handleStartNewApplication() {
      this.router.push({ name: "application-certification" });
    },
    handleStartNewTransfer() {
      this.router.push({ name: "application-transfer" });
    },
  },
});
</script>
