<template>
  <PageContainer>
    <v-row>
      <v-col cols="12">
        <Breadcrumb />
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12">
        <h1>Add campus</h1>
      </v-col>
    </v-row>

    <template v-if="isPrivate">
      <v-row>
        <v-col cols="12">
          <p>
            This process is for institutions with existing programs that are
            recognized by the Registry to seek approval to add a program
            offering of an early childhood education cohort at a new campus
            location.
          </p>
          <p class="mt-3">
            Private institutions may require additional approval from their
            regulator to add new campus location. Contact the Private Training
            Institutions Regulatory Unit (PTIRU) indicating the requested
            change.
          </p>
        </v-col>
      </v-row>
    </template>

    <Loading v-if="isLoading" />

    <template v-else>
      <CampusForm
        ref="campusFormRef"
        :psp-users="pspUsers"
        :institution-name="institutionName"
        :is-private="isPrivate"
        :programs="programs"
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
              {{
                isPrivate ? "Save and continue to program application" : "Save"
              }}
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
  getEducationInstitution,
  updateEducationInstitution,
} from "@/api/education-institution";
import { getUsers } from "@/api/manage-users";
import { getPrograms } from "@/api/program";
import { useAlertStore } from "@/store/alert";
import { useRouter } from "vue-router";
import type { Components, PspUserListItem } from "@/types/openapi";
import type { PspUserItem } from "@/components/inputs/EcePspUser.vue";

export default defineComponent({
  name: "AddCampus",
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
      programs: [] as Components.Schemas.Program[],
      isLoading: true,
      isSaving: false,
    };
  },
  computed: {
    isPrivate(): boolean {
      return this.institution?.institutionType === "Private";
    },
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

    if (!this.isPrivate) {
      const response = await getPrograms("", ["Approved"]);
      this.programs = response.data?.programs ?? [];
    }

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
        const newCampus: Components.Schemas.Campus = {
          ...campus,
          isSatelliteOrTemporaryLocation: false,
        };

        const updatedInstitution: Components.Schemas.EducationInstitution = {
          ...this.institution!,
          campuses: [...(this.institution!.campuses ?? []), newCampus],
        };

        const success = await updateEducationInstitution(updatedInstitution);

        if (success) {
          if (this.isPrivate) {
            const refreshed = await getEducationInstitution();
            const newCampusEntry = refreshed?.campuses?.find(
              (c) =>
                c.name === newCampus.name &&
                !this.institution?.campuses?.some(
                  (existing) => existing.id === c.id,
                ),
            );
            if (newCampusEntry?.id) {
              await this.router.push({
                name: "campus",
                params: {
                  institutionId: this.institutionId,
                  campusId: newCampusEntry.id!,
                },
              });
            } else {
              await this.router.push({
                name: "education-institution",
                params: { institutionId: this.institutionId },
              });
            }
          } else {
            this.alertStore.setSuccessAlert(
              "Campus has been successfully added.",
            );
            await this.router.push({
              name: "education-institution",
              params: { institutionId: this.institutionId },
            });
          }
        } else {
          this.alertStore.setFailureAlert(
            "Failed to add campus. Please try again.",
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
