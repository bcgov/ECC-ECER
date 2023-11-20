<template>
    <div class="weather-component">
        <h1>Applications</h1>

        <div v-if="loading" class="loading"> Loading... </div>

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
    </div>
</template>

<script lang="ts">

import { defineComponent } from 'vue';
import OpenAPIClientAxios from 'openapi-client-axios';
import type { Client, Components } from '@/types/openapi';

interface Data {
    loading: boolean,
    post: null | Components.Schemas.Application[];
}

export default defineComponent({
    data(): Data {
        return {
            loading: false,
            post: null
        };
    },
    created() {
        // fetch the data when the view is created and the data is
        // already being observed
        this.fetchApplications();
    },
    watch: {
        // call again the method if the route changes
        '$route': 'fetchApplications'
    },
    methods: {
        fetchApplications(): void {
            this.post = null;
            this.loading = true;

            const api = new OpenAPIClientAxios({ definition: 'http://localhost:5121/swagger/v1/swagger.json' });
            api.init<Client>().then(c => {
                console.debug(c);
                return c.GetApplications().then(response => {
                    console.debug(response.data.items);
                    this.post = response.data.items;
                    this.loading = false;
                });
            });
        }
    },
});
</script>
 