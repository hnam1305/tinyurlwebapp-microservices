<template>
  <div class="home-container">

    <div class="background">
      <form>

        <div class="overlay-content">
          <h1 class="main-title">Shorten your URLs</h1>
          <p class="subtitle">Shorten your links to make them more manageable and shareable</p>

          <div class="input url-input">
            <input type="text" placeholder="Enter your URL" v-model="inputUrl" />
            <input class="alias" type="text" placeholder="Alias (Optional)" v-model="alias" />
          </div>

          <br />

          <button type="button" class="ui button primary" @click="shorten">Shorten</button>

          <br /><br />

          <div v-if="showUrl" class="shortUrl">
            <input type="text" :value="shortenedUrl" readonly ref="shortInput" />
            <button class="ui button primary" @click="copyUrl">Copy</button>
          </div>

        </div>

      </form>
    </div>

  </div>
</template>

<script>
import { createShortUrl } from "@/helpers/api";
import "@/styles/home.css";

export default {
  name: "Home",

  data() {
    return {
      inputUrl: "",
      alias: "",
      shortenedUrl: "",
      showUrl: false,
    };
  },

  methods: {
    async shorten() {
      if (!this.inputUrl) {
        this.$root.$refs.toast.trigger("Please enter a URL", "error")
        return;
      }

      try {
        const res = await createShortUrl(this.inputUrl, this.alias)
        this.shortenedUrl = res.data.shortUrl
        this.showUrl = true

        this.$root.$refs.toast.trigger("URL shortened!", "success")

      } catch (err) {
        this.$root.$refs.toast.trigger(err?.response?.data || "Failed to create short URL", "error")
      }
    },

    copyUrl() {
      const input = this.$refs.shortInput
      input.select()
      document.execCommand("copy")

      this.$root.$refs.toast.trigger("Copied!", "success")
    }
      },
};
</script>
