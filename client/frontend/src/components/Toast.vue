<template>
  <div v-if="show" class="toast" :class="type">
    {{ message }}
  </div>
</template>

<script>
export default {
  data() {
    return {
      show: false,
      message: "",
      type: ""
    };
  },
  methods: {
    trigger(msg, type = "success") {
      this.message = msg
      this.type = type
      this.show = true
      setTimeout(() => this.show = false, 2500)
    }
  }
}
</script>

<style scoped>
.toast {
  position: fixed;
  top: 20px;
  right: 20px;
  background: #2563eb;
  color: white;
  padding: 12px 18px;
  border-radius: 6px;
  box-shadow: 0 4px 12px rgba(0,0,0,0.15);
  animation: fadeIn .2s ease-out;
  z-index: 3000;
  font-weight: 600;
}

.toast.error { background: #dc2626; }
.toast.success { background: #16a34a; }

@keyframes fadeIn {
  from { opacity: 0; transform: translateY(-10px); }
  to   { opacity: 1; transform: translateY(0); }
}
</style>
