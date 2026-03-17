<template>
  <Card>
    <div class="d-flex flex-column h-100">
      <div class="flex-grow-1">
        <v-row align="center" justify="space-between">
          <v-col cols="auto">
            <h3>{{ campus.generatedName }}</h3>
          </v-col>
          <v-col cols="auto">
            <v-chip :color="chipColour" variant="flat" size="small">
              {{ campus.status }}
            </v-chip>
          </v-col>
        </v-row>

        <div class="mt-2">
          <div v-if="campus.street1">{{ campus.street1 }}</div>
          <div v-if="campus.street2">{{ campus.street2 }}</div>
          <div v-if="campus.street3">{{ campus.street3 }}</div>
          <div v-if="campus.city || campus.province">
            {{ [campus.city, campus.province].filter(Boolean).join(", ") }}
          </div>
          <div v-if="campus.postalCode">{{ campus.postalCode }}</div>
        </div>
      </div>

      <div class="mt-auto pt-6">
        <v-btn
          variant="outlined"
          color="primary"
          @click="$emit('view', campus)"
        >
          View
        </v-btn>
      </div>
    </div>
  </Card>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";
import Card from "@/components/Card.vue";
import type { Components } from "@/types/openapi";

export default defineComponent({
  name: "CampusCard",
  components: { Card },
  props: {
    campus: {
      type: Object as PropType<Components.Schemas.Campus>,
      required: true,
    },
  },
  emits: ["view"],
  computed: {
    chipColour(): string {
      return this.campus.status === "Active" ? "success" : "grey-darkest";
    },
  },
});
</script>
