<template>
  <div v-if="show" class="message-overlay" :class="{ 'show': show }" @click="close">
    <div class="message-box" @click.stop>
      <p>{{ message }}</p>
      <button @click="close">確定</button>
    </div>
  </div>
</template>

<script setup lang="ts">
import { defineProps, defineEmits } from 'vue';

const props = defineProps({
  message: {
    type: String,
    required: true,
  },
  show: {
    type: Boolean,
    required: true,
  },
});

const emit = defineEmits(['close']);

const close = () => {
  emit('close');
};
</script>
