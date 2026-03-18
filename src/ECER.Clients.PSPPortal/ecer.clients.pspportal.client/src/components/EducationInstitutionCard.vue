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
          View Institution Details
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
      if (!this.educationInstitution.institutionType) return "—";
      const institutionTypeMap: Record<
        Components.Schemas.PsiInstitutionType,
        string
      > = {
        Public: "Public",
        Private: "Private",
        PublicOOP: "Public — OOP",
        ContinuingEducation: "Continuing Education",
      };
      return (
        institutionTypeMap[this.educationInstitution.institutionType] ||
        this.educationInstitution.institutionType
      );
    },
    formattedPrivateAuspiceType(): string {
      if (!this.educationInstitution.privateAuspiceType) return "—";
      const privateAuspiceTypeMap: Record<
        Components.Schemas.PrivateAuspiceType,
        string
      > = {
        Theologicalinstitution: "Theological",
        FirstNationsmandatedpostsecondaryinstitute:
          "First Nations mandated post-secondary institute",
        Other: "Other",
        Privatetraininginstitution: "Private training institution",
        Indigenouscontrolledpostsecondaryinstitute:
          "Indigenous controlled post-secondary institute",
      };
      return (
        privateAuspiceTypeMap[this.educationInstitution.privateAuspiceType] ||
        this.educationInstitution.privateAuspiceType
      );
    },
    formattedAddress(): string {
      const parts: string[] = [];
      if (this.educationInstitution.street1)
        parts.push(this.educationInstitution.street1);
      if (this.educationInstitution.street2)
        parts.push(this.educationInstitution.street2);
      if (this.educationInstitution.street3)
        parts.push(this.educationInstitution.street3);

      const cityParts: string[] = [];
      if (this.educationInstitution.city)
        cityParts.push(this.educationInstitution.city);
      if (this.educationInstitution.province)
        cityParts.push(this.educationInstitution.province);
      if (this.educationInstitution.postalCode)
        cityParts.push(this.educationInstitution.postalCode);

      if (cityParts.length > 0) {
        parts.push(cityParts.join(", "));
      }

      return parts.length > 0 ? parts.join(", ") : "—";
    },
  },
});
</script>
