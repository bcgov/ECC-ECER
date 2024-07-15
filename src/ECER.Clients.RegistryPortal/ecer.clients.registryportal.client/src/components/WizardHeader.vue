<template>
  <v-container fluid class="bg-primary">
    <v-container>
      <v-row>
        <v-col class="d-flex justify-space-between">
          <div>
            <ApplicationCertificationTypeHeader :certification-types="applicationStore.draftApplication.certificationTypes ?? []" />
            <a href="#" class="text-white" @click.prevent="toggleChangeCertificationConfirmation">Click to change certification</a>
          </div>
          <div>
            <v-btn v-if="showSaveButton" variant="outlined" @click="saveAndExit">Save and exit</v-btn>
          </div>
        </v-col>
      </v-row>
    </v-container>
    <ConfirmationDialog
      @accept="changeCertification"
      @cancel="toggleChangeCertificationConfirmation"
      :show="showConfirmation"
      title="Are you sure you want to change the type?"
      accept-button-text="Change type"
    >
      <template #confirmation-text>
        <div class="pb-3">When you change the type of certification you're applying for</div>
        <ul class="ml-10">
          <li>It will save the data you've entered</li>
          <li>It may change the type and amount of information you need to provide</li>
        </ul>
      </template>
    </ConfirmationDialog>
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import { useApplicationStore } from "@/store/application";
import ConfirmationDialog from "./ConfirmationDialog.vue";

import ApplicationCertificationTypeHeader from "./ApplicationCertificationTypeHeader.vue";

export default defineComponent({
  name: "WizardHeader",
  components: { ApplicationCertificationTypeHeader, ConfirmationDialog },
  setup() {
    const applicationStore = useApplicationStore();

    return {
      applicationStore,
    };
  },
  props: {
    handleSaveDraft: {
      type: Function,
      required: true,
    },
    showSaveButton: {
      type: Boolean,
      required: true,
    },
  },
  data: () => ({
    items: [
      {
        title: "Home",
        disabled: false,
        href: "/",
      },

      {
        title: "Apply New Certification",
        disabled: true,
        href: "application",
      },
    ],
    showConfirmation: false,
  }),
  methods: {
    toggleChangeCertificationConfirmation() {
      this.showConfirmation = !this.showConfirmation;
    },
    saveAndExit() {
      this.handleSaveDraft();
      this.$router.push({ name: "dashboard" });
    },
    changeCertification() {
      this.showConfirmation = false;
      this.$router.push({ name: "application-certification" });
    },
  },
});
</script>
