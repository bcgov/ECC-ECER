<template>
  <PageContainer>
    <v-row class="justify-space-between mb-4">
      <v-col cols="auto">
        <h1>Review responses</h1>
      </v-col>
      <v-col cols="auto">
        <v-btn rounded="lg" variant="text" @click="printPage()">
          <v-icon
            color="secondary"
            icon="mdi-printer-outline"
            class="mr-2"
          ></v-icon>
          <a class="small">Print Preview</a>
        </v-btn>
      </v-col>
    </v-row>

    <v-card class="mb-4 pb-2" variant="outlined" rounded="lg">
      <v-card-title>
        <div class="d-flex justify-space-between align-center">
          <div>
            <h2 class="text-wrap">Program overview</h2>
          </div>
          <div>
            <v-tooltip location="top">
              <template #activator="{ props }">
                <v-btn
                  icon="mdi-pencil"
                  v-bind="props"
                  :color="'primary'"
                  variant="plain"
                  :to="{
                    name: 'program-application-institute-info',
                    params: { programApplicationId: programApplicationId },
                  }"
                />
              </template>
              <span>Edit institution and program information</span>
            </v-tooltip>
          </div>
        </div>
      </v-card-title>
      <!-- CONTENT -->
      <v-card-text class="text-grey-dark">
        <v-row class="mb-n5">
          <v-col cols="4">
            <p class="small">Institution name</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">
              {{ userStore.educationInstitution?.name }}
            </p>
          </v-col>
        </v-row>
        <v-row class="mb-n5">
          <v-col cols="4">
            <p class="small">Campus</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">
              {{ campus }}
            </p>
          </v-col>
        </v-row>
        <v-row class="mb-n5">
          <v-col cols="4">
            <p class="small">Contact person</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">
              {{ contactPerson }}
            </p>
          </v-col>
        </v-row>
        <v-row class="mb-n5">
          <v-col cols="4">
            <p class="small">Program name</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">
              {{ programApplicationObject?.programApplicationName }}
            </p>
          </v-col>
        </v-row>
        <v-row class="mb-n5">
          <v-col cols="4">
            <p class="small">Program type</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">
              {{ programType }}
            </p>
          </v-col>
        </v-row>
        <v-row class="mb-n5">
          <v-col cols="4">
            <p class="small">Delivery method(s)</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">
              {{ programApplicationObject?.deliveryType }}
            </p>
          </v-col>
        </v-row>
        <v-row class="mb-n5">
          <v-col cols="4">
            <p class="small">Program length</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">
              {{ programApplicationObject?.programLength }}
            </p>
          </v-col>
        </v-row>
        <v-row class="mb-n5">
          <v-col cols="4">
            <p class="small">Program enrollment options</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">
              {{ programApplicationObject?.enrollmentOptions?.join(", ") }}
            </p>
          </v-col>
        </v-row>
        <v-row class="mb-n5">
          <v-col cols="4">
            <p class="small">Admission options</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">
              {{ programApplicationObject?.admissionOptions?.join(", ") }}
            </p>
          </v-col>
        </v-row>
        <v-row class="mb-n5">
          <v-col cols="4">
            <p class="small">Minimum enrollment per course</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">
              {{ programApplicationObject?.minimumEnrollment }}
            </p>
          </v-col>
        </v-row>
        <v-row class="mb-n5">
          <v-col cols="4">
            <p class="small">Maximum enrollment per course</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">
              {{ programApplicationObject?.maximumEnrollment }}
            </p>
          </v-col>
        </v-row>
        <v-row class="mb-n5" v-if="showDeliverySection()">
          <v-col cols="4">
            <p class="small">Online method(s) of instruction</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">
              {{
                programApplicationObject?.onlineMethodOfInstruction?.join(", ")
              }}
            </p>
          </v-col>
        </v-row>
        <v-row class="mb-n5" v-if="showDeliverySection()">
          <v-col cols="4">
            <p class="small">
              Delivery method for practicum instructor supervision
            </p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">
              {{ programApplicationObject?.deliveryMethod?.join(", ") }}
            </p>
          </v-col>
        </v-row>
      </v-card-text>
    </v-card>

    <v-card
      v-for="[name, metaData] in componentAnswer"
      :key="name"
      class="mb-4 pb-2"
      variant="outlined"
      rounded="lg"
    >
      <v-card-title>
        <div class="d-flex justify-space-between align-center">
          <div>
            <h2 class="text-wrap">{{ name }}</h2>
          </div>
          <div>
            <v-tooltip location="top">
              <template #activator="{ props }">
                <v-btn
                  icon="mdi-pencil"
                  v-bind="props"
                  :color="'primary'"
                  variant="plain"
                  :to="
                    '/program-application/' +
                    programApplicationId +
                    '/component/' +
                    metaData.componentGroupId
                  "
                />
              </template>
              <span>Edit {{ name }}</span>
            </v-tooltip>
          </div>
        </div>
      </v-card-title>
      <v-card-text
        v-for="component in metaData.components"
        class="text-grey-dark"
      >
        <v-row class="mb-n5">
          <v-col cols="4">
            <p class="small">{{ component.name }}</p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">
              {{ component.answer }}
            </p>
          </v-col>
        </v-row>
      </v-card-text>
      <v-card-text class="text-grey-dark">
        <v-row class="mb-n5">
          <v-col cols="4">
            <p class="small">
              <v-icon>mdi-attachment</v-icon>
              Attached files
            </p>
          </v-col>
          <v-col>
            <p class="small font-weight-bold">
              {{ mapFileNames(metaData) }}
            </p>
          </v-col>
        </v-row>
      </v-card-text>
    </v-card>

    <v-row class="d-print-none">
      <v-col>
        <v-btn rounded="lg" color="primary" @click="$emit('next')">
          Continue
        </v-btn>
      </v-col>
    </v-row>
  </PageContainer>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import PageContainer from "@/components/PageContainer.vue";
import type {
  Components,
  PspUserListItem,
  ComponentGroupWithComponents,
} from "@/types/openapi";
import {
  mapProgramType,
  getProgramApplicationById,
  getComponentGroupComponents,
} from "@/api/program-application";
import { useUserStore } from "@/store/user";
import { getUsers } from "@/api/manage-users";

interface ComponentGroupMetaData {
  componentGroupId?: string | null;
  components?: Components.Schemas.ProgramApplicationComponent[] | null;
}

export default defineComponent({
  name: "ProgramApplicationReviewResponses",
  components: {
    PageContainer,
  },
  props: {
    programApplicationId: {
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
  emits: ["next"],
  computed: {
    programType(): string {
      const types = this.programApplicationObject?.programTypes;
      if (!types?.length) return "—";
      return types.map(mapProgramType).join(", ");
    },
    campus(): string {
      if (this.programApplicationObject?.programCampuses) {
        var programCampusIds =
          this.programApplicationObject?.programCampuses.map((c) => {
            if (c.campusId !== null && c.campusId !== undefined) {
              return c.campusId;
            }
          });
        var campusObj = this.userStore.educationInstitution?.campuses?.filter(
          (c) => {
            if (c.id !== null && c.id !== undefined) {
              programCampusIds.includes(c.id);
            }
          },
        );
        if (!campusObj?.length) return "-";
        return campusObj.map((c) => c.name).join(", ");
      }
      return "-";
    },
  },
  data() {
    return {
      programApplicationObject:
        null as Components.Schemas.ProgramApplication | null,
      contactPerson: "",
      componentGroup: [] as ComponentGroupWithComponents[] | null | undefined,
      componentAnswer: {} as Map<string, ComponentGroupMetaData> | null,
    };
  },
  async mounted() {
    await this.fetchApplication();
    await this.loadComponents();
  },
  methods: {
    printPage() {
      globalThis.print();
    },
    async fetchApplication() {
      const result = await getProgramApplicationById(this.programApplicationId);
      let users = (await getUsers()) ?? [];
      if (result.error || result.data == null) {
        console.error("Failed to retrieve program application:", result.error);
      } else {
        this.programApplicationObject = result.data;
        const contactPerson = users.find(
          (user: PspUserListItem) =>
            user.id === this.programApplicationObject?.programRepresentativeId,
        );
        this.contactPerson = contactPerson
          ? `${contactPerson.profile?.firstName} ${contactPerson.profile?.lastName}`.trim()
          : "-";
      }
    },
    showDeliverySection(): boolean {
      return this.programApplicationObject?.deliveryType !== "Inperson";
    },
    async loadComponents() {
      const result = await getComponentGroupComponents(
        this.programApplicationId,
      );
      if (result.error) return;
      this.componentGroup = result.data as
        | ComponentGroupWithComponents[]
        | null
        | undefined;

      const map = new Map();

      this.componentGroup?.forEach((c) => {
        const groupName = c.name;
        if (map.has(groupName)) {
          map.get(groupName).components.push(c.components);
        } else {
          map.set(groupName, {
            componentGroupId: c.id,
            components: c.components,
          });
        }
      });
      this.componentAnswer = map;
    },
    mapFileNames(metaData: ComponentGroupMetaData) {
      return metaData.components?.flatMap((f) => f.files).join(", ");
    },
  },
});
</script>
