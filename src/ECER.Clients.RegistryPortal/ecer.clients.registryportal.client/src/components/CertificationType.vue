<template>
  <v-container>
    <Breadcrumb :items="items" />
    <h1 class="mb-6">What type of certification do you want to apply for?</h1>
    <v-form ref="certificationForm">
      <v-expansion-panels v-model="selection" variant="accordion">
        <v-expansion-panel v-for="option in certificationOptions" :key="option.id" :value="option.id" class="rounded-lg">
          <v-expansion-panel-title>
            <v-row no-gutters>
              <v-col cols="12">
                <v-radio-group v-model="selection" :mandatory="true" :hide-details="true">
                  <v-radio color="primary" :label="option.title" :value="option.id"></v-radio>
                </v-radio-group>
              </v-col>
              <v-col v-if="option.id === CertificationType.FIVE_YEAR && !selection?.includes(CertificationType.FIVE_YEAR)" cols="11" offset="1">
                <p>If you are eligible for an ECE Five Year Certificate, you may also be eligible for one or both specializations:</p>
                <v-checkbox
                  v-model="selection"
                  color="primary"
                  label="Infant and Toddler Educator (ITE)"
                  :value="CertificationType.ITE"
                  hide-details="auto"
                ></v-checkbox>
                <v-checkbox
                  v-model="selection"
                  color="primary"
                  label="Special Needs Educator (SNE)"
                  :value="CertificationType.SNE"
                  hide-details="auto"
                ></v-checkbox>
              </v-col>
            </v-row>
          </v-expansion-panel-title>
          <v-expansion-panel-text>
            <Component :is="option.contentComponent" />
          </v-expansion-panel-text>
        </v-expansion-panel>
      </v-expansion-panels>
      <h2 class="mt-10">Certified in Canada?</h2>
      <p class="mt-3">
        If you're certified in another province or territory in Canada, you might be able to transfer you certification to British Columbia. Check the
        <!-- prettier-ignore -->
        <a href="https://www2.gov.bc.ca/gov/content/education-training/early-learning/teach/training-and-professional-development/become-an-early-childhood-educator/transfer-your-certification/certification-from-province-to-province">certification levels from province to province chart</a>
        to find out if your certification transfers.
      </p>
      <p class="mt-6">Are you applying to transfer your certification?</p>
      <v-radio-group
        class="mb-10"
        :model-value="applicationStore.draftApplication.applicationType"
        @update:model-value="handleUpdateCertifiedInCanada"
        :mandatory="true"
        :hide-details="true"
      >
        <v-radio color="primary" label="No" value="New"></v-radio>
        <v-radio
          color="primary"
          label="Yes – I have reviewed the certification levels from province to province chart and my certification is eligible to transfer"
          value="LaborMobility"
        ></v-radio>
      </v-radio-group>
      <v-input v-model="selection" :rules="[Rules.required('Select a certificate type to begin your application')]"></v-input>
    </v-form>

    <v-btn class="mt-6" rounded="lg" color="primary" @click="continueClick">Continue</v-btn>
  </v-container>
</template>

<script lang="ts">
import { mapState } from "pinia";
import { defineComponent } from "vue";
import type { VForm } from "vuetify/components/VForm";

import Breadcrumb from "@/components/Breadcrumb.vue";
import certificationOptions from "@/config/certification-types";
import { useAlertStore } from "@/store/alert";
import { useApplicationStore } from "@/store/application";
import { useCertificationTypeStore } from "@/store/certificationType";
import type { ApplicationTypes, Components } from "@/types/openapi";
import { CertificationType } from "@/utils/constant";
import * as Rules from "@/utils/formRules";
import { useRouter } from "vue-router";
import { useWizardStore } from "@/store/wizard";

export default defineComponent({
  name: "CertificationType",
  components: {
    Breadcrumb,
  },

  setup: () => {
    const applicationStore = useApplicationStore();
    const wizardStore = useWizardStore();
    const certificationTypeStore = useCertificationTypeStore();
    const alertStore = useAlertStore();
    const router = useRouter();

    // If props.modelValue contains "Ite" or "Sne", set the subSelection to those values
    if (applicationStore.isDraftCertificateTypeIte) {
      certificationTypeStore.subSelection.push(CertificationType.ITE);
    }
    if (applicationStore.isDraftCertificateTypeSne) {
      certificationTypeStore.subSelection.push(CertificationType.SNE);
    }

    // Set selection to value in props.modelValue
    if (applicationStore.isDraftCertificateTypeFiveYears) {
      certificationTypeStore.selection = CertificationType.FIVE_YEAR;
    } else if (applicationStore.isDraftCertificateTypeEceAssistant) {
      certificationTypeStore.selection = CertificationType.ECE_ASSISTANT;
    } else if (applicationStore.isDraftCertificateTypeOneYear) {
      certificationTypeStore.selection = CertificationType.ONE_YEAR;
    }

    return { applicationStore, wizardStore, certificationTypeStore, CertificationType, certificationOptions, Rules, alertStore, router };
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
          title: "Application types",
          disabled: true,
          href: "/application/certification",
        },
      ],
    };
  },
  computed: {
    selection: {
      get() {
        return this.certificationTypeStore.selection;
      },

      set(newValue: Components.Schemas.CertificationType) {
        if (newValue !== CertificationType.FIVE_YEAR) {
          this.certificationTypeStore.subSelection = [];
        }
        this.certificationTypeStore.selection = newValue;
      },
    },
    ...mapState(useCertificationTypeStore, ["certificationTypes"]),
  },
  methods: {
    handleUpdateCertifiedInCanada(value: ApplicationTypes | null) {
      if (value) {
        this.applicationStore.$patch({ draftApplication: { applicationType: value } });
      }
    },
    async continueClick() {
      const { valid } = await (this.$refs.certificationForm as VForm).validate();

      if (!valid) {
        this.alertStore.setFailureAlert("You must enter all required fields in the valid format.");
        return;
      }

      this.applicationStore.$patch({ draftApplication: { certificationTypes: this.certificationTypeStore.certificationTypes } });

      if (!this.certificationTypeStore.certificationTypes?.includes(CertificationType.FIVE_YEAR)) {
        this.applicationStore.$patch({ draftApplication: { workExperienceReferences: [] } });
      }
      this.router.push({ name: "application-requirements" });
    },
  },
});
</script>
