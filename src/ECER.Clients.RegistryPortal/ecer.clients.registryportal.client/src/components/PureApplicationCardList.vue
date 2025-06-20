<template>
  <v-row v-if="!hasEceAssistantCertification">
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
        <v-btn variant="flat" size="large" color="primary" id="btnApplyNowEceAssistant" class="mt-12" @click="$emit('apply-now', 'EceAssistant')">
          Apply now
        </v-btn>
      </Card>
    </v-col>
  </v-row>
  <v-row v-if="!hasEceOneYearCertification">
    <v-col cols="12" xlg="6">
      <Card :has-top-border="true" top-border-size="small">
        <div class="d-flex flex-column ga-5">
          <h2>Application</h2>
          <p class="large">ECE One Year</p>
          <p>This certification allows you to work and complete work experience requirements for ECE Five Year certification. It is valid for 1 year.</p>
        </div>
        <v-btn variant="flat" size="large" color="primary" id="btnApplyNowEceAssistant" class="mt-12" @click="$emit('apply-now', 'OneYear')">Apply now</v-btn>
      </Card>
    </v-col>
  </v-row>
  <v-row v-if="!hasEceFiveYearCertification">
    <v-col cols="12" xlg="6">
      <Card :has-top-border="true" top-border-size="small">
        <div class="d-flex flex-column ga-5">
          <h2>Application</h2>
          <p class="large">
            ECE Five Year
            <br />
            + Infant and Toddler Educator and Special Needs Educator
          </p>
          <p>This is the highest level of certification in B.C. and allows you to work alone and/or as the primary educator. It is valid for 5 years. </p>
        </div>
        <v-btn variant="flat" size="large" color="primary" id="btnApplyNowEceAssistant" class="mt-12" @click="$emit('apply-now', 'FiveYears')">Apply now</v-btn>
      </Card>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import Card from "@/components/Card.vue";
import type { Components } from "@/types/openapi";

export default defineComponent({
  name: "PureApplicationCardList",
  components: {
    Card,
  },
  emits: {
    "apply-now": (type: Components.Schemas.CertificationType) => true,
  },
  props: {
    hasEceAssistantCertification: {
      type: Boolean,
      default: false,
    },
    hasEceOneYearCertification: {
      type: Boolean,
      default: false,
    },
    hasEceFiveYearCertification: {
      type: Boolean,
      default: false,
    },
  },
  computed: {},
});
</script>
