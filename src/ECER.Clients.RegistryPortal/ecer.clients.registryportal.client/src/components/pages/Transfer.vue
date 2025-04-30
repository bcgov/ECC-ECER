<template>
  <PageContainer>
    <v-row>
      <v-col cols="12">
        <Breadcrumb :items="items" />
      </v-col>
    </v-row>
    <v-row>
      <v-col class="ml-1" cols="12">
        <h1>Check your transfer eligibility</h1>
        <p class="mt-5">Answer a few questions to see if you are eligible to apply to transfer your certification from another province or territory to B.C.</p>
        <h2 class="mt-5">Out-of-province registrant information</h2>
        <v-row>
          <v-col md="8" lg="6" xl="4">
            <p class="mt-8">In which Canadian province or territory are you certified?</p>
            <v-select
              id="province"
              class="pt-2"
              :items="configStore.provinceList.filter((p) => p.provinceName !== 'British Columbia' && p.provinceName !== 'Other')"
              variant="outlined"
              label=""
              v-model="province"
              item-title="provinceName"
              item-value="provinceId"
              :rules="[Rules.required('Select your province or territory', 'provinceId')]"
              @update:modelValue="onProvinceChange"
              return-object
            ></v-select>
          </v-col>
        </v-row>
        <v-row>
          <v-col v-if="outOfProvinceCertificationTypes.length > 0" md="8" lg="6" xl="4">
            <p class="mt-8">What certification do you have?</p>
            <v-select
              id="outOfProvinceCertification"
              class="pt-2"
              variant="outlined"
              label=""
              :items="outOfProvinceCertificationTypes"
              v-model="outOfProvinceCertification"
              item-title="transferringCertificate.certificationType"
              item-value="transferringCertificate.id"
              :rules="[Rules.required('Select your out of province certification type', 'transferringCertificate')]"
              @update:modelValue="onCertificationChange"
              return-object
            ></v-select>
          </v-col>
          <v-col v-if="outOfProvinceCertificationTypes.length === 0 && province && !outOfProvinceCertificationTypesLoading" class="ml-1" cols="12">
            <Callout type="warning" title="The province or territory you hold certification in is not eligible to be transferred to B.C.">
              <p>
                You need to
                <router-link to="/application/certification">apply for certification</router-link>
                through a different pathway.
              </p>
            </Callout>
          </v-col>
        </v-row>
        <v-row v-if="highestCertificationType === CertificationType.Assistant">
          <v-col class="ml-1" cols="12">
            <Callout type="warning" title="You can apply to transfer your certification to B.C. to an ECE Assistant">
              <p>
                You will be able to work alongside ECEs and/or Infant and Toddler Educators in licensed child care programs for children birth to 5 years of
                age.
              </p>
            </Callout>
          </v-col>
        </v-row>
        <v-row v-else-if="highestCertificationType">
          <v-col class="ml-1" cols="12">
            <h2 class="mt-5">Work experience</h2>
            <p class="mt-5">
              Your Canadian work experience determines your eligibility to apply for a transfer to either an ECE One Year or ECE Five Year certification in B.C.
            </p>
            <p class="mt-5">Your work experience:</p>
            <ul class="mt-3 ml-8">
              <li>Includes any work or volunteer hours completed within the last 5 years</li>
              <li>Cannot include your practicum hours (hours that were a part of your education)</li>
              <li>Must be observed by Canadian-certified ECE</li>
            </ul>
            <p class="mt-5">Do you have 500 hours of Canadian work experience?</p>
            <v-radio-group class="mt-3" id="programConfirmationRadio" v-model="has500HoursWorkExperience" :rules="[Rules.required()]" color="primary">
              <v-radio label="Yes" value="true"></v-radio>
              <v-radio label="No" value="false"></v-radio>
            </v-radio-group>
            <Callout
              v-if="has500HoursWorkExperience === 'false'"
              type="warning"
              title="You can apply to transfer your certification to B.C. to an ECE One Year certification"
            >
              <p class="mt-3">You will be able to work:</p>
              <ul class="mt-2 mb-8 ml-8">
                <li>along and/or as the primay educator in licensed child care programs for children 3 to 5 years of age, and/or</li>
                <li>alongside an Infant and Toddler Educator in licensed programs of children under 36 months</li>
              </ul>
              <p>Once you get 500 hours of work experience observed by a Canadian-certified ECE, you can apply to upgrade to an ECE Five Year certificate.</p>
            </Callout>
            <Callout
              v-else-if="has500HoursWorkExperience === 'true' && highestCertificationType === CertificationType.FiveYearCertificate"
              type="warning"
              title="You can apply to transfer your certification to B.C. to an ECE Five Year"
            >
              <p class="mt-3">You will be able to work:</p>
              <ul class="mt-2 ml-8">
                <li>along and/or as the primay educator in licensed child care programs for children 3 to 5 years of age, and/or</li>
                <li>alongside an Infant and Toddler Educator in licensed programs of children under 36 months</li>
              </ul>
            </Callout>
            <Callout
              v-else-if="has500HoursWorkExperience === 'true' && highestCertificationType === CertificationType.FiveYearCertificateITE_SNE"
              type="warning"
              title="You can apply to transfer your certification to B.C. to an ECE Five Year with Infant and Toddler Educator (ITE) and Special Needs Educator (SNE)"
            >
              <p strong class="mt-3">An ECE Five Year certification allows you to work in licensed child care programs:</p>
              <ul class="mt-2 mb-8 ml-8">
                <li>along and/or as the primay educator in licensed child care programs for children 3 to 5 years of age, and/or</li>
                <li>alongside an Infant and Toddler Educator in licensed programs of children under 36 months</li>
              </ul>
              <p strong class="mt-3">Specialized certifications allow you to work in licensed child care programs:</p>
              <ul class="mt-2 ml-8">
                <li>along and/or as the primay educator with children birth to 5 years (ITE)</li>
                <li>along and/or as the primay educator in inclusive settings with children 3-5 years of age (SNE)</li>
              </ul>
            </Callout>
          </v-col>
        </v-row>
        <v-row>
          <v-col class="ml-1" cols="12">
            <v-btn v-if="showRequirementsButton" @click="handleRequirementsClick" rounded="lg" color="primary">View requirements</v-btn>
          </v-col>
        </v-row>
      </v-col>
    </v-row>
  </PageContainer>
</template>

<script lang="ts">
import { getCertificationComparisonList } from "@/api/configuration";
import Breadcrumb from "@/components/Breadcrumb.vue";
import PageContainer from "@/components/PageContainer.vue";
import ProfileForm from "@/components/ProfileForm.vue";
import { getHighestCertificationType, CertificationType } from "@/utils/functions";
import { useConfigStore } from "@/store/config";
import { useApplicationStore } from "@/store/application";
import { useRouter } from "vue-router";
import * as Rules from "@/utils/formRules";
import type { Components, Province, ComparisonRecord } from "@/types/openapi";
import Callout from "@/components/Callout.vue";
interface EceTransferData {
  province: Province | undefined;
  outOfProvinceCertification: ComparisonRecord | undefined;
}
export default {
  name: "Transfer",
  components: { ProfileForm, Breadcrumb, PageContainer, Callout },
  setup() {
    const configStore = useConfigStore();
    const router = useRouter();
    const applicationStore = useApplicationStore();
    return {
      configStore,
      router,
      applicationStore,
    };
  },
  methods: {
    async onProvinceChange() {
      this.outOfProvinceCertificationTypesLoading = true;
      this.outOfProvinceCertification = undefined;
      this.has500HoursWorkExperience = undefined;
      this.highestCertificationType = undefined;
      if (this.transferData.province?.provinceId) {
        this.outOfProvinceCertificationTypes = (await getCertificationComparisonList(this.transferData.province?.provinceId)) || [];
        this.outOfProvinceCertificationTypesLoading = false;
      } else {
        this.outOfProvinceCertificationTypes = [];
        this.outOfProvinceCertificationTypesLoading = false;
      }
    },
    async onCertificationChange() {
      this.highestCertificationType = undefined;
      this.has500HoursWorkExperience = undefined;
      if (this.transferData.outOfProvinceCertification?.options) {
        this.highestCertificationType = getHighestCertificationType(this.transferData.outOfProvinceCertification?.options);
      }
    },
    handleRequirementsClick() {
      console.log(this.highestCertificationType);
      console.log(this.selfAssessmentOutcome);
      this.applicationStore.$patch({ draftApplication: { certificationTypes: this.selfAssessmentOutcome, applicationType: "LaborMobility" } });
      this.router.push({ name: "application-requirements" });
    },
  },
  computed: {
    transferData(): EceTransferData {
      return {
        province: this.province,
        outOfProvinceCertification: this.outOfProvinceCertification,
      };
    },
    selfAssessmentOutcome(): Components.Schemas.CertificationType[] {
      if (this.highestCertificationType === CertificationType.Assistant) {
        return ["EceAssistant"];
      }
      if (
        this.highestCertificationType === CertificationType.OneYear ||
        (this.highestCertificationType === CertificationType.FiveYearCertificate && this.has500HoursWorkExperience === "false") ||
        (this.highestCertificationType === CertificationType.FiveYearCertificateITE_SNE && this.has500HoursWorkExperience === "false")
      ) {
        return ["OneYear"];
      }
      if (this.highestCertificationType === CertificationType.FiveYearCertificate && this.has500HoursWorkExperience === "true") {
        return ["FiveYears"];
      }
      if (this.highestCertificationType === CertificationType.FiveYearCertificateITE_SNE && this.has500HoursWorkExperience === "true") {
        return ["FiveYears", "Ite", "Sne"];
      }
      return [];
    },
    showRequirementsButton() {
      return this.highestCertificationType === CertificationType.Assistant || this.has500HoursWorkExperience;
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
        title: "Check your transfer eligibility",
        disabled: true,
        href: "/transfer",
      },
    ],
    outOfProvinceCertificationTypesLoading: false,
    has500HoursWorkExperience: undefined,
    highestCertificationType: undefined as CertificationType | undefined,
    outOfProvinceCertificationTypes: [] as Components.Schemas.ComparisonRecord[],
    Rules,
    province: undefined,
    outOfProvinceCertification: undefined,
    CertificationType,
  }),
};
</script>
