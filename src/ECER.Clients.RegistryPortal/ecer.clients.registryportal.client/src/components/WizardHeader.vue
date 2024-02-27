<template>
  <div class="bg-white">
    <v-row>
      <v-col cols="12">
        <v-breadcrumbs :items="items" color="primary">
          <template #divider>
            <v-icon icon="mdi-chevron-right" size="sm"></v-icon>
          </template>
        </v-breadcrumbs>
      </v-col>
    </v-row>
    <v-row justify="space-between">
      <v-col offset="1" cols="12" sm="8">
        <h3>Application for ECE Assistant Certification</h3>
      </v-col>
      <v-col v-if="false" cols="auto" offset="1">
        <v-btn class="mr-2" rounded="lg" variant="outlined" color="primary">Cancel Application</v-btn>
      </v-col>
    </v-row>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "WizardHeader",
  data: () => ({
    items: [
      {
        title: "Dashboard",
        disabled: false,
        href: "/",
      },

      {
        title: "Apply New Certification",
        disabled: true,
        href: "application",
      },
    ],
  }),
});
</script>
