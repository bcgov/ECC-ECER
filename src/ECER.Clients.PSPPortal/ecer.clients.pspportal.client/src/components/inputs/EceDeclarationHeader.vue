<template>
  <h2>Confirmation</h2>
  <br />
  <p>Submitting your program profile will send it to the ECE Registry to complete their review.</p>
  <br />
  <p>Please read the following and complete the required fields below to submit this program profile to the ECE Registry.</p>
  <br />
  <ul class="ml-10">
    <li>I confirm that the information provided in this submission is complete and accurate</li>
    <li>I understand that the ECE Registry may require additional information to complete their review</li>
    <li>I understand that I am submitting this information on behalf of [name of institution]</li>
  </ul>
</template>
<script lang="ts">
import { defineComponent } from "vue";

import { formatDate } from "@/utils/format";

import PreviewCard from "../common/PreviewCard.vue";

export default defineComponent({
  name: "EceDeclarationHeader",
  components: {},
  setup: () => {
    return {};
  },
  computed: {},
});
</script>
