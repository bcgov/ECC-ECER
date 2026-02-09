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
            To begin your application, provide information about the program. When you have completed these questions,
            choose “Continue” to save your responses and create your draft application.
          </p>
          <p>
            You can save and exit the form and continue with your submission later from your PSP Portal dashboard.
          </p>
        </div>
      </v-col>
    </v-row>
    <v-row>
      <v-col cols="12">
        <Callout title="If you need assistance" type="warning">
          <p>
            You may reach out to the Registry for assistance at any time by
            <router-link to="/messages/new">sending a message</router-link>.
          </p>
        </Callout>
      </v-col>
    </v-row>
    <v-form>
      <v-row>
        <v-col cols="12">
          <div class="d-flex flex-column ga-3">
            <h3>Program information</h3>
            <EceTextField label="Program name"></EceTextField>
          </div>
        </v-col>
      </v-row>
      <v-row>
        <v-col>
          <div class="d-flex flex-column ga-3">
            <p class="font-weight-bold">
              The following program information will determine the application questions you need to complete.
              You will not be able to edit these responses later.
            </p>
            <p>
              To apply for a post-basic early childhood education program (ITE and/or SNE), an institution must either:
            </p>
            <ul class="ml-10">
              <li>Already have a recognized basic early childhood education program, or</li>
              <li>Apply for a basic early childhood education and post-basic early childhood education program concurrently.</li>
            </ul>
          </div>
        </v-col>
      </v-row>
      <v-row>
        <v-col>
          <p class="pb-3">
            Provincial certification type
          </p>
          <v-checkbox
            v-model="checkBoxPlaceHolder"
            value="Basic"
            label="Early childhood education (Basic)"
            density="compact"
            hide-details />
          <v-checkbox
            v-model="checkBoxPlaceHolder"
            label="Infant and Toddler Educator (ITE)"
            value="ITE"
            density="compact"
            hide-details />
          <v-checkbox
            v-model="checkBoxPlaceHolder"
            label="Special Needs Educator (SNE)"
            value="SNE"
            density="compact"
            hide-details />
        </v-col>
      </v-row>
      <v-row>
        <v-col>
          <p class="pb-3">
            Delivery method(s)
          </p>
          <v-radio-group
            id="programConfirmationRadio"
            v-model="radiobuttonplaceholder"
            color="primary"
          >
            <v-radio
              label="In-person - All coursework, including faculty practicum supervision, is in person"
              value="Inperson"
            ></v-radio>
            <v-radio
              label="Hybrid - Combination of in-person and online delivery; faculty practicum supervision is in person"
              value="Hybrid"
            ></v-radio>
            <v-radio
              label="Online - All coursework, including faculty practicum supervision, is online"
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
import {defineComponent} from 'vue'
import ECEHeader from "@/components/ECEHeader.vue";
import Callout from "@/components/common/Callout.vue";
import PageContainer from "@/components/PageContainer.vue";
import Breadcrumb from "@/components/Breadcrumb.vue";
import EceTextField from "@/components/inputs/EceTextField.vue";
import EceCheckbox from "@/components/inputs/EceCheckbox.vue";

export default defineComponent({
  name: "BeginProgramApplication",
  components: {EceCheckbox, EceTextField, Breadcrumb, PageContainer, Callout, ECEHeader},
  data() {
    return {
      checkBoxPlaceHolder: [],
      radiobuttonplaceholder: 'InPerson'
    }
  },
  methods: {
    createApplication() {
      console.log('do shit!')
    }
  }
})
</script>
