<template>
  <v-form ref="form" validate-on="input" @submit.prevent>
    <!-- Location info -->
    <v-row class="mt-4">
      <v-col cols="12">
        <h2>Location info</h2>
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12" md="8" lg="6" xl="4">
        <EceTextField
          v-model="name"
          label="Location name"
          :maxLength="100"
          :rules="[Rules.required()]"
        />
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12" md="8" lg="6" xl="4">
        <EceTextField
          v-model="street1"
          label="Street address"
          :maxLength="100"
          :rules="[Rules.required()]"
        />
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12" md="8" lg="6" xl="4">
        <EceTextField v-model="street2" label="Address 2" :maxLength="100" />
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12" md="8" lg="6" xl="4">
        <EceTextField v-model="street3" label="Address 3" :maxLength="100" />
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12" md="8" lg="6" xl="4">
        <EceTextField
          v-model="city"
          label="City"
          :maxLength="100"
          :rules="[Rules.required()]"
        />
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12" md="4" lg="3" xl="2">
        <EceProvince
          :model-value="province"
          @update:model-value="province = $event"
        />
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12" md="8" lg="6" xl="4">
        <EceTextField
          v-model="postalCode"
          label="Postal code"
          :maxLength="10"
          :rules="[Rules.required(), Rules.postalCode()]"
        />
      </v-col>
    </v-row>

    <!-- Key contact -->
    <v-row class="mt-6">
      <v-col cols="12">
        <h2>Key contact</h2>
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12" md="12">
        <EcePspUser
          :model-value="keyContact"
          :users="pspUsers"
          :institution-name="institutionName"
          :rules="[Rules.required('Select a key contact')]"
          @update:model-value="keyContact = $event ?? ''"
        />
      </v-col>
    </v-row>

    <v-row>
      <v-col cols="12" md="8" lg="6" xl="4">
        <EceTextField
          v-model="otherCampusContact"
          label="Other campus contact"
          :maxLength="100"
        />
      </v-col>
    </v-row>

    <!-- Programs offered (public institutions only) -->
    <template v-if="!isPrivate && programs.length > 0">
      <v-row class="mt-6">
        <v-col cols="12">
          <h2>Programs offered</h2>
        </v-col>
      </v-row>

      <v-row>
        <v-col cols="12">
          <p>Select recognized programs offered at this campus</p>
        </v-col>
      </v-row>

      <v-row>
        <v-col cols="12">
          <v-checkbox
            v-for="(program, index) in programs"
            :key="program.id ?? program.programName ?? index"
            v-model="selectedProgramIds"
            :value="program.id"
            :label="programLabel(program)"
            density="compact"
            hide-details
          />
        </v-col>
      </v-row>
    </template>
  </v-form>
</template>

<script lang="ts">
import { defineComponent, type PropType } from "vue";
import type { VForm } from "vuetify/components";
import EceTextField from "@/components/inputs/EceTextField.vue";
import EceProvince from "@/components/inputs/EceProvince.vue";
import EcePspUser, {
  type PspUserItem,
} from "@/components/inputs/EcePspUser.vue";
import type { Components } from "@/types/openapi";
import * as Rules from "@/utils/formRules";

export default defineComponent({
  name: "CampusForm",
  components: { EceTextField, EceProvince, EcePspUser },
  props: {
    pspUsers: {
      type: Array as PropType<PspUserItem[]>,
      default: () => [],
    },
    institutionName: {
      type: String,
      default: "",
    },
    isPrivate: {
      type: Boolean,
      default: false,
    },
    programs: {
      type: Array as PropType<Components.Schemas.Program[]>,
      default: () => [],
    },
    initialData: {
      type: Object as PropType<Components.Schemas.Campus>,
      default: null,
    },
  },
  data() {
    return {
      name: (this.initialData?.name ?? "") as string,
      street1: (this.initialData?.street1 ?? "") as string,
      street2: (this.initialData?.street2 ?? "") as string,
      street3: (this.initialData?.street3 ?? "") as string,
      city: (this.initialData?.city ?? "") as string,
      province: (this.initialData?.province ?? "") as string,
      postalCode: (this.initialData?.postalCode ?? "") as string,
      keyContact: (this.initialData?.keyCampusContactId ?? "") as string,
      otherCampusContact: (this.initialData?.otherCampusContactName ??
        "") as string,
      selectedProgramIds: [] as string[],
      Rules,
    };
  },
  methods: {
    programLabel(program: Components.Schemas.Program): string {
      const name = program.programName ?? program.name ?? "";
      if (!program.startDate) return name;
      const startYear = new Date(program.startDate).getFullYear();
      return `${name} - ${startYear}/${startYear + 1}`;
    },
    async validate() {
      return (this.$refs.form as VForm).validate();
    },
    getData(): {
      campus: Components.Schemas.Campus;
      selectedProgramIds: string[];
    } {
      return {
        campus: {
          name: this.name || null,
          street1: this.street1 || null,
          street2: this.street2 || null,
          street3: this.street3 || null,
          city: this.city || null,
          province: this.province || null,
          postalCode: this.postalCode || null,
          keyCampusContactId: this.keyContact || null,
          otherCampusContactName: this.otherCampusContact || null,
        },
        selectedProgramIds: this.selectedProgramIds,
      };
    },
  },
});
</script>
