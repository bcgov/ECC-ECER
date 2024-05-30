<template>
  <h2>Resend email</h2>
  <p>
    We’ll send another email to the reference. It’ll include a link to the My ECE Registry to provide a reference for you. It may take a few minutes to receive
    the email.
    <br />
    If they do not receive the email:
  </p>
  <ul class="ml-10">
    <li>Ask the person to check their spam folder</li>
    <li>Check you provided the correct email address</li>
  </ul>
  <p>If you need to correct any information, choose to use a new reference below.</p>
</template>

<script lang="ts">
import { defineComponent } from "vue";

export default defineComponent({
  name: "ResendEmail",
});
</script>
