import { ref, computed } from "vue";
import { defineStore } from "pinia";
import type { IAPIResponse, IWebsocketMessage } from "@/types";

export const useApiStore = defineStore("api", () => {
  const count = ref(0);
  const responses = ref<{ [id: string]: IAPIResponse }>({});

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
    if (!responses.value[message.id]) {
      responses.value[message.id] = {
        message: "",
        status: "pending",
        date: new Date().toISOString(),
        role: "bot",
      };
    }
    responses.value[message.id].message += message.content;
  }

  function addUserResponse(message: string) {
    const response: IAPIResponse = {
      message: message,
      status: "",
      date: new Date().toISOString(),
      role: "user",
    };

    responses.value[response.date] = response;
  }

  function getResponse(id: string) {
    return responses.value[id];
  }

  return {
    count,
    doubleCount,
    increment,
    updateResponse,
    addUserResponse,
    getResponse,
    sortedResponses,
  };
});
