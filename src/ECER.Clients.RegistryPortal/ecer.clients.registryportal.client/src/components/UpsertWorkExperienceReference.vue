<template>
  <v-container>
    <Breadcrumb />
    <v-row>
      <v-col>
        <Alert
          v-model="isDuplicateReference"
          type="error"
          title="choose someone else"
          prominent
        >
          <p>
            This person is already your character reference. Your work
            experience reference and character reference must be different
            people.
          </p>
        </Alert>
      </v-col>
    </v-row>
    <div class="d-flex flex-column ga-3">
      <h2 class="mt-10">Add work experience reference</h2>

      <p>Make sure you choose a person that:</p>
      <ul class="ml-10">
        <div v-if="applicationType === 'Renewal'">
          <li>Able to confirm you completed work experience hours</li>
          <li>
            A co-worker, supervisor, or parent/guardian of a child you cared for
          </li>
        </div>
        <div v-if="applicationType === 'New'">
          <li>
            Can speak to your knowledge, skill, ability and competencies as an
            ECE
          </li>
          <li>Has directly supervised (observed) the hours they attest to</li>
          <li>
            Has held a valid Canadian ECE certification/registration during the
            hours they supervised or observed you
          </li>
        </div>
      </ul>
      <p class="mb-6">
        The person
        <b>cannot</b>
        be your character reference.
      </p>
      <EceForm
        ref="upsertWorkExperienceReferenceForm"
        :form="workExperienceReferenceUpsertForm"
        :form-data="formStore.formData"
        @updated-form-data="formStore.setFormData"
      />
      <p class="mt-6">
        After you save, we will send an email to this person requesting a
        reference.
      </p>
    </div>
    <v-row class="mt-6">
      <v-col class="d-flex flex-row ga-3 flex-wrap">
        <v-btn
          size="large"
          color="primary"
          :loading="
            loadingStore.isLoading(
              'application_workexperiencereference_update_post',
            )
          "
          @click="handleSubmitReference"
        >
          Save new reference
        </v-btn>
        <v-btn
          size="large"
          variant="outlined"
          color="primary"
          @click="router.back()"
        >
          Cancel
        </v-btn>
      </v-col>
    </v-row>
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useRouter } from "vue-router";

import { getApplicationStatus } from "@/api/application";
import { upsertWorkExperienceReference } from "@/api/reference";
import Alert from "@/components/Alert.vue";
import EceForm from "@/components/Form.vue";
import workExperienceReferenceUpsertForm from "@/config/work-experience-reference-upsert-form";
import { useAlertStore } from "@/store/alert";
import { useApplicationStore } from "@/store/application";
import { useFormStore } from "@/store/form";
import { useLoadingStore } from "@/store/loading";
import type { Components } from "@/types/openapi";
import Breadcrumb from "@/components/Breadcrumb.vue";

export default defineComponent({
  name: "UpsertWorkExperienceReference",
  components: { EceForm, Alert, Breadcrumb },
  props: {
    applicationId: {
      type: String,
      required: true,
    },
    referenceId: {
      type: String,
      required: false,
      default: "",
    },
  },
  setup: async (props) => {
    const applicationStore = useApplicationStore();
    const alertStore = useAlertStore();
    const formStore = useFormStore();
    const loadingStore = useLoadingStore();
    const router = useRouter();
    const applicationStatus = (await getApplicationStatus(props.applicationId))
      ?.data;

    let reference: Components.Schemas.WorkExperienceReference | undefined =
      undefined;

    if (props.referenceId) {
      // Check store for existing reference
      const reference = applicationStore.workExperienceReferenceById(
        props.referenceId,
      );

      if (!reference) {
        router.back();
      }
    }
    formStore.initializeForm({});

    return {
      applicationStore,
      alertStore,
      reference,
      formStore,
      loadingStore,
      workExperienceReferenceUpsertForm,
      router,
      applicationStatus,
    };
  },
  data() {
    return { isDuplicateReference: false };
  },
  computed: {
    applicationType() {
      return this.applicationStatus?.applicationType!;
    },
  },

  methods: {
    async handleSubmitReference() {
      // Validate the form
      const { valid } = await (
        this.$refs.upsertWorkExperienceReferenceForm as typeof EceForm
      ).$refs[workExperienceReferenceUpsertForm.id].validate();

      if (valid) {
        //check for duplicate reference
        this.isDuplicateReference = false;
        const refSet = new Set<string>();

        if (this.applicationStatus?.characterReferencesStatus) {
          for (const ref of this.applicationStatus.characterReferencesStatus) {
            if (ref.status !== "Rejected") {
              refSet.add(
                `${ref.firstName?.toLowerCase()} ${ref.lastName?.toLowerCase()}`,
              );
            }
          }
        }

        if (
          refSet.has(
            `${this.formStore.formData.firstName.toLowerCase()} ${this.formStore.formData.lastName.toLowerCase()}`,
          )
        ) {
          this.isDuplicateReference = true;
          //scroll to top of page
          globalThis.scrollTo({
            top: 0,
            behavior: "smooth",
          });
        } else {
          const { error } = await upsertWorkExperienceReference(
            {
              application_id: this.applicationId,
              reference_id: this.referenceId,
            },
            this.formStore.formData,
          );
          if (error) {
            this.alertStore.setFailureAlert(
              "Sorry, something went wrong and your changes could not be saved. Try again later.",
            );
          } else {
            this.alertStore.setSuccessAlert(
              "Reference updated. We sent them an email to request a reference.",
            );
            this.router.push({
              name: "manageWorkExperienceReferences",
              params: { applicationId: this.applicationId },
            });
          }
        }
      } else {
        this.alertStore.setFailureAlert(
          "You must enter all required fields in the valid format to continue.",
        );
      }
    },
  },
});
</script>
