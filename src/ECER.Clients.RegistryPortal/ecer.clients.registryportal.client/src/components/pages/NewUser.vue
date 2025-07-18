<template>
  <PageContainer>
    <v-row class="ga-8">
      <!-- Header Section -->
      <v-col cols="12">
        <h1>Create your My ECE Registry account</h1>
        <p v-if="oidcIdentityProvider === 'bcsc'" class="mt-2">
          Welcome {{ cleanPreferredName(oidcUserInfo.firstName!.replace(/^(.*?)(\s.*)?$/, "$1"), oidcUserInfo.lastName) }}.
        </p>
        <p v-if="oidcIdentityProvider === 'bceidbasic'" class="mt-2">
          After you enter your information below, you will need to provide government-issued ID so we can verify your identity.
        </p>
      </v-col>

      <!-- Information from BC Services Card -->
      <v-col cols="12" v-if="oidcIdentityProvider === 'bcsc'">
        <ECEHeader title="Information from your BC Services Card account" />
        <p class="mt-2">We automatically use the following information from your BC Services Card account for your My ECE Registry account.</p>
        <p class="mt-5">Legal name</p>
        <p class="mt-2">{{ oidcUserInfo.firstName }} {{ oidcUserInfo.lastName }}</p>
        <p class="mt-5">Address</p>
        <p class="mt-2">{{ oidcUserInfo.address.street_address }}</p>
        <p>{{ oidcUserInfo.address.locality }}, {{ oidcUserInfo.address.region }} {{ oidcUserInfo.address.postal_code }}</p>
        <p>{{ oidcUserInfo.address.country }}</p>
      </v-col>

      <v-form ref="form" validate-on="input">
        <!-- Information needed from BCeID Basic -->
        <v-col cols="12" v-if="oidcIdentityProvider === 'bceidbasic'">
          <ECEHeader title="Legal name" />
          <p class="mt-2">Must exactly match the name shown on your government-issued ID.</p>
          <v-row>
            <!-- First name Field -->
            <v-col sm="12" md="6" class="mt-8">
              <EceTextField v-model="firstName" label="First name"></EceTextField>
            </v-col>
          </v-row>

          <v-row>
            <!-- Middle name Field -->
            <v-col sm="12" md="6">
              <EceTextField v-model="middleName" label="Middle names (optional)"></EceTextField>
            </v-col>
          </v-row>

          <v-row>
            <!-- Last name Field -->
            <v-col sm="12" md="6">
              <EceTextField v-model="lastName" label="Last name" :rules="[Rules.required()]"></EceTextField>
            </v-col>
          </v-row>

          <v-row>
            <!-- Date of birth Field -->
            <v-col sm="12" md="4" xl="2">
              <EceDateInput
                v-model="dateOfBirth"
                :rules="[Rules.required(), Rules.futureDateNotAllowedRule()]"
                :max="today"
                label="Date of birth"
              ></EceDateInput>
            </v-col>
          </v-row>
        </v-col>

        <!-- Preferred name asked if from BCeID Basic -->
        <v-col cols="12" v-if="oidcIdentityProvider === 'bceidbasic'">
          <ECEHeader title="First name you go by" />
          <p class="mt-2">We will use this when we:</p>
          <ul class="ml-10 mt-2">
            <li>Send you messages in My ECE Registry</li>
            <li>Contact your references</li>
          </ul>
          <v-row>
            <!-- Preferred first name Field -->
            <v-col sm="12" md="6" class="mt-8">
              <EceTextField v-model="preferredName" label="Preferred first name (optional)"></EceTextField>
            </v-col>
          </v-row>
        </v-col>

        <!-- Form Section -->
        <v-col cols="12">
          <v-row class="ga-2">
            <v-col cols="12">
              <ECEHeader title="Contact information" />
              <p class="mt-2">We'll use this to contact you about your account and updates about your application or certificate.</p>
            </v-col>
            <v-col cols="12">
              <v-row>
                <!-- Email Field -->
                <v-col cols="12" md="6" class="mt-3">
                  <EceTextField
                    v-model="email"
                    label="Email"
                    type="email"
                    :rules="[Rules.required(), Rules.email('Enter your email in the format \'name@email.com\'')]"
                  ></EceTextField>
                </v-col>
              </v-row>
            </v-col>
            <v-col cols="12">
              <v-row>
                <!-- Phone Field -->
                <v-col cols="12" sm="6">
                  <EceTextField v-model="phoneNumber" label="Phone number" :rules="[Rules.required(), Rules.phoneNumber()]"></EceTextField>
                </v-col>
              </v-row>
            </v-col>

            <!-- ECE Registration Section -->
            <v-col cols="12">
              <ECEHeader title="Your ECE registration" />
              <p class="mt-2">You can add an existing ECE certificate to your account and manage it online.</p>
              <p class="mt-5">Do you have, or ever had, an ECE certificate in British Columbia?</p>

              <!-- Radio Group for ECE Certificate -->
              <v-radio-group v-model="eceCertificateStatus" :rules="[Rules.requiredRadio('Choose an option')]">
                <v-radio label="Yes" :value="true"></v-radio>
                <v-radio label="No" :value="false"></v-radio>
              </v-radio-group>
            </v-col>

            <!-- ECE Registration Number -->
            <v-col v-if="eceCertificateStatus === true" cols="12" sm="6">
              <EceTextField
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
                    <router-link to="/terms-of-use" target="_blank">Terms of Use</router-link>
                  </div>
                </template>
              </v-checkbox>
            </v-col>
          </v-row>
        </v-col>
      </v-form>

      <!-- Buttons -->
      <v-col cols="12">
        <div>
          <v-btn
            rounded="lg"
            :loading="loadingStore.isLoading('userinfo_post') || loadingStore.isLoading('profile_put') || isRedirecting"
            color="primary"
            class="mr-2"
            @click="submit"
          >
            Create account
          </v-btn>
          <v-btn
            rounded="lg"
            :loading="loadingStore.isLoading('userinfo_post') || loadingStore.isLoading('profile_put') || isRedirecting"
            variant="outlined"
            @click="logout"
          >
            Cancel
          </v-btn>
        </div>
      </v-col>
    </v-row>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent, ref } from "vue";
import type { VForm } from "vuetify/components";
import { cleanPreferredName } from "@/utils/functions";
import EceTextField from "@/components/inputs/EceTextField.vue";
import EceDateInput from "@/components/inputs/EceDateInput.vue";
import { getUserInfo, postUserInfo } from "@/api/user";
import ECEHeader from "@/components/ECEHeader.vue";
import { useLoadingStore } from "@/store/loading";
import { useOidcStore } from "@/store/oidc";
import { useUserStore } from "@/store/user";
import { isNumber } from "@/utils/formInput";
import * as Rules from "@/utils/formRules";

import PageContainer from "../PageContainer.vue";
import { useRouter } from "vue-router";
import { DateTime } from "luxon";
import { formatDate } from "@/utils/format";

export default defineComponent({
  name: "NewUser",
  components: { PageContainer, ECEHeader, EceTextField, EceDateInput },
  props: {
    identityProvider: {
      type: String,
      default: "",
    },
  },
  setup: async () => {
    const userStore = useUserStore();
    const oidcStore = useOidcStore();
    const router = useRouter();
    const oidcUserInfo = await oidcStore.oidcUserInfo();
    const oidcAddress = await oidcStore.oidcAddress();
    const oidcIdentityProvider = await oidcStore.oidcIdentityProvider();
    const loadingStore = useLoadingStore();
    const firstName = ref(oidcUserInfo.firstName);
    const lastName = ref(oidcUserInfo.lastName);
    const middleName = ref(oidcUserInfo.middleName);
    const dateOfBirth = ref(oidcUserInfo.dateOfBirth);
    const preferredName = ref("");
    const phoneNumber = ref(oidcUserInfo.phone);
    const email = ref(oidcUserInfo.email);

    return {
      userStore,
      cleanPreferredName,
      oidcStore,
      phoneNumber,
      email,
      firstName,
      lastName,
      middleName,
      dateOfBirth,
      preferredName,
      loadingStore,
      oidcUserInfo,
      oidcAddress,
      oidcIdentityProvider,
      router,
    };
  },

  data: () => ({
    hasAgreed: false,
    isRedirecting: false,
    eceRegistrationNumber: "",
    eceCertificateStatus: undefined as boolean | undefined,
    Rules,
  }),
  computed: {
    today() {
      return formatDate(DateTime.now().toString());
    },
  },
  methods: {
    isNumber,
    async submit() {
      let { valid } = await (this.$refs.form as VForm).validate();

      if (valid) {
        this.isRedirecting = true;
        const registrationNumber = this.eceCertificateStatus ? this.eceRegistrationNumber : "";
        const userCreated: boolean = await postUserInfo({
          ...this.oidcUserInfo,
          firstName: this.firstName,
          lastName: this.lastName,
          middleName: this.middleName,
          preferredName: this.preferredName,
          dateOfBirth: this.dateOfBirth,
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
        } else {
          this.isRedirecting = false;
        }
      }
    },
    async logout() {
      this.oidcStore.logout();
    },
  },
});
</script>
