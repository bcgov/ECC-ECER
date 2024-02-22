<template>
  <v-card min-width="100%" variant="elevated" rounded="lg" class="custom-card-styling" elevation="4">
    <template #title>
      <v-card-title>
        <p class="font-weight-bold">{{ education.school }}</p>
      </v-card-title>
    </template>
    <template #append>
      <v-btn icon="mdi-pencil" variant="plain" />
      <v-btn icon="mdi-trash-can-outline" variant="plain" />
    </template>
  </v-card>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "EducationCard",
  props: {
    education: {
      type: Object as () => Education,
      required: true,
    },
  },
});
</script>

<style scoped lang="scss">
.custom-card-styling {
  border: 1px solid #adb5bd;
  padding: 0.5rem;
}
</style>
