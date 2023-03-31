import { ref, computed } from "vue";
import { defineStore } from "pinia";
import type {
  IAPIResponse,
  IModelSettings,
  ITokenCount,
  IWebsocketMessage,
} from "@/types";
import { defaultSettings } from "@/constants";

export const useApiStore = defineStore("api", () => {
  const count = ref(0);
  const responses = ref<{ [id: string]: IAPIResponse }>({});

  const modelSettings = ref<IModelSettings>(defaultSettings);

  const doubleCount = computed(() => count.value * 2);
  const sortedResponses = computed(() => {
    const sorted = Object.values(responses.value).sort((a, b) => {
      return new Date(a.date).getTime() - new Date(b.date).getTime();
    });
    return sorted;
  });
  function increment() {
    count.value++;
  }
  function updateResponse(message: IWebsocketMessage) {
    // console.log("updateResponse", message);
    if (!responses.value[message.id]) {
      responses.value[message.id] = {
        message: "",
        status: "pending",
        date: new Date().toISOString(),
        role: "bot",
        included: true,
        tokenCount: { systemPromptTokens: 0, queryTokens: 0, historyTokens: 0 },
      };
    }
    responses.value[message.id].tokenCount.queryTokens += 1;
    // if (message.finishReason) {
    // console.log("FINISH REASON");
    // }
    if (message.finishReason === "stop") {
      // console.log("Complete");
      responses.value[message.id].status = "complete";
    }
    if (message.finishReason === "length") {
      // console.log("Incomplete");
      responses.value[message.id].status = "incomplete";
    }
    responses.value[message.id].message += message.content;
  }
  function updateApiMessageTokenCount(id: string, tokenCount: number) {
    if (responses.value[id]) {
      responses.value[id].tokenCount.queryTokens = tokenCount;
    }
  }
  function cancelPendingMessages() {
    for (const key in responses.value) {
      if (responses.value[key].status === "pending") {
        responses.value[key].status = "cancelled";
      }
    }
  }

  function addErrorResponse(message: string) {
    const response: IAPIResponse = {
      message: message,
      status: "error",
      date: new Date().toISOString(),
      role: "bot",
      included: true,
      tokenCount: { systemPromptTokens: 0, queryTokens: 0, historyTokens: 0 },
    };
    responses.value[response.date] = response;
  }

  function addUserResponse(message: string, tokenCount: ITokenCount) {
    const response: IAPIResponse = {
      message: message,
      status: "complete",
      date: new Date().toISOString(),
      role: "user",
      included: true,
      tokenCount: { ...tokenCount },
    };
    responses.value[response.date] = response;
  }

  function getResponse(id: string) {
    return responses.value[id];
  }
  function getSettings() {
    return modelSettings.value;
  }
  function clearResponses() {
    responses.value = {};
  }

  return {
    count,
    modelSettings,
    responses,
    doubleCount,
    increment,
    updateResponse,
    updateApiMessageTokenCount,
    cancelPendingMessages,
    addUserResponse,
    addErrorResponse,
    getResponse,
    getSettings,
    sortedResponses,
    clearResponses,
  };
});
