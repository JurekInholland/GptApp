interface IWebsocketMessage {
  id: string;
  content: string;
  finishReason: string;
}


interface IAPIResponse {
  message: string;
  status: string;
  date: string;
  role: string
}

export type { IWebsocketMessage, IAPIResponse };
