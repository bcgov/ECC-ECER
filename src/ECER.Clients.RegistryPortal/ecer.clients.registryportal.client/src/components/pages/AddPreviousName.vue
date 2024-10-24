<template>
  <PageContainer>
    <Breadcrumb :items="items" />
    <v-row>
      <v-col>
        <Alert v-model="duplicatePreviousName" type="error" prominent>
          <p>This previous name is already in your account. You cannot add the same name again.</p>
        </Alert>
      </v-col>
    </v-row>
    <v-row>
      <v-col class="ml-1" cols="12">
        <h1>Add previous name</h1>
      </v-col>
    </v-row>
    <div class="d-flex flex-column ga-3 my-6">
      <p>
        This is only needed when you’re applying for certification and a transcript or other document has a different name then your current legal name. For
        example, if your transcript has a different name than the name in your account.
      </p>
      <p>There are 3 steps:</p>
      <ol class="ml-10">
        <li>Enter your previous name below</li>
        <li>Provide government-issued ID</li>
        <li>We’ll review your ID and add this name to your account</li>
      </ol>
      <p>Before you continue, make sure you have one of the accepted goverment-issued IDs:</p>
      <ul class="ml-10">
        <li>Government-issued marriage certificate</li>
        <li>Divorce certificate or papers</li>
        <li>Government-issued change of name document</li>
      </ul>
    </div>
    <v-row class="mt-10">
      <v-col>
        <EceForm ref="addPreviousNameForm" :form="previousNameForm" :form-data="formStore.formData" @updated-form-data="formStore.setFormData" />
      </v-col>
    </v-row>
    <v-row class="mt-6">
      <v-col class="d-flex flex-row ga-3 flex-wrap">
        <v-btn size="large" color="primary" :loading="loadingStore.isLoading('profile_put')" @click="handleSubmitPreviousName">Save and continue</v-btn>
        <v-btn size="large" variant="outlined" color="primary" @click="router.back()">Cancel</v-btn>
      </v-col>
    </v-row>
  </PageContainer>
</template>

<script lang="ts">
import { useRouter } from "vue-router";

import { getProfile, putProfile } from "@/api/profile";
import Alert from "@/components/Alert.vue";
import Breadcrumb from "@/components/Breadcrumb.vue";
import EceForm from "@/components/Form.vue";
import PageContainer from "@/components/PageContainer.vue";
import previousNameForm from "@/config/previous-name-form";
import { useAlertStore } from "@/store/alert";
import { useFormStore } from "@/store/form";
import { useLoadingStore } from "@/store/loading";
import { useUserStore } from "@/store/user";
import type { Components } from "@/types/openapi";

export default {
  name: "AddPreviousName",
  components: { PageContainer, EceForm, Alert, Breadcrumb },
  setup() {
    const formStore = useFormStore();
    const loadingStore = useLoadingStore();
    const userStore = useUserStore();
    const alertStore = useAlertStore();
    const router = useRouter();

    formStore.initializeForm({});

    return { formStore, loadingStore, alertStore, userStore, previousNameForm, router };
  },
  data: () => ({
    items: [
      {
        title: "Home",
        disabled: false,
        href: "/",
      },
      {
        title: "Profile",
        disabled: false,
        href: "/profile",
      },
      {
        title: "Add previous name",
        disabled: true,
        href: "/profile/add-previous-name",
      },
    ],
  }),
  computed: {
    newPreviousName(): Components.Schemas.PreviousName {
      return {
        firstName: this.formStore.formData[previousNameForm.inputs.firstName.id] ?? null,
        middleName: this.formStore.formData[previousNameForm.inputs.middleName.id] ?? null,
        lastName: this.formStore.formData[previousNameForm.inputs.lastName.id],
        source: "Profile",
      };
    },
    duplicatePreviousName() {
      // Check if any existing previous name matches the new previous name
      return this.userStore.userProfile?.previousNames?.some(
        (previousName) =>
          previousName.firstName === this.newPreviousName.firstName &&
          previousName.middleName === this.newPreviousName.middleName &&
          previousName.lastName === this.newPreviousName.lastName,
      );
    },
  },
  methods: {
    async handleSubmitPreviousName() {
      // Validate the form
      const { valid } = await (this.$refs.addPreviousNameForm as typeof EceForm).$refs[previousNameForm.id].validate();
      if (valid) {
        //check for duplicate previous name
        if (!this.duplicatePreviousName) {
          const { error } = await putProfile({
            ...this.userStore.userProfile,
            previousNames: [this.newPreviousName],
          });

          if (error) {
            this.alertStore.setFailureAlert("Sorry, something went wrong and your changes could not be saved. Try again later.");
          } else {
            // Refetch user profile to get the new previous name Id
            const userProfile = await getProfile();
            if (userProfile !== null) {
              this.userStore.setUserProfile(userProfile);
              const latestPreviousName = userProfile.previousNames?.find(
                (previousName) =>
                  previousName.firstName === this.newPreviousName.firstName &&
                  previousName.middleName === this.newPreviousName.middleName &&
                  previousName.lastName === this.newPreviousName.lastName,
              );
              if (latestPreviousName) {
                this.router.push({ name: "verify-previous-name", params: { previousNameId: latestPreviousName.id } });
              }
            }
          }
        } else {
          window.scrollTo(0, 0);
        }
      } else {
        this.alertStore.setFailureAlert("You must enter all required fields in the valid format to continue.");
      }
    },
  },
};
</script>
