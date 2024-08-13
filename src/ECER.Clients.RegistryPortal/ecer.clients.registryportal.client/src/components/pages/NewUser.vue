<template>
  <PageContainer>
    <FormContainer>
      <div class="d-flex flex-column ga-8">
        <div>
          <h1>Profile information</h1>
          <p class="small">The ECE Registry will notify you of important updates regarding your certification</p>
        </div>
        <v-form ref="form" validate-on="input">
          <div class="d-flex flex-column ga-2">
            <v-text-field v-model="email" label="Email" variant="outlined" color="primary" type="email" :rules="emailRules"></v-text-field>
            <v-text-field
              v-model="phoneNumber"
              label="Phone number"
              variant="outlined"
              color="primary"
              :rules="phoneRules"
              @keypress="isNumber($event)"
            ></v-text-field>
            <v-checkbox v-model="hasAgreed" label="" color="primary" :rules="hasAgreedRules">
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
    </FormContainer>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent, ref } from "vue";
import type { VForm } from "vuetify/components";

import { postUserInfo } from "@/api/user";
import { useOidcStore } from "@/store/oidc";
import { useUserStore } from "@/store/user";
import { isNumber } from "@/utils/formInput";
import * as Rules from "@/utils/formRules";

import FormContainer from "../FormContainer.vue";
import PageContainer from "../PageContainer.vue";

export default defineComponent({
  name: "NewUser",
  components: { FormContainer, PageContainer },
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
    hasAgreedRules: [(v: boolean) => !!v || "You must read and accept the Terms of Use"],
    Rules,
  }),
  computed: {
    emailRules() {
      return [this.customEmailRule(), this.Rules.required()];
    },
    phoneRules() {
      return [this.customPhoneRule(), this.Rules.required()];
    },
  },
  methods: {
    customEmailRule() {
      return this.Rules.email("Enter your email in the format 'name@email.com'");
    },
    customPhoneRule() {
      return this.Rules.phoneNumber("Enter your primary 10-digit phone number");
    },

    isNumber,
    async submit() {
      const { valid } = await (this.$refs.form as VForm).validate();
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
