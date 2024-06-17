<template>
  <v-container>
    <v-breadcrumbs class="pl-0" :items="items" color="primary">
      <template #divider>/</template>
    </v-breadcrumbs>
    <div class="d-flex flex-column ga-3">
      <h2 class="mt-10">Add work experience reference</h2>

      <p>
        Make sure you choose a person that:
        <br />
        If they do not receive the email:
      </p>
      <ul class="ml-10">
        <li>Can speak to your knowledge, skill, ability and competencies as an ECE</li>
        <li>Has directly supervised (observed) the hours they attest to</li>
        <li>Has held a valid Canadian ECE certification/registration during the hours they supervised or observed you</li>
      </ul>
      <p class="mb-6">
        The person
        <b>cannot</b>
        be any of your character reference.
      </p>
      <EceForm
        ref="upsertWorkExperienceReferenceForm"
        :form="workExperienceReferenceUpsertForm"
        :form-data="formStore.formData"
        @updated-form-data="formStore.setFormData"
      />
      <p>After you save, we will send an email to this person requesting a reference.</p>
    </div>
    <v-row class="mt-6">
      <v-col class="d-flex flex-row ga-3 flex-wrap">
        <v-btn size="large" color="primary" :loading="loadingStore.isLoading('application_workexperiencereference_update_post')" @click="handleSubmitReference">
          Save new reference
        </v-btn>
        <v-btn size="large" variant="outlined" color="primary" @click="router.back()">Cancel</v-btn>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useRouter } from "vue-router";

import { upsertWorkExperienceReference } from "@/api/reference";
import EceForm from "@/components/Form.vue";
import workExperienceReferenceUpsertForm from "@/config/work-experience-reference-upsert-form";
import { useAlertStore } from "@/store/alert";
import { useApplicationStore } from "@/store/application";
import { useFormStore } from "@/store/form";
import { useLoadingStore } from "@/store/loading";
import type { Components } from "@/types/openapi";

export default defineComponent({
  name: "UpsertWorkExperienceReference",
  components: { EceForm },
  props: {
    applicationId: {
      type: String,
      required: true,
    },
    referenceId: {
      type: String,
      required: false,
      default: "",
    },
  },
  setup: async (props) => {
    const applicationStore = useApplicationStore();
    const alertStore = useAlertStore();
    const formStore = useFormStore();
    const loadingStore = useLoadingStore();
    const router = useRouter();

    let reference: Components.Schemas.WorkExperienceReference | undefined = undefined;

    if (props.referenceId) {
      // Check store for existing reference
      const reference = applicationStore.workExperienceReferenceById(props.referenceId);

      if (!reference) {
        router.back();
      }
    }
    formStore.initializeForm({});

    return { applicationStore, alertStore, reference, formStore, loadingStore, workExperienceReferenceUpsertForm, router };
  },
  data() {
    const items = [
      {
        title: "Home",
        disabled: false,
        href: "/",
      },
      {
        title: "Application",
        disabled: false,
        href: `/manage-application/${this.applicationId}`,
      },
      {
        title: "Work experience references",
        disabled: false,
        href: `/manage-application/${this.applicationId}/work-experience-references`,
      },
    ];

    if (this.referenceId) {
      // Check if referenceId is valid
      items.push({
        title: "Reference",
        disabled: false,
        href: `/manage-application/${this.applicationId}/work-experience-reference/${this.referenceId}`,
      });
      items.push({
        title: "Add",
        disabled: true,
        href: `/manage-application/${this.applicationId}/work-experience-reference/${this.referenceId}/add`,
      });
    } else {
      items.push({
        title: "Add",
        disabled: true,
        href: `/manage-application/${this.applicationId}/work-experience-references/add`,
      });
    }

    return { items };
  },

  methods: {
    async handleSubmitReference() {
      // Validate the form
      const { valid } = await (this.$refs.upsertWorkExperienceReferenceForm as typeof EceForm).$refs[workExperienceReferenceUpsertForm.id].validate();

      if (valid) {
        const { error } = await upsertWorkExperienceReference({ application_id: this.applicationId, reference_id: this.referenceId }, this.formStore.formData);
        if (error) {
          this.alertStore.setFailureAlert("Sorry, something went wrong and your changes could not be saved. Try again later.");
        } else {
          this.alertStore.setSuccessAlert("Reference updated. We sent them an email to request a reference.");
          this.router.push({ name: "manageWorkExperienceReferences", params: { applicationId: this.applicationId } });
        }
      } else {
        this.alertStore.setFailureAlert("You must enter all required fields in the valid format to continue.");
      }
    },
  },
});
</script>
