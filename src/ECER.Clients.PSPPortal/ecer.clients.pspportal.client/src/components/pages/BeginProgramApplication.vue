<template>
  <BeginPostBasicApplication
    v-if="
      applicationType === programApplicationType.NewBasicECEPostBasicProgram
    "
    @create-application="createApplication"
  />
  <BeginNewCampusApplication
    v-if="
      applicationType ===
      programApplicationType.NewCampusatRecognizedPrivateInstitution
    "
    :campusId="campusId || ''"
    @create-application="createApplication"
  />
</template>
<script lang="ts">
import { defineComponent } from "vue";
import { createProgramApplication } from "@/api/program-application";
import type { Components } from "@/types/openapi";
import * as Rules from "@/utils/formRules";
import { useLoadingStore } from "@/store/loading";
import BeginPostBasicApplication from "@/components/common/BeginPostBasicApplication.vue";
import BeginNewCampusApplication from "@/components/common/BeginNewCampusApplication.vue";
import { ProgramApplicationType } from "@/utils/constant";
import type { CreateApplication } from "@/types/helperFunctions";

export default defineComponent({
  name: "BeginProgramApplication",
  components: {
    BeginPostBasicApplication,
    BeginNewCampusApplication,
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
    const loadingStore = useLoadingStore();
    return { loadingStore };
  },
  computed: {
    programApplicationType() {
      return ProgramApplicationType;
    },
  },
  data() {
    return {
      programName: "" as string,
      provincialCertificationTypeValues: [] as string[],
      programConfirmationValue: "" as string,
      Rules,
    };
  },
  methods: {
    async createApplication(createApplicationObject: CreateApplication) {
      const request: Components.Schemas.CreateProgramApplicationRequest = {
        programApplicationName: createApplicationObject.programName,
        programTypes:
          createApplicationObject.provincialCertificationTypeValues as Components.Schemas.ProgramCertificationType[],
        deliveryType: createApplicationObject.deliveryType as
          | Components.Schemas.DeliveryType
          | undefined,
        programApplicationType: createApplicationObject.applicationType,
      };

      const { data, error } = await createProgramApplication(request);

      if (error) {
        return;
      }
      if (!data?.programApplication?.id) {
        return;
      }

      this.$router.push({
        name: "programApplication",
        params: { programApplicationId: data.programApplication.id },
      });
    },
  },
});
</script>
