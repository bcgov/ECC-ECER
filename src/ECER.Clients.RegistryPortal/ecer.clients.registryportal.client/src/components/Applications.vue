<template>
  <div>
    <h1>Applications</h1>

    <div v-if="loading" class="loading">Loading...</div>

    <div v-if="post" class="content">
      <table>
        <thead>
          <tr>
            <th>Application ID</th>
            <th>Applicant ID</th>
            <th>Date submitted</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="application in post" :key="application.submittedOn">
            <td>{{ application.id }}</td>
            <td>{{ application.registrantId }}</td>
            <td>{{ application.submittedOn }}</td>
          </tr>
        </tbody>
      </table>
    </div>

    <div>
      <button type="button" @click="submitAppliction">New Application</button>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";

import { getApplications, postApplication } from "@/api/application";
import type { Components } from "@/types/openapi";

interface Data {
  loading: boolean;
  post: Components.Schemas.Application[] | null | undefined;
}

export default defineComponent({
  data(): Data {
    return {
      loading: false,
      post: null,
    };
  },
  created() {
    this.fetchApplications();
  },
  methods: {
    async submitAppliction(): Promise<void> {
      await postApplication();
      this.fetchApplications();
    },
    async fetchApplications(): Promise<void> {
      this.loading = true;
      this.post = await getApplications();
      this.loading = false;
    },
  },
});
</script>
