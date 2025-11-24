<template>
  <div class="ui container" style="padding-top: 40px; padding-bottom: 40px;">
    <h1 class="ui header" style="font-size: 36px; color:#1e3a8a; text-align:center;">Your Shortened Links</h1>
    <p style="text-align:center; color: gray; margin-bottom: 40px;">Manage and track all links you've shortened</p>

    <div class="ui stackable two column grid">
      <div class="column" v-for="item in ShortUrls" :key="item.shortId">
        <div class="ui raised segment" style="border-radius: 14px;">

          <div class="ui grid">
            <div class="twelve wide column">
              <p class="ui header" style="font-size: 16px; color:#2563eb;">{{ item.shortUrl }}</p>
            </div>
            <div class="four wide column" style="text-align:right;">
              <i class="copy icon link" @click="copy(item.shortUrl)" style="cursor:pointer;"></i>
              <a :href="item.shortUrl" target="blank">
                <i class="external alternate icon"></i>
              </a>
              <i class="edit icon" @click="editurl(item)" style="cursor:pointer"></i>
            </div>
            
          </div>

          <p style="font-size: 13px; color:#374151; word-break: break-all;">{{ item.originalUrl }}</p>

          <div class="ui grid" style="margin-top:10px;">
            <div class="eight wide column">
              <strong>{{ new Date(item.createdAt || Date.now()).toLocaleString() }}</strong>
            </div>
          </div>
          <div style="text-align:right; margin-top:6px;">
            <i class="trash alternate outline icon"
                style="cursor:pointer; color:#dc2626;"
                @click="deleteUrl(item.shortId)">
            </i>
          </div>

        </div>
      </div>
    </div>
    <editModal v-if="showModal" :data="editData" @close="showModal = false" @update="refreshShortUrl"></editModal>
  </div>
</template>

<script>
import { deleteUrl, editUrl, getHistory } from "@/helpers/api";
import editModal from "@/components/editModal.vue";
export default {
  data() {
    return { 
      ShortUrls: [],
      showModal: false,
      editData: {}
    }
  },
  components: {editModal},
  async mounted() {
    try {
      const res = await getHistory();
      this.ShortUrls = res.data;
    } catch (err) {
      console.log("err:",  err.response)
      alert("Login to view history");
    }
    try {
      
    } catch (err) {
      console.log("err:", err.response)
      alert("Login to edit")
    }
  },
  methods: {
    copy(item) {
      navigator.clipboard.writeText(item)
        .then(() => this.$root.$refs.toast.trigger("Copied!", "success"))
        .catch(() => this.$root.$refs.toast.trigger("Failed to copy", "error"))
    },
    editurl(item){
      this.editData = {...item}
      this.showModal = true
    },
    async refreshShortUrl(updatedUrl){
      this.showModal = false
      try {
        await editUrl(this.editData.shortId, updatedUrl.shortId)

        const res = await getHistory()
        this.ShortUrls = res.data

        this.$root.$refs.toast.trigger("Updated successfully!", "success")

      } catch (err) {
        this.$root.$refs.toast.trigger(err.response?.data || "Update failed", "error")
      }
    },
    async deleteUrl(id){
      try {
        await deleteUrl(id)

        const res = await getHistory()
        this.ShortUrls = res.data

        this.$root.$refs.toast.trigger("Deleted!", "success")

      } catch (err) {
        this.$root.$refs.toast.trigger(err.response?.data || "Delete failed", "error")
      }
    }
  }
}
</script>
