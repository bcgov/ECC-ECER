<template>
  <v-container>
    <v-breadcrumbs class="pl-0" :items="items" color="primary">
      <template #divider>/</template>
    </v-breadcrumbs>
    <div class="d-flex flex-column ga-3">
      <h2 class="mt-10">Add character reference</h2>

      <p>
        Make sure you choose a person that:
        <br />
        If they do not receive the email:
      </p>
      <ul class="ml-10">
        <li>Can speak to your character</li>
        <li>Can speak to your ability to educate and care for young children</li>
        <li>Has known you for at least 6 months</li>
        <li>Is not your relative, partner, spouse, or yourself</li>
      </ul>
      <p>We recommend the person is a certified ECE who has directly observed you working with young children.</p>
      <p class="mb-6">
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
      <p>After you save, we will send an email to this person requesting a reference.</p>
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
import { useRouter } from "vue-router";

import { upsertCharacterReference } from "@/api/reference";
import EceForm from "@/components/Form.vue";
import characterReferenceUpsertForm from "@/config/character-references-upsert-form";
import { useAlertStore } from "@/store/alert";
import { useApplicationStore } from "@/store/application";
import { useFormStore } from "@/store/form";
import { useLoadingStore } from "@/store/loading";
import type { Components } from "@/types/openapi";

export default defineComponent({
  name: "UpsertCharacterReference",
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
    const loadingStore = useLoadingStore();
    const router = useRouter();
    const formStore = useFormStore();

    // Check store for existing reference
    const reference: Components.Schemas.CharacterReference | undefined = applicationStore.characterReferenceById(props.referenceId);

    if (!reference) {
      router.back();
    } else {
      formStore.initializeForm({});
    }

    return { applicationStore, alertStore, reference, formStore, loadingStore, characterReferenceUpsertForm, router };
  },
  data() {
    return {
      items: [
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
          title: "Character reference",
          disabled: false,
          href: `/manage-application/${this.applicationId}/character-reference/${this.referenceId}`,
        },
        {
          title: "Add",
          disabled: true,
          href: `/manage-application/${this.applicationId}/character-reference/${this.referenceId}/add`,
        },
      ],
    };
  },

  methods: {
    async handleSubmitReference() {
      // Validate the form
      const { valid } = await (this.$refs.upsertCharacterReferenceForm as typeof EceForm).$refs[characterReferenceUpsertForm.id].validate();

      if (valid) {
        const { error } = await upsertCharacterReference({ application_id: this.applicationId, reference_id: this.referenceId }, this.formStore.formData);
        if (error) {
          this.alertStore.setFailureAlert("Sorry, something went wrong and your changes could not be saved. Try again later.");
        } else {
          this.alertStore.setSuccessAlert("Reference updated. We sent them an email to request a reference.");
          this.router.push(`/manage-application/${this.applicationId}`);
        }
      } else {
        this.alertStore.setFailureAlert("You must enter all required fields in the valid format to continue.");
      }
    },
  },
});
</script>
