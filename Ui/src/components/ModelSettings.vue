<script setup lang="ts">
import { Icon } from '@iconify/vue';
import type { IModelSettings } from '@/types';
import { onMounted, watch, ref, reactive } from 'vue';
import { models, prompts } from '@/constants';
import { useApiStore } from '@/stores/api';
import { countTokens, getTemperatureString, getSystemPromptTokens } from '@/utils'
const store = useApiStore()

interface CustomComponentProps {
    modelValue: IModelSettings,
}
const props = defineProps<CustomComponentProps>()
const modelValue = reactive(props.modelValue)
const emits = defineEmits(['update:modelValue'])

const systemTokenCount = ref(0)

const customPromptShown = ref(props.modelValue.systemPrompt === "custom" ? true : false);
watch(modelValue, (newP, oldP) => {
    console.log("modelValue changed", newP, oldP)

    // if (newVal.systemPrompt === "custom") {
    //     customPromptShown.value = true;
    // }
    // else {
    //     customPromptShown.value = false;
    // }
    // emits('update:modelValue', props.modelValue)
});
// watch(props.modelValue, async () => {
//     console.log("props changed")
//     if (props.modelValue.systemPrompt === "custom") {
//         customPromptShown.value = true;
//     }
//     else {
//         customPromptShown.value = false;
//     }
//     systemTokenCount.value = await countTokens(getSystemPromptTokens(props.modelValue));
//     if (props.modelValue.systemPrompt === "default") {
//         systemTokenCount.value = 0;
//     }
//     else {
//         systemTokenCount.value = systemTokenCount.value + 1;
//     }

//     emits('update:modelValue', props.modelValue)
// })


function updateValue(event: Event) {
    const value = (event.target as HTMLInputElement)?.value
    emits('update:modelValue', value)
}
function clearResponses() {
    store.clearResponses()
}
onMounted(() => {

    // console.log(props.modelValue)
})

</script>
<template>
    <div class="container">
        <section>
            <label for="model">Model
                <p>$0.002 / 1K tokens</p>
                <select v-model="modelValue.model" name="model" id="">
                    <option v-for="[key, val] in Object.entries(models)" :value="key" :disabled="key != 'gpt-3.5-turbo'">{{
                        val }}</option>

                </select>
            </label>
            <label for="system">Persona
                <p>Token count: {{ systemTokenCount }}</p>
                <select v-model="modelValue.systemPrompt" name="system" id="">

                    <option v-for="[key, prompt] in Object.entries(prompts)" :value="key">{{ prompt.name }}</option>

                    <!-- <option value="default">default</option>
                                                                                                                                                                                                                                                        <option value="chadgbd">chadgbd</option>
                                                                                                                                                                                                                                                        <option value="system3">system3</option> -->
                </select>
            </label>
            <label style="min-width: 124px;" for="temp">Temperature: {{ modelValue.temperature }}
                <p>{{ getTemperatureString(parseInt(modelValue.temperature)) }}</p>
                <input class="slider" v-model="modelValue.temperature" name="temp" type="range" min="0" max="2" step="0.1">
            </label>
            <label style="min-width: 129px;" for="maxTokens">Max tokens: {{ modelValue.maxTokens }}
                <p>Request + max cannot exceed 4096</p>
                <input v-model="modelValue.maxTokens" name="maxTokens" type="range" min="0" max="4096" step="1">
            </label>

            <label style="min-width: 172px;width:172px;" for="presencePenalty">Presence Penalty: {{
                modelValue.presencePenalty }}
                <input v-model="modelValue.presencePenalty" name="presencePenalty" type="range" min="-2" max="2" step="0.1">
            </label>
            <label style="min-width: 172px;width:172px;" for="frequencyPenalty">Frequency Penalty: {{
                modelValue.frequencyPenalty }}
                <input v-model="modelValue.frequencyPenalty" name="frequencyPenalty" type="range" min="-2" max="2"
                    step="0.1">
            </label>

            <label for="history">Include history
                <div>
                    <input v-model="modelValue.includeHistory" name="history" type="checkbox">
                </div>
            </label>

            <label @click.prevent="">Clear History
                <div class="center">
                    <button @click="clearResponses">
                        <Icon icon="grommet-icons:clear" />
                    </button>
                </div>
            </label>
            <label v-if="customPromptShown" class="text">Custom Prompt
                <custom-scrollbar class="scroll" style="width: 100%; height: 100%;">
                    <textarea style="width: 100%;" v-model="modelValue.customPrompt" name="prompt"></textarea>
                </custom-scrollbar>
            </label>
        </section>
    </div>
</template>


<style scoped>
.v-enter-active,
.v-leave-active {
    transition: transform .5s ease;
    overflow: hidden;
    height: auto;
}

.v-enter-from,
.v-leave-to {
    transform: translateY(100%);
}

:deep(.scrollbar__wrapper .scrollbar__content) {
    height: 100%;
}


.text>div:first-of-type {
    width: 100%;
    height: 100%;
    display: flex;
}

.scroll {
    display: flex;
    flex-grow: 1;
    width: 100%;
}

.center {
    display: flex;
    justify-content: center;
}

textarea {
    /* overflow: auto; */
    box-sizing: border-box;
    display: block;
    width: 100%;
    height: 100%;
    /* height: 300px; */
    /* height: 200px; */
    /* resize: vertical; */
    padding: .75rem 1rem;
    font-family: Roboto Mono, monospace;
    font-size: 1rem;
    resize: none;
    /* margin: 1rem; */
    color: rgba(255, 255, 255, .75);
    border: none;
    background-color: rgba(255, 255, 255, .05);
    border: 2px solid rgba(255, 255, 255, .25);
}

textarea:focus-visible {
    outline: none !important;
    border: 2px solid var(--color-green-muted);
}

.text {
    width: 100%;
    flex-basis: 100%;
    height: 300px;
    /* overflow: scroll; */
    /* padding-left: 1rem; */
}

button {
    color: red;
    font-size: 1.5rem;
    height: 1.5rem;
    /* margin-top: .25rem; */

}

.container {
    background-color: #2e2e2e;
    position: fixed;
    bottom: 4rem;

    width: calc(100% - 2rem);
    display: flex;
    justify-content: center;
    z-index: 10;
    /* border: 2px solid rgb(69, 133, 136, 1); */
    border: 2px solid #458588;
    /* border: 2px solid rgba(255, 255, 255, .5); */
    border-bottom: 0;
    box-sizing: border-box;
    max-width: 1280px;
}

section {
    /* width: calc(100vw - 4rem); */
    display: flex;
    gap: 1.5rem;
    /* align-items: flex-end; */
    justify-content: space-between;
    flex-wrap: wrap;
    padding: 1rem 0;
    max-width: 1280px;
    /* background-color: rgba(255, 255, 255, .05); */
    overflow: hidden;
    margin: 0 1rem;
}

section input,
section select {
    width: 100%;
}

label {
    align-items: center;
    justify-content: space-between;
    display: flex;
    gap: .5rem;
    flex-direction: column;
    color: rgba(255, 255, 255, .85);
    flex-grow: 1;
    text-align: left;
    flex-basis: 112px;
    white-space: nowrap;
    font-weight: bold;
}

label p {
    font-size: .75rem;
    font-weight: normal;
    color: rgba(255, 255, 255, .5);
    margin: 0;
    margin-top: -.25rem;
    margin-bottom: -.25rem;
    padding: 0;
    line-height: 1.2;
    text-align: center;
    white-space: pre-wrap;
    width: 100%;
}

label :nth-child(1) {
    /* background-color: red; */
    /* overflow: hidden; */
}
</style>