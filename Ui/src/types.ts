export interface IWebsocketMessage {
  id: string;
  content: string;
  finishReason: string;
}

export interface IAPIResponse {
  message: string;
  status: string;
  date: string;
  role: string;
}

export interface IModelSettings {
  model: string;
  systemPrompt: string;
  includeHistory: boolean;
  temperature: number;
  maxTokens: number;
  presencePenalty: number;
  frequencyPenalty: number;
}
export interface ICompletionMessage {
  role: string;
  content: string;
  name?: string;
}

export interface IPrompt {
  name: string;
  slug: string;
  prompt: string;
  description: string;
}


/* 
* This is the request body for the OpenAI API Proxy
*/
export interface IChatCompletionRequest {
  model: string;
  messages: ICompletionMessage[];
  stream: boolean;
  n: number;
  max_tokens: number;
  temperature: number;
  presence_penalty: number;
  frequency_penalty: number;
}
