<template>
  <v-container fluid class="bg-primary d-print-none">
    <v-container>
      <v-row>
        <v-col
          :class="[
            mobile ? 'flex-column' : 'justify-space-between',
            'd-flex',
            'align-center',
          ]"
        >
          <span class="application-heading">
            Application: {{ programApplicationName }}
          </span>
          <div :class="[{ ['text-right mb-2']: mobile }]">
            <v-btn id="btnSaveAndExit" variant="outlined" @click="exit">
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
  methods: {
    exit() {
      this.$router.push("/");
    },
  },
});
</script>
<style>
.application-heading {
  font-size: 24px;
  font-weight: 700;
}
</style>
