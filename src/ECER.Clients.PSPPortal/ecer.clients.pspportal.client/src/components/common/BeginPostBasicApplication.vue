<template>
  <PageContainer>
    <v-row>
      <v-col cols="12">
        <Breadcrumb />
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="12">
        <h1 class="mb-4">Begin an application</h1>
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="12">
        <div class="d-flex flex-column ga-3">
          <p>
            To begin your application, provide information about the program.
            When you have completed these questions, select “Continue” to save
            your responses and create your draft application.
          </p>
          <p>
            You can save and exit the form and continue with your submission
            later from your ECE Post-Secondary Programs portal dashboard.
          </p>
        </div>
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="12">
        <Callout title="If you need assistance" type="warning">
          <p>
            You may reach out to the ECE Registry for assistance at any time by
            <router-link to="/messages/new">sending a message</router-link>
            <span>.</span>
          </p>
        </Callout>
      </v-col>
    </v-row>
    <v-form ref="beginApplicationForm">
      <v-row>
        <v-col cols="12">
          <div class="d-flex flex-column ga-3">
            <h3>Program information</h3>
            <EceTextField
              v-model="programName"
              label="Program name"
              :rules="[Rules.required('Enter a program name')]"
            ></EceTextField>
          </div>
        </v-col>
      </v-row>
      <v-row>
        <v-col>
          <div class="d-flex flex-column ga-3">
            <p class="font-weight-bold">
              The following program information will determine the application
              questions you need to complete. You will not be able to edit these
              responses later.
            </p>
            <p>
              To apply for a post-basic (Infant and Toddler Education [ITE]
              and/or Special Needs Educator [SNE]), an institution must either:
            </p>
            <ul class="ml-10">
              <li>
                Already have a recognized basic early childhood education
                program, or
              </li>
              <li>
                Be applying for a basic and post-basic program concurrently
              </li>
            </ul>
          </div>
        </v-col>
      </v-row>
      <v-row>
        <v-col>
          <p class="pb-3">Provincial certification type</p>
          <v-input
            v-model="provincialCertificationTypeValues"
            label="Provincial certification type"
            :rules="[
              Rules.atLeastOneOptionRequired(
                'Select a provincial certification type',
              ),
            ]"
            class="pb-3"
          >
            <v-row dense>
              <v-col cols="12">
                <v-checkbox
                  v-model="provincialCertificationTypeValues"
                  value="Basic"
                  label="Early Childhood Educator"
                  density="compact"
                  hide-details
                />
              </v-col>
              <v-col cols="12">
                <v-checkbox
                  v-model="provincialCertificationTypeValues"
                  label="Infant and Toddler Educator (ITE)"
                  value="ITE"
                  density="compact"
                  hide-details
                />
              </v-col>
              <v-col cols="12">
                <v-checkbox
                  v-model="provincialCertificationTypeValues"
                  label="Special Needs Educator (SNE)"
                  value="SNE"
                  density="compact"
                  hide-details
                />
              </v-col>
            </v-row>
          </v-input>
        </v-col>
      </v-row>
      <v-row>
        <v-col>
          <p class="pb-3">Delivery method(s)</p>
          <v-radio-group
            id="programConfirmationRadio"
            v-model="deliveryType"
            color="primary"
            :rules="[Rules.required('Select a delivery method')]"
          >
            <v-radio
              label="In-person - programs or courses are delivered on campus or at an approved instructional site"
              value="Inperson"
            ></v-radio>
            <v-radio
              label="Hybrid - courses within a program are delivered both online and in person"
              value="Hybrid"
            ></v-radio>
            <v-radio
              label="Online - programs or courses are delivered entirely through digital platforms, with no requirement for students to attend a physical campus"
              value="Online"
            ></v-radio>
          </v-radio-group>
        </v-col>
      </v-row>
      <v-row>
        <v-col>
          <v-btn
            rounded="lg"
            color="primary"
            :loading="loadingStore.isLoading('program_application_post')"
            :disabled="loadingStore.isLoading('program_application_post')"
            @click="createApplication"
          >
            Continue
          </v-btn>
        </v-col>
      </v-row>
    </v-form>
  </PageContainer>
</template>
<script lang="ts">
import { defineComponent } from "vue";
import ECEHeader from "@/components/ECEHeader.vue";
import type { VForm } from "vuetify/components";
import Callout from "@/components/common/Callout.vue";
import PageContainer from "@/components/PageContainer.vue";
import Breadcrumb from "@/components/Breadcrumb.vue";
import EceTextField from "@/components/inputs/EceTextField.vue";
import EceCheckbox from "@/components/inputs/EceCheckbox.vue";
import * as Rules from "@/utils/formRules";
import { useLoadingStore } from "@/store/loading";
import type { CreateApplication } from "@/types/helperFunctions";

export default defineComponent({
  name: "BeginPostBasicApplication",
  components: {
    EceCheckbox,
    EceTextField,
    Breadcrumb,
    PageContainer,
    Callout,
    ECEHeader,
  },
  setup() {
    const loadingStore = useLoadingStore();
    return { loadingStore };
  },
  emits: ["create-application"],
  data() {
    return {
      programName: "" as string,
      provincialCertificationTypeValues: [] as string[],
      deliveryType: "" as string,
      Rules,
      createApplicationObject: {} as CreateApplication,
    };
  },
  methods: {
    async createApplication() {
      const { valid } = await (
        this.$refs.beginApplicationForm as VForm
      ).validate();

      if (!valid) return;

      this.createApplicationObject = {
        programName: this.programName,
        provincialCertificationTypeValues:
          this.provincialCertificationTypeValues,
        deliveryType: this.deliveryType,
        applicationType: "NewBasicECEPostBasicProgram",
      };
      this.$emit("create-application", this.createApplicationObject);
    },
  },
});
</script>
