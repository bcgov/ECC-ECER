<template>
  <PageContainer>
    <v-row class="ga-8">
      <!-- Header Section -->
      <v-col cols="12">
        <h1>Create your My ECE Registry account</h1>
        <p class="mt-2">Welcome {{ oidcUserInfo.firstName!.replace(/^(.*?)(\s.*)?$/, "$1") }}.</p>
      </v-col>

      <!-- Information from BC Services Card -->
      <v-col cols="12">
        <ECEHeader title="Information from your BC Services Card account" />
        <p class="mt-2">We automatically use the following information from your BC Services Card account for your My ECE Registry account.</p>
        <p class="mt-5">Legal name</p>
        <p class="mt-2">{{ oidcUserInfo.firstName }} {{ oidcUserInfo.lastName }}</p>
        <p class="mt-5">Address</p>
        <p class="mt-2">{{ oidcUserInfo.address.street_address }}</p>
        <p>{{ oidcUserInfo.address.locality }}, {{ oidcUserInfo.address.region }} {{ oidcUserInfo.address.postal_code }}</p>
        <p>{{ oidcUserInfo.address.country }}</p>
      </v-col>

      <!-- Form Section -->
      <v-col cols="12">
        <v-form ref="form" validate-on="input">
          <v-row class="ga-2">
            <v-col cols="12">
              <ECEHeader title="Contact information" />
              <p class="mt-2">We'll use this to contact you about your account and updates about your application or certificate.</p>
            </v-col>
            <v-col cols="12">
              <v-row>
                <!-- Email Field -->
                <v-col cols="12" md="6">
                  <v-text-field
                    v-model="email"
                    label="Email"
                    variant="outlined"
                    color="primary"
                    type="email"
                    :rules="[Rules.required(), Rules.email('Enter your email in the format \'name@email.com\'')]"
                    class="mt-5"
                  ></v-text-field>
                </v-col>
              </v-row>
            </v-col>
            <v-col cols="12">
              <v-row>
                <!-- Phone Field -->
                <v-col cols="12" sm="6">
                  <v-text-field
                    v-model="phoneNumber"
                    label="Phone number"
                    variant="outlined"
                    color="primary"
                    :rules="[Rules.required(), Rules.phoneNumber()]"
                    @keypress="isNumber($event)"
                  ></v-text-field>
                </v-col>
              </v-row>
            </v-col>

            <!-- ECE Registration Section -->
            <v-col cols="12">
              <ECEHeader title="Your ECE registration" />
              <p class="mt-2">
                To add an existing ECE certificate to your account to manage it online, you must enter the number now to link it to your account.
              </p>
              <p class="mt-5">Do you have, or ever had, an ECE certificate in British Columbia?</p>

              <!-- Radio Group for ECE Certificate -->
              <v-radio-group v-model="eceCertificateStatus" :rules="[Rules.requiredRadio('Choose an option')]">
                <v-radio label="Yes" :value="true"></v-radio>
                <v-radio label="No" :value="false"></v-radio>
              </v-radio-group>
            </v-col>

            <!-- ECE Registration Number -->
            <v-col v-if="eceCertificateStatus === true" cols="12" sm="6">
              <v-text-field
                v-model="eceRegistrationNumber"
                label="Your ECE Registration Number"
                variant="outlined"
                color="primary"
                :rules="[Rules.required('Enter your ECE registration number')]"
                @keypress="isNumber($event)"
              />
            </v-col>

            <!-- Checkbox for Terms of Use -->
            <v-col cols="12">
              <v-checkbox v-model="hasAgreed" label="" color="primary" :rules="[Rules.hasCheckbox('You must read and accept the Terms of Use')]">
                <template #label>
                  <div>
                    I have read and accept the
                    <router-link to="/new-user/terms-of-use">Terms of Use</router-link>
                  </div>
                </template>
              </v-checkbox>
            </v-col>
          </v-row>
        </v-form>
      </v-col>

      <!-- Buttons -->
      <v-col cols="12">
        <div>
          <v-btn rounded="lg" :loading="loadingStore.isLoading('userinfo_post')" color="primary" class="mr-2" @click="submit">Save and continue</v-btn>
          <v-btn rounded="lg" variant="outlined" @click="logout">Cancel</v-btn>
        </div>
      </v-col>
    </v-row>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent, ref } from "vue";
import type { VForm } from "vuetify/components";

import { getUserInfo, postUserInfo } from "@/api/user";
import ECEHeader from "@/components/ECEHeader.vue";
import { useLoadingStore } from "@/store/loading";
import { useOidcStore } from "@/store/oidc";
import { useUserStore } from "@/store/user";
import { isNumber } from "@/utils/formInput";
import * as Rules from "@/utils/formRules";

import PageContainer from "../PageContainer.vue";
import { useRouter } from "vue-router";

export default defineComponent({
  name: "NewUser",
  components: { PageContainer, ECEHeader },
  setup: async () => {
    const userStore = useUserStore();
    const oidcStore = useOidcStore();
    const router = useRouter();
    const oidcUserInfo = await oidcStore.oidcUserInfo();
    const oidcAddress = await oidcStore.oidcAddress();
    const loadingStore = useLoadingStore();
    const phoneNumber = ref(oidcUserInfo.phone);
    const email = ref(oidcUserInfo.email);

    return { userStore, oidcStore, phoneNumber, email, loadingStore, oidcUserInfo, oidcAddress, router };
  },

  data: () => ({
    hasAgreed: false,
    eceRegistrationNumber: "",
    eceCertificateStatus: undefined as boolean | undefined,
    Rules,
  }),
  methods: {
    isNumber,
    async submit() {
      let { valid } = await (this.$refs.form as VForm).validate();

      if (valid) {
        const registrationNumber = this.eceCertificateStatus ? this.eceRegistrationNumber : "";
        const userCreated: boolean = await postUserInfo({
          ...this.oidcUserInfo,
          residentialAddress: this.oidcAddress,
          mailingAddress: this.oidcAddress,
          email: this.email,
          phone: this.phoneNumber,
          registrationNumber: registrationNumber,
        });
        // TODO handle error creating user, need clarification from design team
        if (userCreated) {
          const userInfo = await getUserInfo();
          if (userInfo !== null) {
            this.userStore.setUserInfo(userInfo);
          }

          this.router.push("/");
        }
      }
    },

    async logout() {
      this.oidcStore.logout();
    },
  },
});
</script>
