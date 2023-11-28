<template>
  <div class="weather-component">
    <h1>Login</h1>

    <div>
      <button type="button" @click="console.log('hello')">Login</button>
    </div>
  </div>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "Login",
});
</script>
