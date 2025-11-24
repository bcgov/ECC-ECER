<template>
    <PageContainer>
        <v-row>
            <v-col cols="12">
                <Breadcrumb />
            </v-col>
        </v-row>
        <v-row>
            <v-col cols="12">
                <h1>Manage users</h1>
            </v-col>
        </v-row>
        <v-row>
            <v-sheet color=""></v-sheet>
        </v-row>
    </PageContainer>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import PageContainer from "@/components/PageContainer.vue";
import Breadcrumb from "@/components/Breadcrumb.vue";

export default defineComponent({
    name: "ManageUsers",
    components: { PageContainer, Breadcrumb },
});
</script>