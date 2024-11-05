<template>
  <v-container>
    <v-breadcrumbs class="pl-0" :items="items" color="primary">
      <template #divider>/</template>
    </v-breadcrumbs>
    <v-row>
      <v-col>
        <Alert v-model="isDuplicateReference" type="error" title="choose someone else" prominent>
          <p>This person is your work experience reference. Your character reference and work experience reference must be different people.</p>
        </Alert>
      </v-col>
    </v-row>
    <div class="d-flex flex-column ga-3">
      <h2 class="mt-10">Add character reference</h2>

      <p>Make sure you choose a person that:</p>
      <ul class="ml-10">
        <li>Can speak to your character</li>
        <li>Can speak to your ability to educate and care for young children</li>
        <li>Has known you for at least 6 months</li>
        <li>Is not your relative, partner, spouse, or yourself</li>
      </ul>
      <p>We recommend the person is a certified ECE who has directly observed you working with young children.</p>
      <p v-if="applicationStatus?.certificationTypes?.includes(CertificationType.FIVE_YEAR)" class="mb-6">
        The person
        <b>cannot</b>
        be any of your work experience references.
      </p>
      <EceForm
        ref="upsertCharacterReferenceForm"
        :form="characterReferenceUpsertForm"
        :form-data="formStore.formData"
        @updated-form-data="formStore.setFormData"
      />
      <p class="mt-6">After you save, we will send an email to this person requesting a reference.</p>
    </div>
    <v-row class="mt-6">
      <v-col class="d-flex flex-row ga-3 flex-wrap">
        <v-btn size="large" color="primary" :loading="loadingStore.isLoading('application_characterreference_update_post')" @click="handleSubmitReference">
          Save new reference
        </v-btn>
        <v-btn size="large" variant="outlined" color="primary" @click="router.back()">Cancel</v-btn>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useRoute, useRouter } from "vue-router";

import { getApplicationStatus } from "@/api/application";
import { upsertCharacterReference } from "@/api/reference";
import Alert from "@/components/Alert.vue";
import EceForm from "@/components/Form.vue";
import characterReferenceUpsertForm from "@/config/character-references-upsert-form";
import { useAlertStore } from "@/store/alert";
import { useApplicationStore } from "@/store/application";
import { useFormStore } from "@/store/form";
import { useLoadingStore } from "@/store/loading";
import type { Components } from "@/types/openapi";
import { CertificationType } from "@/utils/constant";

export default defineComponent({
  name: "UpsertCharacterReference",
  components: { EceForm, Alert },
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
    const loadingStore = useLoadingStore();
    const router = useRouter();
    const route = useRoute();
    const formStore = useFormStore();
    const applicationStatus = (await getApplicationStatus(route.params.applicationId.toString()))?.data;

    let reference: Components.Schemas.CharacterReference | undefined = undefined;

    if (props.referenceId) {
      // Check store for existing reference
      reference = applicationStore.characterReferenceById(props.referenceId);

      if (!reference) {
        router.back();
      } else {
        formStore.initializeForm({});
      }
    }

    return { applicationStore, alertStore, reference, formStore, loadingStore, characterReferenceUpsertForm, router, applicationStatus, CertificationType };
  },
  data() {
    // Define a base array of always-present items.
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
        title: "Add",
        disabled: true,
        href: `/manage-application/${this.applicationId}/character-reference/${this.referenceId}/add`,
      },
    ];

    // Add the "Character reference" item only if `reference` is defined.
    if (this.reference) {
      items.splice(2, 0, {
        // Insert at index 2, before "Add"
        title: "Character reference",
        disabled: false,
        href: `/manage-application/${this.applicationId}/character-reference/${this.referenceId}`,
      });
    }

    return {
      items,
      isDuplicateReference: false,
    };
  },

  methods: {
    async handleSubmitReference() {
      // Validate the form
      const { valid } = await (this.$refs.upsertCharacterReferenceForm as typeof EceForm).$refs[characterReferenceUpsertForm.id].validate();
      if (valid) {
        //check for duplicate reference

        this.isDuplicateReference = false;

        const refSet = new Set<string>();

        if (this.applicationStatus?.workExperienceReferencesStatus) {
          for (const ref of this.applicationStatus.workExperienceReferencesStatus) {
            if (ref.status !== "Rejected") {
              refSet.add(`${ref.firstName?.toLowerCase()} ${ref.lastName?.toLowerCase()}`);
            }
          }
        }

        if (refSet.has(`${this.formStore.formData.firstName.toLowerCase()} ${this.formStore.formData.lastName.toLowerCase()}`)) {
          this.isDuplicateReference = true;
          //scroll to top of page
          window.scrollTo({
            top: 0,
            behavior: "smooth",
          });
        } else {
          const { error } = await upsertCharacterReference({ application_id: this.applicationId, reference_id: this.referenceId }, this.formStore.formData);
          if (error) {
            this.alertStore.setFailureAlert("Sorry, something went wrong and your changes could not be saved. Try again later.");
          } else {
            this.alertStore.setSuccessAlert("Reference updated. We sent them an email to request a reference.");
            this.router.push(`/manage-application/${this.applicationId}`);
          }
        }
      } else {
        this.alertStore.setFailureAlert("You must enter all required fields in the valid format to continue.");
      }
    },
  },
});
</script>
