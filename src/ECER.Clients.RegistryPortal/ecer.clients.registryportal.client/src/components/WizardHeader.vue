<template>
  <v-container fluid class="bg-primary">
    <v-container>
      <v-row>
        <v-col :class="[mobile ? 'flex-column-reverse' : 'justify-space-between', 'd-flex']">
          <div>
            <ApplicationCertificationTypeHeader :is-renewal="isRenewal" :certification-types="applicationStore.draftApplication.certificationTypes ?? []" />
            <a v-if="!isRenewal && !isRegistrant" href="#" class="text-white" @click.prevent="toggleChangeCertificationConfirmation">
              Change certification type
            </a>
          </div>
          <div :class="[{ ['text-right mb-2']: mobile }]">
            <v-btn v-if="showSaveButton" variant="outlined" :loading="loadingStore.isLoading('draftapplication_put')" @click="saveAndExit">Save and exit</v-btn>
          </div>
        </v-col>
      </v-row>
    </v-container>
    <ConfirmationDialog
      :show="showConfirmation"
      title="Are you sure you want to change the type?"
      accept-button-text="Change type"
      @accept="changeCertification"
      @cancel="toggleChangeCertificationConfirmation"
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
import { useDisplay } from "vuetify";

import { useApplicationStore } from "@/store/application";
import { useLoadingStore } from "@/store/loading";

import ApplicationCertificationTypeHeader from "./ApplicationCertificationTypeHeader.vue";
import ConfirmationDialog from "./ConfirmationDialog.vue";

export default defineComponent({
  name: "WizardHeader",
  components: { ApplicationCertificationTypeHeader, ConfirmationDialog },
  props: {
    handleSaveDraft: {
      type: Function,
      required: true,
    },
    showSaveButton: {
      type: Boolean,
      required: true,
    },
    isRenewal: {
      type: Boolean,
      default: false,
    },
    isRegistrant: {
      type: Boolean,
      default: false,
    },
  },
  setup() {
    const applicationStore = useApplicationStore();
    const loadingStore = useLoadingStore();
    const { mobile } = useDisplay();

    return {
      applicationStore,
      loadingStore,
      mobile,
    };
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
    async saveAndExit() {
      await this.handleSaveDraft();
      this.$router.push({ name: "dashboard" });
    },
    async changeCertification() {
      this.showConfirmation = false;
      this.$router.push({ name: "application-certification" });
    },
  },
});
</script>
