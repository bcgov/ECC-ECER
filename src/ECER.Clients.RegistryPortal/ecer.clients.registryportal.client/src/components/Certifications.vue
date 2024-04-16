<template>
  <div>
    <div v-if="applications" class="content">
      <table>
        <thead>
          <tr>
            <th>Application ID</th>
            <th>Application Status</th>
            <th>Date submitted</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="application in applications" :key="application.submittedOn ? application.submittedOn : 'defaultKey'">
            <td>{{ application.id }}</td>
            <td>{{ application.status }}</td>
            <td>{{ application.submittedOn }}</td>
          </tr>
        </tbody>
      </table>
    </div>
  </div>
</template>

<script lang="ts">
import { mapState } from "pinia";
import { defineComponent } from "vue";

import { useApplicationStore } from "@/store/application";

export default defineComponent({
  name: "Certifications",
  setup() {
    const applicationStore = useApplicationStore();
    return { applicationStore };
  },
  computed: {
    ...mapState(useApplicationStore, ["applications"]),
  },
});
</script>
