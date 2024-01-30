<template>
  <PageContainer>
    <FormContainer>
      <div class="d-flex flex-column ga-8">
        <div>
          <h3>Profile Information</h3>
          <p class="small">The Registry will notify you of important updates regarding your certification</p>
        </div>
        <v-form ref="form" validate-on="blur">
          <div class="d-flex flex-column ga-2">
            <v-text-field
              v-model="email"
              label="Email"
              variant="outlined"
              color="primary"
              type="email"
              :rules="[Rules.email(), Rules.required()]"
            ></v-text-field>
            <v-text-field
              v-model="phoneNumber"
              label="Phone Number"
              variant="outlined"
              color="primary"
              :rules="[Rules.phoneNumber(), Rules.required()]"
              @keypress="isNumber($event)"
            ></v-text-field>
            <v-checkbox v-model="hasAgreed" label="" color="primary" :rules="hasAgreedRules">
              <template #label>
                I have read and accept the&nbsp;
                <router-link to="/new-user/terms-of-use">Terms of Use</router-link>
              </template>
            </v-checkbox>
            <v-row justify="end">
              <v-btn rounded="lg" variant="outlined" class="mr-2" @click="logout">Cancel</v-btn>
              <v-btn rounded="lg" color="primary" @click="submit">Save and Continue</v-btn>
            </v-row>
          </div>
        </v-form>
      </div>
    </FormContainer>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent, ref } from "vue";

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

    const phoneNumber = ref(userStore.oidcUserAsUserInfo.phone);
    const email = ref(userStore.oidcUserAsUserInfo.email);

    return { userStore, oidcStore, phoneNumber, email };
  },

  data: () => ({
    hasAgreed: false,
    hasAgreedRules: [(v: boolean) => !!v || "You must read and accept the Terms of Use"],
    Rules,
  }),
  methods: {
    isNumber,
    async submit() {
      const { valid } = await (this.$refs.form as any).validate();
      if (valid) {
        const userCreated: boolean = await postUserInfo(this.userStore.oidcUserInfo);

        // TODO handle error creating user, need clarification from design team
        if (userCreated) {
          this.userStore.setUserInfo({
            ...this.userStore.oidcUserAsUserInfo,
            phone: this.phoneNumber,
            email: this.email,
          });

          this.$router.push("/");
        }
      }
    },

    async logout() {
      if (this.userStore.isAuthenticated && this.userStore.authority) {
        if (this.userStore.authority == "bceid") {
          await this.oidcStore.logout(this.userStore.authority);
        }
        if (this.userStore.authority == "bcsc") {
          // BCSC does not support a session logout callback endpoint so just remove session data from client
          await this.oidcStore.removeUser(this.userStore.authority);
          this.userStore.$reset();
        }
      }
    },
  },
});
</script>
