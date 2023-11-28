<script setup lang="ts"></script>

<template>
  <main>
    <p>
      <router-link to="/">Go to Home</router-link>
    </p>
    <p>
      <router-link to="/login">Go to Login</router-link>
    </p>
    <router-view></router-view>
  </main>
</template>
