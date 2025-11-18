<template>
    <PageContainer>
        <v-row class="ga-8">
            <!-- Header Section -->
            <v-col cols="12">
                <h1>My contact details</h1>
            </v-col>
            <v-form ref="form" validate-on="input" class="w-100">
                <!-- Information needed from BCeID Basic -->
                <v-col cols="12">
                    <ECEHeader title="Name" />
                    <v-row>
                        <!-- Last name Field -->
                        <v-col sm="12" md="6" class="mt-8">
                            <EceTextField v-model="lastName" label="Last name" :readonly="true"></EceTextField>
                        </v-col>
                    </v-row>
                    <v-row>
                        <!-- First name Field -->
                        <v-col sm="12" md="6" class="mt-8">
                            <EceTextField v-model="firstName" label="First name" :readonly="true"></EceTextField>
                        </v-col>
                    </v-row>
                    <!-- Preferred first name Field -->
                    <v-row>
                        <v-col sm="12" md="6" class="mt-8">
                            <EceTextField v-model="preferredName" label="Preferred first name (optional)">
                            </EceTextField>
                        </v-col>
                    </v-row>
                </v-col>
                <v-col cols="12">
                    <ECEHeader title="Contact information" />
                    <v-row>
                        <!-- Job title Field -->
                        <v-col sm="12" md="6" class="mt-8">
                            <EceTextField v-model="jobTitle" label="Job title" :rules="[Rules.required()]">
                            </EceTextField>
                        </v-col>
                    </v-row>
                    <!-- Phone Field -->
                    <v-row>
                        <v-col sm="12" md="6" class="mt-8">
                            <EceTextField v-model="phone" label="Primary contact number"
                                :rules="[Rules.required(), Rules.phoneNumber()]">
                            </EceTextField>
                        </v-col>
                    </v-row>
                    <!-- Phone extension Field -->
                    <v-row>
                        <v-col sm="12" md="6" class="mt-8">
                            <EceTextField v-model="phoneExtension" label="Phone extension (optional)"
                                @keypress="isNumber($event)"></EceTextField>
                        </v-col>
                    </v-row>
                    <!-- Email Field -->
                    <v-row>
                        <v-col sm="12" md="6" class="mt-8">
                            <EceTextField v-model="email" label="Email"
                                :rules="[Rules.required(), Rules.email('Enter your email in the format \'name@email.com\'')]">
                            </EceTextField>
                        </v-col>
                    </v-row>
                    <!-- Has accepted terms of use Field -->
                    <v-row>
                        <v-col sm="12" md="6" class="mt-8">
                            <EceCheckbox v-model="hasAcceptedTermsOfUse" label="I have read and accept the Terms of Use"
                                :rules="[Rules.hasCheckbox('You must read and accept the Terms of Use')]">
                            </EceCheckbox>
                        </v-col>
                    </v-row>
                </v-col>
            </v-form>

            <v-col cols="12">
                <div>
                    <v-btn rounded="lg" :loading="loadingStore.isLoading('psp_user_profile_put') || isRedirecting"
                        color="primary" class="mr-2" @click="submit">
                        Save
                    </v-btn>
                    <v-btn rounded="lg" :loading="loadingStore.isLoading('psp_user_profile_put') || isRedirecting"
                        variant="outlined" @click="oidcStore.logout()">
                        Cancel
                    </v-btn>
                </div>
            </v-col>
        </v-row>
    </PageContainer>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import PageContainer from "@/components/PageContainer.vue";
import ECEHeader from "@/components/ECEHeader.vue";
import EceTextField from "@/components/inputs/EceTextField.vue";
import EceCheckbox from "@/components/inputs/EceCheckbox.vue";
import * as Rules from "@/utils/formRules";
import { useLoadingStore } from "@/store/loading";
import { useOidcStore } from "@/store/oidc";
import type { VForm } from "vuetify/components/VForm";
import { getPspUserProfile, updatePspUserProfile } from "@/api/psp-rep";
import { useUserStore } from "@/store/user";
import { useRouter } from "vue-router";
import { isNumber } from "@/utils/formInput";


export default defineComponent({
    name: "NewUser",
    components: {
        PageContainer,
        ECEHeader,
        EceTextField,
        EceCheckbox,
    },
    setup() {
        const loadingStore = useLoadingStore();
        const oidcStore = useOidcStore();
        const userStore = useUserStore();
        const router = useRouter();

        return {
            loadingStore,
            oidcStore,
            userStore,
            router,
            Rules,
        };
    },
    data() {
        return {
            firstName: "",
            lastName: "",
            preferredName: "",
            jobTitle: "",
            phone: "",
            phoneExtension: "",
            email: "",
            hasAcceptedTermsOfUse: false,
            isRedirecting: false,
        };
    },
    async mounted() {
        this.firstName = this.userStore.firstName ?? "";
        this.lastName = this.userStore.lastName ?? "";
        this.phone = this.userStore?.phone ?? "";
        this.phoneExtension = this.userStore?.phoneExtension ?? "";
        this.jobTitle = this.userStore?.jobTitle ?? "";
        this.hasAcceptedTermsOfUse = this.userStore?.hasAcceptedTermsOfUse ?? false;
        this.preferredName = this.userStore?.preferredName ?? "";
        this.email = this.userStore?.email ?? "";
    },
    methods: {
        isNumber,
        async submit() {
            let { valid } = await (this.$refs.form as VForm).validate();

            if (valid) {
                this.isRedirecting = true;
                const userUpdated: boolean = await updatePspUserProfile({
                    firstName: this.firstName,
                    lastName: this.lastName,
                    preferredName: this.preferredName,
                    email: this.email,
                    phone: this.phone,
                    phoneExtension: this.phoneExtension,
                    jobTitle: this.jobTitle,
                    hasAcceptedTermsOfUse: this.hasAcceptedTermsOfUse,
                }, true);
                if (userUpdated) {
                    const pspUserProfile = await getPspUserProfile();
                    if (pspUserProfile !== null) {
                        this.userStore.setPspUserProfile(pspUserProfile);
                    }
                    this.router.push("/");
                } else {
                    this.isRedirecting = false;
                }
            }
        }
    }
});
</script>