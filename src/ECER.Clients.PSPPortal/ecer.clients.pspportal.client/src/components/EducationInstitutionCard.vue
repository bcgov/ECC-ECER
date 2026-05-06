<template>
  <Card>
    <v-row align="center" justify="space-between">
      <v-col cols="auto">
        <h2>{{ educationInstitution.name }}</h2>
      </v-col>
      <v-col cols="auto">
        <v-tooltip text="Edit Institution Information" location="top">
          <template #activator="{ props }">
            <v-btn
              v-bind="props"
              icon="mdi-pencil"
              variant="plain"
              @click="router.push({ name: 'edit-education-institution' })"
            />
          </template>
        </v-tooltip>
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12" sm="3">
        <span>Institution type</span>
      </v-col>
      <v-col cols="12" sm="9">
        <span class="font-weight-bold">{{ formattedInstitutionType }}</span>
      </v-col>
    </v-row>

    <v-row v-if="educationInstitution.institutionType === 'Private'">
      <v-col cols="12" sm="3">
        <span>Private institution type</span>
      </v-col>
      <v-col cols="12" sm="9">
        <span class="font-weight-bold">{{ formattedPrivateAuspiceType }}</span>
      </v-col>
    </v-row>

    <v-row v-if="educationInstitution.ptiruInstitutionId">
      <v-col cols="12" sm="3">
        <span>PTIRU identification number</span>
      </v-col>
      <v-col cols="12" sm="9">
        <span class="font-weight-bold">
          {{ educationInstitution.ptiruInstitutionId }}
        </span>
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12" sm="3">
        <span>Address:</span>
      </v-col>
      <v-col cols="12" sm="9">
        <span class="font-weight-bold">{{ formattedAddress }}</span>
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12" sm="3">
        <span>Website:</span>
      </v-col>
      <v-col cols="12" sm="9">
        <a
          v-if="educationInstitution.websiteUrl"
          :href="educationInstitution.websiteUrl"
          target="_blank"
          class="text-links font-weight-bold"
        >
          {{ educationInstitution.websiteUrl }}
        </a>
        <span v-else class="font-weight-bold">—</span>
      </v-col>
    </v-row>

    <v-row v-if="!hideViewButton" class="mt-4">
      <v-col>
        <v-btn
          variant="outlined"
          size="large"
          class="mt-4"
          color="primary"
          id="btnEducationInstitution"
          @click="
            router.push({
              name: 'education-institution',
              params: { institutionId: educationInstitution.id },
            })
          "
        >
          View institution details
        </v-btn>
      </v-col>
    </v-row>
  </Card>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";
import Card from "@/components/Card.vue";
import type { Components } from "@/types/openapi";
import { useRouter } from "vue-router";
import {
  formatAddress,
  formatInstitutionType,
  formatPrivateAuspiceType,
} from "@/utils/format";

export default defineComponent({
  name: "EducationInstitutionCard",
  components: { Card },
  props: {
    educationInstitution: {
      type: Object as PropType<Components.Schemas.EducationInstitution>,
      required: true,
    },
    hideViewButton: {
      type: Boolean,
      default: false,
    },
  },
  setup() {
    const router = useRouter();
    return { router };
  },
  computed: {
    formattedInstitutionType(): string {
      return formatInstitutionType(this.educationInstitution.institutionType);
    },
    formattedPrivateAuspiceType(): string {
      return formatPrivateAuspiceType(
        this.educationInstitution.privateAuspiceType,
      );
    },
    formattedAddress(): string {
      return formatAddress(this.educationInstitution);
    },
  },
});
</script>
