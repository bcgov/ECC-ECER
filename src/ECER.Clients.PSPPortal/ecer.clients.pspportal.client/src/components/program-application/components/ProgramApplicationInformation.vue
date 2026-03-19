<template>
  <ProgramApplicationInfo
    v-if="
      applicationType === programApplicationType.NewBasicECEPostBasicProgram
    "
  />
  <NewCampusProgramApplicationInfo
    v-if="
      applicationType ===
      programApplicationType.NewCampusatRecognizedPrivateInstitution
    "
  />
  <v-row>
    <v-col>
      <v-btn rounded="lg" color="primary" @click="$emit('next', {})">
        Continue
      </v-btn>
    </v-col>
  </v-row>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import Callout from "@/components/common/Callout.vue";
import ECEHeader from "@/components/ECEHeader.vue";
import PageContainer from "@/components/PageContainer.vue";
import Breadcrumb from "@/components/Breadcrumb.vue";
import ProgramApplicationInfo from "@/components/common/ProgramApplicationInfo.vue";
import type { NextStepPayload } from "@/components/program-application/ProgramApplication.vue";
import { ProgramApplicationType } from "@/utils/constant";

export default defineComponent({
  name: "ProgramApplicationInformation",
  components: {
    ProgramApplicationInfo,
    Breadcrumb,
    ECEHeader,
    Callout,
    PageContainer,
  },
  props: {
    programApplicationId: {
      type: String,
      required: false,
    },
    applicationType: {
      type: String,
      required: true,
    },
  },
  computed: {
    programApplicationType() {
      return ProgramApplicationType;
    },
  },
  emits: { next: (_payload: NextStepPayload) => true },
});
</script>
