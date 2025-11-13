<template>
  <Loading v-if="loadingStore.isLoading('icra_status_get')" />
  <v-container v-else>
    <Breadcrumb />
    <h1>Add employment experience reference</h1>
    <br />
    <h2>Provide proof of 2 years of independent ECE practice.</h2>
    <br />

    <p>Your employment experience must:</p>
    <ul class="ml-10">
      <li>Have been completed within the last 5 years</li>
      <li>Not overlap (any overlapping employment experience will only be counted once)</li>
      <li>Have been completed while holding valid ECE certification</li>
    </ul>
    <br />
    <p>If your employment experience was completed at multiple locations:</p>
    <ul class="ml-10">
      <li>Provide a reference from each person</li>
      <li>You may enter up to 6 references</li>
    </ul>
    <br />
    <p>
      The ECE Registry will contact your references to verify your employment. Once we receive your submission, we will send an email to these people containing
      a link to an online reference form.
    </p>
    <br />
    <p>
      If you are eligible for this application pathway, your references will be asked to complete a competencies assessment to verify that you have the
      knowledge, skills and abilities to work as an ECE in British Columbia.
    </p>
    <br />
    <EceForm ref="form" :form="icraEligibilityWorkExperienceReferenceUpsertForm" :form-data="formStore.formData" @updated-form-data="formStore.setFormData" />
    <br />
    <p>After you save, we will send an email to this person requesting a reference.</p>
    <v-row class="mt-6">
      <v-col class="d-flex flex-row ga-3 flex-wrap">
        <v-btn
          size="large"
          color="primary"
          :loading="loadingStore.isLoading('icra_work_reference_replace_post') || loadingStore.isLoading('icra_work_reference_add_post')"
          @click="handleSubmitReference"
        >
          Save new reference
        </v-btn>
        <v-btn
          size="large"
          variant="outlined"
          color="primary"
          :loading="loadingStore.isLoading('icra_work_reference_replace_post') || loadingStore.isLoading('icra_work_reference_add_post')"
          @click="router.back()"
        >
          Cancel
        </v-btn>
      </v-col>
    </v-row>
  </v-container>
</template>
<script lang="ts">
import { defineComponent } from "vue";
import { useRouter } from "vue-router";
import { useDisplay } from "vuetify";
import type { Components } from "@/types/openapi";
import { cleanPreferredName } from "@/utils/functions";

import { addIcraEligibilityWorkExperienceReference, replaceIcraEligibilityWorkExperienceReference } from "@/api/icra";

import Breadcrumb from "@/components/Breadcrumb.vue";
import Loading from "./Loading.vue";
import EceForm from "@/components/Form.vue";
import icraEligibilityWorkExperienceReferenceUpsertForm from "@/config/icra-eligibility/icra-eligibility-work-experience-reference-upsert-form";

import { useLoadingStore } from "@/store/loading";
import { useFormStore } from "@/store/form";
import { useAlertStore } from "@/store/alert";

export default defineComponent({
  name: "IcraEligibilityUpsertWorkExperienceReference",
  components: { Breadcrumb, Loading, EceForm },
  props: {
    icraEligibilityId: {
      type: String,
      required: true,
    },
    referenceId: {
      type: String,
      required: false,
    },
    type: {
      type: String as () => "add" | "replace",
      required: true,
    },
  },
  setup: async (props) => {
    const { smAndUp } = useDisplay();
    const loadingStore = useLoadingStore();
    const formStore = useFormStore();
    const alertStore = useAlertStore();
    const router = useRouter();

    formStore.initializeForm({});

    return {
      smAndUp,
      router,
      alertStore,
      loadingStore,
      formStore,
      icraEligibilityWorkExperienceReferenceUpsertForm,
    };
  },
  data() {
    return {
      icraEligibilityStatus: {} as Components.Schemas.ICRAEligibilityStatus,
    };
  },
  async mounted() {},
  computed: {},
  methods: {
    async handleSubmitReference() {
      // Validate the form
      const { valid } = await (this.$refs.form as typeof EceForm).$refs[icraEligibilityWorkExperienceReferenceUpsertForm.id].validate();

      if (valid) {
        const { error } = await this.submitReferenceApiCall();
        if (error) {
          this.alertStore.setFailureAlert(`Sorry, unable to ${this.type} reference. Try again later.`);
        } else {
          this.alertStore.setSuccessAlert("Reference updated. We sent them an email to request a reference.");
          this.router.push({ name: "manage-icra-eligibility-work-experience-references", params: { icraEligibilityId: this.icraEligibilityId } });
        }
      } else {
        this.alertStore.setFailureAlert("You must enter all required fields in the valid format to continue.");
      }
    },
    async submitReferenceApiCall() {
      switch (this.type) {
        case "add":
          return await addIcraEligibilityWorkExperienceReference(this.icraEligibilityId, this.formStore.formData);
        case "replace":
          return await replaceIcraEligibilityWorkExperienceReference(this.icraEligibilityId, this.referenceId || "", this.formStore.formData);
        default:
          console.warn("unhandled reference upsert type:", this.type);
          return { error: true };
      }
    },
    cleanPreferredName,
  },
});
</script>
