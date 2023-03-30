<script setup lang="ts">
import { Icon } from '@iconify/vue';
import { computed, ref } from 'vue'
import CodeBlock from './CodeBlock.vue';
import Hamburger from './Hamburger.vue';
interface CustomComponentProps {
    modelValue: string,
    settingsOpen: boolean,
    disabled: boolean,
    tokens: number,
}
const props = defineProps<CustomComponentProps>()

const settingsOpen = ref(false)

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
    'update:settingsOpen': (value: boolean) => true,
    'submit': () => true,
})
function updateValue(event: Event) {
    const value = (event.target as HTMLInputElement)?.value
    emits('update:modelValue', value)
}
function onButtonClick() {
    emits('submit')
}
function onSettingsClick() {
    settingsOpen.value = !settingsOpen.value
    emits('update:settingsOpen', settingsOpen.value)
}
</script>

<template>
    <div style="position: relative; display: flex;">
        <div class="background">
            <div class="content">
                <div class="menu">
                    <Hamburger @change="onSettingsClick" />
                    <!-- <button class="settings" @click="onSettingsClick">
                                                                <Icon style="font-size: 2rem;" icon="carbon:menu" />
                                                            </button> -->
                </div>
                <!-- <CodeBlock :code="query" /> -->
                <div class="input">
                    <input @keyup.enter="onButtonClick" placeholder="ask me something BASED" :value="modelValue"
                        @input="updateValue($event)">
                    <p :class="modelValue === '' ? 'hidden' : ''">~ {{ tokens }} tokens</p>
                    <button :disabled="modelValue == '' || disabled" @click="onButtonClick">
                        <Icon style="font-size: 2rem;" icon="carbon:send" />
                    </button>
                </div>
            </div>
        </div>
    </div>
    <!-- <p>Tokens: ~{{ tokenCount.toFixed(0) }}</p>
                                                                                <p>Cost: ~{{ queryCost.toFixed(6) }} €</p> -->
</template>

<style scoped>
.input p {
    white-space: nowrap;
    margin: 0;
    line-height: 1.75rem;
    color: var(--color-green-muted);
    opacity: .75;
    transition: all 0.2s ease;

}

.input p.hidden {
    /* opacity: 0; */
    color: transparent;
}

.content {
    display: flex;
    justify-content: center;
    align-items: center;
    width: 100%;
    max-width: 1280px;
    flex-grow: 1;
    flex-basis: 1280px;
    padding: 0 1rem;
}

.background {
    display: flex;
    justify-content: center;
    align-items: center;
    position: fixed;
    flex-grow: 1;
    /* width: 100%; */
    /* width: calc(100vw - 2rem); */
    bottom: 0;
    left: 0;
    background-color: rgb(62, 62, 62);
    width: 100%;
    height: 4rem;
    z-index: 20;
    /* padding: 0 2rem; */
}

input {
    /* flex-grow: 1; */
    /* height: 1rem; */
    width: 100%;
    border: none;
    outline: none;
    font-size: 1rem;
    background-color: transparent;
    color: rgba(255, 255, 255, 0.86);
}

input::placeholder {
    color: rgba(255, 255, 255, 0.5);
    transition: color 0.1s ease-in-out;
}

.input {
    width: 100%;

    /* flex-grow: 1; */
    /* height: auto; */
    /* width: calc(100% - 12rem); */
    /* max-width: calc(1280px - 24rem); */
    max-height: 3rem;
    overflow: hidden;
    /* border-radius: 12px; */
    border: 2px solid var(--color-green-muted);
    box-sizing: border-box;
    padding: .5rem 1rem;
    background: rgba(0, 0, 0, 0.33);
    display: flex;
    /* margin: 0 auto; */
    bottom: 0;
    /* position: sticky;
    bottom: 1rem; */
    /* margin-right: 1rem; */
    transition: border 0.1s ease-in-out;
    /* padding-left: 2rem; */
    gap: .5rem;


}

.input:focus-within {
    border: 2px solid var(--color-green);

}

button:focus-within>.input {
    border: 2px solid var(--color-green);
}

button {
    transition: color 0.2s ease-in-out;
    color: rgba(255, 255, 255, 0.5);
}

.input:focus-within button {
    color: var(--color-green-muted);
}

.input button {
    height: 2rem;
}

.input button svg {
    transform: translateY(-2px);
}

.input button:hover {
    color: var(--color-green);
}

.input button:disabled {
    pointer-events: none;
    color: rgba(255, 255, 255, 0.25);
    cursor: default;
}

.input:focus-within input::placeholder {
    color: rgba(255, 255, 255, 0.75);
}

.menu {
    /* display: block; */
    width: 3rem;
    height: 4rem;
    position: relative;
    flex-shrink: 0;
}

.settings {
    margin: 1rem 0;
}
</style>