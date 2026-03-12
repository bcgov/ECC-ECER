<template>
  <v-container fluid class="pl-0 d-print-none">
    <v-list color="primary">
      <v-list-item
        :to="{
          name: 'program-application-component-info',
          params: { programApplicationId: programApplicationId },
        }"
      >
        <v-list-item-title>
          <v-icon>mdi-text-box-outline</v-icon>
          Program application info
        </v-list-item-title>
      </v-list-item>
      <div
        v-for="[category, componentGroups] in groupByCategoryName"
        :key="category"
        v-if="applicationStatus === 'Draft'"
      >
        <v-list-item
          v-if="category === 'Institute Info'"
          :to="{
            name: 'program-application-institute-info',
            params: { programApplicationId: programApplicationId },
          }"
        >
          <v-list-item-title>
            <v-icon
              :color="mapStatusColor(getNonCategoryStatus(componentGroups))"
            >
              {{ getNonCategoryStatus(componentGroups) }}
            </v-icon>
            Institution and program info
          </v-list-item-title>
        </v-list-item>
        <v-list-group v-else>
          <template #activator="{ props }">
            <v-list-item v-bind="props">
              <v-list-item-title
                class="text-support-border-info font-weight-bold"
              >
                <v-icon :color="mapStatusColor(categoryStatus(category))">
                  {{ categoryStatus(category) }}
                </v-icon>
                {{ category }}
              </v-list-item-title>
            </v-list-item>
          </template>
          <v-list-item
            v-for="componentGroup in componentGroups"
            :key="componentGroup.id"
            :to="
              '/program-application/' +
              programApplicationId +
              componentGroup.navigationRoute
            "
          >
            <v-list-item>
              <v-list-item-title>
                <v-icon :color="mapStatusColor(componentGroup.statusIcon)">
                  {{ componentGroup.statusIcon }}
                </v-icon>
                {{ componentGroup.name }}
              </v-list-item-title>
            </v-list-item>
          </v-list-item>
        </v-list-group>
      </div>

      <v-list-group
        v-if="
          applicationType === 'NewBasicECEPostBasicProgram' &&
          applicationStatus === 'Draft'
        "
        value="Program profile"
      >
        <template #activator="{ props }">
          <v-list-item
            v-bind="props"
            class="text-support-border-info font-weight-bold"
          >
            <v-icon color="black">mdi-table</v-icon>
            Program profile
          </v-list-item>
        </template>

        <v-list-item
          v-for="(type, index) in programTypes"
          :key="`program-type-${type}-${index}`"
        >
          <v-list-item
            :to="{
              name: 'program-application-program-profile-area-of-instruction',
              params: {
                programApplicationId: programApplicationId,
                programType: type,
              },
            }"
          >
            <v-list-item-title>
              <v-icon>mdi-table</v-icon>
              {{ type }}
            </v-list-item-title>
          </v-list-item>
        </v-list-item>
      </v-list-group>

      <v-list-group value="Review and submit">
        <template #activator="{ props }">
          <v-list-item
            v-bind="props"
            class="text-support-border-info font-weight-bold"
          >
            <v-icon color="grey">mdi-circle-outline</v-icon>
            Review and submit
          </v-list-item>
        </template>

        <v-list-item
          :to="{
            name: 'program-application-review-response',
            params: { programApplicationId: programApplicationId },
          }"
        >
          <v-list-item-title>
            <v-icon>mdi-text-box-outline</v-icon>
            Review Responses
          </v-list-item-title>
        </v-list-item>

        <v-list-item
          :to="{
            name: 'program-application-program-profile-area-of-instruction-review',
            params: { programApplicationId: programApplicationId },
          }"
        >
          <v-list-item-title>
            <v-icon>mdi-text-box-outline</v-icon>
            Review program profiles
          </v-list-item-title>
        </v-list-item>

        <v-list-item>
          <v-list-item-title>
            <v-icon color="grey">mdi-circle-outline</v-icon>
            Submit application
          </v-list-item-title>
        </v-list-item>
      </v-list-group>
    </v-list>
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import type { Components } from "@/types/openapi";
import {
  groupByCategoryName,
  mapStatusColor,
  mapStatusIcons,
} from "@/utils/functions";

export default defineComponent({
  name: "ComponentGroupNavigation",
  props: {
    programApplicationId: {
      type: String,
      required: true,
    },
    programTypes: {
      type: Array as () => Components.Schemas.ProgramTypes[] | null | undefined,
      required: true,
    },
    applicationStatus: {
      type: String as () =>
        | Components.Schemas.ApplicationStatus
        | null
        | undefined,
      required: true,
    },
    applicationType: {
      type: String as () =>
        | Components.Schemas.ApplicationType
        | null
        | undefined,
      required: true,
    },
    componentGroups: {
      type: Array as () => Components.Schemas.NavigationMetadata[],
      required: true,
    },
  },
  computed: {
    groupByCategoryName(): ComponentGroupNavigationMap | undefined {
      return groupByCategoryName(this.componentGroups);
    },
  },
  data() {
    return {};
  },
  methods: {
    mapStatusColor,
    mapStatusIcons,
    categoryStatus(key: string) {
      let statuses = groupByCategoryName(this.componentGroups)
        ?.get(key)
        ?.map((group) => group.status);
      if (statuses !== undefined && statuses.length > 0) {
        if (statuses.every((status) => status === "Completed")) {
          return "mdi-check-circle";
        }
        if (statuses.every((status) => status === "ToDo" || status === null)) {
          return "mdi-circle-outline";
        }
        return "mdi-circle-half-full";
      }
      return "mdi-circle-outline";
    },
    getNonCategoryStatus(data: ComponentGroupNavigation[]): string {
      if (data[0] !== undefined && data.length !== 0 && data.length === 1) {
        return mapStatusIcons(data[0].status);
      }
      return mapStatusIcons("ToDo");
    },
  },
});
</script>
<style scope>
a {
  text-decoration: none;
  color: black !important;
}

a:hover {
  cursor: pointer;
}

.v-list-item--active {
  background-color: light-blue !important;
  border-radius: 8px;
}
</style>
