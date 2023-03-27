<script setup lang="ts">
import { onMounted, ref, watch, nextTick, type Ref, computed, onBeforeUnmount } from 'vue'
import { useSignalR } from '@quangdao/vue-signalr';
import CodeBlock from '../components/CodeBlock.vue';
import type { ICompletionMessage, IChatCompletionRequest, IModelSettings, IWebsocketMessage } from '../types';
import ApiInput from '../components/ApiInput.vue';
import Spinner from '../components/Spinner.vue';
import Header from '@/components/Header.vue';
import ModelSettings from '@/components/ModelSettings.vue';
import { useApiStore } from "@/stores/api";
import { defaultSettings, prompts } from '@/constants';
import gsap from 'gsap';
const store = useApiStore();

const code = "To get started with creating Tetris in Python, you can follow these steps:\n\n1. Install Pygame library: Pygame is a library in Python that is used for creating games. You can install it using pip by running the following command in your terminal: `pip install pygame`.\n\n2. Create a new Python file: Open your preferred code editor and create a new Python file.\n\n3. Import Pygame: To use Pygame in your program, you need to import it at the beginning of your Python file by adding the following code:\n\n   ```\n   import pygame\n   pygame.init()\n   ```\n\n4. Set up the game window: Use Pygame to create a game window by adding the following code:\n\n   ```\n   screen_width = 800\n   screen_height = 600\n   screen = pygame.display.set_mode((screen_width, screen_height))\n   pygame.display.set_caption(\"Tetris\")\n   ```\n\n   This code sets the width and height of the game window, creates a new Pygame display surface, and sets the caption of the game window to \"Tetris\".\n\n5. Create the game loop: The game loop is a while loop that runs continuously until the game is over. Add the following code to create the game loop:\n\n   ```\n   running = True\n   while running:\n       for event in pygame.event.get():\n           if event.type == pygame.QUIT:\n               running = False\n   ```\n\n   This code creates a boolean variable called \"running\" that is set to True. The while loop runs until \"running\" is set to False. The for loop inside the while loop iterates over all the events in the Pygame event queue. If the user clicks the \"x\" button in the top right corner of the game window, the event type is set to pygame.QUIT, and the \"running\" variable is set to False, which will exit the game loop.\n\n6. Draw the game board: Use Pygame to draw the game board by adding the following code:\n\n   ```\n   # Draw the game board\n   block_size = 20\n   board_width = 10\n   board_height = 20\n   board_x = (screen_width - block_size * board_width) / 2\n   board_y = (screen_height - block_size * board_height) / 2\n\n   for i in range(board_width):\n       for j in range(board_height):\n           pygame.draw.rect(screen, (255, 255, 255), (board_x + i * block_size, board_y + j * block_size, block_size, block_size), 1)\n   ```\n\n   This code sets the block size, board width, and board height. It also calculates the x and y coordinates of the game board using the block size, board width, and board height. The for loops inside the code iterate over all the blocks in the game board and draw them using Pygame's draw.rect() function.\n\n7. Update the game window: Use Pygame to update the game window and display the changes by adding the following code:\n\n   ```\n   pygame.display.update()\n   ```\n\n   This code updates the display surface and shows the changes made in the game board.\n\n8. Run the program: Save your Python file and run it in your terminal by typing `python filename.py`. You should see a game window with an empty game board.\n\n9. Add game logic: Start adding game logic to your program such as moving and rotating blocks, checking for completed rows, and scoring points.\n\nThese steps should give you a basic idea of how to create Tetris in Python using Pygame. You can continue to add more features and functionality to your game as you learn more about Python and Pygame.";
const signalr = useSignalR();
const result = ref("");
const userInput = ref("");
let myInput = "";

const settingsContainer = ref();

const offset = ref(16)

const settingsOpen = ref(false);

const connected: Ref<boolean> = ref(true);
const scrollBar = ref<{ scrollEl: HTMLDivElement; }>();

watch(settingsOpen, async (val) => {
    await nextTick();
    if (val) {
        gsap.to(offset, { duration: 0.5, value: settingsContainer.value.$el.offsetHeight + 16 })

        if (isScrolledToBottom()) {
            await nextTick();
            repeatedScroll();
        }
        return;
    }
    gsap.to(offset, { duration: .5, value: 16 })
})
function isScrolledToBottom() {

    const scrollTop = scrollBar.value!.scrollEl.scrollTop;
    const scrollHeight = scrollBar.value!.scrollEl.scrollHeight;
    const clientHeight = scrollBar.value!.scrollEl.clientHeight;
    console.log("is scrolled to bottom", scrollTop, clientHeight, scrollHeight, scrollTop + clientHeight >= scrollHeight * 0.9)
    return scrollTop + clientHeight >= scrollHeight * 0.98;
}

async function repeatedScroll() {
    for (let i = 0; i < 50; i++) {
        setTimeout(() => {
            // scroll();
            scrollBar.value!.scrollEl.scrollTo({ top: scrollBar.value!.scrollEl.scrollHeight + 1000 })
        }, 10 * i);
    }
}

const section = ref();
const bodyElement = ref();

const modelSettings: Ref<IModelSettings> = ref(defaultSettings);

const inProcess = ref(false);
watch(result, async (val) => {
    myInput = val;
    await nextTick();
    scroll();
    // section.value.scrollIntoView( { behavior: "smooth", block: "end" });
})

onMounted(async () => {
    await nextTick();
    // const con = signalr.connection.connectionId;

    setTimeout(async () => {
        let conId: string | null = null;
        try {

            conId = await signalr.invoke('GetConnectionId');
        }
        catch (e) {
            console.log('err');
        }
        if (conId !== null) {
            connected.value = true;
        }
    }, 1000);

    // console.log(con);


    signalr.on('update', receiveStatusUpdate);
    signalr.connection.onclose(() => {
        handleConnectionClose()
    })
    bodyElement.value = document.body;
})
onBeforeUnmount(() => {
    signalr.off('update', receiveStatusUpdate);
})
function handleConnectionClose() {
    console.log("Connection closed");
    connected.value = false;
}


const receiveStatusUpdate = (message: IWebsocketMessage) => {

    store.updateResponse(message);
    result.value += message.content;

    if (message.finishReason != null) {
        inProcess.value = false;
    }
}

const testApi = async () => {
    const conId = signalr.connection.connectionId ?? "";
    result.value = "";
    const searchParams = new URLSearchParams({
        requestMessage: "Hello Chett gbd",
        connectionId: conId
    });

    const res = await fetch(`/api/Chat/StreamResponse?${searchParams}`)
}

const submit = async () => {
    if (inProcess.value) {
        console.log("Already in process");
        return;
    }
    inProcess.value = true;
    const conId = signalr.connection.connectionId ?? "";
    if (conId == "") {
        console.log("No connection id");
        return;
    }
    result.value = "";
    const searchParams = new URLSearchParams({
        // requestMessage: userInput.value,
        connectionId: conId
    });
    store.addUserResponse(userInput.value);
    userInput.value = "";
    // const res = await fetch(`/api/Chat/StreamResponse?${searchParams}`)

    const res = await fetch(`/api/Chat/ServiceRequest?${searchParams}`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(createCompletionRequest())
    })

}

function scroll() {
    // console.log(scrollBar.value)
    if (!isScrolledToBottom()) {
        return;
    }
    scrollBar.value!.scrollEl.scrollTo({ top: scrollBar.value!.scrollEl.scrollHeight + 232 })
    // scrollBar.value!.scrollEl.scrollTop = scrollBar.value!.scrollEl.scrollHeight + 1000;
    // window.scrollTo({ top: window.screenX + 32 }); // , behavior: 'smooth'
}

function createCompletionRequest(): IChatCompletionRequest {
    let messages: ICompletionMessage[] = [];
    console.log("SP", modelSettings.value.systemPrompt)
    // System prompt
    if (modelSettings.value.systemPrompt != 'default') {
        const prompt = prompts[modelSettings.value.systemPrompt];
        messages.push({
            role: 'system',
            content: prompt.prompt,
        });
    }

    // Message history
    if (modelSettings.value.includeHistory) {
        messages = messages.concat(store.sortedResponses.map((item) => {
            return {
                role: item.role === 'bot' ? 'assistant' : 'user',
                content: item.message,
            }
        }));
    }

    const request: IChatCompletionRequest = {
        model: modelSettings.value.model,
        temperature: modelSettings.value.temperature,
        max_tokens: modelSettings.value.maxTokens,
        frequency_penalty: modelSettings.value.frequencyPenalty,
        presence_penalty: modelSettings.value.presencePenalty,
        n: 1,
        stream: true,
        messages: messages,
    }
    console.log(request)
    return request;
}

</script>

<template>
    <custom-scrollbar ref="scrollBar" :style="{ width: '100vw', height: 'calc(100svh - 4rem)' }">

        <div class="top">
            <!-- <button @click="scroll">scrl</button> -->
            <Header />
            <p v-if="!connected">lost connection... (press F5)</p>
        </div>
        <!-- todo: v-auto-animate -->
        <div ref="section" class="cont" :style="{ 'padding-bottom': offset + 'px' }">
            <div class="sec">
                <CodeBlock style="block" v-for="item in store.sortedResponses" :response="item" :key="item.message" />
            </div>
            <Transition>
                <ModelSettings ref="settingsContainer" v-model="modelSettings" v-if="settingsOpen" />
            </Transition>
        </div>
        <ApiInput :disabled="!connected" v-model:model-value="userInput" v-model:settings-open="settingsOpen"
            @submit="submit" />
    </custom-scrollbar>
</template>

<style scoped>
.top {
    display: flex;
    justify-content: center;
    align-items: center;
    gap: 1rem;
    padding-top: 1rem;

}

.top p {
    color: #d75f5f;
}

.block {
    border: 2px solid rgba(223, 191, 142, .75);

}

.content {
    overflow: hidden;
}

/* we will explain what these classes do next! */
.v-enter-active,
.v-leave-active {
    transition: transform .5s ease;
    overflow: hidden;
    height: auto;
}

/* .v-leave-active body, .v-leave-active #app {
    overflow: hidden;
} */

.v-enter-from,
.v-leave-to {
    transform: translateY(100%);
}

.settings {
    padding-bottom: 12rem !important;
}


.cont {
    display: flex;
    flex-direction: column;
    flex-grow: 1;
    justify-content: center;
    align-items: center;
    padding: 1rem;
    overflow: hidden;
    /* margin-bottom: 2.5rem; */
    /* background-color: brown; */

}

.sec {
    display: flex;
    flex-direction: column;
    gap: 2rem;
    box-sizing: border-box;
    width: 100%;
    height: 100%;
    max-width: 1280px;
    /* margin-bottom: 4rem; */
    /* background-color: blue; */
}

.spin {
    /* align-self: flex-end; */
    /* position: relative; */
    width: 100%;
    max-width: 1280px;
    margin-right: 2rem;
    height: 0;
}

.spinner {
    scale: 2;
    /* height: 100%; */
    position: fixed;
    top: 0;
    /* right: 0; */
    left: 0;
    /* right: 2rem; */
    /* float: right; */
    z-index: 5;
    /* transform: translate(-50%, -50%); */
}
</style>
