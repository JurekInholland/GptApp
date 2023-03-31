<script setup lang="ts">
import type { ITokenCount, ITokenResponse, ICompletionMessage, IChatCompletionRequest, IModelSettings, IWebsocketMessage } from '../types';
import { onMounted, ref, watch, nextTick, type Ref, computed, type ComputedRef, onBeforeUnmount } from 'vue'
import { useSignalR } from '@quangdao/vue-signalr';
import CodeBlock from '../components/CodeBlock.vue';
import ApiInput from '../components/ApiInput.vue';
import Spinner from '../components/Spinner.vue';
import Header from '@/components/Header.vue';
import ModelSettings from '@/components/ModelSettings.vue';
import { useApiStore } from "@/stores/api";
import { defaultSettings, prompts } from '@/constants';
import gsap from 'gsap';
import ApiService from '@/api';
const store = useApiStore();

const apiService = new ApiService()

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

const includedMessagesCount = ref(0);

const latestStatus: ComputedRef<string> = computed(() => {
    return store.sortedResponses[store.sortedResponses.length - 1]?.status ?? "pending";
});
function handleResize(event: UIEvent) {
    const currentWin = event.currentTarget as Window;
    const targetWin = event.target as Window;

    console.log('resize', event, currentWin.innerWidth, targetWin.innerWidth);
    animateMargin(settingsOpen.value);


}
onBeforeUnmount(async () => {
    signalr.off('update', receiveStatusUpdate);
    window.removeEventListener('resize', handleResize);

});

onMounted(async () => {
    includedMessagesCount.value = countIncludedMessages(systemPromptText.value.length);
    countSystemPromptTokens();
    countHistoryTokens();
    signalr.on('update', receiveStatusUpdate);
    signalr.connection.onclose(() => {
        handleConnectionClose()
    })
    signalr.connection.onreconnected(() => {
        console.log("reconnected");
        connected.value = true;
    })
    bodyElement.value = document.body;
    window.addEventListener('resize', handleResize);
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



})



watch(latestStatus, async (val: string) => {
    if (val !== "pending") {
        countHistoryTokens();

        // console.log("latest status changed", val);
        // includedMessagesCount.value = store.sortedResponses.length;
    }
});
const tokenCount: Ref<ITokenCount> = ref({
    historyTokens: 0,
    systemPromptTokens: 0,
    queryTokens: 0,
});

watch(settingsOpen, async (val) => {
    await animateMargin(val);
})

async function animateMargin(val: boolean) {
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
}

function isScrolledToBottom() {

    const scrollTop = scrollBar.value!.scrollEl.scrollTop;
    const scrollHeight = scrollBar.value!.scrollEl.scrollHeight;
    const clientHeight = scrollBar.value!.scrollEl.clientHeight;
    if (scrollTop + clientHeight + 150 >= scrollHeight) {
        return true;
    }
    console.log("dbg", scrollTop + clientHeight + 100, scrollHeight)
    const isScrolled = scrollTop + clientHeight + 100 >= scrollHeight;
    console.log("isScrolled", isScrolled)
    console.log("scrollTop", scrollTop, "scrollHeight", scrollHeight, "clientHeight", clientHeight)
    // return isScrolled;
    return false

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

const systemPromptText = computed(() => {
    if (modelSettings.value.systemPrompt === "custom")
        return modelSettings.value.customPrompt;
    return prompts[modelSettings.value.systemPrompt].prompt;
})

const totalTokens = computed(() => {
    return tokenCount.value.historyTokens + tokenCount.value.systemPromptTokens + tokenCount.value.queryTokens;
})
const totalConversationTokens = computed(() => {
    return store.sortedResponses.map(x => getTotalTokenCount(x.tokenCount)).reduce((a, b) => a + b, 0);
})
function getTotalTokenCount(tokenCounts: ITokenCount): number {
    return tokenCounts.historyTokens + tokenCounts.systemPromptTokens + tokenCounts.queryTokens;
}
watch(() => store.sortedResponses, async () => {
    // console.log('sortedResponses changed');
    countHistoryTokens();

    includedMessagesCount.value = countIncludedMessages(systemPromptText.value.length);
});

watch(modelSettings.value, async (newSettings) => {
    await nextTick();
    includedMessagesCount.value = countIncludedMessages(systemPromptText.value.length);
    countHistoryTokens();

    await nextTick();
    setTimeout(() => {
        animateMargin(settingsOpen.value);
    }, 1);
});

watch(systemPromptText, async (old, updated) => {
    countSystemPromptTokens();

});

watch(userInput, async () => {
    tokenCount.value.queryTokens = await countTokens(`role: 'user', content: ${userInput.value}\n`);
    includedMessagesCount.value = countIncludedMessages(systemPromptText.value.length);
})

watch(tokenCount.value, async (count: ITokenCount) => {
    // console.log('tokenCount changed', tokenCount.value);
    if (!modelSettings.value.includeHistory) {
        return;
    }
    const totalTokenCount = getTotalTokenCount(count);
    if (totalTokenCount + parseInt(modelSettings.value.maxTokens) > 2048) {
        console.log("Too many tokens", typeof (totalTokenCount), typeof (modelSettings.value.maxTokens));
    }
})

watch(store.sortedResponses, async () => {
    console.log('sortedResponses changed');
    await countHistoryTokens();

})

async function countHistoryTokens() {
    if (!modelSettings.value.includeHistory) {
        tokenCount.value.historyTokens = 0;
        return;
    }
    tokenCount.value.historyTokens = await countTokens(store.sortedResponses.filter(x => x.included).map(x => `'${x.role}': '${x.message}' `).join(''));
}
async function countSystemPromptTokens() {
    if (modelSettings.value.systemPrompt === "default") {
        tokenCount.value.systemPromptTokens = 0;
        return;
    }
    tokenCount.value.systemPromptTokens = await countTokens(`'system': '${systemPromptText.value}' `);
}

function handleConnectionClose() {
    console.log("Connection closed");
    connected.value = false;
    store.cancelPendingMessages();
}


const receiveStatusUpdate = async (message: IWebsocketMessage) => {

    store.updateResponse(message);
    result.value += message.content;

    if (message.finishReason != null) {
        inProcess.value = false;
        store.updateApiMessageTokenCount(message.id, await countTokens(store.getResponse(message.id).message));
    }
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
    store.addUserResponse(userInput.value, tokenCount.value);
    // result.value = "";
    userInput.value = "";

    // console.log(totalTokens.value + " tokens");
    await nextTick();
    scrollBar.value!.scrollEl.scrollTo({ top: scrollBar.value!.scrollEl.scrollHeight + 1232, behavior: "smooth" })
    const result: any = await apiService.getChatCompletions(createCompletionRequest(), conId);
    if (result.error) {
        inProcess.value = false;
        store.addErrorResponse(`## ${result.error.code}\n${result.error.message}`);
    }
    // console.log("res", `${result}`);

}

function scroll() {
    if (!isScrolledToBottom()) return;
    scrollBar.value!.scrollEl.scrollTo({ top: scrollBar.value!.scrollEl.scrollHeight + 232 })
}

function createQueryString() {
    const systemPromptChars = systemPromptText.value;
    const includedMessages = store.sortedResponses.filter(m => m.included).map(m => m.message);
    const promptChars = userInput.value;
    return systemPromptChars + includedMessages.join('\n') + promptChars;
}

function countRequestCharacters() {
    const systemPromptChars = systemPromptText.value.length;
    const includedMessageChars = store.sortedResponses.filter(m => m.included).reduce((a, b) => a + b.message.length, 0);
    const promptChars = userInput.value.length;
    // console.log("systemPromptChars: " + systemPromptChars)
    // console.log("includedMessages: " + includedMessageChars)
    // console.log("promptChars: " + promptChars)
}

function countIncludedMessages(characters: number = 0) {
    // countRequestCharacters();
    const messages = store.sortedResponses.filter(m => m.included).slice();
    // console.log("count messages: " + messages.length)
    let count = 0;
    while (messages.length > 0 && characters + messages[messages.length - 1].message.length + userInput.value.length < 3072) {
        const message = messages.pop();
        characters += message?.message.length ?? 0;
        count++;
    }
    return count;
}

function createCompletionRequest(): IChatCompletionRequest {

    let messages: ICompletionMessage[] = [];
    // System prompt
    if (modelSettings.value.systemPrompt != 'default') {
        messages.push({
            role: 'system',
            content: systemPromptText.value,
        });
    }
    // const numberOfMessages = countIncludedMessages(messages.reduce((acc, cur) => acc + cur.content.length, 0));

    let numberOfMessages = 0;
    // Message history
    if (!modelSettings.value.includeHistory) {

        numberOfMessages = 1;
    }
    messages = messages.concat(store.sortedResponses.filter(x => x.included).slice(numberOfMessages * -1).map((item) => {
        return {
            role: item.role === 'bot' ? 'assistant' : 'user',
            content: item.message,
        }
    }));

    const request: IChatCompletionRequest = {
        model: modelSettings.value.model,
        temperature: modelSettings.value.temperature,
        max_tokens: parseInt(modelSettings.value.maxTokens),
        frequency_penalty: modelSettings.value.frequencyPenalty,
        presence_penalty: modelSettings.value.presencePenalty,
        n: 1,
        stream: true,
        messages: messages,
    }
    console.log(request)
    return request;
}

async function countTokens(text: string): Promise<number> {
    if (text.length === 0) return 0;
    const requestTokens: ITokenResponse = await apiService.GetTokens(`${text}`) as ITokenResponse;
    return requestTokens.tokenCount;
}

async function tokenTest() {
    const reqToken = await countTokens(userInput.value);
    const systemPromptToken = await countTokens(`role: system, content: ${systemPromptText.value}`);
    const includedMessagesToken = await countTokens(store.sortedResponses.filter(m => m.included).map(m => m.message).join('\n'));
    // const query = createQueryString();
    // const tokens: ITokenResponse = await apiService.GetTokens(query);
    // debugger;
    console.log(reqToken, systemPromptToken, includedMessagesToken);
}
</script>

<template>
    <div class="top">
        <div class="header-content">
            <Header />
            <p v-if="!connected">lost connection... (press F5)</p>
            <p v-if="totalConversationTokens > 0" class="token-count">total conversation tokens: <b>{{
                totalConversationTokens }}</b>
                / <b style="color:rgba(255,255,255,.75);">€{{ (totalConversationTokens * 0.000002).toFixed(2) }}</b></p>
            <div v-else></div>
        </div>
        <!-- <button @click="scroll">scrl</button> -->
    </div>
    <custom-scrollbar ref="scrollBar"
        :style="{ width: '100vw', height: 'calc(100vh - 7.5rem)', 'min-height': '-webkit-fill-available' }">
        <!-- <button @click="tokenTest">Token test</button> -->
        <!-- todo: v-auto-animate -->
        <!-- <p>{{ includedMessagesCount }}MSG</p> -->
        <div ref="section" class="cont" :style="{ 'padding-bottom': offset + 'px' }">

            <div class="sec">
                <!--  :class="store.sortedResponses.length - (i + 1) < includedMessagesCount ? 'ok' : 'grey'" -->
                <CodeBlock v-auto-animate style="block" v-for="(item, i) in store.sortedResponses" :response="item"
                    :key="item.date" />
            </div>
            <Transition>
                <ModelSettings ref="settingsContainer" v-model="modelSettings" v-if="settingsOpen" />
            </Transition>
        </div>
        <ApiInput :disabled="!connected" v-model:model-value="userInput" v-model:settings-open="settingsOpen"
            :tokens="totalTokens" @submit="submit" />
    </custom-scrollbar>
</template>

<style scoped>
.token-count {
    color: var(--color-green-muted) !important;
    /* justify-self: flex-end;
    align-self: flex-end; */
}

.grey {
    opacity: .75;
}

.top {
    display: flex;
    justify-content: center;
    align-items: center;
    gap: 1rem;
    padding: .2rem;
    top: 0;
    position: sticky;
    background-color: #212121;
    z-index: 1;
}


.top p {
    color: #d75f5f;
}

.header-content {
    display: flex;
    width: 100%;
    max-width: 1280px;
    justify-content: space-between;
    align-items: center;
    margin: 0 1rem;
    height: 51px;
    white-space: nowrap;
    gap: 1rem;
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
