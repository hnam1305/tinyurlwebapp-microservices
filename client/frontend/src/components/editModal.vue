<template>
  <div class="modal-overlay">
    <div class="edit-modal">
      <div class="modal-header">
        Edit Short URL
      </div>

      <div class="modal-body">
        <div class="field">
          <label>Short URL:</label>
          <div class="url-display">{{ selectedUrl.shortUrl }}</div>
        </div>

        <div class="field">
          <label>New ID:</label>
          <input type="text" v-model="selectedUrl.shortId" class="modal-input" />
        </div>
      </div>

      <div class="modal-actions">
        <button class="ui button" @click="$emit('close')">Cancel</button>
        <button class="ui primary button" @click="updateUrl">Save</button>
      </div>
    </div>
  </div>
</template>


<script>
export default {
  name: "editModal",
  props: {
    data: {
      type: Object,
      required: true
    }
  },
  data() {
    return {
      selectedUrl: {}
    };
  },
  mounted() {
    this.selectedUrl = JSON.parse(JSON.stringify(this.data));
  },
  methods: {
    updateUrl() { 
      let payload = JSON.parse(JSON.stringify(this.selectedUrl));
      this.$emit("update", payload);
    }
  }
};
</script>
<style scoped>
.modal-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background: rgba(0,0,0,0.4);
  display: flex;
  justify-content: center;
  align-items: center;
  z-index: 2000;
}

.edit-modal {
  background: white;
  width: 450px;
  border-radius: 12px;
  box-shadow: 0 10px 30px rgba(0,0,0,0.2);
  overflow: hidden;
  animation: popup 0.2s ease-out;
}

.modal-header {
  font-size: 22px;
  font-weight: bold;
  padding: 18px 24px;
  border-bottom: 1px solid #eee;
}

.modal-body {
  padding: 20px 24px;
}

.field {
  margin-bottom: 16px;
}

label {
  font-weight: 600;
  color: #444;
}

.url-display {
  font-size: 14px;
  color: #555;
  margin-top: 4px;
  word-break: break-all;
}

.modal-input {
  width: 100%;
  margin-top: 6px;
  padding: 10px;
  border: 1px solid #d1d5db;
  border-radius: 6px;
  font-size: 15px;
  transition: 0.2s;
}

.modal-input:focus {
  border-color: #2563eb;
  box-shadow: 0 0 0 2px rgba(37,99,235,0.2);
  outline: none;
}

.modal-actions {
  display: flex;
  justify-content: flex-end;
  gap: 10px;
  padding: 14px 24px;
  background: #fafafa;
  border-top: 1px solid #eee;
}

@keyframes popup {
  from {
    transform: scale(0.9);
    opacity: 0;
  }
  to {
    transform: scale(1);
    opacity: 1;
  }
}
</style>
