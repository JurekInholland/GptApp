<script setup lang="ts">
import Markdown from 'markdown-it';
import hljs from 'highlight.js';
import 'highlight.js/styles/base16/gruvbox-dark-medium.css';

import { computed, ref, watch } from 'vue';
import type { IAPIResponse } from '@/types';
import { getTotalTokens, getFormattedTokenCount } from '@/utils';
import { Icon } from '@iconify/vue';
import Spinner from './Spinner.vue';

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
  let message = "";
  let dots = "";
  if (props.response.status === 'incomplete') {
    dots = " ...";
    message = "<p style='font-style: italic; color: #d75f5f'><i>Max. token limit reached.</i></p>"
  }
  return md.render(props.response.message.replaceAll("based", "`based`").replaceAll("Based", "`Based`") + dots) + message;
});

const toggleInclusion = () => {
  props.response.included = !props.response.included;
};

</script>


<template>
  <div class="container"
    :class="[response.included ? '' : 'hidden', props.response.role === 'user' ? 'green' : 'beige', response.status === 'error' ? 'error-msg' : '']"
    style="position: relative;">
    <div class="message" :class="[props.response.role === 'user' ? 'human' : 'bot']">
      <div class="top-bar">
        <div class="left-section">
          <p class="author">{{ props.response.role === 'bot' ?
            'chad gbd' : 'user' }}</p>
          <Spinner :size=".75" v-if="response.status === 'pending'" />
          <Icon class="error"
            :icon="response.status !== 'complete' && response.status !== 'pending' ? 'mdi:error' : ''" />
        </div>
        <div class="right-section">
          <p v-if="getTotalTokens(response.tokenCount) > 0" :style="response.tokenCount === null ? 'opacity: 0;' : ''"
            :class="response.included ? '' : 'hidden'">
            {{ getFormattedTokenCount(response) }} tokens</p>
          <button @click="toggleInclusion">
            <Icon :icon="response.included ? 'mdi:hide' : 'mdi:show'" />
          </button>

        </div>
      </div>
      <div ref="message" v-html="content" />
    </div>
  </div>
</template>
<style >
.error-msg h2 {
  /* background-color: red !important;
  background: blue!important; */
  color: #d75f5f;
}

.error {
  color: #d75f5f;
  font-size: 1.25rem;
  margin-left: .5rem;
}

.container {
  display: flex;
  flex-direction: column;
  position: relative;
}

.top-bar {
  display: flex;
  align-items: center;
  justify-content: center;
  z-index: -1;
  justify-content: space-between;
  height: 1.5rem;
  padding-top: .75rem;
  /* line-height: 0rem; */
  margin-bottom: .75rem;

}

.left-section {
  display: flex;
  flex-direction: row;
  align-items: center;
  justify-content: center;
  /* gap: .5rem; */
  line-height: unset;
}

.right-section {
  display: flex;
  /* flex-direction: row; */
  /* justify-content: space-between; */
  align-items: center;
  justify-content: center;
  gap: .5rem;
  /* margin-top: .5rem; */
  line-height: unset;
  max-height: 28px;
}

.right-section p {
  margin: 0;
  color: unset;
  opacity: .5;
  font-size: .8rem;
  text-decoration: none !important;
  transition: all .2 ease;
}

.right-section button {
  margin-top: .45rem;
}

p.hidden {
  opacity: .1 !important;
}

button {
  font-size: 1.5rem;
  color: rgba(255, 255, 255, .1);

}

.hidden button {
  color: rgba(255, 255, 255, .5);

}

button:hover {
  color: rgba(255, 255, 255, .75);
}

/* .grey button {
  opacity: 0;
  pointer-events: 0;
} */

.author {
  display: flex;
  margin: 0;
  padding: 0;
  /* position: absolute;

  top: -10px; */
  /* font-size: 1.25rem; */
  font-weight: bold;
  color: white;
  text-shadow: 0px 3px 3px rgba(0, 0, 0, .75);
}

.green {
  color: var(--color-green);
  /* background-color: #222; */
}

.beige {
  color: #eed9c1;
  /* background-color: #444; */
}

code {
  text-align: left;
  color: var(--color-green);
  font-weight: bold;
  font-family: "Roboto Mono";
  white-space: pre-wrap;

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
  background-color: #2f2f2f;
  border: 2px solid rgba(223, 191, 142, .75);

}

.message {
  /* border-radius: 25px; */
  padding: .1rem .75rem;
  /* color: #eed9c1; */
  /* font-family: system-ui, -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, Oxygen, Ubuntu, Cantarell, 'Open Sans', 'Helvetica Neue', sans-serif; */
  font-size: 18px;
  /* line-height: 1.5rem; */
  box-sizing: border-box;
  /* letter-spacing: .2px; */
  line-height: 28px;
  transition: all .2s ease;

}

.message>div>p {
  color: #eed9c1;
}

.hidden .message,
.hidden .author,
.hidden p {
  text-decoration: line-through;
  /* opacity: .9; */
  opacity: .33;
  /* text-decoration-color: rgb(238, 217, 193);
  color: rgba(238, 217, 193, .5);
  border-color: rgba(238, 217, 193, .33); */
  /* background-color: rgba(255, 255, 255,.1) !important; */
}

:deep(.message p) {
  max-width: calc(100% - 1.5rem);
  padding-top: .33rem;
}

:deep(span.span) {
  color: red !important;
}
</style>
