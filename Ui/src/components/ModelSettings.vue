
<script setup lang="ts">
import { Icon } from '@iconify/vue';
import type { IModelSettings } from '@/types';
import { watch } from 'vue';
import { models, prompts } from '@/constants';
import { useApiStore } from '@/stores/api';

const store = useApiStore()

interface CustomComponentProps {
    modelValue: IModelSettings,
}
const props = defineProps<CustomComponentProps>()

const emits = defineEmits(['update:modelValue'])


watch(props, () => {
    // console.log(props.modelValue)
    emits('update:modelValue', props.modelValue)
})

function updateValue(event: Event) {
    const value = (event.target as HTMLInputElement)?.value
    emits('update:modelValue', value)
}
function clearResponses() {
    store.clearResponses()
}

</script>
<template>
    <div class="container">
        <section>
            <label for="model">Model
                <select v-model="modelValue.model" name="model" id="">
                    <option v-for="[key, val] in Object.entries(models)" :value="key">{{ val }}</option>
                    <!-- <option value="gpt-3.5-turbo">gpt-3.5-turbo</option>
                                <option value="babbage">babbage</option>
                                <option value="davinci">davinci</option>
                            <option value="ada">ada</option> -->
            </select>
        </label>
        <label for="system">System Prompt
            <select v-model="modelValue.systemPrompt" name="system" id="">

                    <option v-for="prompt in Object.keys(prompts)" :value="prompt">{{ prompt }}</option>

                    <!-- <option value="default">default</option>
                        <option value="chadgbd">chadgbd</option>
                        <option value="system3">system3</option> -->
                </select>
            </label>
            <label for="temp">Temperature: {{ modelValue.temperature }}
                <input class="slider" v-model="modelValue.temperature" name="temp" type="range" min="0" max="2" step="0.1">
            </label>
            <label for="maxTokens">Max tokens: {{ modelValue.maxTokens }}
                <input v-model="modelValue.maxTokens" name="maxTokens" type="range" min="0" max="4096" step="1">
            </label>

            <label style="width: 164px;" for="presencePenalty">Presence Penalty: {{ modelValue.presencePenalty }}
                <input v-model="modelValue.presencePenalty" name="presencePenalty" type="range" min="-2" max="2" step="0.1">
            </label>
            <label style="width: 164px;" for="frequencyPenalty">Frequency Penalty: {{ modelValue.frequencyPenalty }}
                <input v-model="modelValue.frequencyPenalty" name="frequencyPenalty" type="range" min="-2" max="2"
                    step="0.1">
            </label>

            <label for="history">Include history
                <input v-model="modelValue.includeHistory" name="history" type="checkbox">
            </label>
            <!-- <label for="topk">Top k
                                        <input name="topk" type="number" min="1" max="100">
                                    </label>
                                    <label for="topp">Top p
                                        <input name="topp" type="number" min="0" max="1" step="0.01">
                                    </label> -->
            <label>Clear History
                <button @click="clearResponses">
                    <Icon icon="grommet-icons:clear" />
                </button>
            </label>
        </section>
    </div>
</template>

<style scoped>
button {
    color: red;
    font-size: 1.5rem;
    height: 1.5rem;
    margin-top: .25rem;

}

.container {
    background-color: #2e2e2e;
    position: fixed;
    bottom: 4rem;

    width: 100%;
    display: flex;
    justify-content: center;
}

section {
    width: calc(100vw - 2rem);
    display: flex;
    gap: 1.5rem;
    /* align-items: flex-end; */
    justify-content: space-between;
    flex-wrap: wrap;
    padding: 1rem 0;
    max-width: 1280px;
    /* background-color: rgba(255, 255, 255, .05); */
    overflow: hidden;

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
    color: white;
    flex-grow: 1;
    text-align: left;
    flex-basis: 120px;
    white-space: nowrap;
}
</style>