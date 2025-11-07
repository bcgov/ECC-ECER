<template>
  <PreviewCard title="Professional development" portal-stage="ProfessionalDevelopment">
    <template #content>
      <div v-for="(professionalDevelopment, index) in professionalDevelopments" :key="professionalDevelopment.id!">
        <v-divider v-if="index !== 0" :thickness="2" color="grey-lightest" class="border-opacity-100 my-6" />
        <v-row>
          <v-col>
            <h4 class="text-black">Name of course or workshop</h4>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="4">
            <p class="small">How many hours was it</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">{{ `${professionalDevelopment.numberOfHours} hours` }}</p>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="4">
            <p class="small">Name of place that hosted the course or workshop</p>
          </v-col>
          <v-col>
            <p id="courseProvince" class="small font-weight-bold">{{ professionalDevelopment.organizationName }}</p>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="4">
            <p class="small">Website with description of course or workshop (optional)</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">{{ professionalDevelopment.courseorWorkshopLink || "—" }}</p>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="4">
            <p class="small">When did you take it?</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">
              {{
                `${formatDate(professionalDevelopment.startDate || "", "LLLL d, yyyy")} to ${formatDate(professionalDevelopment.endDate || "", "LLLL d, yyyy")}`
              }}
            </p>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="4">
            <p class="small">Instructor name</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">
              {{ professionalDevelopment.instructorName || "—" }}
            </p>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="4">
            <p class="small">Phone number</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">
              {{ professionalDevelopment.organizationContactInformation || "—" }}
            </p>
          </v-col>
        </v-row>
        <v-row>
          <v-col cols="4">
            <p class="small">Certificate of document that shows you completed the course</p>
          </v-col>
          <v-col>
            <v-row no-gutters>
              <v-col v-for="(file, childIndex) in professionalDevelopment.files" :key="childIndex" cols="12" class="small font-weight-bold">
                {{ file.name }}
              </v-col>
            </v-row>
          </v-col>
        </v-row>
      </div>
    </template>
  </PreviewCard>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import PreviewCard from "@/components/PreviewCard.vue";
import { useWizardStore } from "@/store/wizard";
import type { Components } from "@/types/openapi";
import { formatDate } from "@/utils/format";

export default defineComponent({
  name: "EceProfessionalDevelopmentPreview",
  components: {
    PreviewCard,
  },

  setup: () => {
    const wizardStore = useWizardStore();

    return {
      wizardStore,
      formatDate,
    };
  },
  computed: {
    professionalDevelopments(): Components.Schemas.ProfessionalDevelopment[] {
      return this.wizardStore.wizardData[this.wizardStore?.wizardConfig?.steps?.professionalDevelopments?.form?.inputs?.professionalDevelopments?.id || ""];
    },
  },
});
</script>
