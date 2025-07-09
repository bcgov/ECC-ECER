<template>
  <!-- Ece Assistant -->
  <v-row v-if="showEceAssistantPathway">
    <v-col cols="12" xlg="6">
      <Card :has-top-border="true" top-border-size="small">
        <div class="d-flex flex-column ga-5">
          <h2>Application</h2>
          <p class="large">ECE Assistant</p>
          <p>
            This certificate allows you to work alongside an ECE if you do not have the requirements (e.g., full educational program, work experience,
            professional development, etc.) for higher certification levels. It is valid for 5 years.
          </p>
        </div>
        <v-btn variant="flat" size="large" color="primary" id="btnApplyNowEceAssistant" class="mt-12" @click="$emit('apply-now', ['EceAssistant'])">
          Apply now
        </v-btn>
      </Card>
    </v-col>
  </v-row>

  <!-- Ece One Year -->
  <v-row v-if="showEceOneYearPathway">
    <v-col cols="12" xlg="6">
      <Card :has-top-border="true" top-border-size="small">
        <div class="d-flex flex-column ga-5">
          <h2>Application</h2>
          <p class="large">ECE One Year</p>
          <p>This certification allows you to work and complete work experience requirements for ECE 5 YR certification. It is valid for 1 year.</p>
        </div>
        <v-btn variant="flat" size="large" color="primary" id="btnApplyNowEceOneYear" class="mt-12" @click="$emit('apply-now', ['OneYear'])">Apply now</v-btn>
      </Card>
    </v-col>
  </v-row>

  <!-- Ece One Year (edge case) -->
  <v-row v-if="showEceOneYearEdgeCasePathway">
    <v-col cols="12" xlg="6">
      <Card :has-top-border="true" top-border-size="small">
        <div class="d-flex flex-column ga-5">
          <h2>Application</h2>
          <p class="large">ECE One Year</p>
          <p>
            This certification allows you to complete renewal requirements for the ECE 5 YR if you were unable to do so during the term of your certificate. It
            is valid for 1 year.
          </p>
        </div>
        <v-btn variant="flat" size="large" color="primary" id="btnApplyNowEceOneYearEdgeCase" class="mt-12" @click="$emit('apply-now', ['OneYear'])">
          Apply now
        </v-btn>
      </Card>
    </v-col>
  </v-row>

  <!-- ECE 5 YR -->
  <v-row v-if="showEceFiveYearPathway">
    <v-col cols="12" xlg="6">
      <Card :has-top-border="true" top-border-size="small">
        <div class="d-flex flex-column ga-5">
          <h2>Application</h2>
          <p class="large">
            ECE 5 YR
            <br />
            + Infant and Toddler Educator and Special Needs Educator
          </p>
          <p>This is the highest level of certification in B.C. and allows you to work alone and/or as the primary educator. It is valid for 5 years. </p>
        </div>
        <v-btn variant="flat" size="large" color="primary" id="btnApplyNowEceFiveYear" class="mt-12" @click="$emit('apply-now', ['FiveYears'])">
          Apply now
        </v-btn>
      </Card>
    </v-col>
  </v-row>

  <!-- Specialized certification (ITE and SNE) -->
  <v-row v-if="showSpecializedCertificationPathway">
    <v-col cols="12" xlg="6">
      <Card :has-top-border="true" top-border-size="small">
        <div class="d-flex flex-column ga-5">
          <h2>Application</h2>
          <p class="large">Specialized certification</p>
          <p>
            This certification adds specializations to your ECE 5 YR certificate that allow you to work with children with different needs. It will also renew
            your ECE 5 YR certificate. It is valid for 5 years.
          </p>
        </div>
        <!-- This is the ITE + SNE upgrade pathway. If user doesn't have a certification type, it will trigger this pathway -->
        <v-btn variant="flat" size="large" color="primary" id="btnApplyNowIteSne" class="mt-12" @click="$emit('apply-now', [])">Apply now</v-btn>
      </Card>
    </v-col>
  </v-row>

  <!-- ITE -->
  <v-row v-if="showItePathway">
    <v-col cols="12" xlg="6">
      <Card :has-top-border="true" top-border-size="small">
        <div class="d-flex flex-column ga-5">
          <h2>Application</h2>
          <p class="large">Infant and Toddler Educator</p>
          <p>
            This certification allows you to to work alone and/or as the primary educator in licensed child care programs for children birth to 5 years of age.
            It will also renew your ECE 5 YR certificate. It is valid for 5 years.
          </p>
        </div>
        <v-btn variant="flat" size="large" color="primary" id="btnApplyNowIte" class="mt-12" @click="$emit('apply-now', ['Ite'])">Apply now</v-btn>
      </Card>
    </v-col>
  </v-row>

  <!-- SNE -->
  <v-row v-if="showSnePathway">
    <v-col cols="12" xlg="6">
      <Card :has-top-border="true" top-border-size="small">
        <div class="d-flex flex-column ga-5">
          <h2>Application</h2>
          <p class="large">Special Needs Educator</p>
          <p>
            This certificate allows you to work alone and/or as the primary educator in inclusive, licensed child care programs for children 3 to 5 years of
            age. It will also renew your ECE 5 YR certificate. It is valid for 5 years.
          </p>
        </div>
        <v-btn variant="flat" size="large" color="primary" id="btnApplyNowSne" class="mt-12" @click="$emit('apply-now', ['Sne'])">Apply now</v-btn>
      </Card>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";
import Card from "@/components/Card.vue";
import type { Components } from "@/types/openapi";
import { useCertificationStore } from "@/store/certification";

export default defineComponent({
  name: "ApplicationCardList",
  components: {
    Card,
  },
  setup() {
    const certificationStore = useCertificationStore();
    return {
      certificationStore,
    };
  },
  emits: {
    "apply-now": (type: Components.Schemas.CertificationType[]) => true,
  },
  props: {
    certifications: {
      type: Array as PropType<Components.Schemas.Certification[]>,
      default: () => [],
    },
  },
  methods: {
    hasSne(activeFiveYearCertifications: Components.Schemas.Certification[]) {
      return activeFiveYearCertifications.some((certification) => certification.levels?.some((level) => level.type === "SNE"));
    },
    hasIte(activeFiveYearCertifications: Components.Schemas.Certification[]) {
      return activeFiveYearCertifications.some((certification) => certification.levels?.some((level) => level.type === "ITE"));
    },
    hasBothIteAndSne(activeFiveYearCertifications: Components.Schemas.Certification[]) {
      return activeFiveYearCertifications.some(
        (certification) => certification.levels?.some((level) => level.type === "ITE") && certification.levels?.some((level) => level.type === "SNE"),
      );
    },
    getActiveFiveYearCertifications() {
      return this.certifications.filter(
        (certification) =>
          certification.levels?.some((level) => level.type === "ECE 5 YR") &&
          (certification.statusCode === "Active" || certification.statusCode === "Suspended"),
      );
    },
  },
  computed: {
    showEceAssistantPathway() {
      // If the user does not have ECE assistant, or all the ECE assistant certifications have been expired for more than 5 years, show the ECE assistant pathway
      const eceAssistantCertifications = this.certifications.filter((certification) => certification.levels?.some((level) => level.type === "Assistant"));

      return (
        eceAssistantCertifications.length === 0 ||
        eceAssistantCertifications.every((certification) => this.certificationStore.expiredMoreThan5Years(certification))
      );
    },
    showEceOneYearPathway() {
      // If the user does not have ECE one year, or all the ECE one year certifications have been expired for more than 5 years, show the ECE one year pathway
      const eceOneYearCertifications = this.certifications.filter((certification) => certification.levels?.some((level) => level.type === "ECE 1 YR"));

      return (
        !this.showEceOneYearEdgeCasePathway &&
        (eceOneYearCertifications.length === 0 ||
          eceOneYearCertifications.every((certification) => this.certificationStore.expiredMoreThan5Years(certification)))
      );
    },
    showEceOneYearEdgeCasePathway() {
      // If the user has an expired ECE 5 YR certification, does not have an active ECE 1 YR certification, and is not showing standard ECE one year pathway, show the ECE one year edge case pathway
      const eceFiveYearCertifications = this.certifications.filter((certification) => certification.levels?.some((level) => level.type === "ECE 5 YR"));
      const eceOneYearCertifications = this.certifications.filter((certification) => certification.levels?.some((level) => level.type === "ECE 1 YR"));

      return (
        eceFiveYearCertifications.length > 0 &&
        eceFiveYearCertifications.every((certification) => certification.statusCode === "Expired") &&
        (eceOneYearCertifications.length === 0 ||
          eceOneYearCertifications.every((certification) => this.certificationStore.expiredMoreThan5Years(certification)))
      );
    },
    showEceFiveYearPathway() {
      // If the user does not have ECE 5 YR, show the ECE 5 YR pathway
      return !this.certifications.some((certification) => certification.levels?.some((level) => level.type === "ECE 5 YR"));
    },
    showSpecializedCertificationPathway() {
      // If the user has an active ECE 5 YR certification, without holding ITE or SNE, show the specialized certification pathway
      const eceFiveYearCertifications = this.getActiveFiveYearCertifications();

      return (
        eceFiveYearCertifications.length > 0 &&
        !this.hasBothIteAndSne(eceFiveYearCertifications) &&
        !this.hasSne(eceFiveYearCertifications) &&
        !this.hasIte(eceFiveYearCertifications)
      );
    },
    showItePathway(): boolean {
      // If the user has an active ECE 5 YR certification, has SNE but does not have ITE, show the ITE pathway
      const eceFiveYearCertifications = this.getActiveFiveYearCertifications();

      return (
        eceFiveYearCertifications.length > 0 &&
        !this.hasBothIteAndSne(eceFiveYearCertifications) &&
        this.hasSne(eceFiveYearCertifications) &&
        !this.hasIte(eceFiveYearCertifications)
      );
    },
    showSnePathway(): boolean {
      // If the user has an active ECE 5 YR certification, has ITE but does not have SNE, show the SNE pathway
      const eceFiveYearCertifications = this.getActiveFiveYearCertifications();

      return (
        eceFiveYearCertifications.length > 0 &&
        !this.hasBothIteAndSne(eceFiveYearCertifications) &&
        !this.hasSne(eceFiveYearCertifications) &&
        this.hasIte(eceFiveYearCertifications)
      );
    },
  },
});
</script>
