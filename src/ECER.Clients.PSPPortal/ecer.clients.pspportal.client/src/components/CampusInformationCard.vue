<template>
  <Card>
    <v-row align="center" justify="space-between">
      <v-col cols="auto">
        <h2>{{ campus.name }}</h2>
      </v-col>
      <v-col cols="auto">
        <v-tooltip text="Edit Campus Information" location="top">
          <template #activator="{ props }">
            <v-btn
              v-bind="props"
              icon="mdi-pencil"
              variant="plain"
              @click="
                router.push(
                  `/education-institution/${institutionId}/campus/${campus.id}/edit`,
                )
              "
            />
          </template>
        </v-tooltip>
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12" sm="3">
        <span>Temporary or satellite location?</span>
      </v-col>
      <v-col cols="12" sm="9">
        <span class="font-weight-bold">
          {{ campus.isSatelliteOrTemporaryLocation ? "Yes" : "No" }}
        </span>
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12" sm="3">
        <span>Status</span>
      </v-col>
      <v-col cols="12" sm="9">
        <span class="font-weight-bold">{{ campus.status ?? "—" }}</span>
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12" sm="3">
        <span>Campus name</span>
      </v-col>
      <v-col cols="12" sm="9">
        <span class="font-weight-bold">{{ campus.name ?? "—" }}</span>
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12" sm="3">
        <span>Address</span>
      </v-col>
      <v-col cols="12" sm="9">
        <span class="font-weight-bold">{{ formattedAddress }}</span>
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12" sm="3">
        <span>Key campus contact</span>
      </v-col>
      <v-col cols="12" sm="9">
        <span class="font-weight-bold">{{ campus.keyCampusContactName }}</span>
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
  name: "CampusInformationCard",
  components: { Card },
  props: {
    campus: {
      type: Object as PropType<Components.Schemas.Campus>,
      required: true,
    },
    institutionId: {
      type: String,
      required: true,
    },
  },
  setup() {
    const router = useRouter();
    return { router };
  },
  computed: {
    keyCampusContactName(): string {
      return this.campus.keyCampusContactName ?? "—";
    },
    formattedAddress(): string {
      const parts: string[] = [];
      if (this.campus.street1) parts.push(this.campus.street1);
      if (this.campus.street2) parts.push(this.campus.street2);
      if (this.campus.street3) parts.push(this.campus.street3);

      const cityParts: string[] = [];
      if (this.campus.city) cityParts.push(this.campus.city);
      if (this.campus.province) cityParts.push(this.campus.province);
      if (this.campus.postalCode) cityParts.push(this.campus.postalCode);

      if (cityParts.length > 0) {
        parts.push(cityParts.join(", "));
      }

      return parts.length > 0 ? parts.join(", ") : "—";
    },
  },
});
</script>
