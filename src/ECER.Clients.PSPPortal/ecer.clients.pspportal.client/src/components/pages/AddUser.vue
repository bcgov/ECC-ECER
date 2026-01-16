<template>
  <PageContainer>
    <v-row>
      <v-col cols="12">
        <Breadcrumb />
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="12">
        <h1>Invite user</h1>
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="12">
        <p>
          Provide the user’s details below to invite them to join you in this PSP Portal. Once the invitation is accepted, this user will have access to the
          {{ educationInstitutionName }} PSP portal.
        </p>
      </v-col>
    </v-row>
    <v-row>
      <v-col class="mt-4" cols="12">
        <EceForm ref="addUserFormRef" :form="inviteUserForm" :form-data="formStore.formData" @updated-form-data="formStore.setFormData" />
        <v-row class="mt-10">
          <v-col>
            <div class="d-flex flex-row justify-start ga-2">
              <v-btn rounded="lg" color="primary" :loading="loadingStore.isLoading('psp_user_add')" @click="saveUser">Send invitation</v-btn>
              <v-btn rounded="lg" color="primary" variant="outlined" :loading="loadingStore.isLoading('psp_user_add')" @click="router.back()">Cancel</v-btn>
            </div>
          </v-col>
        </v-row>
      </v-col>
    </v-row>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import { addUser } from "@/api/manage-users";
import Breadcrumb from "@/components/Breadcrumb.vue";
import EceForm from "@/components/Form.vue";
import PageContainer from "@/components/PageContainer.vue";
import inviteUserForm from "@/config/invite-user-form";
import { useAlertStore } from "@/store/alert";
import { useFormStore } from "@/store/form";
import { useLoadingStore } from "@/store/loading";
import { useRouter } from "vue-router";

export default defineComponent({
  name: "AddUser",
  components: { EceForm, Breadcrumb, PageContainer },
  props: {
    educationInstitutionName: {
      type: String,
      required: true,
    },
  },
  setup() {
    const formStore = useFormStore();
    const alertStore = useAlertStore();
    const loadingStore = useLoadingStore();
    const router = useRouter();

    formStore.setFormData({
      email: "",
      firstName: "",
      lastName: "",
      jobTitle: "",
    });

    return { inviteUserForm, formStore, alertStore, loadingStore, router };
  },
  methods: {
    async saveUser() {
      const { valid } = await (this.$refs.addUserFormRef as typeof EceForm).$refs[inviteUserForm.id].validate();

      if (!valid) {
        this.alertStore.setFailureAlert("You must enter all required fields in the valid format.");
      } else {
        const response = await addUser({
          email: this.formStore.formData[inviteUserForm?.components?.email?.id || ""],
          firstName: this.formStore.formData[inviteUserForm?.components?.firstName?.id || ""],
          lastName: this.formStore.formData[inviteUserForm?.components?.lastName?.id || ""],
          jobTitle: this.formStore.formData[inviteUserForm?.components?.jobTitle?.id || ""],
        });
        if (response?.id) {
          this.alertStore.setSuccessAlert("User has been successfully invited.");
          await this.router.push({
            name: "manage-users",
            params: { educationInstitutionName: this.educationInstitutionName },
          });
        } else {
          this.alertStore.setFailureAlert("User invitation failed, please try again.");
        }
      }
    },
  },
});
</script>
