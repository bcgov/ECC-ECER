<template>
  <div>
    <h1>Certifications</h1>

    <div v-if="applications" class="content">
      <table>
        <thead>
          <tr>
            <th>Application ID</th>
            <th>Applicant ID</th>
            <th>Date submitted</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="application in applications" :key="application.submittedOn">
            <td>{{ application.id }}</td>
            <td>{{ application.registrantId }}</td>
            <td>{{ application.submittedOn }}</td>
          </tr>
        </tbody>
      </table>
    </div>

    <div>
      <button type="button" @click="createApplication">New Application</button>
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
  created() {
    this.applicationStore.fetchApplications();
  },
  methods: {
    async createApplication() {
      await this.applicationStore.newApplication();
      this.applicationStore.fetchApplications();
    },
  },
});
</script>
