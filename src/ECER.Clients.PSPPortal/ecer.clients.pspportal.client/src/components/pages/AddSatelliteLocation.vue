<template>
  <PageContainer>
    <v-row>
      <v-col cols="12">
        <Breadcrumb />
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12">
        <h1>Add satellite location</h1>
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12">
        <p>
          This process is for institutions with existing programs that are
          recognized by the Registry to seek approval to add a
          <strong>temporary offering</strong>
          of an early childhood education cohort at a location in partnership
          with another organization, or outside of the institution.
        </p>
        <!-- prettier-ignore -->
        <p class="mt-3">
          The satellite program offering runs in addition to existing
          programming and has a <strong>fixed end date</strong>.
          Institutions must hold ongoing recognition prior to submitting a
          request to offer a satellite program.
        </p>
      </v-col>
    </v-row>

    <Loading v-if="isLoading" />

    <template v-else>
      <CampusForm
        ref="campusFormRef"
        :psp-users="pspUsers"
        :institution-name="institutionName"
        :is-private="true"
      />

      <v-row class="mt-8">
        <v-col cols="auto">
          <div class="d-flex flex-row ga-2">
            <v-btn
              rounded="lg"
              color="primary"
              :loading="isSaving"
              @click="save"
            >
              Save
            </v-btn>
            <v-btn
              rounded="lg"
              color="primary"
              variant="outlined"
              :loading="isSaving"
              @click="cancel"
            >
              Cancel
            </v-btn>
          </div>
        </v-col>
      </v-row>
    </template>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import PageContainer from "@/components/PageContainer.vue";
import Breadcrumb from "@/components/Breadcrumb.vue";
import Loading from "@/components/Loading.vue";
import CampusForm from "@/components/CampusForm.vue";
import {
  createCampus,
  getEducationInstitution,
} from "@/api/education-institution";
import { getUsers } from "@/api/manage-users";
import { useAlertStore } from "@/store/alert";
import { useRouter } from "vue-router";
import type { Components, PspUserListItem } from "@/types/openapi";
import type { PspUserItem } from "@/components/inputs/EcePspUser.vue";

export default defineComponent({
  name: "AddSatelliteLocation",
  components: { PageContainer, Breadcrumb, Loading, CampusForm },
  props: {
    institutionId: {
      type: String,
      required: true,
    },
  },
  setup() {
    const alertStore = useAlertStore();
    const router = useRouter();
    return { alertStore, router };
  },
  data() {
    return {
      institution: null as Components.Schemas.EducationInstitution | null,
      pspUsers: [] as PspUserItem[],
      isLoading: true,
      isSaving: false,
    };
  },
  computed: {
    institutionName(): string {
      return this.institution?.name ?? "";
    },
  },
  async mounted() {
    const [institution, userList] = await Promise.all([
      getEducationInstitution(),
      getUsers(),
    ]);

    if (!institution) {
      this.alertStore.setFailureAlert(
        "Unable to load institution. Please try again.",
      );
      await this.router.push({
        name: "education-institution",
        params: { institutionId: this.institutionId },
      });
      return;
    }

    this.institution = institution;
    this.pspUsers = (userList ?? []).map((user: PspUserListItem) => ({
      id: user.id ?? "",
      name: `${user.profile?.firstName ?? ""} ${user.profile?.lastName ?? ""}`.trim(),
    }));

    this.isLoading = false;
  },
  methods: {
    async save() {
      const campusFormRef = this.$refs.campusFormRef as InstanceType<
        typeof CampusForm
      >;
      const { valid } = await campusFormRef.validate();

      if (!valid) {
        this.alertStore.setFailureAlert(
          "You must enter all required fields in the valid format.",
        );
        return;
      }

      this.isSaving = true;
      try {
        const { campus } = campusFormRef.getData();
        const newCampusId = await createCampus({
          ...campus,
          isSatelliteOrTemporaryLocation: true,
        });

        if (newCampusId) {
          this.alertStore.setSuccessAlert(
            "Satellite location has been successfully added.",
          );
          await this.router.push({
            name: "education-institution",
            params: { institutionId: this.institutionId },
          });
        } else {
          this.alertStore.setFailureAlert(
            "Failed to add satellite location. Please try again.",
          );
        }
      } finally {
        this.isSaving = false;
      }
    },
    cancel() {
      this.router.push({
        name: "education-institution",
        params: { institutionId: this.institutionId },
      });
    },
  },
});
</script>
