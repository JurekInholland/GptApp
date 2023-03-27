<template>
    <div>
        <h1>Hello, world!</h1>
        <p>This is a paragraph.</p>
    </div>
</template>
  
<script setup lang="ts">
import { defineComponent, h, ref } from 'vue'
import type { DefineComponent } from 'vue'
interface Props {
    message: string
    description: string
}
const props = defineProps<Props>()

const message = ref(props.message)
const description = ref(props.description)
const MyComponent: DefineComponent<{}> = defineComponent({
    render() {
        return h('div', [
            h('h1', 'Hello, world!'),
            h('p', 'This is a paragraph.')
        ])
    }
})
// export default MyComponent

</script>