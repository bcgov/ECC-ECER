<template>
  <PageContainer>
    <v-row>
      <v-col cols="12">
        <Breadcrumb />
      </v-col>
    </v-row>
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
        <v-btn
          rounded="lg"
          color="primary"
          @click="
            router.push({
              name: 'program-application-begin',
              params: { applicationType: applicationType, campusId: campusId },
            })
          "
        >
          Continue
        </v-btn>
      </v-col>
    </v-row>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import Callout from "@/components/common/Callout.vue";
import ECEHeader from "@/components/ECEHeader.vue";
import PageContainer from "@/components/PageContainer.vue";
import Breadcrumb from "@/components/Breadcrumb.vue";
import { useRouter } from "vue-router";
import ProgramApplicationInfo from "@/components/common/ProgramApplicationInfo.vue";
import { ProgramApplicationType } from "@/utils/constant";
import NewCampusProgramApplicationInfo from "@/components/common/NewCampusProgramApplicationInfo.vue";

export default defineComponent({
  name: "ProgramApplicationInfoContainer",
  components: {
    ProgramApplicationInfo,
    Breadcrumb,
    ECEHeader,
    Callout,
    PageContainer,
    NewCampusProgramApplicationInfo,
  },
  props: {
    applicationType: {
      type: String,
      required: true,
    },
    campusId: {
      type: String,
      required: false,
    },
  },
  setup() {
    const router = useRouter();

    return {
      router,
    };
  },
  computed: {
    programApplicationType() {
      return ProgramApplicationType;
    },
  },
});
</script>
