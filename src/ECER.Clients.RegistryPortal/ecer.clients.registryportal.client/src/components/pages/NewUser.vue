<template>
  <PageContainer>
    <div class="d-flex flex-column ga-8">
      <div>
        <h1>Create your My ECE Registry account</h1>
        <p class="small mt-2">Welcome {{ oidcUserInfo.firstName!.replace(/^(.*?)(\s.*)?$/, "$1") }}.</p>
      </div>
      <div>
        <ECEHeader title="Information from your BC Services Card account" />
        <p class="small mt-2">We automatically use the following information from your BC Services Card account for your MY ECE Registry account.</p>
        <p class="small mt-5">Legal name</p>
        <p class="small mt-2">{{ oidcUserInfo.firstName }} {{ oidcUserInfo.lastName }}</p>

        <p class="small mt-5">Address</p>
        <p class="small mt-2">{{ oidcUserInfo.address.street_address }}</p>
        <p class="small">{{ oidcUserInfo.address.locality }}, {{ oidcUserInfo.address.region }} {{ oidcUserInfo.address.postal_code }}</p>
        <p class="small">{{ oidcUserInfo.address.country }}</p>
      </div>
      <v-form ref="form" validate-on="input">
        <div class="d-flex flex-column ga-2">
          <ECEHeader title="Contact Information" />
          <p class="small mt-2">We'll use this to contact you about your account and updates about your application or certificate.</p>
          <v-text-field
            v-model="email"
            class="mt-5 max-width-500"
            label="Email"
            variant="outlined"
            color="primary"
            type="email"
            :rules="[Rules.required(), Rules.email('Enter your reference\'s email in the format \'name@email.com\'')]"
          ></v-text-field>
          <v-text-field
            v-model="phoneNumber"
            label="Phone number"
            variant="outlined"
            color="primary"
            class="max-width-300"
            :rules="phoneRules"
            @keypress="isNumber($event)"
          ></v-text-field>
          <ECEHeader title="Your ECE registration" />
          <p class="small mt-2">
            To add an existing ECE certificate to your account to manage it online, you must enter the number now to link it to your account.
          </p>
          <p class="small mt-5">Do you have, or ever had, an ECE certificate in British Columbia?</p>
          <v-radio-group v-model="eceCertificateStatus" :rules="radioRules">
            <v-radio label="Yes" :value="true"></v-radio>
            <v-radio label="No" :value="false"></v-radio>
          </v-radio-group>

          <v-text-field
            v-if="eceCertificateStatus === true"
            v-model="eceRegistrationNumber"
            label="Your ECE Registration Number"
            variant="outlined"
            color="primary"
            class="max-width-300"
            :rules="eceRegistrationRules"
            @keypress="isNumber($event)"
          />
          <v-checkbox v-model="hasAgreed" class="mt-2" label="" color="primary" :rules="hasAgreedRules">
            <template #label>
              <div>
                I have read and accept the
                <router-link to="/new-user/terms-of-use">Terms of Use</router-link>
              </div>
            </template>
          </v-checkbox>
        </div>
      </v-form>
      <v-row>
        <v-btn rounded="lg" color="primary" class="mr-2" @click="submit">Save and continue</v-btn>
        <v-btn rounded="lg" variant="outlined" @click="logout">Cancel</v-btn>
      </v-row>
    </div>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent, ref } from "vue";
import type { VForm } from "vuetify/components";

import { postUserInfo } from "@/api/user";
import ECEHeader from "@/components/ECEHeader.vue";
import { useOidcStore } from "@/store/oidc";
import { useUserStore } from "@/store/user";
import { isNumber } from "@/utils/formInput";
import * as Rules from "@/utils/formRules";

import PageContainer from "../PageContainer.vue";

export default defineComponent({
  name: "NewUser",
  components: { PageContainer, ECEHeader },
  setup: async () => {
    const userStore = useUserStore();
    const oidcStore = useOidcStore();
    const oidcUserInfo = await oidcStore.oidcUserInfo();

    const phoneNumber = ref(oidcUserInfo.phone);
    const email = ref(oidcUserInfo.email);

    return { userStore, oidcStore, phoneNumber, email, oidcUserInfo };
  },

  data: () => ({
    hasAgreed: false,
    eceRegistrationNumber: "",
    eceCertificateStatus: null as boolean | null,
    hasAgreedRules: [(v: boolean) => !!v || "You must read and accept the Terms of Use"],
    Rules,
  }),

  computed: {
    phoneRules() {
      return [...this.customPhoneRule(), this.Rules.required()];
    },
    eceRegistrationRules() {
      return [
        (v: string) => {
          if (this.eceCertificateStatus === true) {
            return !!v || "Enter your ECE registration number"; // Required if "Yes" selected
          }
          return true; // No validation if "No" selected
        },
      ];
    },
    radioRules() {
      return [
        (v: boolean | null) => v !== null || "Choose an option", // Radio button required
      ];
    },
  },
  methods: {
    customPhoneRule() {
      return [
        (v: string) => !!v || "Enter your 10-digit phone number", // Required validation
        (v: string) => /^\d{10}$/.test(v) || "Enter your 10-digit phone number", // Pattern validation
      ];
    },
    isNumber,
    async submit() {
      let { valid } = await (this.$refs.form as VForm).validate();

      if (valid) {
        const userCreated: boolean = await postUserInfo({ ...this.oidcUserInfo, phone: this.phoneNumber });
        // TODO handle error creating user, need clarification from design team
        if (userCreated) {
          this.userStore.setUserInfo({
            ...this.oidcUserInfo,
            phone: this.phoneNumber,
            email: this.email,
          });

          this.$router.push("/");
        }
      }
    },

    async logout() {
      this.oidcStore.logout();
    },
  },
});
</script>
<style scoped>
.max-width-500 {
  max-width: 500px;
}
.max-width-300 {
  max-width: 300px;
}
</style>
