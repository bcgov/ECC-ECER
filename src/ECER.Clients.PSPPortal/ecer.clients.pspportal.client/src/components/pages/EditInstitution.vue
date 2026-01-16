<template>
  <PageContainer>
    <v-row>
      <v-col cols="12">
        <Breadcrumb />
      </v-col>
    </v-row>
    <v-row>
      <v-col class="ml-1" cols="12">
        <h1>Edit institution contact info</h1>
      </v-col>
    </v-row>
    <v-row>
      <v-col class="ml-1" cols="12">
        <h2>{{ institutionName }}</h2>
      </v-col>
    </v-row>
    <v-row>
      <v-col class="mt-4" cols="12">
        <EceForm ref="editEducationInstitutionFormRef" :form="institutionForm" :form-data="formStore.formData" @updated-form-data="formStore.setFormData" />
        <v-row class="mt-10">
          <v-col>
            <div class="d-flex flex-row justify-start ga-2">
              <v-btn rounded="lg" color="primary" :loading="loadingStore.isLoading('education_institution_put')" @click="saveInstitution">Save</v-btn>
              <v-btn rounded="lg" color="primary" variant="outlined" :loading="loadingStore.isLoading('education_institution_put')" @click="router.back()">
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

import { getEducationInstitution, updateEducationInstitution } from "@/api/education-institution";
import Breadcrumb from "@/components/Breadcrumb.vue";
import EceForm from "@/components/Form.vue";
import PageContainer from "@/components/PageContainer.vue";
import institutionForm from "@/config/institution-form";
import { useAlertStore } from "@/store/alert";
import { useFormStore } from "@/store/form";
import { useLoadingStore } from "@/store/loading";
import { useUserStore } from "@/store/user";
import { useRouter } from "vue-router";

export default defineComponent({
  name: "EditInstitution",
  components: { EceForm, Breadcrumb, PageContainer },
  setup: async () => {
    const formStore = useFormStore();
    const userStore = useUserStore();
    const alertStore = useAlertStore();
    const loadingStore = useLoadingStore();
    const router = useRouter();

    const institution = await getEducationInstitution();
    const institutionId = institution?.id;
    const institutionName = institution?.name;
    if (institution !== null) {
      formStore.initializeForm({
        [institutionForm?.components?.auspice?.id || ""]: institution.auspice,
        [institutionForm?.components?.street1?.id || ""]: institution.street1,
        [institutionForm?.components?.street2?.id || ""]: institution.street2,
        [institutionForm?.components?.street3?.id || ""]: institution.street3,
        [institutionForm?.components?.city?.id || ""]: institution.city,
        [institutionForm?.components?.province?.id || ""]: institution.province,
        [institutionForm?.components?.postalCode?.id || ""]: institution.postalCode,
        [institutionForm?.components?.website?.id || ""]: institution.websiteUrl,
      });
    }

    return { institutionForm, formStore, userStore, alertStore, loadingStore, router, institutionName, institutionId };
  },
  methods: {
    async saveInstitution() {
      const { valid } = await (this.$refs.editEducationInstitutionFormRef as typeof EceForm).$refs[institutionForm.id].validate();

      if (valid) {
        const institution = {
          id: this.institutionId,
          name: this.institutionName,
          auspice: this.formStore.formData[institutionForm?.components?.auspice?.id || ""],
          street1: this.formStore.formData[institutionForm?.components?.street1?.id || ""],
          street2: this.formStore.formData[institutionForm?.components?.street2?.id || ""],
          street3: this.formStore.formData[institutionForm?.components?.street3?.id || ""],
          city: this.formStore.formData[institutionForm?.components?.city?.id || ""],
          province: this.formStore.formData[institutionForm?.components?.province?.id || ""],
          postalCode: this.formStore.formData[institutionForm?.components?.postalCode?.id || ""],
          websiteUrl: this.formStore.formData[institutionForm?.components?.website?.id || ""],
        };
        const institutionUpdated = await updateEducationInstitution(institution);

        if (institutionUpdated) {
          this.alertStore.setSuccessAlert("You have successfully edited your institution information.");
          this.userStore.updateEducationInstitution(institution);
        } else {
          this.alertStore.setFailureAlert("Institution save failed");
        }
      } else {
        this.alertStore.setFailureAlert("You must enter all required fields in the valid format.");
      }
    },
  },
});
</script>
