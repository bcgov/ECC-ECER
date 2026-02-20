<template>
  <v-container fluid>
    <v-list>
      <div
        v-for="[category, componentGroups] in groupByCategoryName"
        :key="category"
      >
        <v-list-group>
          <template #activator="{ props }">
            <v-list-item v-bind="props">
              <v-list-item-title
                class="ml-4 text-support-border-info font-weight-bold"
                v-text="category"
              />
            </v-list-item>
          </template>
          <v-list-item
            v-for="componentGroup in componentGroups"
            :key="componentGroup.id"
          >
            <router-link
              :to="
                '/program-application/' +
                programApplicationId +
                componentGroup.navigationRoute
              "
            >
              <v-list-item class="nav-item">
                <v-list-item-title>
                  <v-icon color="success">
                    {{ componentGroup.statusIcon }}
                  </v-icon>
                  {{ componentGroup.name }}
                </v-list-item-title>
              </v-list-item>
            </router-link>
          </v-list-item>
        </v-list-group>
      </div>
    </v-list>
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { getComponentGroupMetadata } from "@/api/program-application";
import type { Components } from "@/types/openapi";
import { groupByCategoryName } from "@/utils/functions";

export default defineComponent({
  name: "ComponentGroupNavigation",
  props: {
    programApplicationId: {
      type: String,
      required: true,
    },
  },
  computed: {
    groupByCategoryName(): ComponentGroupNavigationMap | undefined {
      return groupByCategoryName(this.componentGroups);
    },
  },
  data() {
    return {
      componentGroups: [] as Components.Schemas.ComponentGroupMetadata[],
    };
  },
  created() {},
  mounted() {
    this.getComponentGroups();
  },
  unmounted() {},
  methods: {
    async getComponentGroups() {
      const response = await getComponentGroupMetadata(
        this.programApplicationId,
      );
      this.componentGroups = response.data || [];
    },
  },
});
</script>
<style scope>
.nav-item {
  color: black;
}

a {
  text-decoration: none;
}

a:hover {
  cursor: pointer;
}
</style>
