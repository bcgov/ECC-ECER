<template>
  <div class="weather-component">
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
import OpenAPIClientAxios from "openapi-client-axios";
import { defineComponent } from "vue";

import type { Client, Components } from "@/types/openapi";

interface Data {
  loading: boolean;
  post: Components.Schemas.Application[] | null | undefined;
}

const api = new OpenAPIClientAxios({
  definition: "swagger/v1/swagger.json",
});

export default defineComponent({
  data(): Data {
    return {
      loading: false,
      post: null,
    };
  },
  watch: {
    // call again the method if the route changes
    $route: "fetchApplications",
  },
  created() {
    // fetch the data when the view is created and the data is
    // already being observed
    this.fetchApplications();
  },
  methods: {
    fetchApplications(): void {
      this.post = null;
      this.loading = true;

      api.init<Client>().then((c) => {
        console.debug(c);
        return c.GetApplications().then((response) => {
          console.debug(response.data.items);
          this.post = response.data.items;
          this.loading = false;
        });
      });
    },
    submitAppliction(): void {
      api.init<Client>().then((c) => {
        c.PostNewApplication({}, {}).then((response) => {
          console.debug(response.data.applicationId);
          this.fetchApplications();
        });
      });
    },
  },
});
</script>
