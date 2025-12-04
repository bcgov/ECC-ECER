<template>
    <PageContainer>
        <Loading v-if="isLoading" />
        <template v-else>
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
                <v-col cols="12">
                    <v-sheet color="support-surface-info " class="w-100 px-12 py-7">
                        <div class="d-flex flex-column ga-3 text-support-border-info">
                            <h2 class="mb-3 ">Adding a new user</h2>
                            <p>You may add a new user from your post-secondary institution.
                            </p>
                            <p>The user:</p>
                            <ol class="ml-8">
                                <li>Will receive an email inviting them to login to this PSP Portal.</li>
                                <li>Will require a Business BCeID account granted from your institution in order to
                                    login.
                                </li>
                            </ol>

                        </div>
                        <v-btn class="mt-4" color="primary"><v-icon class="mr-2">mdi-account-plus</v-icon> Add
                            user</v-btn>
                    </v-sheet>
                </v-col>
            </v-row>
            <v-row>
                <v-col cols="12">
                    <p class="font-weight-bold">The users shown below are registered for access to this portal for
                        {{ educationInstitutionName }}.</p>
                </v-col>
            </v-row>
            <template v-if="activeUsers.length > 0">
                <v-row>
                    <v-col cols="12">
                        <ECEHeader title="Active users" />
                    </v-col>
                </v-row>
                <v-row>
                    <v-col cols="12">
                        <div class="d-flex flex-column ga-3">
                            <p>The Primary contact is the person that the ECE Registry would contact first, if there is
                                a
                                reason
                                to do so.
                            </p>
                            <ul class="ml-8">
                                <li>Setting a user as the Primary contact user does not apply any additional permissions
                                    to
                                    the user in this portal.</li>
                                <li>One active user must be indicated as your Primary contact for your ECE programs.
                                </li>
                            </ul>
                        </div>
                    </v-col>
                </v-row>
                <v-row>
                    <v-col cols="12" md="6" lg="4" v-for="user in activeUsers" :key="user.id as string">
                        <UserCard :user="user" @set-primary="handleSetPrimary" @remove-access="handleRemoveAccess"
                            :currentUserId="userStore.pspUserProfile?.id || ''" />
                    </v-col>
                </v-row>
            </template>
            <template v-if="invitedUsers.length > 0">

                <v-row>
                    <v-col cols="12">
                        <ECEHeader title="Invitations pending" />
                    </v-col>
                </v-row>
                <v-row>
                    <v-col cols="12">
                        <p>New and reactivated users will show as “Invitation pending” until they log in to verify their
                            account.</p>
                    </v-col>
                </v-row>
                <v-row>
                    <v-col cols="12" md="6" lg="4" v-for="user in invitedUsers" :key="user.id as string">
                        <UserCard :user="user" @resend-invitation="handleResendInvitation"
                            :currentUserId="userStore.pspUserProfile?.id || ''" />
                    </v-col>
                </v-row>
            </template>
            <template v-if="inactiveUsers.length > 0">
                <v-row>
                    <v-col cols="12">
                        <ECEHeader title="Inactive users" />
                    </v-col>
                </v-row>
                <v-row>
                    <v-col cols="12">
                        <p>Making a user active will send them a new invitation to login to the portal.</p>
                    </v-col>
                </v-row>
                <v-row>
                    <v-col cols="12" md="6" lg="4" v-for="user in inactiveUsers" :key="user.id as string">
                        <UserCard :user="user" @reactivate="handleReactivate"
                            :currentUserId="userStore.pspUserProfile?.id || ''" />
                    </v-col>
                </v-row>
            </template>
        </template>
    </PageContainer>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import PageContainer from "@/components/PageContainer.vue";
import Breadcrumb from "@/components/Breadcrumb.vue";
import ECEHeader from "@/components/ECEHeader.vue";
import UserCard from "@/components/UserCard.vue";
import Loading from "@/components/Loading.vue";
import { deactivateUser, getUsers, reactivateUser, setPrimaryUser } from "@/api/manage-users";
import type { PspUserListItem } from "@/types/openapi";
import { useUserStore } from "@/store/user";

export default defineComponent({
    name: "ManageUsers",
    components: { PageContainer, Breadcrumb, ECEHeader, UserCard, Loading },
    props: {
        educationInstitutionName: {
            type: String,
            required: true,
        },
    },
    setup() {
        const userStore = useUserStore();
        return {
            userStore,
        };
    },
    data() {
        return {
            isLoading: true,
            users: [] as PspUserListItem[],
        }
    },
    async mounted() {
        await this.loadUsers();
        this.isLoading = false;
    },
    methods: {
        async loadUsers() {
            this.users = await getUsers() ?? [];
        },
        async handleSetPrimary(userId: string | null | undefined) {
            if (userId) {
                await setPrimaryUser(userId);
                await this.loadUsers();
            }
        },
        async handleRemoveAccess(userId: string | null | undefined) {
            if (userId) {
                await deactivateUser(userId);
                await this.loadUsers();
            }
        },
        handleResendInvitation(userId: string | null | undefined) {
            // TODO: Implement resend invitation handler
            console.log("Resend invitation:", userId);
        },
        async handleReactivate(userId: string | null | undefined) {
            if (userId) {
                await reactivateUser(userId);
                await this.loadUsers();
            }
        },
    },
    computed: {
        activeUsers(): PspUserListItem[] {
            return this.users.filter((user: PspUserListItem) => user.accessToPortal === "Active");
        },
        invitedUsers(): PspUserListItem[] {
            return this.users.filter((user: PspUserListItem) => user.accessToPortal === "Invited");
        },
        inactiveUsers(): PspUserListItem[] {
            return this.users.filter((user: PspUserListItem) => user.accessToPortal === "Disabled" || user.accessToPortal == null);
        },
    },
});
</script>
