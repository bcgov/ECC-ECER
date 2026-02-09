<template>
  <PageContainer>
    <v-row>
      <v-col class="ml-1" cols="12">
        <h1>My contact details</h1>
      </v-col>
    </v-row>
    <v-row>
      <v-col class="mt-4" cols="12">
        <EceForm
          ref="newUserFormRef"
          :form="newUserForm"
          :form-data="formStore.formData"
          @updated-form-data="formStore.setFormData"
        />
        <v-row class="mt-10">
          <v-col>
            <div class="d-flex flex-row justify-start ga-2">
              <v-btn
                rounded="lg"
                color="primary"
                :loading="
                  loadingStore.isLoading('psp_user_profile_put') ||
                  isRedirecting
                "
                @click="submit"
              >
                Save
              </v-btn>
              <v-btn
                rounded="lg"
                color="primary"
                variant="outlined"
                :loading="
                  loadingStore.isLoading('psp_user_profile_put') ||
                  isRedirecting
                "
                @click="oidcStore.logout()"
              >
                Cancel
              </v-btn>
            </div>
          </v-col>
        </v-row>
      </v-col>
    </v-row>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import { getPspUserProfile, updatePspUserProfile } from "@/api/psp-rep";
import EceForm from "@/components/Form.vue";
import PageContainer from "@/components/PageContainer.vue";
import newUserForm from "@/config/new-user-form";
import { useAlertStore } from "@/store/alert";
import { useFormStore } from "@/store/form";
import { useLoadingStore } from "@/store/loading";
import { useOidcStore } from "@/store/oidc";
import { useUserStore } from "@/store/user";
import { useRouter } from "vue-router";

export default defineComponent({
  name: "NewUser",
  components: { EceForm, PageContainer },
  setup() {
    const formStore = useFormStore();
    const userStore = useUserStore();
    const alertStore = useAlertStore();
    const loadingStore = useLoadingStore();
    const oidcStore = useOidcStore();
    const router = useRouter();

    // Initialize form data from userStore
    formStore.initializeForm({
      [newUserForm?.components?.firstName?.id || ""]: userStore.firstName ?? "",
      [newUserForm?.components?.lastName?.id || ""]: userStore.lastName ?? "",
      [newUserForm?.components?.preferredFirstName?.id || ""]:
        userStore.preferredName ?? "",
      [newUserForm?.components?.jobTitle?.id || ""]: userStore.jobTitle ?? "",
      [newUserForm?.components?.phoneNumber?.id || ""]: userStore.phone ?? "",
      [newUserForm?.components?.phoneNumberExtension?.id || ""]:
        userStore.phoneExtension ?? "",
      [newUserForm?.components?.email?.id || ""]: userStore.email ?? "",
      [newUserForm?.components?.hasAcceptedTermsOfUse?.id || ""]:
        userStore.hasAcceptedTermsOfUse ?? false,
    });

    return {
      newUserForm,
      formStore,
      alertStore,
      userStore,
      loadingStore,
      oidcStore,
      router,
    };
  },
  data() {
    return {
      isRedirecting: false,
    };
  },
  methods: {
    async submit() {
      const { valid } = await (
        this.$refs.newUserFormRef as typeof EceForm
      ).$refs[newUserForm.id].validate();

      if (!valid) {
        this.alertStore.setFailureAlert(
          "You must enter all required fields in the valid format.",
        );
      } else {
        this.isRedirecting = true;
        const userUpdated = await updatePspUserProfile(
          {
            firstName:
              this.formStore.formData[
                newUserForm?.components?.firstName?.id || ""
              ],
            lastName:
              this.formStore.formData[
                newUserForm?.components?.lastName?.id || ""
              ],
            preferredName:
              this.formStore.formData[
                newUserForm?.components?.preferredFirstName?.id || ""
              ],
            email:
              this.formStore.formData[newUserForm?.components?.email?.id || ""],
            phone:
              this.formStore.formData[
                newUserForm?.components?.phoneNumber?.id || ""
              ],
            phoneExtension:
              this.formStore.formData[
                newUserForm?.components?.phoneNumberExtension?.id || ""
              ],
            jobTitle:
              this.formStore.formData[
                newUserForm?.components?.jobTitle?.id || ""
              ],
            hasAcceptedTermsOfUse:
              this.formStore.formData[
                newUserForm?.components?.hasAcceptedTermsOfUse?.id || ""
              ],
          },
          true,
        );

        if (userUpdated) {
          const pspUserProfile = await getPspUserProfile();
          if (pspUserProfile !== null) {
            this.userStore.setPspUserProfile(pspUserProfile);
          }
          this.router.push("/");
        } else {
          this.alertStore.setFailureAlert("Profile save failed");
          this.isRedirecting = false;
        }
      }
    },
  },
});
</script>
