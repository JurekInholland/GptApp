<script setup lang="ts">
import Markdown from 'markdown-it';
import hljs from 'highlight.js';
import 'highlight.js/styles/base16/gruvbox-dark-medium.css';

import { computed, ref, watch } from 'vue';
import type { IAPIResponse } from '@/types';

const message = ref();

// const scrollDown = () => {
//     message.value?.scrollIntoView(false, { behavior: 'smooth' })
// };
const props = defineProps<{
  response: IAPIResponse
}>();

// watch(props, () => {
//     scrollDown();
// });

const md = Markdown({
  breaks: true,
  highlight: (
    str: string,
    lang: string
  ) => {

    const highlighted = lang
      ? hljs.highlight(str, { language: lang, ignoreIllegals: true }).value
      : hljs.highlightAuto(str).value;
    return `<pre  class="hljs"><code>${highlighted}</code></pre>`;
  },
});
// md.use(mdBr);

const content = computed(() => {
  return md.render(props.response.message.replaceAll("based", "`based`").replaceAll("Based", "`Based`"));
});
</script>


<template>
  <div style="position: relative;">
    <h3 class="author">{{ props.response.role === 'bot' ? 'chad gbd' : 'user' }}</h3>
    <div :class="props.response.role === 'user' ? 'human' : 'bot'" ref="message" class="message" v-html="content" />
  </div>
</template>
<style>
.author {
  position: absolute;
  margin: 0;
  padding: 0;
  top: -10px;
  font-size: 1.25rem;
  color: white;
  /* background-color: #222; */
}

code {
  text-align: left;
  color: var(--color-green);
  font-weight: bold;
  font-family: "Roboto Mono";

}

.hljs {
  margin: 1rem;
  padding: 1rem;
  line-height: 1.33rem;
  overflow-x: auto;

  /* white-space: pre-wrap; */

}

.hljs code {
  color: unset;
  font-weight: unset;

}

.human {
  background-color: #222;
  border: 2px solid var(--color-green);

}

.bot {
  background-color: #444;
  border: 2px solid rgba(223, 191, 142, .75);

}

.message {
  /* border-radius: 25px; */
  padding: .1rem 1rem;
  color: #eed9c1;
  /* font-family: system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, 'Open Sans', 'Helvetica Neue', sans-serif; */
  font-size: 18px;
  line-height: 1.5rem;
  box-sizing: border-box;
  /* letter-spacing: .2px; */
}

/* .message p {
    max-width: 900px
} */
</style>
