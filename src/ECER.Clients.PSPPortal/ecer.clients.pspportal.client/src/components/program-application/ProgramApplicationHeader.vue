<template>
  <v-container fluid class="bg-primary">
    <v-container>
      <v-row>
        <v-col
          :class="[
            mobile ? 'flex-column' : 'justify-space-between',
            'd-flex',
            'align-center',
          ]"
        >
          <h1>Application: {{ programApplicationName }}</h1>
          <div :class="[{ ['text-right mb-2']: mobile }]">
            <v-btn id="btnSaveAndExit" variant="outlined" @click="">
              Save and exit
            </v-btn>
          </div>
        </v-col>
      </v-row>
    </v-container>
  </v-container>
</template>

<script lang="ts">
import { defineComponent } from "vue";
import { useDisplay } from "vuetify";

export default defineComponent({
  name: "ProgramApplicationHeader",
  components: {},
  props: {
    programApplicationName: {
      type: String,
      required: true,
    },
  },
  setup() {
    const { mobile } = useDisplay();

    return {
      mobile,
    };
  },
});
</script>
