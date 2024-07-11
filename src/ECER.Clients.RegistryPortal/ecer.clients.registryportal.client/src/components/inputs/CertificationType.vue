<template>
  <v-container>
    <Breadcrumb :items="items" />
    <h2 class="mb-6">What type of certification do you want to apply for?</h2>
    <v-form ref="certificationForm">
      <v-expansion-panels v-if="mode === 'selection'" v-model="selection" variant="accordion">
        <v-expansion-panel v-for="option in certificationOptions" :key="option.id" :value="option.id" class="rounded-lg">
          <v-expansion-panel-title>
            <v-row no-gutters>
              <v-col cols="12">
                <v-radio-group v-model="selection" :mandatory="true" :hide-details="true">
                  <v-radio color="primary" :label="option.title" :value="option.id"></v-radio>
                </v-radio-group>
              </v-col>
              <v-col v-if="option.id === CertificationType.FIVE_YEAR && !selection?.includes(CertificationType.FIVE_YEAR)" cols="11" offset="1">
                <p class="small">If you are eligible for an ECE Five Year Certificate, you may also be eligible for one or both specializations:</p>
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
      <v-input v-model="selection" :rules="[Rules.required('Select a certificate type to begin your application')]"></v-input>
    </v-form>
    <div v-if="mode === 'terms'">
      <div v-for="certificationType in certificationTypes" :key="certificationType">
        <template v-if="certificationType === CertificationType.ECE_ASSISTANT">
          <ECEAssistantRequirements />
        </template>
        <template v-if="certificationType === CertificationType.ONE_YEAR">
          <ECEOneYearRequirements />
        </template>
        <template v-if="certificationType === CertificationType.FIVE_YEAR">
          <ECEFiveYearRequirements />
        </template>
        <template v-if="certificationType === CertificationType.SNE">
          <SneRequirements />
        </template>
        <template v-if="certificationType === CertificationType.ITE">
          <IteRequirements />
        </template>
      </div>
    </div>
    <v-btn
      v-if="mode === 'terms'"
      class="mt-6 mr-3"
      rounded="lg"
      @click="
        () => {
          mode = 'selection';
        }
      "
    >
      Back to selection
    </v-btn>
    <v-btn class="mt-6" rounded="lg" color="primary" @click="continueClick">Continue</v-btn>
  </v-container>
</template>

<script lang="ts">
import { mapState } from "pinia";
import { defineComponent } from "vue";
import type { VForm } from "vuetify/components/VForm";

import Breadcrumb from "@/components/Breadcrumb.vue";
import ECEAssistantRequirements from "@/components/ECEAssistantRequirements.vue";
import ECEFiveYearRequirements from "@/components/ECEFiveYearRequirements.vue";
import ECEOneYearRequirements from "@/components/ECEOneYearRequirements.vue";
import IteRequirements from "@/components/IteRequirements.vue";
import SneRequirements from "@/components/SneRequirements.vue";
import certificationOptions from "@/config/certification-types";
import { useAlertStore } from "@/store/alert";
import { useApplicationStore } from "@/store/application";
import { useCertificationTypeStore } from "@/store/certificationType";
import type { Components } from "@/types/openapi";
import { CertificationType } from "@/utils/constant";
import * as Rules from "@/utils/formRules";

type Mode = "terms" | "selection";

export default defineComponent({
  name: "CertificationType",
  components: {
    ECEAssistantRequirements,
    ECEOneYearRequirements,
    ECEFiveYearRequirements,
    SneRequirements,
    IteRequirements,
    Breadcrumb,
  },

  setup: () => {
    const applicationStore = useApplicationStore();
    const certificationTypeStore = useCertificationTypeStore();
    const alertStore = useAlertStore();

    // If props.modelValue contains "Ite" or "Sne", set the subSelection to those values
    if (applicationStore.draftApplication.certificationTypes?.includes(CertificationType.ITE)) {
      certificationTypeStore.subSelection.push(CertificationType.ITE);
    }
    if (applicationStore.draftApplication.certificationTypes?.includes(CertificationType.SNE)) {
      certificationTypeStore.subSelection.push(CertificationType.SNE);
    }

    // Set selection to value in props.modelValue
    if (applicationStore.draftApplication.certificationTypes?.includes(CertificationType.FIVE_YEAR)) {
      certificationTypeStore.selection = CertificationType.FIVE_YEAR;
    } else if (applicationStore.draftApplication.certificationTypes?.includes(CertificationType.ECE_ASSISTANT)) {
      certificationTypeStore.selection = CertificationType.ECE_ASSISTANT;
    } else if (applicationStore.draftApplication.certificationTypes?.includes(CertificationType.ONE_YEAR)) {
      certificationTypeStore.selection = CertificationType.ONE_YEAR;
    }

    return { applicationStore, certificationTypeStore, CertificationType, certificationOptions, Rules, alertStore };
  },
  data() {
    return {
      mode: "selection" as Mode,
      items: [
        {
          title: "Home",
          disabled: false,
          href: "/",
        },
        {
          title: "Application Types",
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
    async continueClick() {
      const { valid } = await (this.$refs.certificationForm as VForm).validate();

      if (!valid) {
        this.alertStore.setFailureAlert("You must enter all required fields in the valid format.");
        return;
      }
      if (this.mode === "selection") {
        this.mode = "terms";
      } else {
        this.applicationStore.$patch({ draftApplication: { certificationTypes: this.certificationTypeStore.certificationTypes } });

        if (!this.certificationTypeStore.certificationTypes?.includes(CertificationType.FIVE_YEAR)) {
          this.applicationStore.$patch({ draftApplication: { workExperienceReferences: [] } });
        }
        this.$router.push({ name: "declaration" });
      }
    },
  },
});
</script>
