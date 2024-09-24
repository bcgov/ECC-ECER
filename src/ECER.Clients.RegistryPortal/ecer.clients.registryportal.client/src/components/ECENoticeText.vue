<template>
  <div class="line-with-text mt-10">
    <div class="vertical-line"></div>
    <p>{{ text }}</p>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";
export default defineComponent({
  name: "ECENoticeText",
  props: {
    text: {
      type: String,
      required: true,
    },
  },
});
</script>
<style>
.line-with-text {
  display: flex;
  align-items: center;
}

.vertical-line {
  width: 6px;
  height: 30px; /* Set a fixed height */
  background-color: #ffc72c;
  margin-right: 10px; /* Space between the line and the text */
}
</style>
