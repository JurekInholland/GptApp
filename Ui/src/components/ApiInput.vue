<script setup lang="ts">
import { Icon } from '@iconify/vue';

import { computed, ref } from 'vue'
import CodeBlock from './CodeBlock.vue';
interface CustomComponentProps {
    modelValue: string
}
const props = defineProps<CustomComponentProps>()


// const query = ref('')

// const change = async (query: String) => {
//     console.log(query)
// }
// const tokenCount = computed(() => {
//     return query.value.length / 4;
// });
// const queryCost = computed(() => {
//     return (query.value.length / 4) * 0.000002;
// });

const emits = defineEmits({
    'update:modelValue': (value: string) => true,
    'submit': () => true,
})
function updateValue(event: Event) {
    const value = (event.target as HTMLInputElement)?.value
    emits('update:modelValue', value)
}
function onButtonClick() {
    emits('submit')
}
</script>

<template>
    <div style="position: relative; display: flex;">
        <div class="background">
            <!-- <CodeBlock :code="query" /> -->
            <div class="input">
                <input @keyup.enter="onButtonClick" placeholder="ask me something BASED" :value="modelValue"
                    @input="updateValue($event)">
                <button :disabled="modelValue == ''" @click="onButtonClick">
                    <Icon style="font-size: 2rem;" icon="carbon:send" />
                </button>
            </div>
        </div>
    </div>
    <!-- <p>Tokens: ~{{ tokenCount.toFixed(0) }}</p>
                            <p>Cost: ~{{ queryCost.toFixed(6) }} €</p> -->
</template>

<style scoped>
.background {
    display: flex;
    align-items: center;
    position: fixed;
    flex-grow: 1;
    width: 100%;
    /* width: calc(100vw - 2rem); */
    bottom: 0;
    left: 0;
    background-color: rgb(62, 62, 62);
    /* width: 100%; */
    height: 4rem;
    /* padding: 0 1rem; */
}

input {
    /* flex-grow: 1; */
    /* height: 1rem; */
    width: 100%;
    border: none;
    outline: none;
    font-size: 1rem;
    background-color: transparent;
    color: white;

}

input::placeholder {
    color: rgba(255, 255, 255, 0.5);
    transition: color 0.1s ease-in-out;
}

.input {
    width: calc(100% - 2rem);

    /* flex-grow: 1; */
    height: auto;
    max-width: 1280px;
    max-height: 3rem;
    overflow: hidden;
    /* border-radius: 12px; */
    border: 2px solid var(--color-green-muted);
    box-sizing: border-box;
    padding: .5rem 1rem;
    background: rgba(0, 0, 0, 0.33);
    display: flex;
    margin: 0 auto;
    bottom: 0;
    /* position: sticky;
    bottom: 1rem; */
    transition: border 0.1s ease-in-out;
}

.input:focus-within {
    border: 2px solid var(--color-green);

}

button {
    transition: color 0.2s ease-in-out;
    color: rgba(255, 255, 255, 0.5);
}

.input:focus-within button {
    color: var(--color-green-muted);
}

.input button:hover {
    color: var(--color-green);
}

.input button:disabled {
    color: rgba(255, 255, 255, 0.25);
    cursor: default;
}

.input:focus-within input::placeholder {
    color: rgba(255, 255, 255, 0.75);
}</style>